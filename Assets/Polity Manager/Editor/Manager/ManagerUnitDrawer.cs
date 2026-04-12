using UnityEngine;
using UnityEditor;

namespace Polities
{
    [CustomPropertyDrawer(typeof(Manager.Group))]
    public class UnitDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProp = property.FindPropertyRelative("name");
            var leaderProp = property.FindPropertyRelative("leader");
            var membersProp = property.FindPropertyRelative("members");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            var rect = new Rect(position.x, position.y, position.width, lineHeight);

            EditorGUI.PropertyField(rect, nameProp);
            rect.y += lineHeight + spacing;

            EditorGUI.PropertyField(rect, leaderProp);
            rect.y += lineHeight + spacing;

            if (EditorApplication.isPlaying)
            {
                EditorGUI.PropertyField(rect, membersProp, includeChildren: true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // name + leader
            float height = (lineHeight + spacing) * 2;

            if (EditorApplication.isPlaying)
            {
                var membersProp = property.FindPropertyRelative("members");
                height += EditorGUI.GetPropertyHeight(membersProp, includeChildren: true);
            }

            return height;
        }
    }
}