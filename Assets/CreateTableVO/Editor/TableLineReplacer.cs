using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Amanda.EditorTable
{
    public class TableLineReplacer
    {
        //[MenuItem("Tools/TestActionSkillConfigTableLineReplacer")]
        //public static void ReplaceLineAndCopy()
        //{
        //    ActionSkillConfig actionSkillConfig = new ActionSkillConfig();
        //    actionSkillConfig.ID = 26200011;
        //    ReplaceLineAndCopy(actionSkillConfig.ID, actionSkillConfig, Application.dataPath + "/CreateTableVO/Table/ActionSkillConfigTable.txt");
        //}


        [MenuItem("Tools/LoadChapter")]
        public static void ReplaceLineAndCopy()
        {
            EditorTableMgr<Chapter>.Load();

            bool ret = EditorTableMgr<Chapter>.Get(10001,out Chapter data);
            if (ret)
            {
                Debug.Log(data.ChapterAward[0]);
            }
        }

        public static void ReplaceLineAndCopy(int id, IEditorTable table, string path)
        {
            string[] arrPath = path.Split('/');
            string fileName = arrPath[arrPath.Length - 1];
            string newLine = table.ToDataLine();
            Encoding gb2312 = Encoding.GetEncoding("GB2312");

            // ---------- 1. 以共享方式读取文件 ----------
            List<string> lines = null;
            try
            {
                lines = ReadLinesWithSharedAccess(path, gb2312);
            }
            catch (IOException ex)
            {
                Debug.LogError($"无法读取文件 {fileName}，可能被其他程序占用：{ex.Message}");
                return;
            }

            // ---------- 2. 查找并替换 ----------
            bool found = false;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                string[] parts = line.Split('\t');
                if (parts.Length > 0 && int.TryParse(parts[0], out int txtID))
                {
                    if (txtID == id)
                    {
                        lines[i] = newLine;
                        found = true;
                        break;
                    }
                }
            }

            // ---------- 3. 处理结果 ----------
            if (!found)
            {
                Debug.Log($"{fileName}未找到id：{id}，生成信息已复制到剪切板");
            }
            else
            {
                // ---------- 4. 以共享方式尝试写入 ----------
                try
                {
                    WriteLinesWithSharedAccess(path, lines, gb2312);
                    Debug.Log($"{fileName}替换id：{id}的行成功");
                }
                catch (IOException ex)
                {
                    Debug.LogError($"无法写入文件 {fileName}，请确保文件未被其他程序（如WPS）独占打开。\n错误：{ex.Message}");
                }
            }

            // 规则3：复制到剪贴板
            GUIUtility.systemCopyBuffer = newLine;
        }

        /// <summary>
        /// 使用 FileShare.Read 读取文件，允许其他进程同时读取
        /// </summary>
        private static List<string> ReadLinesWithSharedAccess(string filePath, Encoding encoding)
        {
            var lines = new List<string>();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(fs, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        /// <summary>
        /// 使用 FileShare.Read 写入文件，允许其他进程读取（但若其他进程已独占写入，则此操作会失败）
        /// </summary>
        private static void WriteLinesWithSharedAccess(string filePath, List<string> lines, Encoding encoding)
        {
            // 注意：写入时需要 FileAccess.Write，并允许其他进程读取（FileShare.Read）
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fs, encoding))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}