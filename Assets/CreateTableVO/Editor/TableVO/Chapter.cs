#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：Chapter
     * 生成时间：2026-03-18 21:44:24
     * 请勿手动修改！
     */
    [Serializable]
    public class Chapter
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
        public int[] ChapterAward = new int[9];

        /**
         * 转换为配置表行文本（\t分隔）
         * 统一调用ConvertValue方法处理所有类型值转换
         * 空List（null/Count=0）→""
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
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[0]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[1]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[2]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[3]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[4]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[5]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[6]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[7]));
            columnValues.Add(TableToClassGenerator.ConvertValue(ChapterAward[8]));

            return string.Join("\t", columnValues);
        }
    }
}

#endif
