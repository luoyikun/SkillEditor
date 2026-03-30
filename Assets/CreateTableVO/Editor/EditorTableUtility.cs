/*******************************************************************
 * 功能：编辑器表格工具集
 * 作者：罗翊坤
 * 时间：2026/3/27/周五 11:10:35
*******************************************************************/
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Amanda.EditorTable
{
    public static class EditorTableUtility
    {

        static Dictionary<string, string> m_dicTablePath = new Dictionary<string, string>()
        {
            [typeof(Amanda.EditorTable.Chapter).Name] = "CreateTableVO/Table/Chapter.txt"
        };

        public static string GetTablePathByName(string name)
        {
            if (m_dicTablePath.TryGetValue(name, out string path))
            {
                path = $"{Application.dataPath}/{path}";
                return path;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读取txt每行
        /// </summary>
        public static List<string> ReadLinesWithSharedAccess(string filePath, Encoding encoding = default)
        {
            if (File.Exists(filePath) == false)
            {
                Debug.LogError($"文件不存在{filePath}");
            }
            if (encoding == default)
            {
                encoding = Encoding.GetEncoding("GB2312");
            }
            var lines = new List<string>();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs, encoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"无法读取文件 {filePath}：可能被占用{ex.Message}");
                
            }

            return lines;
        }
    }
}
#endif
