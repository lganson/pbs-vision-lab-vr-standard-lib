using UnityEditor;
using UnityEngine;

namespace Standard_Library.Editor
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
            
                // Draw the field using Unity's native TagField drawer
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use TagSelector only on string fields.");
            }
        }
    }
    #endif
}