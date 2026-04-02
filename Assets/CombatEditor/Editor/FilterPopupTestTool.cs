/*******************************************************************
 * 功能：
 * 作者：罗翊坤
 * 时间：2026/4/2/周四 10:39:44
*******************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FilterPopupTestTool
{
    [MenuItem("工具/打开筛选弹窗（测试）")]
    public static void ShowTestFilterPopup()
    {
        // 测试数据（自带大量示例）
        List<string> testData = new List<string>()
        {
            "玩家角色", "敌人AI", "NPC对话", "任务系统", "背包系统",
            "技能系统", "音效管理", "UI管理", "场景加载", "存档系统",
            "新手引导", "战斗系统", "数值配置", "资源管理", "网络同步",
            "物体池", "碰撞检测", "输入管理", "相机控制", "天气系统",
            "道具掉落", "经验升级", "成就系统", "排行榜", "聊天系统"
        };

        // 打开弹窗
        FilterSelectPopup.OpenPopup(testData, (selectResult) =>
        {
            Debug.Log("你选中了：" + selectResult);
            EditorUtility.DisplayDialog("选择完成", "你选择了：\n" + selectResult, "确定");
        }, "筛选测试");
    }
}