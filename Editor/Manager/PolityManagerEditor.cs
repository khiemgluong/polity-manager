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
        int hoverRow = -1, hoverCol = -1;

        void OnEnable()
        {
            manager = (Manager)target;
            if (manager.RelationMatrix == null)
                manager.LoadRelationMatrix();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode
            && !manager.CheckFactionNames(out string error))
            {
                EditorApplication.isPlaying = false;
                Debug.LogError($"PolityManager: {error}", manager);
            }
        }

        public override void OnInspectorGUI()
        {
            // Ensure we repaint when the mouse moves to update highlighting
            if (Event.current.type == EventType.MouseMove) Repaint();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            DrawFactionStringFields();

            // SerializedProperty factions = serializedObject.FindProperty("factions");
            // EditorGUILayout.PropertyField(factions, true);

            EditorGUI.EndDisabledGroup();

            DataCheck();

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

            foreach (Faction faction in manager.factions)
                faction.Name = faction.Name?.Trim();

            // Save changes
            if (GUI.changed)
            {
                // if (!Application.isPlaying)
                //     manager.SerializeRelationMatrix();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(manager);
            }
        }

        GUIStyle _headerStyle;
        GUIStyle _boldHeaderStyle;

        void RotateText(Rect rect, string text, float angle, bool bold = false)
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };
                _boldHeaderStyle = new GUIStyle(_headerStyle) { fontStyle = FontStyle.Bold };
            }

            Matrix4x4 matrixBackup = GUI.matrix;
            // Recalculate pivot point to be at the center bottom of the initial rectangle.
            Vector2 pivotPoint = new(rect.x + rect.height / 2, rect.y + rect.width / 2);
            GUIUtility.RotateAroundPivot(angle, pivotPoint);
            Rect adjustedRect = new(rect.x - 50, rect.y, headerWidth, gridSize);

            GUI.Label(adjustedRect, text, bold ? _boldHeaderStyle : _headerStyle);
            GUI.matrix = matrixBackup;
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

                EditorGUI.BeginChangeCheck();
                string newName = EditorGUILayout.TextField($"", nameProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    nameProp.stringValue = newName;
                    // Trigger the event-driven logic by setting the property on the object
                    if (i < manager.factions.Count)
                        manager.factions[i].Name = newName;
                }

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
            {
                factionsProp.arraySize++;
                // Undo.RecordObject(manager, "Add Faction");
                // manager.AddFaction($"New Faction {manager.factions.Count}");
                // serializedObject.Update();
            }
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

            int emptyPolityNameCount = manager.factions
                .Count(p => string.IsNullOrEmpty(p.Name));

            if (emptyPolityNameCount > 0)
            {
                string msg = emptyPolityNameCount == 1
                    ? "There is one faction with no name."
                    : $"There are {emptyPolityNameCount} factions with no name.";
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }

            var duplicateNames = manager.factions
                .Where(p => !string.IsNullOrEmpty(p.Name))
                .GroupBy(p => p.Name)
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
