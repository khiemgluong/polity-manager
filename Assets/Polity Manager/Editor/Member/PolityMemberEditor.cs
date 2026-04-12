using UnityEditor;
using UnityEngine;

namespace Polity
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Member))]
    public class PolityMemberEditor : Editor
    {
        Manager manager;
        string polityName;
        Member member;
        SerializedProperty factionProp;
        void OnEnable()
        {
            if (manager == null) manager = FindFirstObjectByType<Manager>();
            member = (Member)target; // Cache before destruction nullifies it
            serializedObject.Update(); // Sync serialized data before reading

            factionProp = serializedObject.FindProperty("faction");
            polityName = factionProp.FindPropertyRelative("name").stringValue;

            if (string.IsNullOrEmpty(polityName))
                polityName = member.faction.name;

            Debug.Log($"OnEnable: Current Member Polity: {polityName}");
        }
        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            if (manager == null)
            {
                GUILayout.Label("No PolityManager found in the Scene.", EditorStyles.boldLabel);
                return;
            }

            serializedObject.Update();

            SerializedProperty groupProp = serializedObject.FindProperty("group");

            EditorGUI.BeginChangeCheck();

            Rect totalRect = EditorGUILayout.GetControlRect();
            float halfWidth = (totalRect.width - EditorGUIUtility.standardVerticalSpacing) / 2f;

            Rect factionRect = new Rect(totalRect.x, totalRect.y, halfWidth, totalRect.height);
            Rect groupRect = new Rect(totalRect.x + halfWidth + EditorGUIUtility.standardVerticalSpacing,
                                        totalRect.y, halfWidth, totalRect.height);

            EditorGUIUtility.labelWidth = halfWidth * 0.4f;

            EditorGUI.showMixedValue = factionProp.hasMultipleDifferentValues;
            EditorGUI.PropertyField(factionRect, factionProp, GUIContent.none);

            EditorGUI.showMixedValue = groupProp.hasMultipleDifferentValues;
            EditorGUI.PropertyField(groupRect, groupProp, GUIContent.none);

            EditorGUI.showMixedValue = false; // always reset
            EditorGUIUtility.labelWidth = 0; // reset to default

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                polityName = factionProp.FindPropertyRelative("name").stringValue;

                // Iterate all selected targets individually
                foreach (var t in targets)
                {
                    var member = (Member)t;
                    member.faction.name = polityName; // Update live object
                    EditorUtility.SetDirty(t);
                }
            }

            serializedObject.ApplyModifiedProperties();

            // Check if the target is part of a prefab
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(member);
            bool isPrefabAsset = prefabAssetType != PrefabAssetType.NotAPrefab;

            // Check if it's a prefab INSTANCE in the scene (not the asset itself)
            bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(member);

            // Check if it's the actual prefab ASSET (in the Project window)
            bool isPrefabSource = PrefabUtility.IsPartOfPrefabAsset(member);

            if (isPrefabInstance)
                EditorGUILayout.HelpBox("This is a Prefab Instance in the scene.", MessageType.Info);
            else if (isPrefabSource)
                EditorGUILayout.HelpBox("This is a Prefab Asset.", MessageType.Info);
            else
                EditorGUILayout.HelpBox("This is NOT a prefab.", MessageType.None);
        }
    }
}