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
        int index = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<Manager>();
                if (manager != null && manager.factions != null)
                {
                    names = new string[manager.factions.Length];
                    for (int i = 0; i < manager.factions.Length; i++)
                        names[i] = manager.factions[i].name;
                }
            }

            if (manager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }
            Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProp = property.FindPropertyRelative("name");
            UpdateFactionNames(index, nameProp);
            EditorGUI.BeginProperty(position, label, property);

            // Polity dropdown
            EditorGUI.BeginChangeCheck();
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(rect, tooltip);
            index = EditorGUI.Popup(rect, index, names);
            if (EditorGUI.EndChangeCheck())
                nameProp.stringValue = names[index];

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            // if (property.propertyPath.Contains("Array.data"))
            //     height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            // else
            // {
            //     // if (groupNames != null && groupNames.Length > 1)
            //     // height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            //     // if (factionNames != null && factionNames.Length > 1)
            //     // height += EditorGUIUtility.singleLineHeight;
            // }
            return height;
        }

        void UpdateFactionNames(int factionIndex, SerializedProperty factionNameProp)
        {

            Manager.Faction[] factions = manager.factions;
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

        // bool GetPolityGroups(string factionName)
        // {
        //     Manager.Faction[] factions = manager.factions;
        //     foreach (Manager.Faction faction in factions)
        //     {
        //         if (faction.name == factionName)
        //         {
        //             if (faction.groups == null || faction.groups.Count == 0)
        //             {
        //                 factionNames = new string[0];
        //                 Debug.LogWarning($"Faction '{faction.name}' has no units. Defaulting to empty options.");
        //                 return false;
        //             }

        //             groupNames = new string[faction.groups.Count + 1];
        //             groupNames[0] = "\t";
        //             for (int i = 0; i < faction.groups.Count; i++)
        //                 groupNames[i + 1] = faction.groups[i].name;
        //             return true;
        //         }
        //     }
        //     groupNames = new string[0];
        //     Debug.LogWarning($"Faction '{factionName}' not found. Defaulting to empty unit options.");
        //     return false;
        // }
    }
}