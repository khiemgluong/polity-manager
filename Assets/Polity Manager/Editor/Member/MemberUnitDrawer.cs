using UnityEditor;
using UnityEngine;
using static Polity.Manager;

namespace Polity
{
    [CustomPropertyDrawer(typeof(Member.Group))]
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
            string currentFactionName = target.faction.name;
            Debug.Log($"Current Member Polity: {currentFactionName}");
            SerializedProperty nameProp = property.FindPropertyRelative("name");

            if (!UpdateGroupNames(currentFactionName))
            {
                EditorGUI.LabelField(position, $"No groups found for faction.");
                return;
            }

            // Get current dropdown index based on stored value
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(nameOptions, nameProp.stringValue));

            Rect dropdownRect = new Rect(position.x, position.y, position.width , EditorGUIUtility.singleLineHeight);
            int selectedIndex = EditorGUI.Popup(dropdownRect, currentIndex, nameOptions);
            if (selectedIndex != currentIndex)
                nameProp.stringValue = nameOptions[selectedIndex];

            // // --- Is Leader Toggle (1/3 width, space-between justified) ---
            // Rect toggleRect = new Rect(position.x + dropdownWidth, position.y, toggleWidth, lineHeight);

            // EditorGUI.BeginProperty(toggleRect, GUIContent.none, isLeaderProp);

            // float toggleSize = EditorGUIUtility.singleLineHeight; // square toggle box

            // Rect labelRect = new Rect(toggleRect.x, toggleRect.y, toggleWidth - toggleSize, lineHeight);
            // Rect boolRect = new Rect(toggleRect.x + toggleRect.width - toggleSize, toggleRect.y, toggleSize, lineHeight);

            // EditorGUI.LabelField(labelRect, "   Leader?");
            // isLeaderProp.boolValue = EditorGUI.Toggle(boolRect, isLeaderProp.boolValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        bool UpdateGroupNames(string factionName)
        {
            Manager.Faction[] factions = polityManager.factions;
            foreach (Manager.Faction faction in factions)
            {
                if (faction.name == factionName)
                {
                    if (faction.groups == null || faction.groups.Count == 0)
                    {
                        nameOptions = new string[0];
                        Debug.LogWarning($"Faction '{faction.name}' has no units. Defaulting to empty options.");
                        return false;
                    }

                    nameOptions = new string[faction.groups.Count + 1];
                    nameOptions[0] = "\t";
                    for (int i = 0; i < faction.groups.Count; i++)
                    {
                        nameOptions[i + 1] = faction.groups[i].name;
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