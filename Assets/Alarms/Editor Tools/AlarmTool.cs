using UnityEngine;
using UnityEditor;

public class AlarmTool : EditorWindow
{
    [MenuItem("Tools/Alarms")]
    public static void ShowWindow()
    {
        thisWindow = GetWindow(typeof(AlarmTool), false, "Alarms");
        windowRect = thisWindow.position;
    }

    private bool lockActionButtons = false;
    private bool disableAlarmChanges = false;
    private bool allowReleaseAll = false;
    private int alarmIndex;

    private Vector2 windowScrollPos;
    private Vector2 allAlarmScrollPos;

    private static EditorWindow thisWindow;
    private static Rect windowRect;

    private void OnGUI()
    {
        if (thisWindow == null)
            thisWindow = GetWindow(typeof(AlarmTool));
        if (windowRect.size != thisWindow.position.size)
        {
            windowRect = thisWindow.position;
        }

        bool playing = Application.isPlaying;
        windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

        #region Options
        EditorGUILayout.LabelField("Alarm Options", EditorStyles.boldLabel);
        Alarm.maxAlarmsAllowed = EditorGUILayout.IntField("Max alarms held in pool:", Alarm.maxAlarmsAllowed);
        EditorGUILayout.LabelField("Decimal places to display:");
        Alarm.alarmPrecision = EditorGUILayout.IntSlider(Alarm.alarmPrecision, 0, 6);
        lockActionButtons = EditorGUILayout.ToggleLeft("Lock alarm action buttons", lockActionButtons);
        disableAlarmChanges = EditorGUILayout.ToggleLeft("Disable changes to alarms", disableAlarmChanges);
        EditorGUILayout.Space();
        #endregion

        if (!playing)
            allowReleaseAll = false;

        #region Play mode only
        EditorGUILayout.LabelField("Current Alarms", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(!playing);

        #region Alarm Selection
        string[] alarms = new string[Alarm.AlarmsInUse.Count + 1];
        alarms[0] = "All";
        for (int i = 0; i < Alarm.AlarmsInUse.Count; i++)
        {
            alarms[i + 1] = Alarm.AlarmsInUse[i].Name;
        }

        if (alarmIndex > Alarm.AlarmsInUse.Count)
            alarmIndex = 0;

        alarmIndex = EditorGUILayout.Popup(alarmIndex, alarms);
        #endregion

        Alarm alarm;

        float width = windowRect.width * 0.95f;

        switch (alarmIndex)
        {
            case 0:
                #region Action All buttons
                EditorGUI.BeginDisabledGroup(lockActionButtons);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(windowRect.width * 0.98f));
                if (GUILayout.Button("Play All"))
                {
                    Alarm.PlayAll();
                }
                if (GUILayout.Button("Pause All"))
                {
                    Alarm.PauseAll();
                }
                if (GUILayout.Button("Stop All"))
                {
                    Alarm.StopAll();
                }
                if (GUILayout.Button("Reset All"))
                {
                    Alarm.ResetAll();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                #endregion

                #region Draw alarm fields
                allAlarmScrollPos = EditorGUILayout.BeginScrollView(allAlarmScrollPos, GUILayout.MaxHeight(windowRect.height / 2));
                for (int i = 0; i < Alarm.AlarmsInUse.Count; i++)
                {
                    alarm = Alarm.AlarmsInUse[i];
                    EditorGUILayout.Space();
                    AlarmDisplay(alarm, width);
                    EditorGUILayout.Space();

                }
                EditorGUILayout.EndScrollView();
                #endregion

                break;
            default:
                int index = alarmIndex - 1;
                alarm = Alarm.AlarmsInUse[index];
                AlarmDisplay(alarm, width);
                for (int i = 0; i < 10; i++)
                {
                    EditorGUILayout.Space();

                }
                break;
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug options", EditorStyles.boldLabel);
        Alarm.disableAllAutoRelease = EditorGUILayout.ToggleLeft("Prevent AutoRelease for all", Alarm.disableAllAutoRelease);
        Alarm.disableAllComplete = EditorGUILayout.ToggleLeft("Disable Complete events for all", Alarm.disableAllComplete);
        EditorGUILayout.Space();
        #region Release All
        EditorGUILayout.BeginHorizontal();
        allowReleaseAll = EditorGUILayout.ToggleLeft("Allow Release All", allowReleaseAll);

        EditorGUI.BeginDisabledGroup(!allowReleaseAll);
        if (GUILayout.Button("Release All"))
        {
            Alarm.ReleaseAll();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        #endregion
        EditorGUI.EndDisabledGroup();
        #endregion
        EditorGUILayout.EndScrollView();
        Repaint();
    }

    private void AlarmDisplay(Alarm alarm, float width)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(width));

        EditorGUILayout.LabelField(alarm.Name, GUILayout.Width(width / 3));
        EditorGUI.BeginDisabledGroup(lockActionButtons);
        if (GUILayout.Button("Play"))
        {
            alarm.Play();
        }
        if (GUILayout.Button("Pause"))
        {
            alarm.Pause();
        }
        if (GUILayout.Button("Stop"))
        {
            alarm.Stop();
        }
        if (GUILayout.Button("Reset"))
        {
            alarm.Reset();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(disableAlarmChanges);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
        EditorGUI.BeginChangeCheck();
        float timeRemaining = EditorGUILayout.FloatField(alarm.TimeRemaining);
        GUILayout.Label("of");
        float timeMax = EditorGUILayout.FloatField(alarm.TimeMax);
        GUILayout.Label("secs remaining");
        GUILayout.EndHorizontal();

        Rect toggleGroup = EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width), GUILayout.ExpandWidth(true));
        bool looping = EditorGUILayout.ToggleLeft("Looping", alarm.Looping, GUILayout.Width(width / 4));
        bool release = EditorGUILayout.ToggleLeft("AutoRelease", alarm.AutoRelease, GUILayout.Width(width / 4));
        EditorGUILayout.Separator();
        if (GUILayout.Button("Release", GUILayout.Width(width / 4)))
        {
            alarm.Release();
        }
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            alarm.SetTimeRemaining(timeRemaining);
            alarm.SetTimeMaximum(timeMax);
            alarm.SetLooping(looping);
            alarm.SetAutoRelease(release);
        }
        EditorGUI.EndDisabledGroup();

    }
}
