using UnityEditor;
using UnityEngine;

namespace Polities
{
    [CustomPropertyDrawer(typeof(Member.Unit))]
    public class UnitPropertyDrawer : PropertyDrawer
    {
        private readonly string[] nameOptions = { "testA", "testB", "testC" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty isLeaderProp = property.FindPropertyRelative("leader");

            // Get current dropdown index based on stored value
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(nameOptions, nameProp.stringValue));

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float halfWidth = (position.width - spacing) * 0.5f;

            // --- Name Dropdown (left half) ---
            Rect dropdownRect = new Rect(position.x, position.y, halfWidth, lineHeight);
            int selectedIndex = EditorGUI.Popup(dropdownRect, currentIndex, nameOptions);
            if (selectedIndex != currentIndex)
                nameProp.stringValue = nameOptions[selectedIndex];

            // --- Is Leader Toggle (right half) ---
            Rect toggleRect = new Rect(position.x + halfWidth + spacing, position.y, halfWidth, lineHeight);

            // Inline label + toggle on the same line
            EditorGUI.BeginProperty(toggleRect, GUIContent.none, isLeaderProp);
            Rect labelRect = new Rect(toggleRect.x, toggleRect.y, 60f, lineHeight);
            Rect boolRect = new Rect(toggleRect.x + 64f, toggleRect.y, toggleRect.width - 64f, lineHeight);
            EditorGUI.LabelField(labelRect, "Leader");
            isLeaderProp.boolValue = EditorGUI.Toggle(boolRect, isLeaderProp.boolValue);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Single line height — no foldout, no extra rows
            return EditorGUIUtility.singleLineHeight;
        }
    }
}