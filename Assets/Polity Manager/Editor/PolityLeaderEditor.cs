using UnityEditor;
using UnityEngine;

namespace Polity
{

    [CustomEditor(typeof(Leader))]
    public class LeaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Leader leader = (Leader)target;
            bool hasMember = leader.GetComponent<IMember>() != null;
            Debug.Log($"Leader {leader.name} has member: {hasMember}");

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true); // Enter first property (m_Script)

            while (property.NextVisible(false))
            {
                if (property.name == "Faction" && hasMember)
                    continue;

                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.IntField("Members", leader.members.Count);
        }
    }
}