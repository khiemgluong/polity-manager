using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Polities
{
    using static Manager;
    [CustomPropertyDrawer(typeof(Polity), true)]
    public class PolityDrawer : PropertyDrawer
    {
        Manager manager;
        string[] names;
        int index = 0;
        string[] factionNames = new string[0];
        string[] groupNames = new string[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<Manager>();
                if (manager != null && manager.polities != null)
                {
                    names = new string[manager.polities.Length];
                    for (int i = 0; i < manager.polities.Length; i++)
                        names[i] = manager.polities[i].name;
                }
            }

            if (manager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }

            SerializedProperty nameProp = property.FindPropertyRelative("name");

            UpdateFactionNames(index, nameProp);

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Polity dropdown
            EditorGUI.BeginChangeCheck();
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(rect, tooltip);
            index = EditorGUI.Popup(rect, index, names);
            if (EditorGUI.EndChangeCheck())
                nameProp.stringValue = names[index];

            SerializedProperty factionNameProp = property.FindPropertyRelative("faction");
            GetPolityFactions();
            int factionIndex = Mathf.Max(0, System.Array.IndexOf(factionNames, factionNameProp.stringValue));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            int selectedFactionIndex = EditorGUI.Popup(rect, "Faction", factionIndex, factionNames);
            if (selectedFactionIndex != factionIndex)
                factionNameProp.stringValue = factionNames[selectedFactionIndex];
            Debug.LogError($"Selected faction index: {selectedFactionIndex}, Faction name: {factionNameProp.stringValue}");

            SerializedProperty groupNameProp = property.FindPropertyRelative("group");
            int groupIndex = Mathf.Max(0, System.Array.IndexOf(groupNames, groupNameProp.stringValue));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            int selectedGroupIndex = EditorGUI.Popup(rect, "Group", groupIndex, groupNames);
            if (selectedGroupIndex != groupIndex)
                groupNameProp.stringValue = groupNames[selectedGroupIndex];
            GetPolityGroups(factionNameProp.stringValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.propertyPath.Contains("Array.data"))
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            else
            {
                // if (groupNames != null && groupNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // if (factionNames != null && factionNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight;
                Debug.Log($"Calculated property height: {height} (GroupNames: {groupNames?.Length}, FactionNames: {factionNames?.Length})");
            }
            return height;
        }

        void UpdateFactionNames(int factionIndex, SerializedProperty factionNameProp)
        {

            Manager.Polity[] factions = manager.polities;
            names = new string[factions.Length];
            Debug.Log($"Updating faction names for faction index {factionIndex} with {factions.Length} factions.");
            for (int i = 0; i < factions.Length; i++)
                names[i] = factions[i].name;
            if (factionNameProp.stringValue != null)
            {
                int index = System.Array.IndexOf(names, factionNameProp.stringValue);
                if (index >= 0)
                    this.index = index;
                Debug.Log($"Set faction index to {this.index} based on faction name '{factionNameProp.stringValue}'.");
            }
        }

        void GetPolityFactions()
        {
            Manager.Polity[] factions = manager.polities;
            factionNames = new string[factions.Length];
            for (int i = 0; i < factions.Length; i++)
                factionNames[i] = factions[i].name;
        }

        void GetPolityGroups(string factionName)
        {
            Manager.Polity[] factions = manager.polities;
            foreach (Manager.Polity faction in factions)
            {
                if (faction.name == factionName)
                {
                    if (faction.groups == null || faction.groups.Count == 0)
                    {
                        factionNames = new string[0];
                        Debug.LogWarning($"Faction '{faction.name}' has no units. Defaulting to empty options.");
                        return;
                    }

                    groupNames = new string[faction.groups.Count];
                    for (int i = 0; i < faction.groups.Count; i++)
                    {
                        groupNames[i] = faction.groups[i].name;
                        Debug.Log($"Updated unit name option {i} to '{groupNames[i]}' for faction '{factionName}'.");
                    }
                    return;
                }
            }
            groupNames = new string[0];
            Debug.LogWarning($"Faction '{factionName}' not found. Defaulting to empty unit options.");
        }
    }
}