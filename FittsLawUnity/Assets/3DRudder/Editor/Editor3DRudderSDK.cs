/*using UnityEngine;
using UnityEditor;
using Unity3DRudder;

public class Editor3DRudderSDK : EditorWindow
{
    private Rudder rudder;
    private bool[] show = new bool[s3DRudderManager._3DRUDDER_SDK_MAX_DEVICE];
    private static Texture logo;

    const int sizeSensor = 100;
    private readonly Vector2 sliderAxes = new Vector2(-1f, 1f);

    [MenuItem("3dRudder/Controllers")]
    public static void ShowWindow()
    {        
        EditorWindow window = GetWindow(typeof(Editor3DRudderSDK), false);
        window.minSize = new Vector2(500, 300);
        if (logo == null)
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3DRudder/Editor/3dRudderIcon.png");
        GUIContent titleContent = new GUIContent("3dRudder", logo);
        window.titleContent = titleContent;
        window.Show();
    }

    void OnGUI()
    {
        // The actual window code goes here
        for (int i = 0; i < s3DRudderManager._3DRUDDER_SDK_MAX_DEVICE; ++i)
            DisplayRudder(i);
    }

    void DisplayRudder(int i)
    {
        rudder = s3DRudderManager.Instance.GetRudder(i);
        ns3DRudder.Status status = rudder.GetStatus();
        string info = status == ns3DRudder.Status.NoStatus ? "Not Connected" : "Connected FW : " + rudder.GetVersion().ToString("X4");
        show[i] = EditorGUILayout.Foldout(show[i], "3dRudder " + i + " (" + info + ")");
        if (show[i])
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Axis", EditorStyles.boldLabel);
            ns3DRudder.Axis axis = rudder.GetAxis();
            EditorGUILayout.Slider("Roll (X)", ParsePrecision(axis.GetXAxis()), sliderAxes.x, sliderAxes.y);
            EditorGUILayout.Slider("Pitch (Y)", ParsePrecision(axis.GetYAxis()), sliderAxes.x, sliderAxes.y);
            EditorGUILayout.Slider("Up/Down (Z)", ParsePrecision(axis.GetZAxis()), sliderAxes.x, sliderAxes.y);
            EditorGUILayout.Slider("Yaw (Z rotation)", ParsePrecision(axis.GetZRotation()), sliderAxes.x, sliderAxes.y);

            EditorGUILayout.Space();

                EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
                var style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = status > ns3DRudder.Status.StayStill ? new Color(0, 0.75f, 0) : new Color(0.75f, 0, 0);
            EditorGUILayout.BeginHorizontal();
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 40;
                EditorGUILayout.LabelField("Status", status.ToString(), style);
                if (GUILayout.Button("Test sound"))
                    rudder.PlaySnd(4400, 100);
                if (GUILayout.Button(rudder.IsFrozen() ? "Unfreeze" : "Freeze"))
                    rudder.SetFreeze(!rudder.IsFrozen());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Sensors", EditorStyles.boldLabel);
                DisplaySensor(rudder);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }

    void DisplaySensor(Rudder rudder)
    {
        EditorGUIUtility.labelWidth = 20;
        for (uint i = 0; i < 6; ++i)
            EditorGUILayout.FloatField(i.ToString(), rudder.GetSensor(i), GUILayout.MaxWidth(sizeSensor));        
    }

    float ParsePrecision(float value)
    {
        return float.Parse(value.ToString("0.00"));
    }

    public void OnInspectorUpdate()
    {        
        // Needed to repaint the window editor
        Repaint();
    }
}
*/