using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 自动生成的配置表类：ActionConfig
 * 生成时间：2026-03-18 09:52:37
 * 请勿手动修改！
 */
[Serializable]
public class ActionConfig
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
     * 数组：按索引拆分 | List：|分隔 | Float：调用工具类格式化
     */
    public string ToDataLine()
    {
        List<string> columnValues = new List<string>();

        // ID
        columnValues.Add(ID.ToString());

        // Info
        columnValues.Add(Info.ToString());

        // DestroyTime
        columnValues.Add(DestroyTime.ToString());

        // IsBreakStopEffect
        columnValues.Add(IsBreakStopEffect.ToString());

        // AniName
        columnValues.Add(AniName.ToString());

        // IsAniPercentDelay
        columnValues.Add(IsAniPercentDelay.ToString());

        // AniLoop
        columnValues.Add(AniLoop.ToString());

        // FadeTime
        columnValues.Add(FadeTime.ToString());

        // FadeOutTime
        columnValues.Add(FadeOutTime.ToString());

        // HideWeapon
        columnValues.Add(HideWeapon.ToString());

        // FreezeLastFrame
        columnValues.Add(FreezeLastFrame.ToString());

        // SoundId
        if (SoundId != null)
        {
            columnValues.Add(string.Join("|", SoundId));
        }
        else
        {
            columnValues.Add(string.Empty);
        }

        // SoundIdDelayTime
        if (SoundIdDelayTime != null)
        {
            columnValues.Add(string.Join("|", SoundIdDelayTime));
        }
        else
        {
            columnValues.Add(string.Empty);
        }

        // IsAutoStopSound
        columnValues.Add(IsAutoStopSound.ToString());

        // CameraShakeId
        columnValues.Add(CameraShakeId.ToString());

        // CameraShakeTime
        columnValues.Add(CameraShakeTime.ToString());

        // CameraShakeIndex
        columnValues.Add(CameraShakeIndex.ToString());

        // CameraShakeNoiseIndex
        columnValues.Add(CameraShakeNoiseIndex.ToString());

        // EffectId1
        columnValues.Add(EffectId1.ToString());

        // EffectDelayTime1
        columnValues.Add(EffectDelayTime1.ToString());

        // RepeatInterval1
        columnValues.Add(RepeatInterval1.ToString());

        // EffectIsBind1
        columnValues.Add(EffectIsBind1.ToString());

        // EffectWeapon1
        columnValues.Add(EffectWeapon1.ToString());

        // EffectMountPoint1
        columnValues.Add(EffectMountPoint1.ToString());

        // EffectOffsetX1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX1));

        // EffectOffsetY1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY1));

        // EffectOffsetZ1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ1));

        // EffectRotationX1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX1));

        // EffectRotationY1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY1));

        // EffectRotationZ1
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ1));

        // EffectEndByAct1
        columnValues.Add(EffectEndByAct1.ToString());

        // EffectId2
        columnValues.Add(EffectId2.ToString());

        // EffectDelayTime2
        columnValues.Add(EffectDelayTime2.ToString());

        // RepeatInterval2
        columnValues.Add(RepeatInterval2.ToString());

        // EffectIsBind2
        columnValues.Add(EffectIsBind2.ToString());

        // EffectWeapon2
        columnValues.Add(EffectWeapon2.ToString());

        // EffectMountPoint2
        columnValues.Add(EffectMountPoint2.ToString());

        // EffectOffsetX2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX2));

        // EffectOffsetY2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY2));

        // EffectOffsetZ2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ2));

        // EffectRotationX2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX2));

        // EffectRotationY2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY2));

        // EffectRotationZ2
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ2));

        // EffectEndByAct2
        columnValues.Add(EffectEndByAct2.ToString());

        // EffectId3
        columnValues.Add(EffectId3.ToString());

        // EffectDelayTime3
        columnValues.Add(EffectDelayTime3.ToString());

        // RepeatInterval3
        columnValues.Add(RepeatInterval3.ToString());

        // EffectIsBind3
        columnValues.Add(EffectIsBind3.ToString());

        // EffectWeapon3
        columnValues.Add(EffectWeapon3.ToString());

        // EffectMountPoint3
        columnValues.Add(EffectMountPoint3.ToString());

        // EffectOffsetX3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX3));

        // EffectOffsetY3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY3));

        // EffectOffsetZ3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ3));

        // EffectRotationX3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX3));

        // EffectRotationY3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY3));

        // EffectRotationZ3
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ3));

        // EffectEndByAct3
        columnValues.Add(EffectEndByAct3.ToString());

        // EffectId4
        columnValues.Add(EffectId4.ToString());

        // EffectDelayTime4
        columnValues.Add(EffectDelayTime4.ToString());

        // RepeatInterval4
        columnValues.Add(RepeatInterval4.ToString());

        // EffectIsBind4
        columnValues.Add(EffectIsBind4.ToString());

        // EffectWeapon4
        columnValues.Add(EffectWeapon4.ToString());

        // EffectMountPoint4
        columnValues.Add(EffectMountPoint4.ToString());

        // EffectOffsetX4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX4));

        // EffectOffsetY4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY4));

        // EffectOffsetZ4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ4));

        // EffectRotationX4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX4));

        // EffectRotationY4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY4));

        // EffectRotationZ4
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ4));

        // EffectEndByAct4
        columnValues.Add(EffectEndByAct4.ToString());

        // EffectId5
        columnValues.Add(EffectId5.ToString());

        // EffectDelayTime5
        columnValues.Add(EffectDelayTime5.ToString());

        // RepeatInterval5
        columnValues.Add(RepeatInterval5.ToString());

        // EffectIsBind5
        columnValues.Add(EffectIsBind5.ToString());

        // EffectWeapon5
        columnValues.Add(EffectWeapon5.ToString());

        // EffectMountPoint5
        columnValues.Add(EffectMountPoint5.ToString());

        // EffectOffsetX5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX5));

        // EffectOffsetY5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY5));

        // EffectOffsetZ5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ5));

        // EffectRotationX5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX5));

        // EffectRotationY5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY5));

        // EffectRotationZ5
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ5));

        // EffectEndByAct5
        columnValues.Add(EffectEndByAct5.ToString());

        // EffectId6
        columnValues.Add(EffectId6.ToString());

        // EffectDelayTime6
        columnValues.Add(EffectDelayTime6.ToString());

        // RepeatInterval6
        columnValues.Add(RepeatInterval6.ToString());

        // EffectIsBind6
        columnValues.Add(EffectIsBind6.ToString());

        // EffectWeapon6
        columnValues.Add(EffectWeapon6.ToString());

        // EffectMountPoint6
        columnValues.Add(EffectMountPoint6.ToString());

        // EffectOffsetX6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX6));

        // EffectOffsetY6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY6));

        // EffectOffsetZ6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ6));

        // EffectRotationX6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX6));

        // EffectRotationY6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY6));

        // EffectRotationZ6
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ6));

        // EffectEndByAct6
        columnValues.Add(EffectEndByAct6.ToString());

        // EffectId7
        columnValues.Add(EffectId7.ToString());

        // EffectDelayTime7
        columnValues.Add(EffectDelayTime7.ToString());

        // RepeatInterval7
        columnValues.Add(RepeatInterval7.ToString());

        // EffectIsBind7
        columnValues.Add(EffectIsBind7.ToString());

        // EffectWeapon7
        columnValues.Add(EffectWeapon7.ToString());

        // EffectMountPoint7
        columnValues.Add(EffectMountPoint7.ToString());

        // EffectOffsetX7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX7));

        // EffectOffsetY7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY7));

        // EffectOffsetZ7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ7));

        // EffectRotationX7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX7));

        // EffectRotationY7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY7));

        // EffectRotationZ7
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ7));

        // EffectEndByAct7
        columnValues.Add(EffectEndByAct7.ToString());

        // EffectId8
        columnValues.Add(EffectId8.ToString());

        // RepeatInterval8
        columnValues.Add(RepeatInterval8.ToString());

        // EffectDelayTime8
        columnValues.Add(EffectDelayTime8.ToString());

        // EffectIsBind8
        columnValues.Add(EffectIsBind8.ToString());

        // EffectWeapon8
        columnValues.Add(EffectWeapon8.ToString());

        // EffectMountPoint8
        columnValues.Add(EffectMountPoint8.ToString());

        // EffectOffsetX8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetX8));

        // EffectOffsetY8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetY8));

        // EffectOffsetZ8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectOffsetZ8));

        // EffectRotationX8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationX8));

        // EffectRotationY8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationY8));

        // EffectRotationZ8
        columnValues.Add(TableToClassGenerator.FormatFloat(EffectRotationZ8));

        // EffectEndByAct8
        columnValues.Add(EffectEndByAct8.ToString());

        return string.Join("\t", columnValues);
    }
}
