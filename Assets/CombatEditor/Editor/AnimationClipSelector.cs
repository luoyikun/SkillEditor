using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CustomAnimationClipSelector : EditorWindow
{
    private static CustomAnimationClipSelector _window;
    private Vector2 _scrollPosition;
    private List<AnimationClip> _allowedClips;
    private System.Action<AnimationClip> _callback;
    private string _searchFilter = "";
    private List<AnimationClip> _filteredClips;
    private AnimationClip _selectedClip;

    public static void Show(List<string> folderPaths, System.Action<AnimationClip> callback)
    {
        // 遍历所有指定文件夹（包括子文件夹）获取.anim文件
        List<AnimationClip> clips = new List<AnimationClip>();

        foreach (string folderPath in folderPaths)
        {
            // 确保文件夹路径格式正确
            string assetPath = folderPath;
            if (!assetPath.StartsWith("Assets/"))
            {
                assetPath = "Assets/" + assetPath;
            }

            // 方法1：使用AssetDatabase.FindAssets查找所有.anim文件
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { assetPath });
  
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // 只添加.anim文件，排除FBX中的嵌入动画
                if (path.EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                {
                    AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    if (clip != null)
                    {
                        clips.Add(clip);
                        Debug.Log($"找到.anim文件: {clip.name} 路径: {path}");
                    }
                }
            }

        }

        // 去重并排序
        clips = clips.Distinct().OrderBy(c => c.name).ToList();

        Debug.Log($"找到 {clips.Count} 个.anim文件");

        if (clips.Count == 0)
        {
            Debug.LogWarning($"在指定文件夹 {string.Join(", ", folderPaths)} 中没有找到任何.anim文件！");
        }

        // 调用原有的 Show 方法
        Show(clips, callback);
    }

    public static void Show(List<AnimationClip> clips, System.Action<AnimationClip> callback)
    {
        if (_window == null)
        {
            _window = CreateInstance<CustomAnimationClipSelector>();
        }

        _window._allowedClips = clips;
        _window._filteredClips = new List<AnimationClip>(clips);
        _window._callback = callback;
        _window._searchFilter = "";
        _window._selectedClip = null;

        _window.titleContent = new GUIContent("Select Animation Clip");
        _window.minSize = new Vector2(350, 400);
        _window.ShowUtility();
    }

    private void OnGUI()
    {
        DrawSearchBar();
        DrawClipList();
        DrawBottomBar();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUI.SetNextControlName("SearchField");
        string newSearch = GUILayout.TextField(_searchFilter, EditorStyles.toolbarSearchField);
        if (newSearch != _searchFilter)
        {
            _searchFilter = newSearch;
            FilterClips();
        }

        if (GUILayout.Button("清除", EditorStyles.toolbarButton, GUILayout.Width(40)))
        {
            _searchFilter = "";
            FilterClips();
            GUI.FocusControl("SearchField");
        }

        GUILayout.EndHorizontal();

        // 自动聚焦
        if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(_searchFilter))
        {
            EditorGUI.FocusTextInControl("SearchField");
        }
    }

    private void FilterClips()
    {
        if (string.IsNullOrEmpty(_searchFilter))
        {
            _filteredClips = new List<AnimationClip>(_allowedClips);
        }
        else
        {
            _filteredClips = _allowedClips.FindAll(
                clip => clip.name.IndexOf(_searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0
            );
        }
    }

    private void DrawClipList()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Available Animation Clips", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition,
            GUILayout.ExpandHeight(true));

        // None选项
        DrawClipItem("None", null);

        // 分隔线
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // 剪辑列表
        foreach (var clip in _filteredClips)
        {
            DrawClipItem(clip.name, clip);
        }

        if (_filteredClips.Count == 0)
        {
            EditorGUILayout.HelpBox("没有匹配的动画剪辑", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawClipItem(string displayName, AnimationClip clip)
    {
        Rect rect = EditorGUILayout.GetControlRect();

        // 悬停效果
        if (rect.Contains(Event.current.mousePosition))
        {
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.5f, 0.8f, 0.3f));
        }

        // 选中效果
        if (clip == _selectedClip)
        {
            EditorGUI.DrawRect(rect, new Color(0.24f, 0.48f, 0.90f, 0.6f));
        }

        // 点击处理
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            _selectedClip = clip;
            _callback?.Invoke(clip);
            _window.Close();
            Event.current.Use();
        }

        // 显示图标和名称
        if (clip != null)
        {
            GUIContent content = new GUIContent(displayName, AssetPreview.GetMiniThumbnail(clip));
            EditorGUI.LabelField(rect, content);

            // 在右侧显示文件夹路径（小字）
            string path = AssetDatabase.GetAssetPath(clip);
            string folderName = Path.GetDirectoryName(path).Replace("Assets/", "");
            Rect pathRect = new Rect(rect.x + 200, rect.y, rect.width - 200, rect.height);
            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.alignment = TextAnchor.MiddleRight;
            pathStyle.normal.textColor = Color.gray;
            EditorGUI.LabelField(pathRect, folderName, pathStyle);
        }
        else
        {
            EditorGUI.LabelField(rect, displayName);
        }
    }

    private void DrawBottomBar()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (_selectedClip != null)
        {
            EditorGUILayout.LabelField("Selected: " + _selectedClip.name);
        }
        else
        {
            EditorGUILayout.LabelField("Selected: None");
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
            _window.Close();
        }

        EditorGUILayout.EndHorizontal();
    }
}

// 使用示例
public class TestSelector : MonoBehaviour
{
    [MenuItem("Tools/查找.anim文件")]
    static void FindAnimFiles()
    {
        // 指定要遍历的文件夹列表
        List<string> folderPaths = new List<string>
        {
            "Assets/CombatEditor/Animations",
            "Assets/",
            // 可以添加更多文件夹
            // "Assets/Animations",
        };

        CustomAnimationClipSelector.Show(folderPaths, (selectedClip) =>
        {
            if (selectedClip != null)
            {
                Debug.Log($"选择了: {selectedClip.name}");
                Debug.Log($"路径: {AssetDatabase.GetAssetPath(selectedClip)}");
            }
            else
            {
                Debug.Log("选择了: None");
            }
        });
    }

    [MenuItem("Tools/调试显示所有.anim文件")]
    static void DebugShowAllAnimFiles()
    {
        string folderPath = "Assets/CombatEditor/Animations";

        // 使用System.IO直接查找
        string fullPath = System.IO.Path.Combine(Application.dataPath, "..", folderPath);
        string[] animFiles = System.IO.Directory.GetFiles(fullPath, "*.anim", System.IO.SearchOption.AllDirectories);

        Debug.Log($"找到 {animFiles.Length} 个.anim文件：");
        foreach (string file in animFiles)
        {
            Debug.Log($"  - {file}");
        }
    }
}