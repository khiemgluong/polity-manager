using UnityEditor;
using UnityEngine;
namespace Polities
{
    [CustomEditor(typeof(Member))]
    public class PolityMemberEditor : Editor
    {
        Manager manager;
        string polityName;
        string unitName;
        void OnEnable()
        {
            if (manager == null) manager = FindFirstObjectByType<Manager>();
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        void OnDisable()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        void OnHierarchyChanged()
        {
            // target will be null if the GameObject was destroyed
            if (target == null)
            {
                // RemoveMemberFromCurrentUnit(member);
                EditorUtility.SetDirty(manager);
            }
        }

        public override void OnInspectorGUI()
        {
            Member member = (Member)target;
            if (manager == null)
            {
                GUILayout.Label("No PolityManager found in the Scene.", EditorStyles.boldLabel);
                return;
            }

            serializedObject.Update();
            SerializedProperty polityProp = serializedObject.FindProperty("polity");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(polityProp, true);
            if (EditorGUI.EndChangeCheck())
            {
                polityName = polityProp.FindPropertyRelative("name").stringValue;
                Debug.Log($"Polity changed to: {polityName}");
                // serializedObject.ApplyModifiedProperties();
                // EditorUtility.SetDirty(target);
            }
            // GUI.enabled = false;
            // GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            SerializedProperty unitProp = serializedObject.FindProperty("unit");
            EditorGUILayout.PropertyField(unitProp, true);
            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log($"Unit changed to: {unit.stringValue ?? "None"}");
                string unitName = unitProp.FindPropertyRelative("name").stringValue;
                Debug.Log($"Unit changed to: {unitName}");
                if (!string.IsNullOrEmpty(unitName))
                {
                    Manager.Polity polity1 = manager.GetPolity(polityProp.FindPropertyRelative("name").stringValue);
                    foreach (var unit1 in polity1.units)
                        if (unit1.name.Equals(unitName) && !unit1.members.Contains(member))
                        {
                            unit1.members.Add(member);
                            Debug.Log($"Added Member to Unit '{unitName}' in Polity '{polityProp.FindPropertyRelative("name").stringValue}'");
                        }

                    Debug.Log($"Unit '{unitName}' assigned to Member in Polity '{polityProp.FindPropertyRelative("name").stringValue}'");
                }
                else
                    Debug.Log($"Unit cleared for Member in Polity '{polityProp.FindPropertyRelative("name").stringValue}'");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(target);
        }
    }
}