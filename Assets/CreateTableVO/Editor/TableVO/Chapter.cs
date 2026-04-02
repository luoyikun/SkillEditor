#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：Chapter
     * 生成时间：2026-04-02 16:58:54
     * 请勿手动修改！
     */
    [Serializable]
    public partial class Chapter : IEditorTable
    {
        /**
         * #ID
         */
        public int ID;

        /**
         * 章节序号文本
         */
        public int ChapterOrder;

        /**
         * 章节名称.txt中的A列）
         */
        public int Name;

        /**
         * 声音列表
         */
        public List<int> Sound = new List<int>();

        /**
         * 章节包装
         */
        public int Desc;

        /**
         * 背景图资源
         */
        public int Picture;

        /**
         * 章节奖励1
         */
        public const int ChapterAwardMaxLength = 9;
        public int[] ChapterAward;

        /**
         * 声音延迟
         */
        public List<int> SoundDelay = new List<int>();

        public string ToDataLine()
        {
            List<string> columnValues = new List<string>();

            columnValues.Add(TableToClassGenerator.ConvertValue(ID));

            columnValues.Add(TableToClassGenerator.IsDefaultValue(ChapterOrder) ? string.Empty : TableToClassGenerator.ConvertValue(ChapterOrder));

            columnValues.Add(TableToClassGenerator.IsDefaultValue(Name) ? string.Empty : TableToClassGenerator.ConvertValue(Name));

            if (Sound != null && Sound.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in Sound) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            columnValues.Add(TableToClassGenerator.IsDefaultValue(Desc) ? string.Empty : TableToClassGenerator.ConvertValue(Desc));

            columnValues.Add(TableToClassGenerator.IsDefaultValue(Picture) ? string.Empty : TableToClassGenerator.ConvertValue(Picture));

            for (int i = 0; i < ChapterAwardMaxLength; i++)
            {
                if (ChapterAward != null && i < ChapterAward.Length)
                    columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[i]));
                else
                    columnValues.Add(string.Empty);
            }

            if (SoundDelay != null && SoundDelay.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in SoundDelay) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            return string.Join("\t", columnValues);
        }

        public void LoadLine(string sLine)
        {
            if (string.IsNullOrEmpty(sLine)) return;
            string[] cells = sLine.Split('\t');
            int index = 0;

            // ID
            if (index < cells.Length)
                ID = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                ID = default;

            // ChapterOrder
            if (index < cells.Length)
                ChapterOrder = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                ChapterOrder = default;

            // Name
            if (index < cells.Length)
                Name = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                Name = default;

            // Sound 列表（每次加载清空重建）
            Sound = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    Sound.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

            // Desc
            if (index < cells.Length)
                Desc = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                Desc = default;

            // Picture
            if (index < cells.Length)
                Picture = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                Picture = default;

            // ChapterAward 动态数组（自动统计非空）
            int validCount_ChapterAward = 0;
            int startIndex_ChapterAward = index;

            for (int i = 0; i < ChapterAwardMaxLength && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    validCount_ChapterAward++;
                index++;
            }

            ChapterAward = new int[validCount_ChapterAward];
            index = startIndex_ChapterAward;

            for (int i = 0; i < validCount_ChapterAward && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    ChapterAward[i] = int.TryParse(cells[index], out var val) ? val : 0;
                index++;
            }

            // SoundDelay 列表（每次加载清空重建）
            SoundDelay = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    SoundDelay.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

        }
    }
}
#endif
