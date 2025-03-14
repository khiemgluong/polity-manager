using System;
using UnityEditor;
using UnityEngine;

namespace KL
{
    using static PolityManager;
    [CustomEditor(typeof(PolityManager))]
    public class PolityManagerEditor : Editor
    {
        Vector2 scrollPosition;
        const float gridSize = 20, headerWidth = 120;
        void OnEnable()
        {
            PolityManager manager = (PolityManager)target;
            if (manager.RelationMatrix == null)
                manager.LoadPolityRelationMatrix();
        }
        public override void OnInspectorGUI()
        {
            PolityManager manager = (PolityManager)target;
            GUILayoutOption width = GUILayout.Width(gridSize);
            GUILayoutOption height = GUILayout.Height(gridSize);
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            SerializedProperty polities = serializedObject.FindProperty("polities");
            EditorGUILayout.PropertyField(polities, true);
            EditorGUI.EndDisabledGroup();

            /* -------------------------------------------------------------------------- */
            /*                           POLITY RELATION MATRIX                           */
            /* -------------------------------------------------------------------------- */
            if (manager.polities.Length > 0)
            {
                SerializedProperty persist = serializedObject.FindProperty("persist");
                EditorGUILayout.PropertyField(persist, true);
                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                                                    GUILayout.ExpandHeight(true));

                // Create the matrix GUI with headers
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(-47);
                EditorGUILayout.LabelField("", GUILayout.Width(headerWidth));
                if (manager.polities.Length > 0)
                    for (int j = manager.polities.Length - 1; j >= 0; j--)
                    {
                        GUILayout.Space(-1);
                        Rect labelRect = GUILayoutUtility.GetRect(new(manager.polities[j].name),
                                                                    GUI.skin.label, width,
                                                                    GUILayout.Height(headerWidth));
                        RotateText(labelRect, manager.polities[j].name, 270);
                    }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < manager.polities.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(manager.polities[i].name, new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleRight }, GUILayout.Width(headerWidth));
                    // Create a grid but only for entries above the diagonal
                    for (int j = manager.polities.Length - 1; j > i; j--)
                    {
                        string tooltipText = manager.polities[i].name +
                                            " & " + manager.polities[j].name +
                                            " | " + manager.RelationMatrix[i, j];

                        GUIContent buttonContent = new("", tooltipText);
                        Rect gridRect = EditorGUILayout.GetControlRect(width, height);

                        if (GUI.Button(gridRect, buttonContent))
                        {
                            switch (Event.current.button)
                            {
                                case 0: // Left mouse button
                                    GetNextRelationship(manager, i, j); break;
                                case 1: // Right mouse button
                                    GetBackRelationship(manager, i, j); break;
                                default: break;
                            }
                            //Set reciprocal
                            manager.RelationMatrix[j, i] = manager.RelationMatrix[i, j];
                            if (Application.isPlaying) OnRelationChange?.Invoke();
                        }

                        Color color = GetColorForRelationship(manager.RelationMatrix[i, j]);
                        EditorGUI.DrawRect(gridRect, color);
                        GUI.Label(gridRect, ""); // Optionally add labels or icons
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView(); GUILayout.EndVertical();
            }
            /* -------------------------------------------------------------------------- */
            /*                         END POLITY RELATION MATRIX                         */
            /* -------------------------------------------------------------------------- */
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (!Application.isPlaying) if (GUILayout.Button("Polity Family Graph"))
                    EditorWindow.GetWindow<PolityFamilyGraph>("Polity Family Graph");
            GUIStyle rightAlignedStyle = new(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField("Version 2.0.0", rightAlignedStyle);
            EditorGUILayout.EndHorizontal();
            // Save changes
            if (GUI.changed)
            {
                if (!Application.isPlaying)
                    manager.SerializeRelationMatrix();
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

        Color GetColorForRelationship(PolityRelation relationship)
        {
            return relationship switch
            {
                PolityRelation.Neutral => Color.yellow,
                PolityRelation.Allies => Color.green,
                PolityRelation.Enemies => Color.red,
                _ => Color.white,
            };
        }

        void GetNextRelationship(PolityManager manager, int i, int j)
        {
            PolityRelation relation = manager.RelationMatrix[i, j];
            manager.RelationMatrix[i, j] = relation switch
            {
                PolityRelation.Neutral => PolityRelation.Allies,
                PolityRelation.Allies => PolityRelation.Enemies,
                PolityRelation.Enemies => PolityRelation.Neutral,
                _ => PolityRelation.Neutral,
            };
        }
        void GetBackRelationship(PolityManager manager, int i, int j)
        {
            PolityRelation relation = manager.RelationMatrix[i, j];
            manager.RelationMatrix[i, j] = relation switch
            {
                PolityRelation.Neutral => PolityRelation.Enemies,
                PolityRelation.Enemies => PolityRelation.Allies,
                PolityRelation.Allies => PolityRelation.Neutral,
                _ => PolityRelation.Neutral,
            };
        }
    }
}