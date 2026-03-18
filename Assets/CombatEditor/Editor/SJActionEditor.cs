using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.VersionControl;

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
    public bool isPlaying = false; //是否已经播放了，播放了不要管，不然一直重叠播放
}

public class SJActionEditor : EditorWindow
{
    // 核心时间数据：分离动画时长和总表现时间
    private int totalTimelineMs = 1000;  // 整体表现时间（可自定义）
    private int animationDurationMs = 1000; // 动画自身时长（由动画片段决定）
    private List<EffectTrack> effectTracks = new List<EffectTrack>();
    private List<AudioTrack> audioTracks = new List<AudioTrack>();
    private AnimationClip currentAnimationClip; // 当前选中的动画片段
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
    private GameObject aniPrefab = null;

    const string AniPath = "Assets/TLSJ/Models/{0}/{1}/{2}/Animation/anim/";

    [MenuItem("Tools/赛季动作编辑器")]
    static void OpenWindow()
    {
        var window = GetWindow<SJActionEditor>();
        window.titleContent = new GUIContent("赛季动作编辑器");
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
        ResetAllMediaState(); // 使用统一的重置方法

        // 移除编辑器刷新回调
        EditorApplication.update -= UpdateEditor;
    }

    // 强制编辑器每帧刷新，确保粒子状态更新
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

            using (new EditorGUILayout.HorizontalScope())
            {
                // 绘制下拉按钮
                if (EditorGUILayout.DropdownButton(new GUIContent("+ 添加轨道"), FocusType.Keyboard, GUILayout.Width(80)))
                {
                    // 创建下拉菜单
                    GenericMenu menu = new GenericMenu();

                    // 添加特效轨道选项
                    menu.AddItem(new GUIContent("特效轨道"), false, () =>
                    {
                        effectTracks.Add(new EffectTrack
                        {
                            trackName = $"特效{effectTracks.Count + 1}",
                            effect = new EffectData { startTimeMs = currentTimeMs, resID = 0 }
                        });
                        Repaint();
                    });

                    // 添加音频轨道选项
                    menu.AddItem(new GUIContent("音频轨道"), false, () =>
                    {
                        audioTracks.Add(new AudioTrack
                        {
                            trackName = $"音频{audioTracks.Count + 1}",
                            audio = new AudioData { startTimeMs = currentTimeMs, durationMs = 1000 }
                        });
                        Repaint();
                    });

                    // 显示菜单
                    menu.ShowAsContext();
                }
            }

