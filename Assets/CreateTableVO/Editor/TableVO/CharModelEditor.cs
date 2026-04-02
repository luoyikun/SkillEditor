#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：CharModelEditor
     * 生成时间：2026-04-01 15:07:29
     * 请勿手动修改！
     */
    [Serializable]
    public partial class CharModelEditor : IEditorTable
    {
        /**
         * #ID
         */
        public int ID;

        /**
         * $Info
         */
        public string Info;

        /**
         * 分组
         */
        public int GroupId;

        /**
         * ModelResID
         */
        public int ModelResID;

        /**
         * 缩放比例
         */
        public float Scale;

        /**
         * 动作表
         */
        public int ActionConfigID;

        /**
         * 基础动画组
         */
        public int AnimationGroup;

        /**
         * 坐骑武器表中的列索引从0开始(只针对坐骑模型)
         */
        public int RideWeaponColIndex;

        /**
         * 碰撞盒大小
         */
        public List<float> BoxSize = new List<float>();

        /**
         * 碰撞盒中心
         */
        public List<float> BoxCenter = new List<float>();

        /**
         * 名字版显示位置（相对碰撞盒子中心位置偏移量）
         */
        public float NamePadOffset;

        /**
         * 质量倍数
         */
        public float MassRate;

        /**
         * 硬度值
         */
        public float hardness;

        /**
         * 摄像机焦点X
         */
        public const int CameraFocusMaxLength = 3;
        public float[] CameraFocus;

        /**
         * 坐骑模式摄像机焦点X
         */
        public const int RideCameraFocusMaxLength = 3;
        public float[] RideCameraFocus;

        /**
         * 音效包名
         */
        public string SoundBankName;

        /**
         * 音效材质枚举(1皮革 2金属 3石头 4木头)
         */
        public int SoundMaterialEnum;

        /**
         * LookIK配置
         */
        public int LookIK;

        /**
         * 战损时长
         */
        public List<int> BattleDamageTime = new List<int>();

        /**
         * 战损效果1
         */
        public List<int> BattleDamageEffect1 = new List<int>();

        /**
         * 战损效果2
         */
        public List<int> BattleDamageEffect2 = new List<int>();

        /**
         * 战损效果3
         */
        public List<int> BattleDamageEffect3 = new List<int>();

        public string ToDataLine()
        {
            List<string> columnValues = new List<string>();

            columnValues.Add(TableToClassGenerator.ConvertValue(ID));

            columnValues.Add(TableToClassGenerator.ConvertValue(Info));

            columnValues.Add(TableToClassGenerator.ConvertValue(GroupId));

            columnValues.Add(TableToClassGenerator.ConvertValue(ModelResID));

            columnValues.Add(TableToClassGenerator.ConvertValue(Scale));

            columnValues.Add(TableToClassGenerator.ConvertValue(ActionConfigID));

            columnValues.Add(TableToClassGenerator.ConvertValue(AnimationGroup));

            columnValues.Add(TableToClassGenerator.ConvertValue(RideWeaponColIndex));

            if (BoxSize != null && BoxSize.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BoxSize) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            if (BoxCenter != null && BoxCenter.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BoxCenter) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            columnValues.Add(TableToClassGenerator.ConvertValue(NamePadOffset));

            columnValues.Add(TableToClassGenerator.ConvertValue(MassRate));

            columnValues.Add(TableToClassGenerator.ConvertValue(hardness));

            for (int i = 0; i < CameraFocusMaxLength; i++)
            {
                if (CameraFocus != null && i < CameraFocus.Length)
                    columnValues.Add(TableToClassGenerator.ConvertValue(CameraFocus[i]));
                else
                    columnValues.Add(string.Empty);
            }

            for (int i = 0; i < RideCameraFocusMaxLength; i++)
            {
                if (RideCameraFocus != null && i < RideCameraFocus.Length)
                    columnValues.Add(TableToClassGenerator.ConvertValue(RideCameraFocus[i]));
                else
                    columnValues.Add(string.Empty);
            }

            columnValues.Add(TableToClassGenerator.ConvertValue(SoundBankName));

            columnValues.Add(TableToClassGenerator.ConvertValue(SoundMaterialEnum));

            columnValues.Add(TableToClassGenerator.ConvertValue(LookIK));

            if (BattleDamageTime != null && BattleDamageTime.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BattleDamageTime) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            if (BattleDamageEffect1 != null && BattleDamageEffect1.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BattleDamageEffect1) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            if (BattleDamageEffect2 != null && BattleDamageEffect2.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BattleDamageEffect2) tmp.Add(TableToClassGenerator.ConvertValue(v));
                columnValues.Add(string.Join("|", tmp));
            }
            else columnValues.Add(string.Empty);

            if (BattleDamageEffect3 != null && BattleDamageEffect3.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (var v in BattleDamageEffect3) tmp.Add(TableToClassGenerator.ConvertValue(v));
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

            // Info
            if (index < cells.Length)
                Info = cells[index++];
            else
                Info = default;

            // GroupId
            if (index < cells.Length)
                GroupId = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                GroupId = default;

            // ModelResID
            if (index < cells.Length)
                ModelResID = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                ModelResID = default;

            // Scale
            if (index < cells.Length)
                Scale = float.TryParse(cells[index++], out var val) ? val : 0f;
            else
                Scale = default;

            // ActionConfigID
            if (index < cells.Length)
                ActionConfigID = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                ActionConfigID = default;

            // AnimationGroup
            if (index < cells.Length)
                AnimationGroup = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                AnimationGroup = default;

            // RideWeaponColIndex
            if (index < cells.Length)
                RideWeaponColIndex = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                RideWeaponColIndex = default;

            // BoxSize 列表（每次加载清空重建）
            BoxSize = new List<float>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BoxSize.Add(float.TryParse(p.Trim(), out var val) ? val : 0f);
                }
            }
            else index++;

            // BoxCenter 列表（每次加载清空重建）
            BoxCenter = new List<float>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BoxCenter.Add(float.TryParse(p.Trim(), out var val) ? val : 0f);
                }
            }
            else index++;

            // NamePadOffset
            if (index < cells.Length)
                NamePadOffset = float.TryParse(cells[index++], out var val) ? val : 0f;
            else
                NamePadOffset = default;

            // MassRate
            if (index < cells.Length)
                MassRate = float.TryParse(cells[index++], out var val) ? val : 0f;
            else
                MassRate = default;

            // hardness
            if (index < cells.Length)
                hardness = float.TryParse(cells[index++], out var val) ? val : 0f;
            else
                hardness = default;

            // CameraFocus 动态数组（自动统计非空）
            int validCount_CameraFocus = 0;
            int startIndex_CameraFocus = index;

            for (int i = 0; i < CameraFocusMaxLength && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    validCount_CameraFocus++;
                index++;
            }

            CameraFocus = new float[validCount_CameraFocus];
            index = startIndex_CameraFocus;

            for (int i = 0; i < validCount_CameraFocus && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    CameraFocus[i] = float.TryParse(cells[index], out var val) ? val : 0f;
                index++;
            }

            // RideCameraFocus 动态数组（自动统计非空）
            int validCount_RideCameraFocus = 0;
            int startIndex_RideCameraFocus = index;

            for (int i = 0; i < RideCameraFocusMaxLength && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    validCount_RideCameraFocus++;
                index++;
            }

            RideCameraFocus = new float[validCount_RideCameraFocus];
            index = startIndex_RideCameraFocus;

            for (int i = 0; i < validCount_RideCameraFocus && index < cells.Length; i++)
            {
                if (!string.IsNullOrEmpty(cells[index]))
                    RideCameraFocus[i] = float.TryParse(cells[index], out var val) ? val : 0f;
                index++;
            }

            // SoundBankName
            if (index < cells.Length)
                SoundBankName = cells[index++];
            else
                SoundBankName = default;

            // SoundMaterialEnum
            if (index < cells.Length)
                SoundMaterialEnum = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                SoundMaterialEnum = default;

            // LookIK
            if (index < cells.Length)
                LookIK = int.TryParse(cells[index++], out var val) ? val : 0;
            else
                LookIK = default;

            // BattleDamageTime 列表（每次加载清空重建）
            BattleDamageTime = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BattleDamageTime.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

            // BattleDamageEffect1 列表（每次加载清空重建）
            BattleDamageEffect1 = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BattleDamageEffect1.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

            // BattleDamageEffect2 列表（每次加载清空重建）
            BattleDamageEffect2 = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BattleDamageEffect2.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

            // BattleDamageEffect3 列表（每次加载清空重建）
            BattleDamageEffect3 = new List<int>();
            if (index < cells.Length && !string.IsNullOrEmpty(cells[index]))
            {
                string[] parts = cells[index++].Split('|');
                foreach (var p in parts)
                {
                    BattleDamageEffect3.Add(int.TryParse(p.Trim(), out var val) ? val : 0);
                }
            }
            else index++;

        }
    }
}
#endif
