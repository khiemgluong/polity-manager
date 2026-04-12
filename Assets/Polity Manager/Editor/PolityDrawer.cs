using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Polities
{
    using static Manager;
    [CustomPropertyDrawer(typeof(Polity), true)]
    public class PolityDrawer : PropertyDrawer
    {
        Manager polityManager;
        string[] factionNames;
        int factionIndex = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (polityManager == null)
            {
                polityManager = Object.FindFirstObjectByType<Manager>();
                if (polityManager != null && polityManager.factions != null)
                {
                    factionNames = new string[polityManager.factions.Length];
                    for (int i = 0; i < polityManager.factions.Length; i++)
                        factionNames[i] = polityManager.factions[i].name;
                }
            }

            if (polityManager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }

            SerializedProperty factionNameProp = property.FindPropertyRelative("name");

            UpdateFactionNames(factionIndex, factionNameProp);

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Polity dropdown
            EditorGUI.BeginChangeCheck();
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(rect, tooltip);
            factionIndex = EditorGUI.Popup(rect, factionIndex, factionNames);
            if (EditorGUI.EndChangeCheck())
                factionNameProp.stringValue = factionNames[factionIndex];


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (property.propertyPath.Contains("Array.data"))
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            else
            {
                // if (coalitionNames != null && coalitionNames.Length > 1)
                //     height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // if (factionNames != null && factionNames.Length > 1)
                //     height += EditorGUIUtility.singleLineHeight;
            }
            return height;
        }

        void UpdateFactionNames(int factionIndex, SerializedProperty factionNameProp)
        {

            Manager.Polity[] factions = polityManager.factions;
            factionNames = new string[factions.Length];
            Debug.Log($"Updating faction names for faction index {factionIndex} with {factions.Length} factions.");
            for (int i = 0; i < factions.Length; i++)
                factionNames[i] = factions[i].name;
            if (factionNameProp.stringValue != null)
            {
                int index = System.Array.IndexOf(factionNames, factionNameProp.stringValue);
                if (index >= 0)
                    this.factionIndex = index;
                Debug.Log($"Set faction index to {this.factionIndex} based on faction name '{factionNameProp.stringValue}'.");
            }
        }
    }
}