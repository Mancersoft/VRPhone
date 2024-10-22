﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace SolAR
{
    [CustomEditor(typeof(SolARPipeline), true)]
    public class SolARPipelineEditor : Editor
    {
#pragma warning disable IDE1006 // Styles d'affectation de noms
        new SolARPipeline target { get { return (SolARPipeline)base.target; } }
#pragma warning restore IDE1006 // Styles d'affectation de noms

        GUIStyle _windowStyle;
        GUIStyle windowStyle { get { return _windowStyle ?? (_windowStyle = new GUIStyle(GUI.skin.window) { richText = true, stretchHeight = false }); } }

        readonly AnimBool animConfiguration = new AnimBool(true);

        protected void OnEnable()
        {
            LoadPipelines();
            SelectPipeline(target.m_selectedPipeline);
            animConfiguration.valueChanged.AddListener(new UnityAction(Repaint));
        }

        protected void OnDisable()
        {
            animConfiguration.valueChanged.RemoveAllListeners();
        }

        private void SelectPipeline(int num_pipeline)
        {
            serializedObject.ApplyModifiedProperties();
            var serializer = new XmlSerializer(typeof(ConfXml));
            if (target.m_pipelinesPath.Length <= 0)
                return;
            if (target.m_pipelinesPath.Length <= num_pipeline)
                num_pipeline = target.m_pipelinesPath.Length - 1;
            string selectedPipelinePath = Application.dataPath + target.m_pipelinesPath.ElementAt(num_pipeline);
            using (var stream = File.OpenRead(selectedPipelinePath))
            {
                target.conf = (ConfXml)serializer.Deserialize(stream);
                target.m_uuid = target.m_pipelinesUUID.ElementAt(num_pipeline);
                target.m_configurationPath = target.m_pipelinesPath.ElementAt(num_pipeline);
            }
            serializedObject.Update();
        }

        private void LoadPipelines()
        {
            serializedObject.ApplyModifiedProperties();
            string[] files = Directory.GetFiles(Application.dataPath + target.m_pipelineFolder, "*.xml");
            List<string> namesList = new List<string>();
            List<string> uuidList = new List<string>();
            List<string> pathList = new List<string>();
            foreach (var file in files)
            {
                int index = file.IndexOf(Application.dataPath);
                if (index == 0)
                {
                    string file_temp = file.Replace("\\", "/");
                    string relative_file_temp = file_temp.Remove(0, Application.dataPath.Length);

                    XElement root = null;
                    try
                    {
                        root = XElement.Load(file_temp);
                    }
                    catch (System.Xml.XmlException e)
                    {
                        Debug.Log("Configuration file " + file_temp + " has an error:" + e.Message);
                    }

                    if (root != null)
                    {
                        foreach (XElement component_interface in root.Descendants("interface"))
                        {
                            if (component_interface.Attributes("name").First().Value == "IPipeline")
                            {
                                XElement component = component_interface.Ancestors("component").First();
                                string pipelineName = component.Attribute("name").Value;
                                string pipelineUuid = component.Attribute("uuid").Value;
                                if (!string.IsNullOrEmpty(pipelineName))
                                {
                                    namesList.Add(pipelineName);
                                    uuidList.Add(pipelineUuid);
                                    pathList.Add(relative_file_temp);
                                }
                            }
                        }
                    }
                }

            }
            target.m_pipelinesName = namesList.ToArray();
            target.m_pipelinesPath = pathList.ToArray();
            target.m_pipelinesUUID = uuidList.ToArray();

            if (namesList.Count == 0)
            {
                target.m_selectedPipeline = -1;
            }
            else if ((target.m_selectedPipeline >= namesList.Count()) || target.m_selectedPipeline == -1)
            {
                target.m_selectedPipeline = 0;
            }
            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            target.m_Unity_Webcam = EditorGUILayout.Toggle("Use Unity Webcam", target.m_Unity_Webcam);
            if (target.m_Unity_Webcam)
            {
                WebCamDevice[] webCams = WebCamTexture.devices;
                List<string> webCamNames = new List<string>();

                foreach (WebCamDevice webCam in webCams)
                {
                    string webCamName = webCam.name;
                    if (webCam.isFrontFacing)
                        webCamName += " (front)";
                    webCamNames.Add(webCamName);
                }
                GUIContent label = new GUIContent("Video Camera");
                target.m_webCamNum = EditorGUILayout.Popup(label, target.m_webCamNum, webCamNames.ToArray());

                target.focalX = EditorGUILayout.FloatField("focalX ", target.focalX);
                target.focalY = EditorGUILayout.FloatField("focalY ", target.focalY);
                target.centerX = EditorGUILayout.FloatField("centerX ", target.centerX);
                target.centerY = EditorGUILayout.FloatField("centerY ", target.centerY);
                target.width = EditorGUILayout.IntField("width ", target.width);
                target.height = EditorGUILayout.IntField("height ", target.height);

            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //target.m_CustomCanvas = EditorGUILayout.Toggle("Custom_Canvas", target.m_CustomCanvas);

            //if (target.m_CustomCanvas)
            //{
            //    target.m_canvas =   (Canvas) EditorGUILayout.ObjectField( "Static Canvas UI",       target.m_canvas,    typeof(Canvas), true);
            //    target.m_material = (Material)EditorGUILayout.ObjectField("Static Canvas Material", target.m_material,  typeof(Material), true);
            //}

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            using (new GUILayout.VerticalScope(new GUIContent("<b>Pipelines</b>"), windowStyle, GUILayout.ExpandHeight(false)))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Pipelines Folder"))
                {
                    bool folderSelected = false;
                    while (!folderSelected)
                    {
                        string folder = EditorUtility.OpenFolderPanel("Select a new pipelines folder", target.m_pipelineFolder, "");
                        if (!string.IsNullOrEmpty(folder))
                        {
                            int indexApplicationDataPath = folder.IndexOf(Application.dataPath);
                            int indexStreamingAssetsPath = folder.IndexOf(Application.streamingAssetsPath);
                            if (indexApplicationDataPath != 0 || indexStreamingAssetsPath != -1)
                                EditorUtility.DisplayDialog("Pipelines folder selction error", "The folder for your pipelines must be under the Asset folder of your Unity project, but not in the StreamingAssets directory.", "OK");
                            else
                            {
                                target.m_pipelineFolder = folder.Remove(indexApplicationDataPath, Application.dataPath.Length);
                                LoadPipelines();
                                folderSelected = true;
                            }
                        }
                        else
                            folderSelected = true;
                    }
                }

                GUILayout.EndHorizontal();

                if (target.m_pipelinesName != null)
                {
                    if (target.m_pipelinesName.Length > 0 && target.m_selectedPipeline >= 0)
                    {
                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            int popupFontSize = EditorStyles.popup.fontSize;
                            EditorStyles.popup.fontSize = 12;
                            float popupFixedeight = EditorStyles.popup.fixedHeight;
                            EditorStyles.popup.fixedHeight = 15.0f;

                            target.m_selectedPipeline = EditorGUILayout.Popup(target.m_selectedPipeline, target.m_pipelinesName);

                            EditorStyles.popup.fontSize = popupFontSize;
                            EditorStyles.popup.fixedHeight = popupFixedeight;

                            if (scope.changed)
                            {
                                SelectPipeline(target.m_selectedPipeline);
                            }
                        }

                        var conf = serializedObject.FindProperty("conf");

                        bool modified = false;
                        if (conf != null)
                            OnConfGUI(conf, out modified);

                        serializedObject.ApplyModifiedProperties();

                        if (modified)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ConfXml));
                            target.conf.autoAlias = "true";

                            StringWriter stringWriter = new StringWriter();
                            serializer.Serialize(stringWriter, target.conf);
                            stringWriter.Close();

                            // Remove empties attribute "name" for factory bindings
                            var doc = XDocument.Parse(stringWriter.ToString());

                            if (doc.Element("xpcf-registry").Element("factory") != null)
                            {
                                var bindings = doc.Element("xpcf-registry").Element("factory").Elements("bindings");
                                var newBindings = new XElement("bindings");
                                foreach (var element in bindings.Elements("bind"))
                                {
                                    if (element.Attribute("name").Value.Equals(""))
                                    {
                                        element.Attribute("name").Remove();
                                    }
                                    newBindings.Add(element);
                                }
                                doc.Element("xpcf-registry").Element("factory").ReplaceNodes(newBindings);
                                StreamWriter sw = new StreamWriter(Application.dataPath + target.m_configurationPath);
                                sw.Write(doc);
                                sw.Close();
                            }
                        }
                    }
                    else
                    {
                        target.m_uuid = "";
                        target.m_configurationPath = "";
                        target.conf = null;
                        serializedObject.Update();
                    }
                }
            }

        }

        void OnConfGUI(SerializedProperty conf, out bool modified)
        {
            modified = false;
            var configuration = conf.FindPropertyRelative("properties");

            OnConfigurationGUI(configuration, out modified);
        }

        void OnConfigurationGUI(SerializedProperty configuration, out bool modified)
        {
            modified = false;
            using (new EditorGUI.IndentLevelScope())
            {
                animConfiguration.target = EditorGUILayout.Foldout(animConfiguration.target, "Configuration");

                using (var scope = new EditorGUILayout.FadeGroupScope(animConfiguration.faded))
                {
                    if (scope.visible)
                    {
                        var componentsConf = configuration.FindPropertyRelative("configure");
                        OnComponentsConfGUI(componentsConf, out modified);
                    }
                }
            }
        }

        void OnComponentsConfGUI(SerializedProperty componentsConf, out bool modified)
        {
            modified = false;
            //using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < componentsConf.arraySize; ++i)
                {
                    var componentConf = componentsConf.GetArrayElementAtIndex(i);
                    bool modified_temp = false;
                    //EditorGUILayout.PropertyField(component);
                    OnComponentConfGUI(componentConf, out modified_temp);
                    modified = modified_temp ? true : modified;
                }
            }
        }

        void OnComponentConfGUI(SerializedProperty componentConf, out bool modified)
        {
            modified = false;
            //var uuid = componentConf.FindPropertyRelative("uuid");
            var name = componentConf.FindPropertyRelative("component");
            var properties = componentConf.FindPropertyRelative("properties");
            var description = componentConf.FindPropertyRelative("description");

            var type = componentConf.FindPropertyRelative("type");
            if (string.IsNullOrEmpty(type.stringValue))
            {
                var test = name.stringValue;
                var component = target.conf.modules
                    .SelectMany(m => m.components)
                    .FirstOrDefault(c => c.name == test);
                type.stringValue = component?.name ?? "<color=red><b>Component not declared !</b></color>";
                if (component != null)
                    description.stringValue = component.description;
            }

            var label = string.Format("<b>{0}</b> ({1})", type.stringValue, name.stringValue);
            // Remove tooltip for components as its scope is on the group what deactivate the tooltips for properties !
            //var tooltip = string.Format("{0} ({1})", description.stringValue, uuid.stringValue);
            var content = new GUIContent(label);
            using (new GUILayout.VerticalScope(content, windowStyle, GUILayout.ExpandHeight(false)))
            {
                OnPropertiesGUI(properties, out modified);
            }
        }

        void OnPropertiesGUI(SerializedProperty properties, out bool modified)
        {
            modified = false;
            for (int i = 0; i < properties.arraySize; ++i)
            {
                var property = properties.GetArrayElementAtIndex(i);
                bool modified_temp = false;
                //EditorGUILayout.PropertyField(component);
                OnPropertyGUI(property, out modified_temp);
                modified = modified_temp ? true : modified;
            }
        }

        void OnPropertyGUI(SerializedProperty property, out bool modified)
        {
            modified = false;
            var name = property.FindPropertyRelative("name");
            var type = property.FindPropertyRelative("type");
            var value = property.FindPropertyRelative("value");
            var values = property.FindPropertyRelative("values");
            var description = property.FindPropertyRelative("description");

            GUIContent content;
            if (string.IsNullOrEmpty(description.stringValue))
                content = new GUIContent(name.stringValue, type.stringValue);
            else
                content = new GUIContent(name.stringValue, description.stringValue);

            switch (type.stringValue.ToLowerInvariant())
            {
                default:
                //EditorGUILayout.LabelField(content, type.stringValue);
                //break;
                case "string":
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        if (type.stringValue.ToLowerInvariant() != "string") Debug.LogError(type.stringValue);
                        string f;
                        f = EditorGUILayout.TextField(content, value.stringValue);
                        if (scope.changed)
                        {
                            value.stringValue = f;
                            modified = true;
                        }
                    }
                    break;
                case "double":
                    {
                        var enumerable = Enumerable
                            .Range(0, values.arraySize)
                            .Select(values.GetArrayElementAtIndex)
                            .DefaultIfEmpty(value);
                        foreach (var v in enumerable)
                        {
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                double f;
                                double.TryParse(v.stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                f = EditorGUILayout.DelayedDoubleField(content, f);
                                if (scope.changed)
                                {
                                    v.stringValue = f.ToString();
                                    modified = true;
                                }
                            }
                        }
                    }
                    break;
                case "float":
                    switch (values.arraySize)
                    {
                        case 0:
                        case 1:
                            if (values.arraySize == 1) value = values.GetArrayElementAtIndex(0);
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                float f;
                                float.TryParse(value.stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                f = EditorGUILayout.FloatField(content, f);
                                if (scope.changed)
                                {
                                    value.stringValue = f.ToString();
                                    modified = true;
                                }
                            }
                            break;
                        case 2:
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                var v = new Vector2();
                                for (int i = 0; i < 2; ++i)
                                {
                                    float f;
                                    float.TryParse(values.GetArrayElementAtIndex(i).stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                    v[i] = f;
                                }
                                v = EditorGUILayout.Vector2Field(content, v);
                                if (scope.changed)
                                {
                                    for (int i = 0; i < 2; ++i)
                                    {
                                        values.GetArrayElementAtIndex(i).stringValue = v[i].ToString();
                                    }
                                    modified = true;
                                }
                            }
                            break;
                        case 3:
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                var v = new Vector3();
                                for (int i = 0; i < 3; ++i)
                                {
                                    float f;
                                    float.TryParse(values.GetArrayElementAtIndex(i).stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                    v[i] = f;
                                }
                                v = EditorGUILayout.Vector3Field(content, v);
                                if (scope.changed)
                                {
                                    for (int i = 0; i < 3; ++i)
                                    {
                                        values.GetArrayElementAtIndex(i).stringValue = v[i].ToString();
                                    }
                                    modified = true;
                                }
                            }
                            break;
                        default:
                            Debug.LogError(values.arraySize);
                            var enumerable = Enumerable
                                .Range(0, values.arraySize)
                                .Select(values.GetArrayElementAtIndex)
                                .DefaultIfEmpty(value);
                            foreach (var v in enumerable)
                            {
                                using (var scope = new EditorGUI.ChangeCheckScope())
                                {
                                    float f;
                                    float.TryParse(v.stringValue, out f);
                                    f = EditorGUILayout.FloatField(content, f);
                                    if (scope.changed)
                                    {
                                        v.stringValue = f.ToString();
                                    }
                                    modified = true;
                                }
                            }
                            break;
                    }
                    break;
                case "integer":
                case "unsignedinteger":
                    switch (values.arraySize)
                    {
                        case 0:
                        case 1:
                            if (values.arraySize == 1) value = values.GetArrayElementAtIndex(0);
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                int f;
                                int.TryParse(value.stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                f = EditorGUILayout.IntField(content, f);
                                if (scope.changed)
                                {
                                    value.stringValue = f.ToString();
                                    modified = true;
                                }
                            }
                            break;
                        case 2:
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                var v = new Vector2Int();
                                for (int i = 0; i < 2; ++i)
                                {
                                    int f;
                                    int.TryParse(values.GetArrayElementAtIndex(i).stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                    v[i] = f;
                                }
                                v = EditorGUILayout.Vector2IntField(content, v);
                                if (scope.changed)
                                {
                                    for (int i = 0; i < 2; ++i)
                                    {
                                        values.GetArrayElementAtIndex(i).stringValue = v[i].ToString();
                                    }
                                    modified = true;
                                }
                            }
                            break;
                        case 3:
                            using (var scope = new EditorGUI.ChangeCheckScope())
                            {
                                var v = new Vector3Int();
                                for (int i = 0; i < 3; ++i)
                                {
                                    int f;
                                    int.TryParse(values.GetArrayElementAtIndex(i).stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out f);
                                    v[i] = f;
                                }
                                if (content.text.ToLowerInvariant().Contains("color"))
                                {
                                    var c = new Color32((byte)v.x, (byte)v.y, (byte)v.z, 0xFF);
                                    c = EditorGUILayout.ColorField(content, c);
                                    v = new Vector3Int(c.r, c.g, c.b);
                                }
                                else
                                {
                                    v = EditorGUILayout.Vector3IntField(content, v);
                                }
                                if (scope.changed)
                                {
                                    for (int i = 0; i < 3; ++i)
                                    {
                                        values.GetArrayElementAtIndex(i).stringValue = v[i].ToString();
                                    }
                                    modified = true;
                                }
                            }
                            break;
                        default:
                            Debug.LogError(values.arraySize);
                            var enumerable = Enumerable
                                .Range(0, values.arraySize)
                                .Select(values.GetArrayElementAtIndex)
                                .DefaultIfEmpty(value);
                            foreach (var v in enumerable)
                            {
                                using (var scope = new EditorGUI.ChangeCheckScope())
                                {
                                    int f;
                                    int.TryParse(v.stringValue, out f);
                                    f = EditorGUILayout.IntField(content, f);
                                    if (scope.changed)
                                    {
                                        v.stringValue = f.ToString();
                                        modified = true;
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        }
    }
}