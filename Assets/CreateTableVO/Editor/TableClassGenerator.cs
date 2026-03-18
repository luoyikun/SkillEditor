#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 配置表转C#类工具（仅修复中文乱码，兼容原有窗口）
/// 核心：强制GBK读取策划表 + UTF8带BOM写入，解决中文注释乱码
/// </summary>
public static class TableToClassGenerator
{
    /// <summary>
    /// 生成配置表对应的C#类文件
    /// </summary>
    /// <param name="tablePath">配置表文件路径（.txt）</param>
    /// <param name="outputPath">生成的.cs文件路径</param>
    /// <param name="className">生成的类名</param>
    /// <returns>是否生成成功</returns>
    public static bool GenerateClassFromTable(string tablePath, string outputPath, string className)
    {
        // 1. 校验文件是否存在
        if (!File.Exists(tablePath))
        {
            Debug.LogError($"配置表不存在：{tablePath}");
            return false;
        }

        // 2. 【核心修复乱码】强制GBK编码读取策划表（99%策划表都是GBK/GB2312编码）
        string[] allLines;
        try
        {
            using (FileStream fs = new FileStream(tablePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GBK"))) // 强制GBK读取
            {
                string content = sr.ReadToEnd();
                // 兼容所有换行符，避免拆分错误
                allLines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
        }
        catch (Exception e)
        {
            // 异常时降级为UTF8读取（兼容少数UTF8编码表）
            Debug.LogWarning($"GBK读取失败，降级为UTF8读取：{e.Message}");
            allLines = File.ReadAllLines(tablePath, Encoding.UTF8);
        }

        // 3. 校验表结构（至少3行：类型行、注释行、字段名行）
        if (allLines.Length < 3)
        {
            Debug.LogError($"配置表行数不足，至少需要3行（类型、注释、字段名）：{tablePath}");
            return false;
        }

        // 4. 拆分列（仅\t分隔，保留空列）
        string typeLine = allLines[0].TrimEnd();
        string commentLine = allLines[1].TrimEnd();
        string fieldNameLine = allLines[2].TrimEnd();

        string[] typeArray = typeLine.Split('\t');
        string[] commentArray = commentLine.Split('\t');
        string[] fieldNameArray = fieldNameLine.Split('\t');

        // 5. 补全空列，确保列数一致
        int maxColCount = Math.Max(Math.Max(typeArray.Length, commentArray.Length), fieldNameArray.Length);
        typeArray = ComplementEmptyColumns(typeArray, maxColCount);
        commentArray = ComplementEmptyColumns(commentArray, maxColCount);
        fieldNameArray = ComplementEmptyColumns(fieldNameArray, maxColCount);

        // 6. 合并同名字段（核心规则）
        List<FieldInfo> fieldInfos = MergeSameNameFields(typeArray, commentArray, fieldNameArray);

        // 7. 拼接C#类代码（保留命名空间 + UNITY_EDITOR宏）
        string classCode = BuildClassCode(className, fieldInfos);

        // 8. 【核心修复乱码】UTF8带BOM写入，确保中文注释正常显示
        try
        {
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            // 必须使用new UTF8Encoding(true)（带BOM），否则中文乱码
            File.WriteAllText(outputPath, classCode, new UTF8Encoding(true));
            Debug.Log($"类文件生成成功：{outputPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存类文件失败：{e.Message}");
            return false;
        }
    }

    #region 公共静态工具方法（供生成的类调用）
    /// <summary>
    /// Float格式化：移除末尾多余的0和小数点（公共静态方法）
    /// 示例：12 → "12" | 12.34 → "12.34" | 12.0 → "12"
    /// </summary>
    public static string FormatFloat(float value)
    {
        string str = value.ToString();
        if (str.Contains('.'))
        {
            str = str.TrimEnd('0');
            str = str.TrimEnd('.');
        }
        return str;
    }
    #endregion

    #region 内部辅助结构体和方法
    /// <summary>
    /// 字段信息结构体
    /// </summary>
    private struct FieldInfo
    {
        public string FieldName;    // 字段名（C#合法）
        public string BaseType;     // 基础类型（int/string/float）
        public string Comment;      // 注释（保留原标点，不修改）
        public int ArrayLength;     // 数组长度（List为1）
        public bool IsArray;        // 是否为数组
        public bool IsList;         // 是否为List
    }

    /// <summary>
    /// 合并同名字段（核心规则）
    /// [INT]标记：连续同名字段≥2→数组；单个→List
    /// 普通类型：单个→单字段；连续同名字段→数组
    /// </summary>
    private static List<FieldInfo> MergeSameNameFields(string[] typeArray, string[] commentArray, string[] fieldNameArray)
    {
        List<FieldInfo> fieldInfos = new List<FieldInfo>();
        int currentCol = 0;

        while (currentCol < typeArray.Length)
        {
            string typeStr = typeArray[currentCol].Trim().ToUpper();
            string rawFieldName = fieldNameArray[currentCol].Trim();
            string comment = commentArray[currentCol].Trim(); // 注释原封不动，不做任何处理

            // 跳过空字段名
            if (string.IsNullOrWhiteSpace(rawFieldName))
            {
                currentCol++;
                continue;
            }

            // 解析类型标记（是否带[]）
            bool isListMark = typeStr.StartsWith("[") && typeStr.EndsWith("]");
            string baseType = isListMark ? typeStr.Substring(1, typeStr.Length - 2).Trim() : typeStr;
            baseType = MapTableTypeToCSharpType(baseType);

            // 跳过不支持的类型
            if (baseType == null)
            {
                Debug.LogWarning($"不支持的类型 {typeStr}，跳过列 {currentCol + 1}");
                currentCol++;
                continue;
            }

            // 查找连续同名字段
            int sameFieldCount = 1;
            int nextCol = currentCol + 1;

            while (nextCol < typeArray.Length)
            {
                string nextTypeStr = typeArray[nextCol].Trim().ToUpper();
                string nextFieldName = fieldNameArray[nextCol].Trim();
                bool nextIsListMark = nextTypeStr.StartsWith("[") && nextTypeStr.EndsWith("]");
                string nextBaseType = nextIsListMark ? nextTypeStr.Substring(1, nextTypeStr.Length - 2).Trim() : nextTypeStr;
                nextBaseType = MapTableTypeToCSharpType(nextBaseType);

                // 合并条件：同名字段 + 同基础类型 + 同List标记
                if (nextFieldName == rawFieldName && nextBaseType == baseType && nextIsListMark == isListMark)
                {
                    sameFieldCount++;
                    nextCol++;
                }
                else
                {
                    break;
                }
            }

            // 构建字段信息
            FieldInfo fieldInfo = new FieldInfo
            {
                FieldName = SanitizeFieldName(rawFieldName),
                BaseType = baseType,
                Comment = comment, // 注释直接赋值，不做任何清理/转义
                ArrayLength = sameFieldCount,
                IsArray = isListMark && sameFieldCount >= 2,  // [INT] + 同名字段≥2 → 数组
                IsList = isListMark && sameFieldCount == 1    // [INT] + 单个字段 → List
            };

            fieldInfos.Add(fieldInfo);
            currentCol = nextCol;
        }

        return fieldInfos;
    }

    /// <summary>
    /// 表类型映射到C#类型
    /// </summary>
    private static string MapTableTypeToCSharpType(string tableType)
    {
        return tableType switch
        {
            "INT" => "int",
            "STRING" => "string",
            "FLOAT" => "float",
            "DOUBLE" => "double",
            "BOOL" => "bool",
            _ => null
        };
    }

    /// <summary>
    /// 补全空列，确保数组长度一致
    /// </summary>
    private static string[] ComplementEmptyColumns(string[] array, int targetLength)
    {
        if (array.Length >= targetLength) return array;

        string[] newArray = new string[targetLength];
        Array.Copy(array, newArray, array.Length);
        for (int i = array.Length; i < targetLength; i++)
        {
            newArray[i] = "";
        }
        return newArray;
    }

    /// <summary>
    /// 清理字段名，确保符合C#标识符规则（仅清理字段名，不碰注释）
    /// </summary>
    private static string SanitizeFieldName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName)) return "UnnamedField";

        // 仅保留字母、数字、下划线
        char[] validChars = rawName.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        string sanitizedName = new string(validChars);

        // 数字开头添加前缀
        if (sanitizedName.Length > 0 && char.IsDigit(sanitizedName[0]))
        {
            sanitizedName = "Col_" + sanitizedName;
        }

        // 空名称兜底
        return string.IsNullOrEmpty(sanitizedName) ? "UnnamedField" : sanitizedName;
    }

