/*******************************************************************
 * 功能：编辑器表格管理器
 * 作者：罗翊坤
 * 时间：2026/3/27/周五 11:29:59
*******************************************************************/
#if UNITY_EDITOR
using Amanda.EditorTable;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /// <summary>
    /// 【通用泛型配置表管理器】
    /// </summary>
    public static class EditorTableMgr<T> where T : IEditorTable, new()
    {
        // 每个类型 T 自动单例 + 独立字典
        private static readonly Dictionary<int, T> _dict = new Dictionary<int, T>();

        /// <summary>
        /// 加载当前表
        /// </summary>
        public static void Load()
        {
            _dict.Clear();

            // 表名 = 类名
            string tableName = typeof(T).Name;
            string path = EditorTableUtility.GetTablePathByName(tableName);

            List<string> lines = EditorTableUtility.ReadLinesWithSharedAccess(path);

            foreach (var line in lines)
            {
                string trimLine = line.Trim();
                if (string.IsNullOrEmpty(trimLine) || trimLine.StartsWith("#"))
                    continue;

                string[] cells = trimLine.Split('\t');
                if (cells.Length == 0) continue;

                if (int.TryParse(cells[0], out int id))
                {
                    T data = new T();
                    data.LoadLine(trimLine);
                    _dict[id] = data;
                }
            }
        }

        /// <summary>
        /// 根据ID获取配置
        /// </summary>
        public static bool Get(int id, out T data)
        {
            if (_dict.TryGetValue(id, out data))
            {
                return true;
            }
            Debug.LogError($"{typeof(T).Name} 未找到ID{id}");
            return false;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public static IReadOnlyDictionary<int, T> All => _dict;
    }
}
#endif