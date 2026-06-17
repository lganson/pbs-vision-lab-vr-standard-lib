using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Standard_Library.Editor
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] only on strings.");
                return;
            }

            // Get all active scenes in the Build Settings
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            if (scenes.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "No scenes in Build Settings.");
                return;
            }

            List<string> sceneNames = new List<string>();
            int selectedIndex = 0;

            for (int i = 0; i < scenes.Length; i++)
            {
                // Extract the scene name from its file path
                string path = scenes[i].path;
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                sceneNames.Add(name);

                // Check if this is the currently saved scene string
                if (property.stringValue == name)
                {
                    selectedIndex = i;
                }
            }

            // Display the dropdown menu in the inspector
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames.ToArray());

            // Update the actual string property value with the chosen scene
            property.stringValue = sceneNames[selectedIndex];
        }
    }
    #endif
}
