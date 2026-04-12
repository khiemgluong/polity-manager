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
        // SerializedProperty factionProp;
        void OnEnable()
        {
            if (manager == null) manager = FindFirstObjectByType<Manager>();
            member = (Member)target; // Cache before destruction nullifies it
            serializedObject.Update(); // Sync serialized data before reading

            // polityName = factionProp.FindPropertyRelative("name").stringValue;

            if (string.IsNullOrEmpty(polityName))
                polityName = member.faction.name;

            Debug.Log($"OnEnable: Current Member Polity: {polityName}");
        }
        void OnDisable()
        {

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                // Skip the script field
                if (prop.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
        // serializedObject.ApplyModifiedProperties();

        // // Check if the target is part of a prefab
        // PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(member);
        // bool isPrefabAsset = prefabAssetType != PrefabAssetType.NotAPrefab;

        // // Check if it's a prefab INSTANCE in the scene (not the asset itself)
        // bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(member);

        // // Check if it's the actual prefab ASSET (in the Project window)
        // bool isPrefabSource = PrefabUtility.IsPartOfPrefabAsset(member);

        // if (isPrefabInstance)
        //     EditorGUILayout.HelpBox("This is a Prefab Instance in the scene.", MessageType.Info);
        // else if (isPrefabSource)
        //     EditorGUILayout.HelpBox("This is a Prefab Asset.", MessageType.Info);
        // else
        //     EditorGUILayout.HelpBox("This is NOT a prefab.", MessageType.None);
    }
}