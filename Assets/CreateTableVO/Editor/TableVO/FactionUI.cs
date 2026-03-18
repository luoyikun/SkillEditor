using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 自动生成的配置表类：FactionUI
 * 生成时间：2026-03-18 09:51:58
 * 请勿手动修改！
 */
[Serializable]
public class FactionUI
{
    /**
     * #����ID
     */
    public int ID;

    /**
     * $����
     */
    public string desc;

    /**
     * ����
     */
    public int[] Name = new int[2];

    /**
     * ͼ��
     */
    public int Icon;

    /**
     * ������Ŀ��ͼ��
     */
    public int TargetIcon;

    /**
     * ���
     */
    public int DesInt;

    /**
     * ���tips���
     */
    public int TipsDesInt;

    /**
     * 转换为配置表行文本（\t分隔）
     * 数组：按索引拆分 | List：|分隔 | Float：调用工具类格式化
     */
    public string ToDataLine()
    {
        List<string> columnValues = new List<string>();

        // ID
        columnValues.Add(ID.ToString());

        // desc
        columnValues.Add(desc.ToString());

        // Name
        columnValues.Add(Name[0].ToString());
        columnValues.Add(Name[1].ToString());

        // Icon
        columnValues.Add(Icon.ToString());

        // TargetIcon
        columnValues.Add(TargetIcon.ToString());

        // DesInt
        columnValues.Add(DesInt.ToString());

        // TipsDesInt
        columnValues.Add(TipsDesInt.ToString());

        return string.Join("\t", columnValues);
    }
}
