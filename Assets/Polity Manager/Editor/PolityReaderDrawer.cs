using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KL
{
    using static PolityManager;

    [CustomPropertyDrawer(typeof(PolityReader))]
    public class PolityReaderDrawer : PropertyDrawer
    {
        PolityManager polityManager;
        string[] polityNames, classNames, factionNames;
        int polityIndex;
        int classIndex;
        int factionIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (polityManager == null)
            {
                polityManager = Object.FindFirstObjectByType<PolityManager>();
                string sceneName = SceneManager.GetActiveScene().name;
                if (polityManager != null && polityManager.polities != null)
                {
                    polityNames = new string[polityManager.polities.Length];
                    for (int i = 0; i < polityManager.polities.Length; i++)
                        polityNames[i] = polityManager.polities[i].name;
                }
            }

            if (polityManager == null)
            {
                EditorGUI.LabelField(position, "No PolityManager found in the Scene.");
                return;
            }

            SerializedProperty polityProp = property.FindPropertyRelative("polityStruct");
            SerializedProperty selectedPolityIndexProp = property.FindPropertyRelative("polityIndex");
            SerializedProperty selectedClassIndexProp = property.FindPropertyRelative("classIndex");
            SerializedProperty selectedFactionIndexProp = property.FindPropertyRelative("factionIndex");

            polityIndex = selectedPolityIndexProp.intValue;
            classIndex = selectedClassIndexProp.intValue;
            factionIndex = selectedFactionIndexProp.intValue;

            // Initialize class and faction names
            UpdateClassNames(polityIndex);
            UpdateFactionNames(polityIndex, classIndex);

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Polity dropdown
            EditorGUI.BeginChangeCheck();
            polityIndex = EditorGUI.Popup(rect, "Polity", polityIndex, polityNames);
            if (EditorGUI.EndChangeCheck())
            {
                polityProp.FindPropertyRelative("polityName").stringValue = polityNames[polityIndex];
                selectedPolityIndexProp.intValue = polityIndex;
                UpdateClassNames(polityIndex);
                classIndex = 0;
                selectedClassIndexProp.intValue = classIndex;
                polityProp.FindPropertyRelative("className").stringValue = "";
                factionIndex = 0;
                selectedFactionIndexProp.intValue = factionIndex;
                polityProp.FindPropertyRelative("factionName").stringValue = "";
            }

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Class dropdown
            if (classNames != null && classNames.Length > 1)
            {
                EditorGUI.BeginChangeCheck();
                classIndex = EditorGUI.Popup(rect, "Class", classIndex, classNames);
                if (EditorGUI.EndChangeCheck())
                {
                    polityProp.FindPropertyRelative("className").stringValue = classNames[classIndex];
                    selectedClassIndexProp.intValue = classIndex;
                    UpdateFactionNames(polityIndex, classIndex);
                    factionIndex = 0;
                    selectedFactionIndexProp.intValue = factionIndex;
                    polityProp.FindPropertyRelative("factionName").stringValue = "";
                }

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            // Faction dropdown
            if (classIndex > 0 && factionNames != null && factionNames.Length > 1)
            {
                EditorGUI.BeginChangeCheck();
                factionIndex = EditorGUI.Popup(rect, "Faction", factionIndex, factionNames);
                if (EditorGUI.EndChangeCheck())
                {
                    polityProp.FindPropertyRelative("factionName").stringValue = factionNames[factionIndex];
                    selectedFactionIndexProp.intValue = factionIndex;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty selectedClassIndexProp = property.FindPropertyRelative("classIndex");
            SerializedProperty selectedFactionIndexProp = property.FindPropertyRelative("factionIndex");

            if (classNames != null && classNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (selectedClassIndexProp.intValue > 0 && factionNames != null && factionNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        void UpdateClassNames(int polityIndex)
        {
            if (polityManager.polities[polityIndex].classes != null && polityManager.polities[polityIndex].classes.Length > 0)
            {
                classNames = new string[polityManager.polities[polityIndex].classes.Length + 1];
                classNames[0] = "\t";
                for (int i = 0; i < polityManager.polities[polityIndex].classes.Length; i++)
                    classNames[i + 1] = polityManager.polities[polityIndex].classes[i].name;
            }
            else
            {
                classNames = new string[1];
                classNames[0] = "\t";
            }
        }

        void UpdateFactionNames(int polityIndex, int classIndex)
        {
            int adjustedClassIndex = classIndex - 1;
            if (polityIndex >= 0 && polityIndex < polityManager.polities.Length &&
                adjustedClassIndex >= 0 && adjustedClassIndex < polityManager.polities[polityIndex].classes.Length)
            {
                Class _class = polityManager.polities[polityIndex].classes[adjustedClassIndex];
                if (_class.factions != null && _class.factions.Count > 0)
                {
                    factionNames = new string[_class.factions.Count + 1];
                    factionNames[0] = "\t";
                    for (int i = 0; i < _class.factions.Count; i++)
                        factionNames[i + 1] = _class.factions[i].name;
                }
                else
                {
                    factionNames = new string[1];
                    factionNames[0] = "\t";
                }
            }
        }
    }
}