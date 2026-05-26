#if UNITY_EDITOR
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
                bool foundHover = false;
                bool isMiddleMouse = Event.current.button == 2;
                bool isDragOrDown = Event.current.type == EventType.MouseDown
                                || Event.current.type == EventType.MouseDrag;
                bool isMiddleMouseHeld = isMiddleMouse && isDragOrDown;

                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                                                    GUILayout.ExpandHeight(true));

                // Create the matrix GUI with headers
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(-47);
                EditorGUILayout.LabelField("", GUILayout.Width(headerWidth));
                if (manager.factions.Count > 0)
                {
                    for (int j = manager.factions.Count - 1; j >= 0; j--)
                    {
                        GUILayout.Space(-1);
                        Rect labelRect = GUILayoutUtility.GetRect(
                            new(manager.factions[j].Name),
                            GUI.skin.label, width,
                            GUILayout.Height(headerWidth));

                        // Highlight header if it's the hovered column
                        bool isHovered = (j == hoverCol);
                        if (isHovered)
                        {
                            Rect highlightRect = new(
                                labelRect.x + 50,
                                labelRect.y,
                                labelRect.width,
                                labelRect.height);
                            EditorGUI.DrawRect(highlightRect, new(0.5f, 0.5f, 0.5f, 0.2f));

                            // Draw vertical bar for the column
                            Rect columnBarRect = new(
                                labelRect.x + 50,
                                labelRect.y + labelRect.height,
                                gridSize,
                                j * gridSize);
                            EditorGUI.DrawRect(columnBarRect, new(0.5f, 0.5f, 0.5f, 0.35f));
                        }

                        RotateText(new(labelRect.x, labelRect.y,
                            labelRect.width, labelRect.height),
                            manager.factions[j].Name, 270, isHovered);
                    }
                    EditorGUILayout.EndHorizontal();

                    GUIStyle sideLabelStyle = new(GUI.skin.label)
                    { alignment = TextAnchor.MiddleRight };
                    GUIStyle boldSideLabelStyle = new(sideLabelStyle)
                    { fontStyle = FontStyle.Bold };

                    for (int i = 0; i < manager.factions.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        // Side label highlighting and bolding
                        Rect sideLabelRect = EditorGUILayout.GetControlRect(
                            GUILayout.Width(headerWidth), GUILayout.Height(gridSize));
                        bool isRowHighlight = (i == hoverRow);
                        if (isRowHighlight)
                        {
                            EditorGUI.DrawRect(sideLabelRect, new(0.5f, 0.5f, 0.5f, 0.2f));

                            // Draw horizontal bar for the row
                            Rect rowBarRect = new(
                                sideLabelRect.x + sideLabelRect.width, sideLabelRect.y,
                                (manager.factions.Count - i - 1) * gridSize, gridSize);
                            EditorGUI.DrawRect(rowBarRect, new(0.5f, 0.5f, 0.5f, 0.35f));
                        }

                        EditorGUI.LabelField(sideLabelRect, manager.factions[i].Name,
                            isRowHighlight ? boldSideLabelStyle : sideLabelStyle);

                        // Create a grid but only for entries above the diagonal
                        for (int j = manager.factions.Count - 1; j > i; j--)
                        {
                            string tooltipText =
                                manager.factions[i].Name + " & " + manager.factions[j].Name +
                                " | " + ToPluralString(manager.RelationMatrix[i, j]);

                            GUIContent buttonContent = new("", tooltipText);
                            Rect gridRect = EditorGUILayout.GetControlRect(width, height);

                            // Hover detection (only if middle mouse is held)
                            if (isMiddleMouseHeld && gridRect.Contains(Event.current.mousePosition))
                            {
                                foundHover = true;
                                if (hoverRow != i || hoverCol != j)
                                {
                                    hoverRow = i;
                                    hoverCol = j;
                                    Repaint();
                                }
                            }

                            Color color = GetColorForRelationship(manager.RelationMatrix[i, j]);
                            EditorGUI.DrawRect(gridRect, color);

                            if (GUI.Button(gridRect, buttonContent, GUIStyle.none))
                            {
                                switch (Event.current.button)
                                {
                                    case 0: // Left mouse button
                                        GetNextRelationship(manager, i, j); break;
                                    case 1: // Right mouse button
                                        GetBackRelationship(manager, i, j); break;
                                    default: break;
                                }
                                manager.RelationMatrix[j, i] = manager.RelationMatrix[i, j];
                                manager.SerializeRelationMatrix();
                                if (Application.isPlaying) Manager.OnRelationChange?.Invoke();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();

                if (Event.current.type != EventType.Layout &&
                    Event.current.type != EventType.Repaint &&
                    (!foundHover || !isMiddleMouseHeld))
                    if (hoverRow != -1 || hoverCol != -1)
                    {
                        hoverRow = -1;
                        hoverCol = -1;
                        Repaint();
                    }

                GUILayout.EndVertical();
            }


            string ToPluralString(Relation relation)
            {
                return relation switch
                {
                    Relation.Neutral => "Neutral",
                    Relation.Ally => "Allies",
                    Relation.Enemy => "Enemies",
                    _ => relation.ToString()
                };
            }

            void GetNextRelationship(Manager manager, int i, int j)
            {
                Relation relation = manager.RelationMatrix[i, j];
                manager.RelationMatrix[i, j] = relation switch
                {
                    Relation.Neutral => Relation.Ally,
                    Relation.Ally => Relation.Enemy,
                    Relation.Enemy => Relation.Neutral,
                    _ => Relation.Neutral,
                };
            }
            void GetBackRelationship(Manager manager, int i, int j)
            {
                Relation relation = manager.RelationMatrix[i, j];
                manager.RelationMatrix[i, j] = relation switch
                {
                    Relation.Neutral => Relation.Enemy,
                    Relation.Enemy => Relation.Ally,
                    Relation.Ally => Relation.Neutral,
                    _ => Relation.Neutral,
                };
            }
        }
    }
}
#endif