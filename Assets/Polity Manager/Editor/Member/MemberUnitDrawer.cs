using UnityEditor;
using UnityEngine;
using static Polities.Manager;

namespace Polities
{
    [CustomPropertyDrawer(typeof(Member.Unit))]
    public class MemberUnitDrawer : PropertyDrawer
    {
        Manager polityManager;
        string[] nameOptions = new string[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (polityManager == null)
                polityManager = Object.FindFirstObjectByType<Manager>();
            if (polityManager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }

            Member target = property.serializedObject.targetObject as Member;
            if (target == null)
            {
                EditorGUI.LabelField(position, "Target object is not a Member.");
                return;
            }
            string currentPolityName = target.polity.name;
            Debug.Log($"Current Member Polity: {currentPolityName}");
            if (!UpdateUnitNames(currentPolityName))
            {
                EditorGUI.LabelField(position, $"No units found for polity '{currentPolityName}'.");
                return;
            }

            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty isLeaderProp = property.FindPropertyRelative("leader");

            // Get current dropdown index based on stored value
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(nameOptions, nameProp.stringValue));

            float lineHeight = EditorGUIUtility.singleLineHeight;

            float dropdownWidth = position.width * (2f / 3f);
            float toggleWidth = position.width * (1f / 3f);

            Rect dropdownRect = new Rect(position.x, position.y, dropdownWidth, lineHeight);
            int selectedIndex = EditorGUI.Popup(dropdownRect, currentIndex, nameOptions);
            if (selectedIndex != currentIndex)
                nameProp.stringValue = nameOptions[selectedIndex];

            // --- Is Leader Toggle (1/3 width, space-between justified) ---
            Rect toggleRect = new Rect(position.x + dropdownWidth, position.y, toggleWidth, lineHeight);

            EditorGUI.BeginProperty(toggleRect, GUIContent.none, isLeaderProp);

            float toggleSize = EditorGUIUtility.singleLineHeight; // square toggle box

            Rect labelRect = new Rect(toggleRect.x, toggleRect.y, toggleWidth - toggleSize, lineHeight);
            Rect boolRect = new Rect(toggleRect.x + toggleRect.width - toggleSize, toggleRect.y, toggleSize, lineHeight);

            EditorGUI.LabelField(labelRect, "   Leader?");
            isLeaderProp.boolValue = EditorGUI.Toggle(boolRect, isLeaderProp.boolValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        bool UpdateUnitNames(string factionName)
        {
            Manager.Polity[] factions = polityManager.factions;
            foreach (Manager.Polity faction in factions)
            {
                if (faction.name == factionName)
                {
                    if (faction.units == null || faction.units.Count == 0)
                    {
                        nameOptions = new string[0];
                        Debug.LogWarning($"Faction '{faction.name}' has no units. Defaulting to empty options.");
                        return false;
                    }

                    nameOptions = new string[faction.units.Count + 1];
                    nameOptions[0] = "\t";
                    for (int i = 0; i < faction.units.Count; i++)
                    {
                        nameOptions[i + 1] = faction.units[i].name;
                        Debug.Log($"Updated unit name option {i} to '{nameOptions[i]}' for polity '{factionName}'.");
                    }
                    return true;
                }
            }
            if (nameOptions.Length == 0)
            {
                nameOptions = new string[0];
                Debug.LogWarning($"No units found for polity '{factionName}'. Defaulting to placeholder option.");
            }
            return false;
        }
    }
}