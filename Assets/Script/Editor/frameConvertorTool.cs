using UnityEngine;
using UnityEditor;
using UnityEditor.Recorder;

public class FrameConvertorTool : EditorWindow
{
    float targetFPS = 30;
    float timeToConvert = 1;
    float result = 0;
    

    // Add menu named "My Window" to the Window menu
    [MenuItem("Dancing Cat Tool/Seconds to Frame Tool")]
    static void InitFrameTool()
    {
        // Get existing open window or if none, make a new one:
        FrameConvertorTool window = (FrameConvertorTool)EditorWindow.GetWindow(typeof(FrameConvertorTool));
        window.Show();
    }
    
    [MenuItem("Dancing Cat Tool/Video Exporter")]
    static void InitRecorderShortcut()
    {
        // Get existing open window or if none, make a new one:
        RecorderWindow window = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        window.Show();
    }

    void OnGUI()
    {
        targetFPS = EditorGUILayout.FloatField("Target FPS:", targetFPS);
        timeToConvert = EditorGUILayout.FloatField("Time to convert:", timeToConvert);
        result = ConvertSecondsToFrame();
        result = EditorGUILayout.FloatField("Result:", result);
    }

    float ConvertSecondsToFrame()
    {
        return timeToConvert * targetFPS;
    }
}