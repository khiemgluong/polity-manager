#if UNITY_EDITOR
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

            // if (Application.isPlaying)
            // {
            if (property.objectReferenceValue == null)
            {
                // Do not render anything if not assigned
            }
            else if (!IsSelfReference(property))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(position, property, typeof(Leader), label);
                EditorGUI.EndDisabledGroup();
            }
            // }
            // else
            //     EditorGUI.ObjectField(position, property, typeof(Leader), label);

            EditorGUI.EndProperty();
        }

        bool IsSelfReference(SerializedProperty property)
        {
            var leader = property.objectReferenceValue as Leader;
            if (leader == null)
                return false;

            var targetObject = property.serializedObject.targetObject;

            // If NPC is a MonoBehaviour
            if (targetObject is Component comp)
                return leader.gameObject == comp.gameObject;

            // If NPC is a serialized class inside a MonoBehaviour
            var mb = property.serializedObject.targetObject as MonoBehaviour;
            if (mb != null)
                return leader.gameObject == mb.gameObject;

            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null || IsSelfReference(property))
                return -EditorGUIUtility.standardVerticalSpacing;
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif