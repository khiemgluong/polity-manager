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
            if (PrefabUtility.IsPartOfPrefabAsset(property.serializedObject.targetObject))
            {
                EditorGUI.LabelField(position, label.text, "Uninstantiated prefab cannot set faction");
                return;
            }

            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<Manager>();
                if (manager != null && manager.factions != null)
                {
                    names = new string[manager.factions.Count];
                    for (int i = 0; i < manager.factions.Count; i++)
                        names[i] = manager.factions[i].Name;
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
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            EditorGUI.BeginChangeCheck();
            int currentFactionIndex = Mathf.Max(0, System.Array.IndexOf(names, nameProp.stringValue));
            GUIContent tooltip = new("", "Faction");
            EditorGUI.LabelField(nameRect, tooltip);
            int factionIndex = EditorGUI.Popup(nameRect, "Faction", currentFactionIndex, names);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"Faction changed from '{nameProp.stringValue}' to '{names[factionIndex]}'");
                if (Application.isPlaying)
                {
                    var targetObj = property.serializedObject.targetObject;
                    // Get the actual Faction instance via reflection using the property path
                    Faction faction = fieldInfo.GetValue(targetObj) as Faction;
                    if (faction != null)
                        faction.Name = names[factionIndex];
                }
            }
            nameProp.stringValue = names[factionIndex];

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
            var factions = manager.factions;
            names = new string[factions.Count];
            for (int i = 0; i < factions.Count; i++)
                names[i] = factions[i].Name;
        }

    }
}