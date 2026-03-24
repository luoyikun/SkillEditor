#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：Chapter
     * 生成时间：2026-03-23 22:38:24
     * 请勿手动修改！
     */
    [Serializable]
    public class Chapter : IEditorTable
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
        public int[] ChapterAward = new int[ChapterAwardMaxLength];

        /**
         * 转换为配置表行文本（\t分隔）
         * 统一调用ConvertValue方法处理所有类型值转换
         * 空List（null/Count=0）→""
         * 数组输出按常量最大长度循环，不足补空串
         * 实现IEditorTable接口的核心方法
         */
        public string ToDataLine()
        {
            List<string> columnValues = new List<string>();

            // ID
            columnValues.Add(TableToClassGenerator.ConvertValue(ID));

            // ChapterOrder
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterOrder));

            // Name
            columnValues.Add(TableToClassGenerator.ConvertValue(Name));

            // Desc
            columnValues.Add(TableToClassGenerator.ConvertValue(Desc));

            // Picture
            columnValues.Add(TableToClassGenerator.ConvertValue(Picture));

            // ChapterAward
            int maxLen = ChapterAwardMaxLength;
            for (int i = 0; i < maxLen; i++)
            {
                if (ChapterAward != null && i < ChapterAward.Length)
                {
                    columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[i]));
                }
                else
                {
                    columnValues.Add(string.Empty);
                }
            }

            return string.Join("\t", columnValues);
        }
    }
}

#endif
