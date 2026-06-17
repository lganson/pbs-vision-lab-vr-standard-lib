using System;
using UnityEditor;
using UnityEngine;

namespace Standard_Library.Editor
{
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] only on strings.");
                return;
            }
            
        }
    }
}