#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：Chapter
     * 生成时间：2026-03-18 16:16:44
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
         * 数组：按索引拆分 | List：|分隔 | Float：调用工具类格式化
         */
        public string ToDataLine()
        {
            List<string> columnValues = new List<string>();

            // ID
            columnValues.Add(ID.ToString());

            // ChapterOrder
            columnValues.Add(ChapterOrder.ToString());

            // Name
            columnValues.Add(Name.ToString());

            // Desc
            columnValues.Add(Desc.ToString());

            // Picture
            columnValues.Add(Picture.ToString());

            // ChapterAward
            columnValues.Add(ChapterAward[0].ToString());
            columnValues.Add(ChapterAward[1].ToString());
            columnValues.Add(ChapterAward[2].ToString());
            columnValues.Add(ChapterAward[3].ToString());
            columnValues.Add(ChapterAward[4].ToString());
            columnValues.Add(ChapterAward[5].ToString());
            columnValues.Add(ChapterAward[6].ToString());
            columnValues.Add(ChapterAward[7].ToString());
            columnValues.Add(ChapterAward[8].ToString());

            return string.Join("\t", columnValues);
        }
    }
}

#endif
