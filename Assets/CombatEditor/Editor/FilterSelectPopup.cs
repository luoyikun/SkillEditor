using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity编辑器 模糊匹配筛选选择弹窗（仿原生样式：紧凑行高 + 悬浮高亮，无残留）
/// </summary>
public class FilterSelectPopup : EditorWindow
{
    private List<string> _sourceList;
    private List<string> _filteredList;
    private string _searchText = string.Empty;
    private Vector2 _scrollPos;
    private Action<string> _onSelectCallback;
    private int _selectedIndex = -1;   // 仅用于“确定”按钮（可选）

    private static readonly Color _hoverColor = new Color(0.3f, 0.5f, 0.8f, 0.3f);
    private static readonly Color _selectedColor = new Color(0.24f, 0.48f, 0.90f, 0.6f);

    public static void OpenPopup(List<string> sourceList, Action<string> onSelectCallback, string windowTitle = "筛选选择")
    {
        var window = CreateInstance<FilterSelectPopup>();
        window.titleContent = new GUIContent(windowTitle);
        window._sourceList = sourceList;
        window._filteredList = new List<string>(sourceList);
        window._onSelectCallback = onSelectCallback;
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 600);

        window.CenterWindow();
        window.ShowUtility();
    }

    private void CenterWindow()
    {
        Vector2 size = minSize;
        Rect mainWindowRect = EditorGUIUtility.GetMainWindowPosition();
        float x = mainWindowRect.x + (mainWindowRect.width - size.x) / 2;
        float y = mainWindowRect.y + (mainWindowRect.height - size.y) / 2;
        position = new Rect(x, y, size.x, size.y);
    }

    private void OnGUI()
    {
        // 搜索栏
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("搜索:", EditorStyles.miniLabel, GUILayout.Width(35));
        string newSearch = EditorGUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);
        if (newSearch != _searchText)
        {
            _searchText = newSearch;
            FilterList();
        }
        EditorGUILayout.EndHorizontal();

        // 数量提示
        EditorGUILayout.LabelField($"结果：{_filteredList.Count}", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(16));

        // 滚动列表
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));

        for (int i = 0; i < _filteredList.Count; i++)
        {
            DrawItem(i, _filteredList[i]);
        }

        EditorGUILayout.EndScrollView();

        if (_filteredList.Count == 0)
            EditorGUILayout.HelpBox("无匹配项", MessageType.Info);

        // 底部按钮（取消/确定）
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("取消", EditorStyles.toolbarButton, GUILayout.Width(60))) Close();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawItem(int index, string text)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 22); // 紧凑行高
        Event evt = Event.current;

        // 实时悬浮判断（无残留）
        bool isHover = rect.Contains(evt.mousePosition);
        bool isSelected = index == _selectedIndex;

        // 绘制背景高亮
        if (isSelected)
            EditorGUI.DrawRect(rect, _selectedColor);
        else if (isHover)
            EditorGUI.DrawRect(rect, _hoverColor);

        // 点击立即选择并关闭（行为与 CustomAnimationClipSelector 一致）
        if (evt.type == EventType.MouseDown && isHover)
        {
            _selectedIndex = index;
            _onSelectCallback?.Invoke(_filteredList[index]);
            Close();
            evt.Use();
        }

        // 绘制文字
        GUI.color = isSelected ? Color.white : GUI.contentColor;
        Rect labelRect = rect;
        labelRect.xMin += 4;
        EditorGUI.LabelField(labelRect, text);
        GUI.color = Color.white;
    }

    private void FilterList()
    {
        if (string.IsNullOrEmpty(_searchText))
            _filteredList = new List<string>(_sourceList);
        else
            _filteredList = _sourceList.Where(s => s.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        _selectedIndex = -1;
        // 注意：hover 不再需要重置，因为每帧实时判断
    }

    private void ConfirmSelect()
    {
        if (_selectedIndex >= 0 && _selectedIndex < _filteredList.Count)
            _onSelectCallback?.Invoke(_filteredList[_selectedIndex]);
        Close();
    }
}