#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Amanda.EditorTable
{
    public static class ChapterLineReplacer
    {
        /// <summary>
        /// 替换Chapter.txt中ID=10001的行（GB2312编码保存），并复制到剪贴板
        /// </summary>
        [MenuItem("Tools/Replace Chapter Line 10001")]
        public static void ReplaceChapterLineAndCopy()
        {
            // 1. 构造目标Chapter对象，ID=10001
            Chapter a = new Chapter();
            a.ID = 10001;
            a.ChapterOrder = 5400444;
            a.Name = 60007002;
            a.Desc = 540011;
            a.Picture = 60007001;

            // 填充奖励：3组奖励ID、数量、绑定
            a.ChapterAward[0] = 30008002;
            a.ChapterAward[1] = 1;
            a.ChapterAward[2] = 1;
            a.ChapterAward[3] = 30008003;
            a.ChapterAward[4] = 1;
            a.ChapterAward[5] = 1;
            a.ChapterAward[6] = 30900015;
            a.ChapterAward[7] = 1;
            a.ChapterAward[8] = 1;

            // 2. 生成目标行文本
            string newLine = a.ToDataLine();

            // 3. 读取Chapter.txt（请自行修正路径）
            string filePath = Application.dataPath + "/CreateTableVO/Table/Chapter.txt";
            if (!File.Exists(filePath))
            {
                Debug.LogError("文件不存在：" + filePath);
                return;
            }

            // 关键修改1：用GB2312编码读取文件
            Encoding gb2312 = Encoding.GetEncoding("GB2312");
            List<string> lines = File.ReadAllLines(filePath, gb2312).ToList();
            bool found = false;

            // 4. 查找ID=10001并替换
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                string[] parts = line.Split('\t');
                if (parts.Length > 0 && int.TryParse(parts[0], out int id))
                {
                    if (id == 10001)
                    {
                        lines[i] = newLine;
                        found = true;
                        break;
                    }
                }
            }

            // 规则2：没找到直接返回
            if (!found)
            {
                Debug.Log("未找到ID=10001，不执行操作");
                return;
            }

            // 关键修改2：用GB2312编码写入文件
            File.WriteAllLines(filePath, lines, gb2312);

            // 规则3：复制到剪贴板
            GUIUtility.systemCopyBuffer = newLine;
            Debug.Log("替换完成（GB2312编码保存），已复制到剪贴板：\n" + newLine);
        }
    }
}
#endif