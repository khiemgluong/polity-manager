using UnityEditor;
using UnityEngine;

namespace Polity
{
    using static Manager;

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
            if (manager.factions.Count > 0)
            {
                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                                                    GUILayout.ExpandHeight(true));

                // Create the matrix GUI with headers
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(-47);
                EditorGUILayout.LabelField("", GUILayout.Width(headerWidth));
                if (manager.factions.Count > 0)
                    for (int j = manager.factions.Count - 1; j >= 0; j--)
                    {
                        GUILayout.Space(-1);
                        Rect labelRect = GUILayoutUtility.GetRect(new(manager.factions[j].Name),
                                                                    GUI.skin.label, width,
                                                                    GUILayout.Height(headerWidth));
                        RotateText(labelRect, manager.factions[j].Name, 270);
                    }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < manager.factions.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(manager.factions[i].Name, new GUIStyle(GUI.skin.label)
                    { alignment = TextAnchor.MiddleRight }, GUILayout.Width(headerWidth));
                    // Create a grid but only for entries above the diagonal
                    for (int j = manager.factions.Count - 1; j > i; j--)
                    {
                        string tooltipText = manager.factions[i].Name +
                                            " & " + manager.factions[j].Name +
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
                            manager.SerializeRelationMatrix();
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

        void GetNextRelationship(Manager manager, int i, int j)
        {
            Relation relation = manager.RelationMatrix[i, j];
            manager.RelationMatrix[i, j] = relation switch
            {
                Relation.Neutral => Relation.Allies,
                Relation.Allies => Relation.Enemies,
                Relation.Enemies => Relation.Neutral,
                _ => Relation.Neutral,
            };
        }
        void GetBackRelationship(Manager manager, int i, int j)
        {
            Relation relation = manager.RelationMatrix[i, j];
            manager.RelationMatrix[i, j] = relation switch
            {
                Relation.Neutral => Relation.Enemies,
                Relation.Enemies => Relation.Allies,
                Relation.Allies => Relation.Neutral,
                _ => Relation.Neutral,
            };
        }


    }
}