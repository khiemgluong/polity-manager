using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Polity
{
    using static Manager;
    [CustomPropertyDrawer(typeof(Faction), true)]
    public class PolityDrawer : PropertyDrawer
    {
        Manager manager;
        string[] names;
        string[] groups;
        const float minWidth = 400f;

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
                        {
                            groups[j] = manager.factions[i].groups[j].name;
                            Debug.Log($"Faction '{manager.factions[i].name}' has group: '{manager.factions[i].groups[j].name}'");
                        }
                    }
                }
            }

            if (manager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

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

            SerializedProperty nameProp = property.FindPropertyRelative("name");
            UpdateFactionNames(nameProp);

            EditorGUI.BeginChangeCheck();
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(names, nameProp.stringValue));
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(nameRect, tooltip);
            int index = EditorGUI.Popup(nameRect, currentIndex, names);
            if (EditorGUI.EndChangeCheck())
                nameProp.stringValue = names[index];


            if (!GetPolityGroups(nameProp.stringValue))
            {
                EditorGUI.LabelField(groupRect, $"No groups found for faction.");
                EditorGUI.EndProperty();
                return;
            }
            SerializedProperty groupProp = property.FindPropertyRelative("group");
            EditorGUI.BeginChangeCheck();
            int currentGroupIndex = Mathf.Max(0, System.Array.IndexOf(groups, groupProp.stringValue));
            GUIContent tooltip1 = new("", "Group");
            EditorGUI.LabelField(groupRect, tooltip1);
            int selectedGroupIndex = EditorGUI.Popup(groupRect, currentGroupIndex, groups);
            if (EditorGUI.EndChangeCheck())
                groupProp.stringValue = groups[selectedGroupIndex];

            EditorGUI.EndProperty();
        }

        private bool _stacked;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            _stacked = EditorGUIUtility.currentViewWidth < minWidth;
            return _stacked ? (lineHeight * 2) + spacing : lineHeight;
        }

        void UpdateFactionNames(SerializedProperty factionNameProp)
        {

            Manager.Faction[] factions = manager.factions;
            names = new string[factions.Length];
            for (int i = 0; i < factions.Length; i++)
                names[i] = factions[i].name;
            if (factionNameProp.stringValue != null)
            {
                // int index = System.Array.IndexOf(names, factionNameProp.stringValue);
                // if (index >= 0)
                //     this.index = index;
                // Debug.Log($"Set faction index to {this.index} based on faction name '{factionNameProp.stringValue}'.");
            }
        }

        bool GetPolityGroups(string factionName)
        {
            Manager.Faction[] factions = manager.factions;
            foreach (Manager.Faction faction in factions)
            {
                if (faction.name.Equals(factionName))
                {
                    Debug.Log($"Found faction '{factionName}' with {faction.groups.Count} groups.");
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
            Debug.LogWarning($"Faction '{factionName}' not found. Defaulting to empty unit options.");
            return false;
        }
    }
}