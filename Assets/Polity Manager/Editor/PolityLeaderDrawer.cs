// Editor/LeaderDrawer.cs
using UnityEditor;
using UnityEngine;

namespace Polity
{
    // Custom property drawer to show the Leader reference in the inspector as read-only.
    // This makes it clear that the Leader is assigned via code and not meant to be set by the user.

    [CustomPropertyDrawer(typeof(Leader))]
    public class LeaderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.objectReferenceValue == null)
            {
                // EditorGUI.BeginDisabledGroup(true);
                // EditorGUI.LabelField(position, label, new GUIContent("— Assign via code only —"));
                // EditorGUI.EndDisabledGroup();
            }
            else
            {
                // Show the reference but locked — user can see it, not change it
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(position, property, typeof(Leader), label);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return 0;
            return EditorGUIUtility.singleLineHeight;
        }
    }
}