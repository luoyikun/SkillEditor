using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// 简化的技能数据类：纯数据驱动，每行仅保留核心字段（1个特效文件）
[System.Serializable]
public class SkillEffectData
{
    // 可根据你的需求调整字段，保持"一行一个特效文件"的核心结构
    public string skillName = "新技能";    // 技能名称
    public int effectId = 0;               // 特效ID
    public GameObject effectPrefab;        // 单个特效文件（核心字段）
}

// 纯数据驱动的编辑器窗口（无SerializedObject，无.so依赖）
public class SimpleSkillEditor : EditorWindow
{
    private Vector2 scrollPos;             // 滚动条位置
    private List<SkillEffectData> skillList = new List<SkillEffectData>(); // 纯数据列表
    private int indexToDelete = -1;        // 延迟删除标记（避免布局错乱）

    [MenuItem("Tools/纯数据特效编辑器")]
    static void OpenWindow()
    {
        SimpleSkillEditor window = GetWindow<SimpleSkillEditor>();
        window.titleContent = new GUIContent("技能特效列表");
        window.minSize = new Vector2(450, 300); // 适配单行显示
        window.Show();
    }

    // 初始化示例数据（纯数据填充，无任何序列化依赖）
    void OnEnable()
    {
        if (skillList.Count == 0)
        {
            skillList.Add(new SkillEffectData { skillName = "剑技冲锋", effectId = 1001 });
            skillList.Add(new SkillEffectData { skillName = "火球术", effectId = 1002 });
        }
    }

    void OnGUI()
    {
        // 1. 延迟删除：布局绘制完成后再修改列表（核心避坑点）
        if (indexToDelete != -1 && indexToDelete < skillList.Count)
        {
            skillList.RemoveAt(indexToDelete);
            indexToDelete = -1;
        }

        // 2. 顶部操作栏（添加按钮）
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("添加技能行", GUILayout.Width(120)))
            {
                skillList.Add(new SkillEffectData()); // 纯数据添加，无序列化
            }
            GUILayout.FlexibleSpace(); // 右对齐占位
        }

        EditorGUILayout.Space(8);

        // 3. 带滚动条的列表（纯数据遍历）
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        {
            // 遍历列表副本，避免布局中修改原列表
            List<SkillEffectData> tempList = new List<SkillEffectData>(skillList);
            for (int i = 0; i < tempList.Count; i++)
            {
                int realIndex = skillList.IndexOf(tempList[i]);
                if (realIndex == -1) continue;

                // 绘制单行：仅包含技能名、特效ID、单个特效文件
                DrawSingleEffectRow(tempList[i], realIndex);
                EditorGUILayout.Separator(); // 行分隔
            }
        }
        EditorGUILayout.EndScrollView();
    }

    // 核心：绘制单行（纯数据操作，无.so文件）
    private void DrawSingleEffectRow(SkillEffectData data, int index)
    {
        // 行标题
        EditorGUILayout.LabelField($"技能 {index + 1}", EditorStyles.boldLabel);

        // 第一行：技能名 + 特效ID（横向布局）
        using (new EditorGUILayout.HorizontalScope())
        {
            data.skillName = EditorGUILayout.TextField("技能名称", data.skillName, GUILayout.Width(200));
            data.effectId = EditorGUILayout.IntField("特效ID", data.effectId, GUILayout.Width(100));
        }

        // 第二行：单个特效文件（核心需求：一行仅1个特效文件）
        data.effectPrefab = (GameObject)EditorGUILayout.ObjectField(
            "特效预制体",    // 标签
            data.effectPrefab, // 当前值
            typeof(GameObject), // 仅接受GameObject
            true              // 允许拖拽场景/资源文件
        );

        // 第三行：删除按钮（右对齐）
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("删除此行", GUILayout.Width(100)))
            {
                indexToDelete = index; // 标记删除，延迟执行
            }
        }
    }

    // 可选：导出/导入数据（纯数据驱动的扩展）
    [MenuItem("Tools/导出特效数据")]
    static void ExportEffectData()
    {
        string json = JsonUtility.ToJson(new SkillDataWrapper { skillList = GetWindow<SimpleSkillEditor>().skillList }, true);
        string path = EditorUtility.SaveFilePanel("保存特效数据", Application.dataPath, "SkillEffectData", "json");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }
    }

    // 数据包装类（用于JSON序列化List）
    [System.Serializable]
    private class SkillDataWrapper
    {
        public List<SkillEffectData> skillList;
    }
}