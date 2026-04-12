using UnityEditor;
using UnityEngine;

namespace Polities
{
    public partial class PolityManagerEditor : Editor
    {
        void FactionsMatrix()
        {
            /* -------------------------------------------------------------------------- */
            /*                          FACTIONS RELATION MATRIX                          */
            /* -------------------------------------------------------------------------- */
            Manager manager = (Manager)target;
            GUILayoutOption width = GUILayout.Width(gridSize);
            GUILayoutOption height = GUILayout.Height(gridSize);
            if (manager.factions.Length > 0)
            {
                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                                                    GUILayout.ExpandHeight(true));

                // Create the matrix GUI with headers
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(-47);
                EditorGUILayout.LabelField("", GUILayout.Width(headerWidth));
                if (manager.factions.Length > 0)
                    for (int j = manager.factions.Length - 1; j >= 0; j--)
                    {
                        GUILayout.Space(-1);
                        Rect labelRect = GUILayoutUtility.GetRect(new(manager.factions[j].name),
                                                                    GUI.skin.label, width,
                                                                    GUILayout.Height(headerWidth));
                        RotateText(labelRect, manager.factions[j].name, 270);
                    }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < manager.factions.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(manager.factions[i].name, new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleRight }, GUILayout.Width(headerWidth));
                    // Create a grid but only for entries above the diagonal
                    for (int j = manager.factions.Length - 1; j > i; j--)
                    {
                        string tooltipText = manager.factions[i].name +
                                            " & " + manager.factions[j].name +
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
                            if (Application.isPlaying) Manager.OnRelationChange?.Invoke();
                        }

                        Color color = GetColorForRelationship(manager.RelationMatrix[i, j]);
                        EditorGUI.DrawRect(gridRect, color);
                        GUI.Label(gridRect, ""); // Optionally add labels or icons
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView(); GUILayout.EndVertical();
            }
        }
    }
}