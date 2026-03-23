#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amanda.EditorTable
{
    /**
     * 自动生成的配置表类：ActionSkillConfig
     * 生成时间：2026-03-19 09:34:48
     * 请勿手动修改！
     */
    [Serializable]
    public class ActionSkillConfig : IEditorTable
    {
        /**
         * "#ID(范围[28000000,29999999])"
         */
        public int ID;

        /**
         * $策划保留字段
         */
        public string Info;

        /**
         * 销毁时间毫秒
         */
        public int DestroyTime;

        /**
         * 是否被打断时停止特效音频
         */
        public bool IsBreakStopEffect;

        /**
         * 动画名
         */
        public string AniName;

        /**
         * 是否按动画时长百分比延迟播放特效
         */
        public bool IsAniPercentDelay;

        /**
         * 是否循环 默认非循环 0非循环 1表示循环
         */
        public int AniLoop;

        /**
         * 淡入时间毫秒 0表示没有动作融合 大于0表示按照指定的时间融合
         */
        public int FadeTime;

        /**
         * 淡出时间毫秒 从此时间开始淡出 并和其它动画融合 不配置或者0表示没有淡出时间 大于0表示指定淡出时间
         */
        public int FadeOutTime;

        /**
         * 是否隐藏武器 默认或0表示非隐藏 1隐藏
         */
        public int HideWeapon;

        /**
         * 是否将动画冻结在最后一帧 默认非冻结 0非冻结 1冻结
         */
        public int FreezeLastFrame;

        /**
         * 声音ID（没有声音必须要填成-1）
         */
        public List<int> SoundId = new List<int>();

        /**
         * "声音延迟播放时间（单位：毫秒）；如果动画百分比延迟，填[0,100]"
         */
        public List<int> SoundIdDelayTime = new List<int>();

        /**
         * 是否跟随动作自动停止声音 默认和0表示不停止 1表示停止
         */
        public int IsAutoStopSound;

        /**
         * 摄像机震动ID
         */
        public int CameraShakeId;

        /**
         * 震屏开始时刻，ms，-1表示无震屏(必须要填-1)
         */
        public int CameraShakeTime;

        /**
         * 相机震动效果索引
         */
        public int CameraShakeIndex;

        /**
         * 相机震动二次噪音效果索引
         */
        public int CameraShakeNoiseIndex;

        /**
         * 特效ID1
         */
        public int EffectId1;

        /**
         * "特效延迟播放时间1；如果动画百分比延迟，填[0,100]"
         */
        public int EffectDelayTime1;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval1;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind1;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon1;

        /**
         * 挂点
         */
        public string EffectMountPoint1;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX1;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY1;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ1;

        /**
         * 旋转角度
         */
        public float EffectRotationX1;

        /**
         * 旋转角度
         */
        public float EffectRotationY1;

        /**
         * 旋转角度
         */
        public float EffectRotationZ1;

        /**
         * 特效随动作结束1
         */
        public bool EffectEndByAct1;

        /**
         * 特效ID2
         */
        public int EffectId2;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime2;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval2;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind2;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon2;

        /**
         * 挂点
         */
        public string EffectMountPoint2;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX2;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY2;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ2;

        /**
         * 旋转角度
         */
        public float EffectRotationX2;

        /**
         * 旋转角度
         */
        public float EffectRotationY2;

        /**
         * 旋转角度
         */
        public float EffectRotationZ2;

        /**
         * 特效随动作结束2
         */
        public bool EffectEndByAct2;

        /**
         * 特效ID3
         */
        public int EffectId3;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime3;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval3;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind3;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon3;

        /**
         * 挂点
         */
        public string EffectMountPoint3;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX3;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY3;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ3;

        /**
         * 旋转角度
         */
        public float EffectRotationX3;

        /**
         * 旋转角度
         */
        public float EffectRotationY3;

        /**
         * 旋转角度
         */
        public float EffectRotationZ3;

        /**
         * 特效随动作结束3
         */
        public bool EffectEndByAct3;

        /**
         * 特效ID4
         */
        public int EffectId4;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime4;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval4;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind4;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon4;

        /**
         * 挂点
         */
        public string EffectMountPoint4;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX4;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY4;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ4;

        /**
         * 旋转角度
         */
        public float EffectRotationX4;

        /**
         * 旋转角度
         */
        public float EffectRotationY4;

        /**
         * 旋转角度
         */
        public float EffectRotationZ4;

        /**
         * 特效随动作结束4
         */
        public bool EffectEndByAct4;

        /**
         * 特效ID5
         */
        public int EffectId5;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime5;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval5;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind5;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon5;

        /**
         * 挂点
         */
        public string EffectMountPoint5;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX5;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY5;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ5;

        /**
         * 旋转角度
         */
        public float EffectRotationX5;

        /**
         * 旋转角度
         */
        public float EffectRotationY5;

        /**
         * 旋转角度
         */
        public float EffectRotationZ5;

        /**
         * 特效随动作结束5
         */
        public bool EffectEndByAct5;

        /**
         * 特效ID6
         */
        public int EffectId6;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime6;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval6;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind6;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon6;

        /**
         * 挂点
         */
        public string EffectMountPoint6;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX6;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY6;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ6;

        /**
         * 旋转角度
         */
        public float EffectRotationX6;

        /**
         * 旋转角度
         */
        public float EffectRotationY6;

        /**
         * 旋转角度
         */
        public float EffectRotationZ6;

        /**
         * 特效随动作结束6
         */
        public bool EffectEndByAct6;

        /**
         * 特效ID7
         */
        public int EffectId7;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime7;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval7;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind7;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon7;

        /**
         * 挂点
         */
        public string EffectMountPoint7;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX7;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY7;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ7;

        /**
         * 旋转角度
         */
        public float EffectRotationX7;

        /**
         * 旋转角度
         */
        public float EffectRotationY7;

        /**
         * 旋转角度
         */
        public float EffectRotationZ7;

        /**
         * 特效随动作结束7
         */
        public bool EffectEndByAct7;

        /**
         * 特效ID8
         */
        public int EffectId8;

        /**
         * 重复播放间隔 毫秒
         */
        public int RepeatInterval8;

        /**
         * 特效延迟播放时间
         */
        public int EffectDelayTime8;

        /**
         * 是否挂接到父物体上 默认和0表示非挂接 1表示挂接
         */
        public int EffectIsBind8;

        /**
         * 是否挂在武器上 默认和0表示挂在身体上 1挂在主武器 2挂在副武器上
         */
        public int EffectWeapon8;

        /**
         * 挂点
         */
        public string EffectMountPoint8;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetX8;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetY8;

        /**
         * 特效坐标偏移
         */
        public float EffectOffsetZ8;

        /**
         * 旋转角度
         */
        public float EffectRotationX8;

        /**
         * 旋转角度
         */
        public float EffectRotationY8;

        /**
         * 旋转角度
         */
        public float EffectRotationZ8;

        /**
         * 特效随动作结束8
         */
        public bool EffectEndByAct8;

        /**
         * 转换为配置表行文本（\t分隔）
         * 统一调用ConvertValue方法处理所有类型值转换
         * 空List（null/Count=0）→""
         * 实现IEditorTable接口的核心方法
         */
        public string ToDataLine()
        {
            List<string> columnValues = new List<string>();

            // ID
            columnValues.Add(TableToClassGenerator.ConvertValue(ID));

            // Info
            columnValues.Add(TableToClassGenerator.ConvertValue(Info));

            // DestroyTime
            columnValues.Add(TableToClassGenerator.ConvertValue(DestroyTime));

            // IsBreakStopEffect
            columnValues.Add(TableToClassGenerator.ConvertValue(IsBreakStopEffect));

            // AniName
            columnValues.Add(TableToClassGenerator.ConvertValue(AniName));

            // IsAniPercentDelay
            columnValues.Add(TableToClassGenerator.ConvertValue(IsAniPercentDelay));

            // AniLoop
            columnValues.Add(TableToClassGenerator.ConvertValue(AniLoop));

            // FadeTime
            columnValues.Add(TableToClassGenerator.ConvertValue(FadeTime));

            // FadeOutTime
            columnValues.Add(TableToClassGenerator.ConvertValue(FadeOutTime));

            // HideWeapon
            columnValues.Add(TableToClassGenerator.ConvertValue(HideWeapon));

            // FreezeLastFrame
            columnValues.Add(TableToClassGenerator.ConvertValue(FreezeLastFrame));

            // SoundId
            if (SoundId != null && SoundId.Count > 0)
            {
                List<string> valList = new List<string>();
                foreach (var val in SoundId)
                {
                    valList.Add(TableToClassGenerator.ConvertValue(val));
                }
                columnValues.Add(string.Join("|", valList));
            }
            else
            {
                columnValues.Add(string.Empty);
            }

            // SoundIdDelayTime
            if (SoundIdDelayTime != null && SoundIdDelayTime.Count > 0)
            {
                List<string> valList = new List<string>();
                foreach (var val in SoundIdDelayTime)
                {
                    valList.Add(TableToClassGenerator.ConvertValue(val));
                }
                columnValues.Add(string.Join("|", valList));
            }
            else
            {
                columnValues.Add(string.Empty);
            }

            // IsAutoStopSound
            columnValues.Add(TableToClassGenerator.ConvertValue(IsAutoStopSound));

            // CameraShakeId
            columnValues.Add(TableToClassGenerator.ConvertValue(CameraShakeId));

            // CameraShakeTime
            columnValues.Add(TableToClassGenerator.ConvertValue(CameraShakeTime));

            // CameraShakeIndex
            columnValues.Add(TableToClassGenerator.ConvertValue(CameraShakeIndex));

            // CameraShakeNoiseIndex
            columnValues.Add(TableToClassGenerator.ConvertValue(CameraShakeNoiseIndex));

            // EffectId1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId1));

            // EffectDelayTime1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime1));

            // RepeatInterval1
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval1));

            // EffectIsBind1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind1));

            // EffectWeapon1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon1));

            // EffectMountPoint1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint1));

            // EffectOffsetX1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX1));

            // EffectOffsetY1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY1));

            // EffectOffsetZ1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ1));

            // EffectRotationX1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX1));

            // EffectRotationY1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY1));

            // EffectRotationZ1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ1));

            // EffectEndByAct1
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct1));

            // EffectId2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId2));

            // EffectDelayTime2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime2));

            // RepeatInterval2
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval2));

            // EffectIsBind2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind2));

            // EffectWeapon2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon2));

            // EffectMountPoint2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint2));

            // EffectOffsetX2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX2));

            // EffectOffsetY2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY2));

            // EffectOffsetZ2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ2));

            // EffectRotationX2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX2));

            // EffectRotationY2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY2));

            // EffectRotationZ2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ2));

            // EffectEndByAct2
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct2));

            // EffectId3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId3));

            // EffectDelayTime3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime3));

            // RepeatInterval3
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval3));

            // EffectIsBind3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind3));

            // EffectWeapon3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon3));

            // EffectMountPoint3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint3));

            // EffectOffsetX3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX3));

            // EffectOffsetY3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY3));

            // EffectOffsetZ3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ3));

            // EffectRotationX3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX3));

            // EffectRotationY3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY3));

            // EffectRotationZ3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ3));

            // EffectEndByAct3
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct3));

            // EffectId4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId4));

            // EffectDelayTime4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime4));

            // RepeatInterval4
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval4));

            // EffectIsBind4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind4));

            // EffectWeapon4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon4));

            // EffectMountPoint4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint4));

            // EffectOffsetX4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX4));

            // EffectOffsetY4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY4));

            // EffectOffsetZ4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ4));

            // EffectRotationX4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX4));

            // EffectRotationY4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY4));

            // EffectRotationZ4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ4));

            // EffectEndByAct4
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct4));

            // EffectId5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId5));

            // EffectDelayTime5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime5));

            // RepeatInterval5
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval5));

            // EffectIsBind5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind5));

            // EffectWeapon5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon5));

            // EffectMountPoint5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint5));

            // EffectOffsetX5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX5));

            // EffectOffsetY5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY5));

            // EffectOffsetZ5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ5));

            // EffectRotationX5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX5));

            // EffectRotationY5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY5));

            // EffectRotationZ5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ5));

            // EffectEndByAct5
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct5));

            // EffectId6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId6));

            // EffectDelayTime6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime6));

            // RepeatInterval6
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval6));

            // EffectIsBind6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind6));

            // EffectWeapon6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon6));

            // EffectMountPoint6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint6));

            // EffectOffsetX6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX6));

            // EffectOffsetY6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY6));

            // EffectOffsetZ6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ6));

            // EffectRotationX6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX6));

            // EffectRotationY6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY6));

            // EffectRotationZ6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ6));

            // EffectEndByAct6
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct6));

            // EffectId7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId7));

            // EffectDelayTime7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime7));

            // RepeatInterval7
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval7));

            // EffectIsBind7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind7));

            // EffectWeapon7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon7));

            // EffectMountPoint7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint7));

            // EffectOffsetX7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX7));

            // EffectOffsetY7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY7));

            // EffectOffsetZ7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ7));

            // EffectRotationX7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX7));

            // EffectRotationY7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY7));

            // EffectRotationZ7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ7));

            // EffectEndByAct7
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct7));

            // EffectId8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectId8));

            // RepeatInterval8
            columnValues.Add(TableToClassGenerator.ConvertValue(RepeatInterval8));

            // EffectDelayTime8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectDelayTime8));

            // EffectIsBind8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectIsBind8));

            // EffectWeapon8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectWeapon8));

            // EffectMountPoint8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectMountPoint8));

            // EffectOffsetX8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetX8));

            // EffectOffsetY8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetY8));

            // EffectOffsetZ8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectOffsetZ8));

            // EffectRotationX8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationX8));

            // EffectRotationY8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationY8));

            // EffectRotationZ8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectRotationZ8));

            // EffectEndByAct8
            columnValues.Add(TableToClassGenerator.ConvertValue(EffectEndByAct8));

            return string.Join("\t", columnValues);
        }
    }
}

#endif
