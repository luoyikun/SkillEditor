using Amanda.EditorTable;
using System.IO;
using UnityEditor;

using UnityEngine;

/// <summary>
/// 表转类编辑器窗口（选择文件夹 + 自动拼文件名）
/// </summary>
public class TableToClassWindow : EditorWindow
{
    private string tablePath = "";
    private string className = "";
    private string outputFolder = ""; // 改为文件夹路径

    [MenuItem("Tools/表转类工具")]
    public static void ShowWindow()
    {
        GetWindow<TableToClassWindow>("表转类工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("选择配置表（TXT）", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        tablePath = EditorGUILayout.TextField("表路径", tablePath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            tablePath = EditorUtility.OpenFilePanel("选择配置表", Application.dataPath, "txt");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("生成配置", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField("类名", className);

        EditorGUILayout.BeginHorizontal();
        outputFolder = EditorGUILayout.TextField("输出文件夹", outputFolder);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            // 改为选择文件夹
            outputFolder = EditorUtility.SaveFolderPanel("选择输出文件夹", Application.dataPath, "");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        if (GUILayout.Button("生成数据类", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(tablePath) || !File.Exists(tablePath))
            {
                EditorUtility.DisplayDialog("错误", "请选择有效的配置表文件！", "确定");
                return;
            }
            if (string.IsNullOrEmpty(className))
            {
                EditorUtility.DisplayDialog("错误", "请输入类名！", "确定");
                return;
            }
            if (string.IsNullOrEmpty(outputFolder) || !Directory.Exists(outputFolder))
            {
                EditorUtility.DisplayDialog("错误", "请选择有效的输出文件夹！", "确定");
                return;
            }

            // 自动拼接输出文件路径：文件夹 + 类名 + .cs
            string outputPath = Path.Combine(outputFolder, $"{className}.cs");

            // 调用生成逻辑
            bool success = TableToClassGenerator.GenerateClassFromTable(tablePath, outputPath, className);
            if (success)
            {
                EditorUtility.DisplayDialog("成功", $"类文件生成成功：\n{outputPath}", "确定");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("失败", "生成类文件失败，请查看控制台日志！", "确定");
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("规则说明：", EditorStyles.boldLabel);
        GUILayout.Label("1. 第1行：类型（INT/STRING/[INT]等）");
        GUILayout.Label("2. 第2行：中文注释");
        GUILayout.Label("3. 第3行：字段名");
    }
}