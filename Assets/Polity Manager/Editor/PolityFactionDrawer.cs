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
                manager = Object.FindAnyObjectByType<Manager>();
            }

            if (manager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }

            UpdateFactionNames();

            if (names == null || names.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "No factions defined in Manager.");
                return;
            }

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            position.y += spacing;

            Rect nameRect = new Rect(position.x, position.y, position.width, lineHeight);

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProp = property.FindPropertyRelative("name");
            
            EditorGUI.BeginChangeCheck();
            
            bool isMixed = nameProp.hasMultipleDifferentValues;
            int currentFactionIndex = isMixed ? -1 : System.Array.IndexOf(names, nameProp.stringValue);
            
            EditorGUI.showMixedValue = isMixed;
            int newFactionIndex = EditorGUI.Popup(nameRect, "Faction", currentFactionIndex, names);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                if (newFactionIndex >= 0 && newFactionIndex < names.Length)
                {
                    string selectedName = names[newFactionIndex];
                    nameProp.stringValue = selectedName;
                    
                    if (Application.isPlaying)
                    {
                        foreach (var target in property.serializedObject.targetObjects)
                        {
                            // Note: fieldInfo.GetValue only works if the field is a direct member of the target object.
                            // For nested properties, this reflection logic would need to be more robust.
                            if (fieldInfo.GetValue(target) is Faction faction)
                            {
                                faction.Name = selectedName;
                            }
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        void UpdateFactionNames()
        {
            if (manager == null || manager.factions == null)
            {
                names = new string[0];
                return;
            }

            var factions = manager.factions;
            if (names == null || names.Length != factions.Count)
            {
                names = new string[factions.Count];
            }

            for (int i = 0; i < factions.Count; i++)
            {
                names[i] = factions[i].Name;
            }
        }

    }
}