            DrawCurrentTimePointerWithExtraSpace();
        }
        EditorGUILayout.EndScrollView();

        DrawBottomInfoPanel();
        UpdatePlaybackAndSample();
    }

    #region 顶部控制栏
    void DrawTopControlBar()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("<<", GUILayout.Width(30)))
            {
                // 回到起点并强制重置所有状态
                ResetToInitialState();
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
            GUILayout.Label("场景单位:", GUILayout.Width(70));
            GameObject newTargetObject = (GameObject)EditorGUILayout.ObjectField(
                targetObject,
                typeof(GameObject),
                true,
                GUILayout.MaxWidth(200)
            );

            // 目标物体输入框（左移+加宽）
            GUILayout.Label("动画预制体:", GUILayout.Width(70));
            GameObject newAniObject = (GameObject)EditorGUILayout.ObjectField(
                aniPrefab,
                typeof(GameObject),
                false,
                GUILayout.MaxWidth(200)
            );

            if (newAniObject != aniPrefab)
            {
                aniPrefab = newAniObject;
            }

            // 检测目标物体是否变更
            if (newTargetObject != targetObject)
            {
                targetObject = newTargetObject;
                if (targetObject != null)
                {
                    targetGameObject = targetObject;
                    targetAnimation = targetGameObject.GetComponent<Animation>();
                    // 清空当前选中的动画
                    currentAnimationClip = null;
                    animationDurationMs = 1000;
                }
                else
                {
                    targetAnimation = null;
                    currentAnimationClip = null;
                    animationDurationMs = 1000;
                }
            }

            if (GUILayout.Button("选择动画", EditorStyles.miniButton, GUILayout.Width(80)))
            {
                if (targetGameObject == null || targetAnimation == null)
                {
                    Debug.LogWarning("还没选择场景中物体");
                    return;
                }
                string aniPath = GetAniPathByPrefab();
                if (aniPath != string.Empty)
                {
                    CustomAnimationClipSelector.ShowByOnePath(aniPath, (selectedClip) =>
                    {
                        if (selectedClip != null)
                        {
                            Debug.Log($"选择了: {selectedClip.name}");
                            Debug.Log($"路径: {AssetDatabase.GetAssetPath(selectedClip)}");

                            currentAnimationClip = selectedClip;
                            AnimationClip animClip = targetAnimation.GetClip(selectedClip.name);
                            if (animClip != selectedClip)
                            {
                                if (animClip != null)
                                {
                                    targetAnimation.RemoveClip(animClip);
                                }
                                targetAnimation.AddClip(selectedClip, selectedClip.name);
                            }

                            animationDurationMs = Mathf.RoundToInt(currentAnimationClip.length * 1000);
                            Repaint();

                        }
                        else
                        {
                            Debug.Log("选择了: None");
                        }
                    });
                }
                else
                {
                    Debug.LogWarning("没有动画预制体");
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

    #region 核心新增：重置方法
    /// <summary>
    /// 重置到初始状态（时间归零 + 所有媒体停止）
    /// </summary>
    private void ResetToInitialState()
    {
        currentTimeMs = 0;
        isPlaying = false;

        // 1. 重置动画状态
        if (targetAnimation != null && currentAnimationClip != null)
        {
            targetAnimation.Stop();
            targetAnimation[currentAnimationClip.name].time = 0;
            targetAnimation.Sample(); // 强制刷新到第一帧
        }

        // 2. 停止所有音频
        StopAllAudio();

        // 3. 重置所有特效（粒子）
        ResetAllEffects();

        Repaint();
    }

    /// <summary>
    /// 重置所有特效状态（停止粒子 + 销毁实例）
    /// </summary>
    private void ResetAllEffects()
    {
        foreach (var track in effectTracks)
        {
            track.effect.isParticleInit = false;

            if (track.effect.effectInstance != null)
            {
                // 停止所有粒子系统
                var psList = track.effect.effectInstance.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var ps in psList)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ps.Clear(true); // 清空所有粒子
                }

                // 隐藏实例（保留实例避免重复创建，也可选择销毁）
                track.effect.effectInstance.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 重置所有媒体状态（动画+音频+特效）
    /// </summary>
    private void ResetAllMediaState()
    {
        // 停止动画
        if (targetAnimation != null)
            targetAnimation.Stop();

        // 停止音频
        StopAllAudio();

        // 重置特效
        ResetAllEffects();

        // 清理音频源
        if (audioSource != null)
        {
            DestroyImmediate(audioSource.gameObject);
            audioSource = null;
        }

        // 销毁特效实例
        foreach (var track in effectTracks)
        {
            if (track.effect.effectInstance != null)
            {
                DestroyImmediate(track.effect.effectInstance);
                track.effect.effectInstance = null;
            }
        }
    }
    #endregion

    string GetAniPathByPrefab()
    {
        if (aniPrefab != null)
        {
            string aniPrefabPath = AssetDatabase.GetAssetPath(aniPrefab);
            string[] paramArray = aniPrefabPath.Split('/');
            if (paramArray.Length >= 4)
            {
                string aniPath = string.Format(AniPath, paramArray[paramArray.Length - 4], paramArray[paramArray.Length - 3], paramArray[paramArray.Length - 2]);
                Debug.Log($"动画文件夹{aniPath}");
                return aniPath;
            }
        }
        return string.Empty;
    }

    #region 轨道绘制
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

    #region 底部信息面板
    void DrawBottomInfoPanel()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("轨道信息", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        if (selectedEffectTrack != null)
        {
            var effect = selectedEffectTrack.effect;
            EditorGUILayout.LabelField($"轨道名称: {selectedEffectTrack.trackName}");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"  特效ID", GUILayout.Width(50));
            effect.resID = EditorGUILayout.IntField(effect.resID, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            effect.startTimeMs = EditorGUILayout.IntField("开始时间:", effect.startTimeMs);

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

    #region 基础界面绘制
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
        if (currentAnimationClip != null && animationDurationMs > 0 && animationDurationMs < totalTimelineMs)
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

        // 显示当前选中的动画名称
        if (currentAnimationClip != null)
        {
            Rect position = new Rect(0, y - 10, labelWidth, 20);
            currentAnimationClip = (AnimationClip)EditorGUI.ObjectField(
                position,
                currentAnimationClip,
                typeof(AnimationClip),
                false
            );
        }
        else
        {
            string animName = currentAnimationClip != null ? currentAnimationClip.name : "未选择动画";
            GUI.Label(new Rect(0, y - 10, labelWidth, trackHeight), $"动画: {animName}");
        }

        // 动画时长显示：左侧可复制不可编辑文本框
        GUILayout.BeginArea(new Rect(0, y + 10, labelWidth, trackHeight));
        GUI.enabled = false;
        string durationText = currentAnimationClip != null ? $"{animationDurationMs} ms" : "0 ms";
        EditorGUILayout.TextField(durationText);
        GUI.enabled = true;
        GUILayout.EndArea();

        // 只有选中动画后才绘制动画条
        if (currentAnimationClip != null)
        {
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

    #region 交互逻辑
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
                // 点击刻度后，直接进入拖拽指针状态，支持后续拖动
                isDraggingPointer = true;
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

    #region 动画与特效采样
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

    /// <summary>
    /// 采样全部粒子。规则粒子轨道的时长为A，如果是-1，则是表演总时长。每个粒子预制体包含多个多个粒子系统。当前时刻为B，轨道开始时刻为C。如果当前时刻B需要在轨道的时间区间内。如果粒子系统是持续型的，按照loop的时长为D，（当前时刻B-开始C）%D =(int) E，采样百分比为E/D。如果粒子系统不是持续型的，改粒子持续时长为F，如果B-C >F,不管该粒子，如果小于按照 (B-C)/F，进行采样显示
    /// </summary>
    /// <param name="timeMs"></param>
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

            // 轨道核心参数计算
            float effectStart = track.effect.startTimeMs; // 轨道开始时刻C（ms）
                                                          // 轨道总时长A：-1则取表演总时长，否则取配置的durationMs
            float trackTotalDurationMs = track.effect.durationMs == -1 ? totalTimelineMs - effectStart : track.effect.durationMs;
            float effectEnd = effectStart + trackTotalDurationMs; // 轨道结束时刻（ms）

            bool isInTrackRange = timeMs >= effectStart && timeMs <= effectEnd; // 当前时刻B是否在轨道区间内
            float trackElapsedTimeMs = timeMs - effectStart; // B - C（ms）

            track.effect.effectInstance.SetActive(isInTrackRange);
            SampleParticleSystem(track.effect.effectInstance, trackElapsedTimeMs, isInTrackRange, track);
        }
    }

    void SampleParticleSystem(GameObject effectInstance, float trackElapsedTimeMs, bool isInTrackRange, EffectTrack track)
    {
        if (effectInstance == null) return;
        ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>(true);
        if (particleSystems.Length == 0)
        {
            Debug.LogWarning($"实例 {effectInstance.name} 未找到ParticleSystem组件！");
            return;
        }

        // 轨道开始时刻C（ms）、轨道总时长A（ms）
        float effectStart = track.effect.startTimeMs;
        float trackTotalDurationMs = track.effect.durationMs == -1 ? totalTimelineMs : track.effect.durationMs;

        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            // 粒子系统自身核心参数
            bool isLoop = main.loop; // 是否循环（持续型）
            float psSelfDurationSec = main.duration; // 粒子自身时长D/F（秒）
            float psSelfDurationMs = psSelfDurationSec * 1000; // 转毫秒，方便计算

            // 兜底：粒子自身时长为0时，用轨道时长兜底
            if (psSelfDurationMs <= 0)
            {
                psSelfDurationMs = trackTotalDurationMs;
                psSelfDurationSec = psSelfDurationMs / 1000;
            }

            // 仅在轨道时间区间内处理粒子采样
            if (isInTrackRange)
            {
                float sampleTimeSec = 0f;
                float trackElapsedTimeSec = trackElapsedTimeMs / 1000; // B-C（秒）

                if (isLoop)
                {
                    // 循环粒子（持续型）：(B-C) % D = E → 采样百分比E/D
                    float elapsedModDuration = trackElapsedTimeSec % psSelfDurationSec; // E（秒）
                    sampleTimeSec = elapsedModDuration; // 采样时间=E
                }
                else
                {
                    // 非循环粒子：如果B-C > F → 跳过；否则 (B-C)/F 采样
                    if (trackElapsedTimeSec <= psSelfDurationSec)
                    {
                        sampleTimeSec = trackElapsedTimeSec; // 采样时间=B-C
                    }
                    else
                    {
                        // B-C > F，停止并清空粒子，不处理
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        continue;
                    }
                }

                // 粒子采样核心逻辑
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // 先停止并清空
                ps.Simulate(sampleTimeSec, true, true); // 模拟到采样时间
                ps.Play(true); // 播放粒子
            }
            else
            {
                // 不在轨道区间内，停止粒子
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
        {
            audioSource.Stop();
            audioSource.clip = null; // 清空音频片段，避免残留
        }
    }
    #endregion

    void UpdatePlaybackAndSample()
    {
        if (!isPlaying) return;

        float deltaTime = (float)(EditorApplication.timeSinceStartup - lastEditorTime);
        currentTimeMs = Mathf.RoundToInt(currentTimeMs + deltaTime * 1000);
        lastEditorTime = EditorApplication.timeSinceStartup;

        // 播放完整个时间轴后，自动重置到初始状态
        if (currentTimeMs >= totalTimelineMs)
        {
            // 核心修改：调用统一的重置方法
            ResetToInitialState();
            return; // 提前返回，避免继续执行采样逻辑
        }

        SampleAnimation(currentTimeMs);
        SampleAllEffects(currentTimeMs);
        UpdateAudioPlayback(currentTimeMs);
        Repaint();
    }
}