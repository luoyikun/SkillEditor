using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectData
{
    public string name = "特效";
    public int startTimeMs = 0;          // 特效开始时间（毫秒）
    public int durationMs = -1; // 特效时长：-1表示持续到时间轴末尾
    public GameObject effectPrefab;  // 特效预制体
    public string anchorPoint = "Root"; // 锚点
    public int resID = 0;  // 特效资源ID
    public bool isHidden = false;     // 是否隐藏

    [HideInInspector] public GameObject effectInstance;
    [HideInInspector] public bool isParticleInit = false; // 新增：标记粒子是否初始化
}

[System.Serializable]
public class EffectTrack
{
    public string trackName = "特效轨道";
    public EffectData effect = new EffectData();
}

[System.Serializable]
public class AudioData
{
    public string name = "音频";
    public int startTimeMs = 0;          // 音频开始时间（毫秒）
    public int durationMs = 1000;    // 音频时长（可修改）
    public AudioClip audioClip;      // 音频片段
    public bool isHidden = false;    // 是否隐藏
    public int resID = 0;  // 资源ID
}

[System.Serializable]
public class AudioTrack
{
    public string trackName = "音频轨道";
    public AudioData audio = new AudioData();
}

public class FinalTimelineEditor : EditorWindow
{
    // 核心时间数据：分离动画时长和总表现时间
    private int totalTimelineMs = 1000;  // 整体表现时间（可自定义）
    private int animationDurationMs = 1000; // 动画自身时长（由动画片段决定）
    private List<EffectTrack> effectTracks = new List<EffectTrack>();
    private List<AudioTrack> audioTracks = new List<AudioTrack>();
    private AnimationClip currentAnimationClip;
    private GameObject targetGameObject;
    private Animation targetAnimation;
    private AudioSource audioSource; // 音频播放源

    // 播放控制
    private int currentTimeMs = 0; // 改为整数
    private bool isPlaying = false;
    private double lastEditorTime = 0;

    // 视图缩放与滚动
    private Vector2 scrollPos;
    private float timeScale = 0.548f;

    // 界面常量
    private const float trackHeight = 30f;
    private const float trackPadding = 5f;
    private const float labelWidth = 100f;
    private const float timeAxisHeight = 30f;
    private const float deleteBtnWidth = 25f;
    private const float topExtraSpace = 60f;
    private const float rightBtnMargin = 0f; // 右侧按钮边距

    // 刻度精度
    private const int smallSnapMs = 1;
    private const int midSnapMs = 50;
    private const int bigSnapMs = 500;

    // 拖动状态
    private bool isDraggingTimeline = false;
    private bool isDraggingPointer = false;
    private EffectData draggedEffect = null;
    private AudioData draggedAudio = null;
    private EffectTrack selectedEffectTrack = null;
    private AudioTrack selectedAudioTrack = null;

    private Vector2 dragStartMousePos;
    private Vector2 dragStartScrollPos;
    private int dragStartStartTimeMs;

