using UnityEngine;
using UnityEditor;

namespace Polity
{
    [CustomPropertyDrawer(typeof(Manager.Group))]
    public class UnitDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProp = property.FindPropertyRelative("name");
            // var logoProp = property.FindPropertyRelative("logo");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            var rect = new Rect(position.x, position.y, position.width, lineHeight);

            EditorGUI.PropertyField(rect, nameProp);
            rect.y += lineHeight + spacing;

          

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // name
            float height = lineHeight + spacing;
            // logo
            // height += lineHeight + spacing;
            // if (EditorApplication.isPlaying)
            // {
            //     //members
            //     var membersProp = property.FindPropertyRelative("members");
            //     height += EditorGUI.GetPropertyHeight(membersProp, includeChildren: true);
            // }

            return height;
        }
    }
}