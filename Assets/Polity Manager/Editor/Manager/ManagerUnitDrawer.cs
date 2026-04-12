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

            // EditorGUI.PropertyField(rect, logoProp);
            // rect.y += lineHeight + spacing;

            // Draw the object field manually
            // var currentMember = leaderProp.objectReferenceValue as Member;

            // EditorGUI.BeginChangeCheck();
            // var newMember = (Member)EditorGUI.ObjectField(rect, "Leader", currentMember, typeof(Member), true); // true = allow scene objects
            // if (EditorGUI.EndChangeCheck())
            // {
            //     if (newMember == null)
            //     {
            //         leaderProp.objectReferenceValue = null;
            //     }
            //     else
            //     {
            //         bool isSceneInstance = !EditorUtility.IsPersistent(newMember);
            //         bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(newMember);

            //         if (isSceneInstance || isPrefabInstance)
            //             leaderProp.objectReferenceValue = newMember;
            //         else
            //             Debug.LogWarning($"{newMember.name} is a prefab asset and cannot be assigned as a leader. Use a scene instance instead.");
            //     }
            // }

            if (EditorApplication.isPlaying)
            {
                // EditorGUI.PropertyField(rect, leaderProp);
                // rect.y += lineHeight + spacing;
                // EditorGUI.PropertyField(rect, membersProp, includeChildren: true);
            }

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