    private GameObject targetObject = null;

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
        if (effectTracks.Count == 0)
        {
            effectTracks.Add(new EffectTrack { trackName = "特效1", effect = new EffectData { startTimeMs = 0, resID = 1 } });
            effectTracks.Add(new EffectTrack { trackName = "特效2", effect = new EffectData { startTimeMs = 600, resID = 2 } });
            effectTracks.Add(new EffectTrack { trackName = "特效3", effect = new EffectData { startTimeMs = 100, resID = 3 } });
        }
        // 强制编辑器刷新
        EditorApplication.update += UpdateEditor;
    }

    void OnDisable()
    {
        isPlaying = false;
        if (targetAnimation != null)
            targetAnimation.Stop();
        StopAllAudio();

        foreach (var track in effectTracks)
        {
            if (track.effect.effectInstance != null)
            {
                DestroyImmediate(track.effect.effectInstance);
                track.effect.effectInstance = null;
                track.effect.isParticleInit = false;
            }
        }

        if (audioSource != null)
            DestroyImmediate(audioSource.gameObject);

        // 移除编辑器刷新回调
        EditorApplication.update -= UpdateEditor;
    }

    // 新增：强制编辑器每帧刷新，确保粒子状态更新
    void UpdateEditor()
    {
        if (isPlaying)
        {
            Repaint();
        }
    }

    void OnGUI()
    {
        DrawTopControlBar();
        EditorGUILayout.Space(5);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        {
            ProcessMainInteraction();
            DrawTimeAxisWithExtraSpace(); // 刻度对齐修复
            DrawAnimationTrackWithExtraSpace(); // 动画条对齐修复 + 动画时长显示

            // 绘制特效轨道
            for (int i = 0; i < effectTracks.Count; i++)
                DrawSingleEffectTrackWithOps(effectTracks[i], i + 1);

            // 绘制音频轨道
            for (int i = 0; i < audioTracks.Count; i++)
                DrawSingleAudioTrackWithOps(audioTracks[i], effectTracks.Count + i + 1);

            // 添加轨道按钮
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+ 添加特效轨道", GUILayout.Width(150)))
                {
                    effectTracks.Add(new EffectTrack
                    {
                        trackName = $"特效{effectTracks.Count + 1}",
                        effect = new EffectData { startTimeMs = currentTimeMs, resID = 0 }
                    });
                }
                if (GUILayout.Button("+ 添加音频轨道", GUILayout.Width(150)))
                {
                    audioTracks.Add(new AudioTrack
                    {
                        trackName = $"音频{audioTracks.Count + 1}",
                        audio = new AudioData { startTimeMs = currentTimeMs, durationMs = 1000 }
                    });
                }
            }

            DrawCurrentTimePointerWithExtraSpace();
        }
        EditorGUILayout.EndScrollView();

        DrawBottomInfoPanel();
        UpdatePlaybackAndSample();
    }

    #region 顶部控制栏（播放一次后回到开头）
    void DrawTopControlBar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("<<", GUILayout.Width(30)))
            {
                currentTimeMs = 0;
                isPlaying = false;
                SampleAnimation(0);
                SampleAllEffects(0);
                StopAllAudio();
            }

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
                    // 重置粒子初始化状态
                    foreach (var track in effectTracks)
                    {
                        track.effect.isParticleInit = false;
                    }
                }
            }

            // 显示当前时间/总表现时间（整数）
            GUILayout.Label($"时间: {currentTimeMs} / {totalTimelineMs} ms");
            currentTimeMs = Mathf.RoundToInt(EditorGUILayout.Slider(currentTimeMs, 0, totalTimelineMs, GUILayout.Width(250)));
            if (GUI.changed)
            {
                SampleAnimation(currentTimeMs);
                SampleAllEffects(currentTimeMs);
                UpdateAudioPlayback(currentTimeMs);
                // 拖动时间轴时重置粒子初始化
                foreach (var track in effectTracks)
                {
                    track.effect.isParticleInit = false;
                }
            }

            // 总表现时间（可自定义）
            GUILayout.Label("总表现时间:", GUILayout.Width(70));
            totalTimelineMs = EditorGUILayout.IntField(totalTimelineMs, GUILayout.Width(80));

            // 目标物体输入框（左移+加宽）
            GUILayout.FlexibleSpace();
            targetObject = (GameObject)EditorGUILayout.ObjectField(
                "目标物体",
                targetObject,
                typeof(GameObject),
                true,
                GUILayout.Width(300) // 加宽显示
            );
            if (targetObject != null && targetGameObject != targetObject)
            {
                targetGameObject = targetObject;
                targetAnimation = targetGameObject.GetComponent<Animation>();
                if (targetAnimation != null && targetAnimation.clip != null)
                {
                    currentAnimationClip = targetAnimation.clip;
                    // 更新动画时长（由动画片段决定）
                    animationDurationMs = Mathf.RoundToInt(currentAnimationClip.length * 1000);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该GameObject无Animation组件或未绑定动画片段！", "确定");
                }
            }

            // 缩放控制
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("缩放:", GUILayout.Width(50));
            timeScale = EditorGUILayout.Slider(timeScale, 0.01f, 1f, GUILayout.Width(200));
            GUILayout.Label($"{timeScale:F3} 像素/ms");
        }
    }
    #endregion

    #region 轨道绘制（删除按钮移到左侧）
    void DrawSingleEffectTrackWithOps(EffectTrack track, int index)
    {
        float y = topExtraSpace + timeAxisHeight + trackPadding + (index) * (trackHeight + trackPadding);
        Rect trackRect = new Rect(labelWidth, y, position.width - labelWidth - rightBtnMargin - 20, trackHeight);

        Color bgColor = selectedEffectTrack == track ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.2f, 0.2f, 0.2f);
        EditorGUI.DrawRect(trackRect, bgColor);

        GUILayout.BeginArea(new Rect(0, y, labelWidth, trackHeight));
        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(track.trackName, EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    selectedEffectTrack = track;
                    selectedAudioTrack = null;
                    Repaint();
                }
                track.effect.isHidden = GUILayout.Toggle(track.effect.isHidden, "隐", GUILayout.Width(40));
            }
            // 删除按钮放在左侧隐藏下方
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(60);
                Color originalColor = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("×", GUILayout.Width(deleteBtnWidth), GUILayout.Height(15)))
                {
                    if (track.effect.effectInstance != null)
                        DestroyImmediate(track.effect.effectInstance);
                    effectTracks.Remove(track);
                    if (selectedEffectTrack == track) selectedEffectTrack = null;
                    Repaint();
                }
                GUI.color = originalColor;
            }
        }
        GUILayout.EndArea();

        if (!track.effect.isHidden)
            DrawEffectClip(track.effect, y);
    }

    void DrawSingleAudioTrackWithOps(AudioTrack track, int index)
    {
        float y = topExtraSpace + timeAxisHeight + trackPadding + (index) * (trackHeight + trackPadding);
        Rect trackRect = new Rect(labelWidth, y, position.width - labelWidth - rightBtnMargin - 20, trackHeight);

        Color bgColor = selectedAudioTrack == track ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.2f, 0.2f, 0.2f);
        EditorGUI.DrawRect(trackRect, bgColor);

        GUILayout.BeginArea(new Rect(0, y, labelWidth, trackHeight));
        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(track.trackName, EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    selectedAudioTrack = track;
                    selectedEffectTrack = null;
                    Repaint();
                }
                track.audio.isHidden = GUILayout.Toggle(track.audio.isHidden, "隐", GUILayout.Width(40));
            }
            // 删除按钮放在左侧隐藏下方
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(60);
                Color originalColor = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("×", GUILayout.Width(deleteBtnWidth), GUILayout.Height(18)))
                {
                    audioTracks.Remove(track);
                    if (selectedAudioTrack == track) selectedAudioTrack = null;
                    Repaint();
                }
                GUI.color = originalColor;
            }
        }
        GUILayout.EndArea();

        if (!track.audio.isHidden)
            DrawAudioClip(track.audio, y);
    }

    void DrawEffectClip(EffectData effect, float y)
    {
        float x = labelWidth + effect.startTimeMs * timeScale - scrollPos.x;
        float width;
        if (effect.durationMs == -1)
        {
            // 持续到时间轴末尾
            width = (totalTimelineMs - effect.startTimeMs) * timeScale;
        }
        else
        {
            width = effect.durationMs * timeScale;
        }
        Rect clipRect = new Rect(x, y, width, trackHeight);
        EditorGUI.DrawRect(clipRect, Color.green);
        GUI.Label(clipRect, effect.name);
    }

    void DrawAudioClip(AudioData audio, float y)
    {
        float x = labelWidth + audio.startTimeMs * timeScale - scrollPos.x;
        float width = audio.durationMs * timeScale;
        Rect clipRect = new Rect(x, y, width, trackHeight);
        EditorGUI.DrawRect(clipRect, Color.blue); // 音频滑块为蓝色
        GUI.Label(clipRect, audio.name);
    }
    #endregion

    #region 底部信息面板（持续时间可编辑）
    void DrawBottomInfoPanel()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("详细信息", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (selectedEffectTrack != null)
        {
            var effect = selectedEffectTrack.effect;
            EditorGUILayout.LabelField($"轨道名称: {selectedEffectTrack.trackName}");
            EditorGUILayout.LabelField($"特效ID: {effect.resID}");
            EditorGUILayout.LabelField($"开始时间: {effect.startTimeMs} ms");
            // 持续时间可编辑，-1也允许输入
            effect.durationMs = EditorGUILayout.IntField("持续时间(ms):", effect.durationMs);
            EditorGUILayout.LabelField($"锚点: {effect.anchorPoint}");
            effect.effectPrefab = (GameObject)EditorGUILayout.ObjectField("特效资源:", effect.effectPrefab, typeof(GameObject), true);
        }
        else if (selectedAudioTrack != null)
        {
            var audio = selectedAudioTrack.audio;
            EditorGUILayout.LabelField($"轨道名称: {selectedAudioTrack.trackName}");
            EditorGUILayout.LabelField($"开始时间: {audio.startTimeMs} ms");
            audio.durationMs = EditorGUILayout.IntField("持续时间(ms):", audio.durationMs);
            audio.audioClip = (AudioClip)EditorGUILayout.ObjectField("音频片段:", audio.audioClip, typeof(AudioClip), true);
        }
        else
        {
            EditorGUILayout.LabelField("请选择一个轨道查看详情");
        }

        EditorGUILayout.EndVertical();
    }
    #endregion

    #region 基础界面绘制（动画时长显示移到指定位置）
    // 刻度对齐修复：统一使用labelWidth作为起始X坐标，长度与轨道对齐
    void DrawTimeAxisWithExtraSpace()
    {
        // 刻度长度与轨道完全对齐：position.width - labelWidth - rightBtnMargin - 20
        Rect axisRect = new Rect(labelWidth, topExtraSpace, position.width - labelWidth - rightBtnMargin - 20, timeAxisHeight);
        EditorGUI.DrawRect(axisRect, new Color(0.15f, 0.15f, 0.15f, 1f));

        // 刻度基于总表现时间绘制
        for (int t = 0; t <= totalTimelineMs; t += smallSnapMs)
        {
            float x = labelWidth + t * timeScale - scrollPos.x;
            if (x < labelWidth - 20 || x > labelWidth + (position.width - labelWidth - rightBtnMargin - 20)) continue;

            float lineHeight = (t % bigSnapMs == 0) ? timeAxisHeight * 0.7f : (t % midSnapMs == 0) ? timeAxisHeight * 0.4f : timeAxisHeight * 0.2f;
            Color lineColor = (t % bigSnapMs == 0) ? Color.white : Color.gray; // 大刻度更醒目
            EditorGUI.DrawRect(new Rect(x, axisRect.y, 1, lineHeight), lineColor);

            if (t % bigSnapMs == 0)
                GUI.Label(new Rect(x - 20, axisRect.y, 30, timeAxisHeight * 0.5f), $"{t}");
        }

        // 绘制动画时长分割线（区分动画和后续表现时间）
        if (animationDurationMs > 0 && animationDurationMs < totalTimelineMs)
        {
            float splitX = labelWidth + animationDurationMs * timeScale - scrollPos.x;
            EditorGUI.DrawRect(new Rect(splitX, axisRect.y, 2, timeAxisHeight), Color.yellow);
            GUI.Label(new Rect(splitX - 30, axisRect.y + timeAxisHeight * 0.5f, 60, 20), $"动画结束");
        }
    }

    // 动画条对齐修复：显示动画自身时长 + 动画时长文本框移到左侧
    void DrawAnimationTrackWithExtraSpace()
    {
        float y = topExtraSpace + timeAxisHeight + trackPadding;
        Rect trackRect = new Rect(labelWidth, y, position.width - labelWidth - rightBtnMargin - 20, trackHeight);
        EditorGUI.DrawRect(trackRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        GUI.Label(new Rect(0, y - 10, labelWidth, trackHeight), "动画长度");

        // 动画时长显示：左侧可复制不可编辑文本框
        GUILayout.BeginArea(new Rect(0, y + 10, labelWidth, trackHeight));
        GUI.enabled = false;
        EditorGUILayout.TextField($"{animationDurationMs}");
        GUI.enabled = true;
        GUILayout.EndArea();


        // 动画条宽度基于动画自身时长
        float barWidth = animationDurationMs * timeScale;
        EditorGUI.DrawRect(new Rect(labelWidth - scrollPos.x, y, barWidth, trackHeight), Color.yellow);

        // 绘制总表现时间背景（灰色）
        if (totalTimelineMs > animationDurationMs)
        {
            float extraWidth = (totalTimelineMs - animationDurationMs) * timeScale;
            float extraX = labelWidth + animationDurationMs * timeScale - scrollPos.x;
            EditorGUI.DrawRect(new Rect(extraX, y, extraWidth, trackHeight), new Color(0.3f, 0.3f, 0.3f, 0.8f));
            GUI.Label(new Rect(extraX + 10, y, 100, trackHeight), "后续表现时间");
        }
    }

    void DrawCurrentTimePointerWithExtraSpace()
    {
        float x = labelWidth + currentTimeMs * timeScale - scrollPos.x;
        int totalTracks = effectTracks.Count + audioTracks.Count + 1;
        float totalHeight = timeAxisHeight + trackPadding + totalTracks * (trackHeight + trackPadding) - trackPadding;
        Rect pointerRect = new Rect(x - 1, topExtraSpace, 4, totalHeight);
        EditorGUI.DrawRect(pointerRect, Color.red);
    }
    #endregion

    #region 交互逻辑（播放一次后回到开头）
    void ProcessMainInteraction()
    {
        Event evt = Event.current;
        if (evt.type == EventType.Layout || evt.type == EventType.Repaint) return;

        Rect timeArea = new Rect(labelWidth, topExtraSpace, position.width - labelWidth - rightBtnMargin - 20, position.height);

        if (draggedEffect != null)
        {
            ProcessEffectDrag();
            if (evt.type == EventType.MouseUp) draggedEffect = null;
            evt.Use(); return;
        }

        if (draggedAudio != null)
        {
            ProcessAudioDrag();
            if (evt.type == EventType.MouseUp) draggedAudio = null;
            evt.Use(); return;
        }

        bool clickedOnClip = false;
        foreach (var track in effectTracks)
        {
            if (track.effect.isHidden) continue;
            float y = topExtraSpace + timeAxisHeight + trackPadding + (effectTracks.IndexOf(track) + 1) * (trackHeight + trackPadding);
            float x = labelWidth + track.effect.startTimeMs * timeScale - scrollPos.x;
            float w;
            if (track.effect.durationMs == -1)
                w = (totalTimelineMs - track.effect.startTimeMs) * timeScale;
            else
                w = track.effect.durationMs * timeScale;
            Rect clipRect = new Rect(x, y, w, trackHeight);

            if (evt.type == EventType.MouseDown && clipRect.Contains(evt.mousePosition))
            {
                draggedEffect = track.effect;
                dragStartStartTimeMs = track.effect.startTimeMs;
                dragStartMousePos = evt.mousePosition;
                selectedEffectTrack = track;
                selectedAudioTrack = null;
                clickedOnClip = true;
                evt.Use();
            }
        }
        if (clickedOnClip) return;

        foreach (var track in audioTracks)
        {
            if (track.audio.isHidden) continue;
            float y = topExtraSpace + timeAxisHeight + trackPadding + (effectTracks.Count + audioTracks.IndexOf(track) + 1) * (trackHeight + trackPadding);
            float x = labelWidth + track.audio.startTimeMs * timeScale - scrollPos.x;
            float w = track.audio.durationMs * timeScale;
            Rect clipRect = new Rect(x, y, w, trackHeight);

            if (evt.type == EventType.MouseDown && clipRect.Contains(evt.mousePosition))
            {
                draggedAudio = track.audio;
                dragStartStartTimeMs = track.audio.startTimeMs;
                dragStartMousePos = evt.mousePosition;
                selectedAudioTrack = track;
                selectedEffectTrack = null;
                clickedOnClip = true;
                evt.Use(); break;
            }
        }
        if (clickedOnClip) return;

        if (evt.type == EventType.MouseDown)
        {
            Rect pointerDragRect = new Rect(labelWidth + currentTimeMs * timeScale - 2, topExtraSpace, 4, timeAxisHeight);
            if (pointerDragRect.Contains(evt.mousePosition))
            {
                isDraggingPointer = true;
                evt.Use();
            }
            // 刻度点击区域对齐修复
            else if (new Rect(labelWidth, topExtraSpace, position.width - labelWidth - rightBtnMargin - 20, timeAxisHeight).Contains(evt.mousePosition))
            {
                float clickX = evt.mousePosition.x;
                float timeX = clickX - labelWidth + scrollPos.x;
                float newTime = timeX / timeScale;
                newTime = Mathf.RoundToInt(newTime / smallSnapMs) * smallSnapMs;
                currentTimeMs = Mathf.Clamp(Mathf.RoundToInt(newTime), 0, totalTimelineMs);

                SampleAnimation(currentTimeMs);
                SampleAllEffects(currentTimeMs);
                UpdateAudioPlayback(currentTimeMs);
                isPlaying = false;
                // 点击刻度时重置粒子初始化
                foreach (var track in effectTracks)
                {
                    track.effect.isParticleInit = false;
                }
                Repaint();
                evt.Use();
            }
            else if (timeArea.Contains(evt.mousePosition))
            {
                isDraggingTimeline = true;
                dragStartMousePos = evt.mousePosition;
                dragStartScrollPos = scrollPos;
                evt.Use();
            }
        }

        if (evt.type == EventType.MouseDrag && isDraggingTimeline)
        {
            Vector2 delta = evt.mousePosition - dragStartMousePos;
            scrollPos.x = dragStartScrollPos.x - delta.x;
            Repaint();
            evt.Use();
        }

        if (isDraggingPointer && evt.type == EventType.MouseDrag)
        {
            float mouseX = evt.mousePosition.x;
            float timeX = mouseX - labelWidth + scrollPos.x;
            float newTime = timeX / timeScale;
            newTime = Mathf.RoundToInt(newTime / smallSnapMs) * smallSnapMs;
            currentTimeMs = Mathf.Clamp(Mathf.RoundToInt(newTime), 0, totalTimelineMs);

            SampleAnimation(currentTimeMs);
            SampleAllEffects(currentTimeMs);
            UpdateAudioPlayback(currentTimeMs);
            Repaint();
            evt.Use();
        }

        if (evt.type == EventType.MouseUp)
        {
            isDraggingTimeline = false;
            isDraggingPointer = false;
        }
    }

    void ProcessEffectDrag()
    {
        Event evt = Event.current;
        if (evt.type != EventType.MouseDrag) return;

        Vector2 delta = evt.mousePosition - dragStartMousePos;
        int deltaMs = Mathf.RoundToInt(delta.x / timeScale);
        int newStartTime = dragStartStartTimeMs + deltaMs;
        newStartTime = Mathf.Max(0, Mathf.RoundToInt(newStartTime / (float)smallSnapMs) * smallSnapMs); // 起始位置不可小于0
        draggedEffect.startTimeMs = newStartTime;
        Repaint();
        evt.Use();
    }

    void ProcessAudioDrag()
    {
        Event evt = Event.current;
        if (evt.type != EventType.MouseDrag) return;

        Vector2 delta = evt.mousePosition - dragStartMousePos;
        int deltaMs = Mathf.RoundToInt(delta.x / timeScale);
        int newStartTime = dragStartStartTimeMs + deltaMs;
        newStartTime = Mathf.Max(0, Mathf.RoundToInt(newStartTime / (float)smallSnapMs) * smallSnapMs); // 起始位置不可小于0
        draggedAudio.startTimeMs = newStartTime;
        Repaint();
        evt.Use();
    }
    #endregion

    #region 动画与特效采样（修复粒子系统采样播放）
    void SampleAnimation(float timeMs)
    {
        if (targetAnimation == null || currentAnimationClip == null) return;

        // 动画只播放自身时长，超过后停止
        float clampedTimeMs = Mathf.Min(timeMs, animationDurationMs);
        float timeSec = clampedTimeMs / 1000f;

        targetAnimation.Play(currentAnimationClip.name);
        targetAnimation[currentAnimationClip.name].time = timeSec;
        targetAnimation.Sample();
        targetAnimation.Stop();
    }

    void SampleAllEffects(float timeMs)
    {
        foreach (var track in effectTracks)
        {
            if (track.effect.isHidden || track.effect.effectPrefab == null)
            {
                if (track.effect.effectInstance != null)
                {
                    DestroyImmediate(track.effect.effectInstance);
                    track.effect.effectInstance = null;
                }
                continue;
            }

            if (track.effect.effectInstance == null)
            {
                track.effect.effectInstance = Instantiate(track.effect.effectPrefab);
                track.effect.effectInstance.name = $"[{track.trackName}]_Instance";
                track.effect.effectInstance.SetActive(false);

                if (targetGameObject != null)
                {
                    track.effect.effectInstance.transform.SetParent(targetGameObject.transform);
                    track.effect.effectInstance.transform.localPosition = Vector3.zero;
                    track.effect.effectInstance.transform.localRotation = Quaternion.identity;
                }
            }

            float effectStart = track.effect.startTimeMs;
            float effectEnd = track.effect.durationMs == -1 ? totalTimelineMs : effectStart + track.effect.durationMs;
            float progress;

            if (timeMs < effectStart)
            {
                progress = 0f;
                track.effect.effectInstance.SetActive(false);
            }
            else if (timeMs > effectEnd)
            {
                progress = 1f;
                track.effect.effectInstance.SetActive(false);
            }
            else
            {
                progress = (timeMs - effectStart) / (effectEnd - effectStart);
                track.effect.effectInstance.SetActive(true);
            }

            SampleParticleSystem(track.effect.effectInstance, progress, timeMs >= effectStart && timeMs <= effectEnd, track);
        }
    }

    void SampleParticleSystem(GameObject effectInstance, float progress, bool isActive, EffectTrack track)
    {
        if (effectInstance == null) return;
        ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
        if (particleSystems.Length == 0)
        {
            Debug.LogWarning($"实例 {effectInstance.name} 未找到ParticleSystem组件！");
            return;
        }

        foreach (var ps in particleSystems)
        {
            float psDuration = ps.main.duration;
            if (psDuration <= 0)
                psDuration = track.effect.durationMs == -1 ? (totalTimelineMs - track.effect.startTimeMs) / 1000f : track.effect.durationMs / 1000f;
            float sampleTime = psDuration * progress;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Simulate(sampleTime, true, true);

            if (isActive)
            {
                ps.Play(true);
            }
            else
            {
                ps.Stop(true);
            }
        }
    }


    #endregion

    #region 音频播放逻辑
    void UpdateAudioPlayback(float timeMs)
    {
        if (audioSource == null)
        {
            GameObject go = new GameObject("TimelineAudioSource");
            audioSource = go.AddComponent<AudioSource>();
        }

        foreach (var track in audioTracks)
        {
            if (track.audio.isHidden || track.audio.audioClip == null) continue;

            float audioStart = track.audio.startTimeMs;
            float audioEnd = audioStart + track.audio.durationMs;

            if (timeMs >= audioStart && timeMs < audioEnd)
            {
                if (!audioSource.isPlaying || audioSource.clip != track.audio.audioClip)
                {
                    audioSource.clip = track.audio.audioClip;
                    audioSource.time = (timeMs - audioStart) / 1000f;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.clip == track.audio.audioClip && audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }

    void StopAllAudio()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
    #endregion

    void UpdatePlaybackAndSample()
    {
        if (!isPlaying) return;

        float deltaTime = (float)(EditorApplication.timeSinceStartup - lastEditorTime);
        currentTimeMs = Mathf.RoundToInt(currentTimeMs + deltaTime * 1000);
        lastEditorTime = EditorApplication.timeSinceStartup;

        // 播放一次后回到开头并停止
        if (currentTimeMs >= totalTimelineMs)
        {
            currentTimeMs = 0;
            isPlaying = false;
            // 播放结束后重置所有粒子
            foreach (var track in effectTracks)
            {
                track.effect.isParticleInit = false;
                if (track.effect.effectInstance != null)
                {
                    var psList = track.effect.effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (var ps in psList)
                    {
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                }
            }
        }

        SampleAnimation(currentTimeMs);
        SampleAllEffects(currentTimeMs);
        UpdateAudioPlayback(currentTimeMs);
        Repaint();
    }
}