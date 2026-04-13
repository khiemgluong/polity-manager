using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [CustomEditor(typeof(Manager))]
    public partial class PolityManagerEditor : Editor
    {
        Vector2 scrollPosition;
        Manager manager;
        const float gridSize = 20, headerWidth = 120;
        void OnEnable()
        {
            manager = (Manager)target;
            if (manager.RelationMatrix == null)
                manager.LoadRelationMatrix();
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            DrawFactionStringFields();

            // SerializedProperty factions = serializedObject.FindProperty("factions");
            // EditorGUILayout.PropertyField(factions, true);

            EditorGUI.EndDisabledGroup();

            DataCheck();

            // if (factions.isExpanded)
            FactionsMatrix();

            GUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();
            GUIStyle rightAlignedStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField($"Version {VERSION}", rightAlignedStyle);
            EditorGUILayout.EndHorizontal();
            // Save changes
            if (GUI.changed)
            {
                // if (!Application.isPlaying)
                //     manager.SerializeRelationMatrix();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(manager);
            }
        }

        void RotateText(Rect rect, string text, float angle)
        {
            Matrix4x4 matrixBackup = GUI.matrix;
            // Recalculate pivot point to be at the center bottom of the initial rectangle.
            Vector2 pivotPoint = new(rect.x + rect.height / 2, rect.y + rect.width / 2);
            GUIUtility.RotateAroundPivot(angle, pivotPoint);
            Rect adjustedRect = new(rect.x - 50, rect.y, headerWidth, gridSize);

            // EditorGUI.DrawRect(adjustedRect, new Color(0.8f, 0.8f, 0.8f, 0.5f));
            GUIStyle style = new(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
            GUI.Label(adjustedRect, text, style); GUI.matrix = matrixBackup;
        }

        void DrawFactionStringFields()
        {
            SerializedProperty factionsProp = serializedObject.FindProperty("factions");

            // Array size field
            // EditorGUILayout.PropertyField(factionsProp.FindPropertyRelative("Array.size"));

            for (int i = 0; i < factionsProp.arraySize; i++)
            {
                SerializedProperty factionProp = factionsProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = factionProp.FindPropertyRelative("name");

                EditorGUILayout.BeginHorizontal();

                nameProp.stringValue = EditorGUILayout.TextField($"", nameProp.stringValue);

                // Remove button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    factionsProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Add button
            if (GUILayout.Button("Add Faction"))
                factionsProp.arraySize++;
        }

        Color GetColorForRelationship(Relation relationship)
        {
            return relationship switch
            {
                Relation.Neutral => Color.yellow,
                Relation.Allies => Color.green,
                Relation.Enemies => Color.red,
                _ => Color.white,
            };
        }

        void DataCheck()
        {
            EditorGUILayout.BeginHorizontal();

            int emptyPolityNameCount = 0;
            foreach (var polity in manager.factions)
            {
                if (string.IsNullOrEmpty(polity.name))
                    emptyPolityNameCount++;
            }
            if (emptyPolityNameCount > 0)
            {
                if (emptyPolityNameCount == 1) EditorGUILayout.HelpBox(
                    $"There is one faction with no name.", MessageType.Error);
                else EditorGUILayout.HelpBox(
                    $"There are {emptyPolityNameCount} factions with no name.", MessageType.Error);
            }

            var duplicateNames = manager.factions
                .Where(p => !string.IsNullOrEmpty(p.name))
                .GroupBy(p => p.name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Count > 0)
            {
                string names = string.Join(", ", duplicateNames.Select(n => $"\"{n}\""));
                string message = duplicateNames.Count == 1
                    ? $"Duplicate faction name: {names}"
                    : $"Duplicate faction names: {names}";

                EditorGUILayout.HelpBox(message, MessageType.Error);
            }
            EditorGUILayout.EndHorizontal();

        }

    }
}
