using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SingleEffectData
{
    public string name = "特效";
    public int startTimeMs;          // 特效开始时间（毫秒）
    public readonly int fixedDurationMs = 500; // 特效固定时长（不可修改）
    public GameObject effectPrefab;  // 特效预制体
}

[System.Serializable]
public class SingleEffectTrack
{
    public string trackName = "特效轨道";
    public SingleEffectData effect = new SingleEffectData();
}

// 最终版：修复Layout报错 + 绿块拖动 + 游标交互
public class FinalTimelineEditor : EditorWindow
{
    // 核心时间数据（毫秒单位）
    private int animationTotalMs = 1000;  // 动画总时长（默认1000ms）
    private List<SingleEffectTrack> effectTracks = new List<SingleEffectTrack>();

    // 播放控制
    private float currentTimeMs = 0f;
    private bool isPlaying = false;
    private double lastEditorTime = 0;

    // 视图缩放与滚动
    private Vector2 scrollPos;
    private float timeScale = 0.1f;       // 像素/毫秒（缩放比例）

    // 界面常量
    private const float trackHeight = 30f;
    private const float trackPadding = 5f;
    private const float labelWidth = 100f;
    private const float timeAxisHeight = 30f;

    // 刻度精度
    private const int smallSnapMs = 10;   // 小刻度：10ms
    private const int bigSnapMs = 500;    // 大刻度：500ms

    // 拖动状态
    private bool isDraggingTimeline = false;
    private bool isDraggingPointer = false;
    private SingleEffectData draggedEffect = null;

    private Vector2 dragStartMousePos;
    private Vector2 dragStartScrollPos;
    private int dragStartStartTimeMs;

    [MenuItem("Tools/最终版时间轴编辑器")]
    static void OpenWindow()
    {
        var window = GetWindow<FinalTimelineEditor>();
        window.titleContent = new GUIContent("最终版时间轴编辑器");
        window.minSize = new Vector2(800, 400);
        window.Show();
    }

    void OnEnable()
    {
        // 初始化默认轨道
        if (effectTracks.Count == 0)
        {
            effectTracks.Add(new SingleEffectTrack { trackName = "特效1", effect = new SingleEffectData { startTimeMs = 0 } });
            effectTracks.Add(new SingleEffectTrack { trackName = "特效2", effect = new SingleEffectData { startTimeMs = 600 } });
            effectTracks.Add(new SingleEffectTrack { trackName = "特效3", effect = new SingleEffectData { startTimeMs = 100 } });
        }
    }

    void OnGUI()
    {
        // 1. 播放控制栏
        DrawPlaybackControls();
        EditorGUILayout.Space(5);

        // 2. 时间轴主区域
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        {
            // 核心交互处理（绿块优先）
            ProcessMainInteraction();

            // 绘制刻度轴
            DrawTimeAxis();

            // 绘制动画长度轨道
            DrawAnimationTrack();

            // 绘制所有特效轨道
            for (int i = 0; i < effectTracks.Count; i++)
            {
                DrawSingleEffectTrack(effectTracks[i], i + 1);
            }

            // 添加新轨道按钮
            if (GUILayout.Button("+ 添加特效轨道", GUILayout.Width(150)))
            {
                effectTracks.Add(new SingleEffectTrack
                {
                    trackName = $"特效{effectTracks.Count + 1}",
                    effect = new SingleEffectData { startTimeMs = Mathf.RoundToInt(currentTimeMs) }
                });
            }

            // 绘制红色时间游标
            DrawCurrentTimePointer();
        }
        EditorGUILayout.EndScrollView();

        // 3. 播放逻辑
        UpdatePlayback();
    }

    #region 界面绘制
    // 播放控制与缩放栏
    void DrawPlaybackControls()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            // 重置按钮
            if (GUILayout.Button("<<", GUILayout.Width(30)))
            {
                currentTimeMs = 0;
                isPlaying = false;
            }

            // 播放/暂停按钮
            if (isPlaying)
            {
                if (GUILayout.Button("暂停", GUILayout.Width(60)))
                    isPlaying = false;
            }
            else
            {
                if (GUILayout.Button("播放", GUILayout.Width(60)))
                {
                    isPlaying = true;
                    lastEditorTime = EditorApplication.timeSinceStartup;
                }
            }

            // 时间显示与滑块
            GUILayout.Label($"时间: {currentTimeMs:F0} / {animationTotalMs} ms");
            currentTimeMs = EditorGUILayout.Slider(currentTimeMs, 0, animationTotalMs, GUILayout.Width(250));

            // 动画总时长设置
            animationTotalMs = EditorGUILayout.IntField("动画总时长(ms)", animationTotalMs, GUILayout.Width(120));

