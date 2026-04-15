using UnityEditor;
using UnityEngine;

namespace Polity
{
    [CustomEditor(typeof(Leader))]
    public class LeaderEditor : Editor
    {
        private bool membersFoldout = false;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Leader leader = (Leader)target;
            bool hasMember = leader.GetComponent<IMember>() != null;

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true); // Enter first property (m_Script)

            while (property.NextVisible(false))
            {
                if (property.name == "Faction" && hasMember)
                    continue;

                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();

            if (leader.members.Count != 0)
            {
                membersFoldout = EditorGUILayout.Foldout(membersFoldout, $"Members ({leader.members.Count})", true);
                if (membersFoldout)
                {
                    EditorGUI.indentLevel++;

                    foreach (IMember m in leader.members)
                    {
                        if (m == null) continue;
                        if (GUILayout.Button(m.transform.name, EditorStyles.objectField))
                        {
                            // Selection.activeGameObject = m.transform.gameObject;
                            EditorGUIUtility.PingObject(m.transform.gameObject);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }

        }

        void OnSceneGUI()
        {
            Leader leader = (Leader)target;
            if (leader.formation == null || leader.members.Count == 0) return;
            Formation(leader);
        }

        private const float RayLength = 1.5f;
        private const float SlotRadius = 0.18f;
        private static readonly Color SlotColor = new Color(0.3f, 0.85f, 1f, 0.9f);
        private static readonly Color RayColor = new Color(0.3f, 0.85f, 1f, 0.6f);
        private static readonly Color LabelColor = new Color(1f, 1f, 1f, 0.75f);
        void Formation(Leader leader)
        {
            var formation = leader.formation;
            int index = 0;

            foreach (var (member, localOffset) in formation.Offsets)
            {
                // Mirror Formation.GetPosition — rotate offset by leader facing
                Vector3 rotated = leader.transform.rotation * localOffset;
                Vector3 worldTarget = leader.transform.position + rotated;

                // Draw slot disc
                Handles.color = SlotColor;
                Handles.DrawSolidDisc(worldTarget, Vector3.up, SlotRadius);

                // Draw upward ray
                Handles.color = RayColor;
                Handles.DrawLine(worldTarget, worldTarget + Vector3.up * RayLength, 2f);

                // Draw dashed line from leader to slot
                Handles.color = new Color(1f, 1f, 1f, 0.2f);
                Handles.DrawDottedLine(leader.transform.position, worldTarget, 4f);

                // Label (slot index + member name if available)
                string label = $"[{index}] {member?.transform?.name ?? "empty"}";
                Handles.Label(worldTarget + Vector3.up * (RayLength + 0.1f), label,
                    new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = LabelColor } });
                // Debug.Log($"Formation slot {index}: {label} at {worldTarget}");
                index++;
            }

            // Outline the whole formation bounding area
            Handles.color = new Color(0.3f, 0.85f, 1f, 0.12f);
            Handles.DrawWireDisc(leader.transform.position, Vector3.up, formation.Offsets.Count * 0.4f);
        }
    }
}