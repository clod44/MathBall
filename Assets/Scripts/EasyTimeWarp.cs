using UnityEditor;
using UnityEngine;

public class EasyTimeWarp : EditorWindow
{
    private static EasyTimeWarp instance;

    // Keys for EditorPrefs
    private const string TimeScaleKey = "TimeScale";
    private const string TargetTimeScaleKey = "TargetTimeScale";
    private const string InterpolationSpeedKey = "InterpolationSpeed";

    private float timeScale = 1.0f; // Default time scale
    private float targetTimeScale = 1.0f; // Target time scale
    public float interpolationSpeed = 0.05f; // Step size for adjusting time scale
    public float defaultTimeScale = 1f;

    [MenuItem("Window/EasyTimeWarp")]
    public static void ShowWindow()
    {
        GetWindow<EasyTimeWarp>("EasyTimeWarp");
        instance = GetWindow<EasyTimeWarp>();

        // Retrieve stored values from EditorPrefs
        instance.timeScale = EditorPrefs.GetFloat(TimeScaleKey, 1.0f);
        instance.targetTimeScale = EditorPrefs.GetFloat(TargetTimeScaleKey, 1.0f);
        instance.interpolationSpeed = EditorPrefs.GetFloat(InterpolationSpeedKey, 0.05f);
    }

    public static EasyTimeWarp Instance
    {
        get { return instance; }
    }

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Slider("Current Time Speed", timeScale, 0.1f, 2.0f);
        EditorGUI.EndDisabledGroup();

        float newTargetTimeScale = EditorGUILayout.Slider("Target Time Speed", targetTimeScale, 0.1f, 2.0f);
        float newInterpolationSpeed = EditorGUILayout.Slider("Interpolation Speed", interpolationSpeed, 0.001f, 4.0f);

        if (newTargetTimeScale != targetTimeScale || newInterpolationSpeed != interpolationSpeed)
        {
            targetTimeScale = newTargetTimeScale;
            interpolationSpeed = newInterpolationSpeed;

            SetTimeScale(targetTimeScale);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            ResetTimeScale();
        }
        GUILayout.EndHorizontal();
    }

    public void ResetTimeScale()
    {
        targetTimeScale = defaultTimeScale;
        SetTimeScale(targetTimeScale);
    }

    private void Update()
    {
        timeScale = Mathf.Lerp(timeScale, targetTimeScale, interpolationSpeed * Time.deltaTime);
        Time.timeScale = timeScale;

        // Store the updated value in EditorPrefs during Update
        EditorPrefs.SetFloat(TimeScaleKey, timeScale);
    }

    public void SetTimeScale(float newTimeScale)
    {
        targetTimeScale = newTimeScale;
        Repaint(); // Refresh the window to reflect the new value immediately

        // Store the updated values in EditorPrefs
        EditorPrefs.SetFloat(TargetTimeScaleKey, targetTimeScale);
        EditorPrefs.SetFloat(InterpolationSpeedKey, interpolationSpeed);
    }
}
