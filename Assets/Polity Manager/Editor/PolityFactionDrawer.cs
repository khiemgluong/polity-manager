using UnityEditor;
using UnityEngine;

namespace Polity
{
    [CustomPropertyDrawer(typeof(Faction), true)]
    public class PolityFactionDrawer : PropertyDrawer
    {
        Manager manager;
        string[] names;

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
            if (Application.isPlaying) property.serializedObject.Update();

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            position.y += spacing;

            Rect nameRect;
            nameRect = new Rect(position.x, position.y, position.width, lineHeight);

            EditorGUI.BeginProperty(position, label, property);

            UpdateFactionNames();
            SerializedProperty factionProp = property.FindPropertyRelative("name");
            // EditorGUI.BeginChangeCheck();
            int currentFactionIndex = Mathf.Max(0, System.Array.IndexOf(names, factionProp.stringValue));
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(nameRect, tooltip);
            int factionIndex = EditorGUI.Popup(nameRect, currentFactionIndex, names);
            // if (EditorGUI.EndChangeCheck())
            factionProp.stringValue = names[factionIndex];


            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return lineHeight + spacing;
        }

        void UpdateFactionNames()
        {

            Faction[] factions = manager.factions;
            names = new string[factions.Length];
            for (int i = 0; i < factions.Length; i++)
                names[i] = factions[i].name;
        }

        // bool GetPolityGroups(string factionName)
        // {
        //     Manager.Faction[] factions = manager.factions;
        //     foreach (Manager.Faction faction in factions)
        //     {
        //         if (faction.name.Equals(factionName))
        //         {
        //             if (faction.groups == null || faction.groups.Count == 0)
        //             {
        //                 groups = new string[0];
        //                 return false;
        //             }

        //             groups = new string[faction.groups.Count + 1];
        //             groups[0] = "\t";
        //             for (int i = 0; i < faction.groups.Count; i++)
        //                 groups[i + 1] = faction.groups[i].name;
        //             return true;
        //         }
        //     }
        //     groups = new string[0];
        //     // Debug.LogWarning($"Faction '{factionName}' not found. Defaulting to empty unit options.");
        //     return false;
        // }
    }
}