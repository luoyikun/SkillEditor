#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 配置表转C#类工具（统一值转换入口：仅ConvertValue）
/// 规则：
/// 1. string：null/空→"" | 有值→原值
/// 2. float：0→"0"；有值→去掉末尾0（12.040→"12.04"、1.0→"1"）
/// 3. int/bool：0/false→"0"，true→"1"
/// 4. 空List（null/Count=0）→""，List元素按上述规则处理
/// </summary>
public static class TableToClassGenerator
{
    /// <summary>
    /// 生成配置表对应的C#类文件（实现IEditorTable接口）
    /// </summary>
    /// <param name="tablePath">配置表文件路径（.txt）</param>
    /// <param name="outputPath">生成的.cs文件路径</param>
    /// <param name="className">生成的类名</param>
    /// <returns>是否生成成功</returns>
    public static bool GenerateClassFromTable(string tablePath, string outputPath, string className)
    {
        if (!File.Exists(tablePath))
        {
            Debug.LogError($"配置表不存在：{tablePath}");
            return false;
        }

        // 强制GBK编码读取策划表（解决中文乱码）
        string[] allLines;
        try
        {
            using (FileStream fs = new FileStream(tablePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GBK")))
            {
                string content = sr.ReadToEnd();
                allLines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GBK读取失败，降级为UTF8读取：{e.Message}");
            allLines = File.ReadAllLines(tablePath, Encoding.UTF8);
        }

        if (allLines.Length < 3)
        {
            Debug.LogError($"配置表行数不足，至少需要3行（类型、注释、字段名）：{tablePath}");
            return false;
        }

        string typeLine = allLines[0].TrimEnd();
        string commentLine = allLines[1].TrimEnd();
        string fieldNameLine = allLines[2].TrimEnd();

        string[] typeArray = typeLine.Split('\t');
        string[] commentArray = commentLine.Split('\t');
        string[] fieldNameArray = fieldNameLine.Split('\t');

        int maxColCount = Math.Max(Math.Max(typeArray.Length, commentArray.Length), fieldNameArray.Length);
        typeArray = ComplementEmptyColumns(typeArray, maxColCount);
        commentArray = ComplementEmptyColumns(commentArray, maxColCount);
        fieldNameArray = ComplementEmptyColumns(fieldNameArray, maxColCount);

        List<FieldInfo> fieldInfos = MergeSameNameFields(typeArray, commentArray, fieldNameArray);
        string classCode = BuildClassCode(className, fieldInfos);

        try
        {
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            // UTF8带BOM写入（解决中文乱码）
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

    #region 唯一值转换方法（整合float格式化逻辑）
    /// <summary>
    /// 通用值转换（唯一入口，整合所有类型转换规则）
    /// 1. float：0→"0"；有值→去掉末尾多余的0和小数点
    /// 2. string：null/空→"" | 有值→原值
    /// 3. int/long/short：0→"0" | 有值→原值字符串
    /// 4. bool：false→"0"，true→"1"
    /// 5. 其他类型：正常转字符串
    /// </summary>
    public static string ConvertValue<T>(T value)
    {
        if (value == null)
        {
            // null值根据类型区分：string→""，其他→"0"
            if (typeof(T) == typeof(string)) return "";
            return "0";
        }
        string str = "";

        // float类型处理
        if (value is float f)
        {
            if (Math.Abs(f) < 0.000001f) return "0";
            str = f.ToString();
            if (str.Contains('.'))
            {
                str = str.TrimEnd('0');
                str = str.TrimEnd('.');
            }
            return str;
        }

        // double类型兼容
        else if (value is double d)
        {
            if (Math.Abs(d) < 0.000001d) return "0";
            str = d.ToString();
            if (str.Contains('.'))
            {
                str = str.TrimEnd('0');
                str = str.TrimEnd('.');
            }
            return str;
        }

        // 整数类型
        if (value is int i) return i.ToString();
        if (value is long l) return l.ToString();
        if (value is short s) return s.ToString();

        // bool类型
        if (value is bool b) return b ? "1" : "0";

        // string类型
        if (value is string strValue) return string.IsNullOrEmpty(strValue) ? "" : strValue;

        // 其他类型
        return value.ToString();
    }
    #endregion

    #region 内部辅助结构体和方法
    private struct FieldInfo
    {
        public string FieldName;
        public string BaseType;
        public string Comment;
        public int ArrayLength;
        public bool IsArray;
        public bool IsList;
    }

    private static List<FieldInfo> MergeSameNameFields(string[] typeArray, string[] commentArray, string[] fieldNameArray)
    {
        List<FieldInfo> fieldInfos = new List<FieldInfo>();
        int currentCol = 0;

        while (currentCol < typeArray.Length)
        {
            string typeStr = typeArray[currentCol].Trim().ToUpper();
            string rawFieldName = fieldNameArray[currentCol].Trim();
            string comment = commentArray[currentCol].Trim();

            if (string.IsNullOrWhiteSpace(rawFieldName))
            {
                currentCol++;
                continue;
            }

            bool isListMark = typeStr.StartsWith("[") && typeStr.EndsWith("]");
            string baseType = isListMark ? typeStr.Substring(1, typeStr.Length - 2).Trim() : typeStr;
            baseType = MapTableTypeToCSharpType(baseType);

            if (baseType == null)
            {
                Debug.LogWarning($"不支持的类型 {typeStr}，跳过列 {currentCol + 1}");
                currentCol++;
                continue;
            }

            int sameFieldCount = 1;
            int nextCol = currentCol + 1;

            while (nextCol < typeArray.Length)
            {
                string nextTypeStr = typeArray[nextCol].Trim().ToUpper();
                string nextFieldName = fieldNameArray[nextCol].Trim();
                bool nextIsListMark = nextTypeStr.StartsWith("[") && nextTypeStr.EndsWith("]");
                string nextBaseType = nextIsListMark ? nextTypeStr.Substring(1, nextTypeStr.Length - 2).Trim() : nextTypeStr;
                nextBaseType = MapTableTypeToCSharpType(nextBaseType);

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

            fieldInfos.Add(new FieldInfo
            {
                FieldName = SanitizeFieldName(rawFieldName),
                BaseType = baseType,
                Comment = comment,
                ArrayLength = sameFieldCount,
                IsArray = isListMark && sameFieldCount >= 2,
                IsList = isListMark && sameFieldCount == 1
            });

            currentCol = nextCol;
        }

        return fieldInfos;
    }

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

    private static string SanitizeFieldName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName)) return "UnnamedField";
        char[] validChars = rawName.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        string sanitizedName = new string(validChars);
        if (sanitizedName.Length > 0 && char.IsDigit(sanitizedName[0]))
        {
            sanitizedName = "Col_" + sanitizedName;
        }
        return string.IsNullOrEmpty(sanitizedName) ? "UnnamedField" : sanitizedName;
    }

    /// <summary>
    /// 拼接C#类代码（实现IEditorTable接口）
    /// </summary>
    private static string BuildClassCode(string className, List<FieldInfo> fieldInfos)
    {
        StringBuilder codeBuilder = new StringBuilder();

        // 1. 宏包裹
        codeBuilder.AppendLine("#if UNITY_EDITOR");
        codeBuilder.AppendLine();

        // 2. 引用命名空间
        codeBuilder.AppendLine("using System;");
        codeBuilder.AppendLine("using System.Collections.Generic;");
        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine();

        // 3. 命名空间
        codeBuilder.AppendLine("namespace Amanda.EditorTable");
        codeBuilder.AppendLine("{");

        // 4. 类定义（关键修改：实现IEditorTable接口）
        codeBuilder.AppendLine("    /**");
        codeBuilder.AppendLine($"     * 自动生成的配置表类：{className}");
        codeBuilder.AppendLine($"     * 生成时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        codeBuilder.AppendLine("     * 请勿手动修改！");
        codeBuilder.AppendLine("     */");
        codeBuilder.AppendLine("    [Serializable]");
        codeBuilder.AppendLine($"    public class {className} : IEditorTable"); // 实现接口
        codeBuilder.AppendLine("    {");

        // 5. 生成字段
        foreach (var fieldInfo in fieldInfos)
        {
            if (string.IsNullOrWhiteSpace(fieldInfo.FieldName)) continue;

            codeBuilder.AppendLine("        /**");
            codeBuilder.AppendLine($"         * {(!string.IsNullOrWhiteSpace(fieldInfo.Comment) ? fieldInfo.Comment : "未填写注释")}");
            codeBuilder.AppendLine("         */");

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

        // 6. ToDataLine方法（接口实现，无需override，接口方法默认public）
        codeBuilder.AppendLine("        /**");
        codeBuilder.AppendLine("         * 转换为配置表行文本（\\t分隔）");
        codeBuilder.AppendLine("         * 统一调用ConvertValue方法处理所有类型值转换");
        codeBuilder.AppendLine("         * 空List（null/Count=0）→\"\"");
        codeBuilder.AppendLine("         * 实现IEditorTable接口的核心方法");
        codeBuilder.AppendLine("         */");
        codeBuilder.AppendLine("        public string ToDataLine()"); // 接口方法实现
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
                // 数组：遍历每个元素调用ConvertValue
                for (int i = 0; i < fieldInfo.ArrayLength; i++)
                {
                    codeBuilder.AppendLine($"            columnValues.Add(TableToClassGenerator.ConvertValue({fieldInfo.FieldName}[{i}]));");
                }
            }
            else if (fieldInfo.IsList)
            {
                // List：空List返回""，非空则用|分隔
                codeBuilder.AppendLine($"            if ({fieldInfo.FieldName} != null && {fieldInfo.FieldName}.Count > 0)");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine("                List<string> valList = new List<string>();");
                codeBuilder.AppendLine($"                foreach (var val in {fieldInfo.FieldName})");
                codeBuilder.AppendLine("                {");
                codeBuilder.AppendLine("                    valList.Add(TableToClassGenerator.ConvertValue(val));");
                codeBuilder.AppendLine("                }");
                codeBuilder.AppendLine("                columnValues.Add(string.Join(\"|\", valList));");
                codeBuilder.AppendLine("            }");
                codeBuilder.AppendLine("            else");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine("                columnValues.Add(string.Empty);");
                codeBuilder.AppendLine("            }");
            }
            else
            {
                // 普通字段：直接调用ConvertValue
                codeBuilder.AppendLine($"            columnValues.Add(TableToClassGenerator.ConvertValue({fieldInfo.FieldName}));");
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