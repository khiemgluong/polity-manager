using UnityEditor;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [CustomPropertyDrawer(typeof(Reader), true)]
    public class PolityReaderDrawer : PropertyDrawer
    {
        Manager manager;
        string[] names;
        string[] groups;
        bool _stacked;
        const float minWidth = 300f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<Manager>();
                if (manager != null && manager.factions != null)
                {
                    names = new string[manager.factions.Length];
                    for (int i = 0; i < manager.factions.Length; i++)
                    {
                        names[i] = manager.factions[i].name;

                        groups = new string[manager.factions[i].groups.Count];
                        for (int j = 0; j < manager.factions[i].groups.Count; j++)
                            groups[j] = manager.factions[i].groups[j].name;

                    }
                }
            }

            if (manager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }
            if (Application.isPlaying) property.serializedObject.Update();

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            position.y += spacing;

            Rect nameRect, groupRect;
            if (_stacked)
            {
                nameRect = new Rect(position.x, position.y, position.width, lineHeight);
                groupRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
            }
            else
            {
                float halfWidth = (position.width - spacing) / 2f;
                nameRect = new Rect(position.x, position.y, halfWidth, lineHeight);
                groupRect = new Rect(position.x + halfWidth + spacing, position.y, halfWidth, lineHeight);
            }

            EditorGUI.BeginProperty(position, label, property);

            UpdateFactionNames();
            SerializedProperty factionProp = property.FindPropertyRelative("faction");
            // EditorGUI.BeginChangeCheck();
            int currentFactionIndex = Mathf.Max(0, System.Array.IndexOf(names, factionProp.stringValue));
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(nameRect, tooltip);
            int factionIndex = EditorGUI.Popup(nameRect, currentFactionIndex, names);
            // if (EditorGUI.EndChangeCheck())
            factionProp.stringValue = names[factionIndex];

            SerializedProperty groupProp = property.FindPropertyRelative("group");
            if (!GetPolityGroups(names[factionIndex]))
            {
                EditorGUI.LabelField(groupRect, $"No groups found for faction.");
                groupProp.stringValue = null;
                EditorGUI.EndProperty();
                return;
            }
            // EditorGUI.BeginChangeCheck();
            int currentGroupIndex = Mathf.Max(0, System.Array.IndexOf(groups, groupProp.stringValue));
            GUIContent tooltip1 = new("", "Group");
            EditorGUI.LabelField(groupRect, tooltip1);
            int selectedGroupIndex = EditorGUI.Popup(groupRect, currentGroupIndex, groups);
            // if (EditorGUI.EndChangeCheck())
            groupProp.stringValue = groups[selectedGroupIndex];

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            _stacked = EditorGUIUtility.currentViewWidth < minWidth;
            return (_stacked ? (lineHeight * 2) + spacing : lineHeight) + spacing;
        }

        void UpdateFactionNames()
        {

            Faction[] factions = manager.factions;
            names = new string[factions.Length];
            for (int i = 0; i < factions.Length; i++)
                names[i] = factions[i].name;
        }

        bool GetPolityGroups(string factionName)
        {
            Faction[] factions = manager.factions;
            foreach (Faction faction in factions)
            {
                if (faction.name.Equals(factionName))
                {
                    if (faction.groups == null || faction.groups.Count == 0)
                    {
                        groups = new string[0];
                        return false;
                    }

                    groups = new string[faction.groups.Count + 1];
                    groups[0] = "\t";
                    for (int i = 0; i < faction.groups.Count; i++)
                        groups[i + 1] = faction.groups[i].name;
                    return true;
                }
            }
            groups = new string[0];
            // Debug.LogWarning($"Faction '{factionName}' not found. Defaulting to empty unit options.");
            return false;
        }
    }
}