    /// <summary>
    /// 拼接C#类代码（保留命名空间 + UNITY_EDITOR宏，注释原封不动）
    /// </summary>
    private static string BuildClassCode(string className, List<FieldInfo> fieldInfos)
    {
        StringBuilder codeBuilder = new StringBuilder();

        // 1. 包裹 #if UNITY_EDITOR 宏
        codeBuilder.AppendLine("#if UNITY_EDITOR");
        codeBuilder.AppendLine();

        // 2. 引用命名空间
        codeBuilder.AppendLine("using System;");
        codeBuilder.AppendLine("using System.Collections.Generic;");
        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine();

        // 3. 添加命名空间 Amanda.EditorTable
        codeBuilder.AppendLine("namespace Amanda.EditorTable");
        codeBuilder.AppendLine("{");

        // 4. 类注释
        codeBuilder.AppendLine("    /**");
        codeBuilder.AppendLine($"     * 自动生成的配置表类：{className}");
        codeBuilder.AppendLine($"     * 生成时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        codeBuilder.AppendLine("     * 请勿手动修改！");
        codeBuilder.AppendLine("     */");
        codeBuilder.AppendLine("    [Serializable]");
        codeBuilder.AppendLine($"    public class {className}");
        codeBuilder.AppendLine("    {");

        // 5. 生成字段（注释原封不动）
        foreach (var fieldInfo in fieldInfos)
        {
            if (string.IsNullOrWhiteSpace(fieldInfo.FieldName)) continue;

            // 字段注释（原封不动输出，不做任何修改）
            codeBuilder.AppendLine("        /**");
            codeBuilder.AppendLine($"         * {(!string.IsNullOrWhiteSpace(fieldInfo.Comment) ? fieldInfo.Comment : "未填写注释")}");
            codeBuilder.AppendLine("         */");

            // 字段声明（保留数组格式：int[] Name = new int[2];）
            if (fieldInfo.IsArray)
            {
                codeBuilder.AppendLine($"        public {fieldInfo.BaseType}[] {fieldInfo.FieldName} = new {fieldInfo.BaseType}[{fieldInfo.ArrayLength}];");
            }
            else if (fieldInfo.IsList)
            {
                codeBuilder.AppendLine($"        public List<{fieldInfo.BaseType}> {fieldInfo.FieldName} = new List<{fieldInfo.BaseType}>();");
            }
            else
            {
                codeBuilder.AppendLine($"        public {fieldInfo.BaseType} {fieldInfo.FieldName};");
            }
            codeBuilder.AppendLine();
        }

        // 6. ToDataLine方法（调用静态FormatFloat）
        codeBuilder.AppendLine("        /**");
        codeBuilder.AppendLine("         * 转换为配置表行文本（\\t分隔）");
        codeBuilder.AppendLine("         * 数组：按索引拆分 | List：|分隔 | Float：调用工具类格式化");
        codeBuilder.AppendLine("         */");
        codeBuilder.AppendLine("        public string ToDataLine()");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            List<string> columnValues = new List<string>();");
        codeBuilder.AppendLine();

        // 拼接字段值
        foreach (var fieldInfo in fieldInfos)
        {
            if (string.IsNullOrWhiteSpace(fieldInfo.FieldName)) continue;

            codeBuilder.AppendLine($"            // {fieldInfo.FieldName}");
            if (fieldInfo.IsArray)
            {
                // 数组：按索引逐个添加（调用静态FormatFloat）
                for (int i = 0; i < fieldInfo.ArrayLength; i++)
                {
                    if (fieldInfo.BaseType == "float")
                    {
                        codeBuilder.AppendLine($"            columnValues.Add(TableToClassGenerator.FormatFloat({fieldInfo.FieldName}[{i}]));");
                    }
                    else
                    {
                        codeBuilder.AppendLine($"            columnValues.Add({fieldInfo.FieldName}[{i}].ToString());");
                    }
                }
            }
            else if (fieldInfo.IsList)
            {
                // List：|分隔（调用静态FormatFloat）
                codeBuilder.AppendLine($"            if ({fieldInfo.FieldName} != null)");
                codeBuilder.AppendLine("            {");
                if (fieldInfo.BaseType == "float")
                {
                    codeBuilder.AppendLine("                List<string> floatStrList = new List<string>();");
                    codeBuilder.AppendLine($"                foreach (float val in {fieldInfo.FieldName})");
                    codeBuilder.AppendLine("                {");
                    codeBuilder.AppendLine("                    floatStrList.Add(TableToClassGenerator.FormatFloat(val));");
                    codeBuilder.AppendLine("                }");
                    codeBuilder.AppendLine("                columnValues.Add(string.Join(\"|\", floatStrList));");
                }
                else
                {
                    codeBuilder.AppendLine($"                columnValues.Add(string.Join(\"|\", {fieldInfo.FieldName}));");
                }
                codeBuilder.AppendLine("            }");
                codeBuilder.AppendLine("            else");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine("                columnValues.Add(string.Empty);");
                codeBuilder.AppendLine("            }");
            }
            else
            {
                // 普通字段（调用静态FormatFloat）
                if (fieldInfo.BaseType == "float")
                {
                    codeBuilder.AppendLine($"            columnValues.Add(TableToClassGenerator.FormatFloat({fieldInfo.FieldName}));");
                }
                else
                {
                    codeBuilder.AppendLine($"            columnValues.Add({fieldInfo.FieldName}.ToString());");
                }
            }
            codeBuilder.AppendLine();
        }

        codeBuilder.AppendLine("            return string.Join(\"\\t\", columnValues);");
        codeBuilder.AppendLine("        }");

        // 7. 闭合类、命名空间、宏
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine("}");
        codeBuilder.AppendLine();
        codeBuilder.AppendLine("#endif");

        return codeBuilder.ToString();
    }
    #endregion
}
#endif