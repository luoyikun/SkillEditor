using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatEditor : EditorWindow // 注意：继承自 EditorWindow 而非 Editor
{
    // 这个变量将存储用户通过拖拽或选择器赋值的物体
    private GameObject targetObject = null; // 改为 private 更符合封装规范

    [MenuItem("Tools/我的简单对象窗口")]
    static void Init()
    {
        CombatEditor window = GetWindow<CombatEditor>();
        window.titleContent = new GUIContent("对象字段示例");
        window.minSize = new Vector2(300, 100); // 设置窗口最小尺寸，避免缩太小看不到内容
        window.Show();
    }

    void OnGUI()
    {
        // 改用 Unity 推荐的自动布局方式（代替固定 Rect），解决拖拽交互问题
        EditorGUILayout.Space(10); // 顶部留空，优化视觉

        // 核心修复：使用 EditorGUILayout.ObjectField 自动布局
        // 自动布局会适配窗口大小，且交互逻辑更稳定
        targetObject = (GameObject)EditorGUILayout.ObjectField(
            "技能目标物体",  // 标签文字
            targetObject,    // 当前引用的对象
            typeof(GameObject), // 允许的类型
            true             // 是否允许拖入场景中的对象
        );

        // 显示选中对象的名称
        if (targetObject != null)
        {
            EditorGUILayout.Space(5); // 增加间距
            EditorGUILayout.LabelField("已选择: " + targetObject.name);
        }
    }
}