            // 缩放控制
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("缩放:", GUILayout.Width(50));
            timeScale = EditorGUILayout.Slider(timeScale, 0.01f, 1f, GUILayout.Width(200));
            GUILayout.Label($"{timeScale:F2} 像素/ms");
        }
    }

    // 绘制时间刻度轴（10ms小刻度，500ms大刻度）
    void DrawTimeAxis()
    {
        Rect axisRect = new Rect(labelWidth, 0, position.width - labelWidth - 20, timeAxisHeight);
        EditorGUI.DrawRect(axisRect, new Color(0.15f, 0.15f, 0.15f, 1f));

        // 绘制刻度线
        for (int t = 0; t <= animationTotalMs; t += smallSnapMs)
        {
            float x = labelWidth + t * timeScale - scrollPos.x;
            if (x < labelWidth - 20 || x > position.width - 20) continue;

            // 不同刻度高度区分
            float lineHeight;
            if (t % bigSnapMs == 0) lineHeight = timeAxisHeight * 0.7f;   // 大刻度
            else if (t % 100 == 0) lineHeight = timeAxisHeight * 0.4f;   // 中等刻度
            else lineHeight = timeAxisHeight * 0.2f;                      // 小刻度

            EditorGUI.DrawRect(new Rect(x, axisRect.y, 1, lineHeight), Color.gray);

            // 大刻度显示数字
            if (t % bigSnapMs == 0)
                GUI.Label(new Rect(x - 20, axisRect.y, 30, timeAxisHeight * 0.5f), $"{t}");
        }
    }

    // 绘制动画长度轨道
    void DrawAnimationTrack()
    {
        float y = timeAxisHeight + trackPadding;
        Rect trackRect = new Rect(labelWidth, y, position.width - labelWidth - 20, trackHeight);

        EditorGUI.DrawRect(trackRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        GUI.Label(new Rect(0, y, labelWidth, trackHeight), "动画长度");

        // 黄色动画时长条
        float barWidth = animationTotalMs * timeScale;
        EditorGUI.DrawRect(new Rect(labelWidth - scrollPos.x, y, barWidth, trackHeight), Color.yellow);
    }

    // 绘制单个特效轨道
    void DrawSingleEffectTrack(SingleEffectTrack track, int index)
    {
        float y = timeAxisHeight + (index) * (trackHeight + trackPadding);
        Rect trackRect = new Rect(labelWidth, y, position.width - labelWidth - 20, trackHeight);

        // 轨道背景
        EditorGUI.DrawRect(trackRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        GUI.Label(new Rect(0, y, labelWidth, trackHeight), track.trackName);

        // 绘制绿色特效块
        DrawEffectClip(track.effect, y);

        // 特效预制体选择框
        track.effect.effectPrefab = (GameObject)EditorGUI.ObjectField(
            new Rect(trackRect.xMax + 10, y, 200, trackHeight),
            track.effect.effectPrefab,
            typeof(GameObject),
            true
        );
    }

    // 绘制绿色特效块（仅绘制，事件交给ProcessMainInteraction处理）
    void DrawEffectClip(SingleEffectData effect, float y)
    {
        float x = labelWidth + effect.startTimeMs * timeScale - scrollPos.x;
        float width = effect.fixedDurationMs * timeScale;
        Rect clipRect = new Rect(x, y, width, trackHeight);

        EditorGUI.DrawRect(clipRect, Color.green);
        GUI.Label(clipRect, effect.name);
    }

    // 绘制红色时间游标（从顶部延伸到底部）
    void DrawCurrentTimePointer()
    {
        float x = labelWidth + currentTimeMs * timeScale - scrollPos.x;
        float totalHeight = timeAxisHeight + (1 + effectTracks.Count) * (trackHeight + trackPadding);

        // 红色游标（加宽便于点击）
        Rect pointerRect = new Rect(x - 1, 0, 4, totalHeight);
        EditorGUI.DrawRect(pointerRect, Color.red);
    }
    #endregion

    #region 交互逻辑
    // 核心交互处理（绿块优先响应）
    void ProcessMainInteraction()
    {
        Event evt = Event.current;

        // 关键修复：跳过Layout和Repaint事件，只处理交互事件
        if (evt.type == EventType.Layout || evt.type == EventType.Repaint)
            return;

        Rect timeArea = new Rect(labelWidth, 0, position.width - labelWidth - 20, position.height);

        // 1. 优先处理绿块拖动
        if (draggedEffect != null)
        {
            ProcessEffectDrag();
            if (evt.type == EventType.MouseUp)
            {
                draggedEffect = null;
            }
            evt.Use();
            return;
        }

        // 2. 检测是否点击绿块（触发拖动）
        bool clickedOnClip = false;
        foreach (var track in effectTracks)
        {
            float y = timeAxisHeight + (effectTracks.IndexOf(track) + 1) * (trackHeight + trackPadding);
            float x = labelWidth + track.effect.startTimeMs * timeScale - scrollPos.x;
            float w = track.effect.fixedDurationMs * timeScale;
            Rect clipRect = new Rect(x, y, w, trackHeight);

            if (evt.type == EventType.MouseDown && clipRect.Contains(evt.mousePosition))
            {
                draggedEffect = track.effect;
                dragStartStartTimeMs = track.effect.startTimeMs;
                dragStartMousePos = evt.mousePosition;
                clickedOnClip = true;
                evt.Use();
                break;
            }
        }
        if (clickedOnClip) return;

        // 3. 处理游标拖动/刻度点击/时间轴拖动
        if (evt.type == EventType.MouseDown)
        {
            // 游标拖动（仅顶部刻度区响应）
            Rect pointerDragRect = new Rect(labelWidth + currentTimeMs * timeScale - 2, 0, 4, timeAxisHeight);
            if (pointerDragRect.Contains(evt.mousePosition))
            {
                isDraggingPointer = true;
                evt.Use();
            }
            // 点击刻度区跳转游标
            else if (new Rect(labelWidth, 0, position.width - labelWidth - 20, timeAxisHeight).Contains(evt.mousePosition))
            {
                float clickX = evt.mousePosition.x;
                float timeX = clickX - labelWidth + scrollPos.x;
                float newTime = timeX / timeScale;

                // 对齐到10ms刻度
                newTime = Mathf.RoundToInt(newTime / smallSnapMs) * smallSnapMs;
                currentTimeMs = Mathf.Clamp(newTime, 0, animationTotalMs);

                isPlaying = false;
                Repaint();
                evt.Use();
            }
            // 拖动时间轴画布
            else if (timeArea.Contains(evt.mousePosition))
            {
                isDraggingTimeline = true;
                dragStartMousePos = evt.mousePosition;
                dragStartScrollPos = scrollPos;
                evt.Use();
            }
        }

        // 拖动时间轴画布
        if (evt.type == EventType.MouseDrag && isDraggingTimeline)
        {
            Vector2 delta = evt.mousePosition - dragStartMousePos;
            scrollPos.x = dragStartScrollPos.x - delta.x;
            Repaint();
            evt.Use();
        }

        // 拖动游标（仅顶部刻度区）
        if (isDraggingPointer && evt.type == EventType.MouseDrag)
        {
            float mouseX = evt.mousePosition.x;
            float timeX = mouseX - labelWidth + scrollPos.x;
            float newTime = timeX / timeScale;

            // 对齐到10ms刻度
            newTime = Mathf.RoundToInt(newTime / smallSnapMs) * smallSnapMs;
            currentTimeMs = Mathf.Clamp(newTime, 0, animationTotalMs);

            Repaint();
            evt.Use();
        }

        // 释放所有拖动状态
        if (evt.type == EventType.MouseUp)
        {
            isDraggingTimeline = false;
            isDraggingPointer = false;
        }
    }

    // 绿块拖动逻辑（仅调整位置，固定宽度）
    void ProcessEffectDrag()
    {
        Event evt = Event.current;

        // 关键修复：只处理MouseDrag事件
        if (evt.type != EventType.MouseDrag) return;

        // 计算拖动偏移
        Vector2 delta = evt.mousePosition - dragStartMousePos;
        int deltaMs = Mathf.RoundToInt(delta.x / timeScale);

        // 计算新位置（对齐10ms刻度）
        int newStartTime = dragStartStartTimeMs + deltaMs;
        newStartTime = Mathf.RoundToInt(newStartTime / (float)smallSnapMs) * smallSnapMs;

        // 限制在动画时长范围内
        newStartTime = Mathf.Clamp(newStartTime, 0, animationTotalMs - draggedEffect.fixedDurationMs);

        // 更新位置
        draggedEffect.startTimeMs = newStartTime;
        Repaint();
        evt.Use();
    }
    #endregion

    #region 播放逻辑
    void UpdatePlayback()
    {
        if (!isPlaying) return;

        // 计算流逝时间（转换为毫秒）
        float deltaTime = (float)(EditorApplication.timeSinceStartup - lastEditorTime);
        currentTimeMs += deltaTime * 1000;
        lastEditorTime = EditorApplication.timeSinceStartup;

        // 循环播放
        if (currentTimeMs >= animationTotalMs)
            currentTimeMs = 0;

        Repaint();
    }
    #endregion

    void OnDisable()
    {
        isPlaying = false;
    }
}