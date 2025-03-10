using UnityEditor;
using UnityEngine;
namespace KL
{
    using static PolityManager;
    [CustomEditor(typeof(PolityMember))]
    public class PolityMemberEditor : Editor
    {
        PolityManager polityManager;

        void OnEnable() => GetPolityManagerData();
        void GetPolityManagerData()
        {
            if (polityManager == null) polityManager = FindFirstObjectByType<PolityManager>();
        }
        public override void OnInspectorGUI()
        {
            if (polityManager == null)
            { GUILayout.Label("No PolityManager found in the Scene.", EditorStyles.boldLabel); return; }

            serializedObject.Update();
            SerializedProperty iD = serializedObject.FindProperty("iD");
            EditorGUILayout.PropertyField(iD, true);
            SerializedProperty polityReader = serializedObject.FindProperty("reader");
            EditorGUILayout.PropertyField(polityReader, true);
            SerializedProperty family = serializedObject.FindProperty("family");
            EditorGUILayout.PropertyField(family, true);

            SerializedProperty parentsSerializedProp = serializedObject.FindProperty("parents");
            SerializedProperty partnersSerializedProp = serializedObject.FindProperty("partners");
            SerializedProperty childrenSerializedProp = serializedObject.FindProperty("children");

            ReadOnlyPropertyField(parentsSerializedProp);
            ReadOnlyPropertyField(partnersSerializedProp);
            ReadOnlyPropertyField(childrenSerializedProp);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }

        bool InteractiveFoldout(bool foldout, string content)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 22f, new GUIStyle { fontStyle = FontStyle.Bold });
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                foldout = !foldout;
                Event.current.Use(); // Mark the event as used so it doesn't propagate further
            }
            EditorGUI.Foldout(rect, foldout, content, true);
            return foldout;
        }
        void ReadOnlyPropertyField(SerializedProperty listProperty)
        {
            if (listProperty != null && listProperty.isArray)
                if (listProperty.arraySize > 0)
                {
                    listProperty.isExpanded = InteractiveFoldout(listProperty.isExpanded, listProperty.displayName);
                    if (listProperty.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        // Temporarily disable GUI to make the properties read-only
                        GUI.enabled = false;
                        for (int i = 0; i < listProperty.arraySize; i++)
                        {
                            SerializedProperty item = listProperty.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(item, new GUIContent("Element " + i));
                        }
                        GUI.enabled = true; // Re-enable GUI after drawing the properties
                        EditorGUI.indentLevel--;
                    }
                }
        }
    }
}