using System.Reflection;
using UnityEditor;
using UnityEngine;

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
        bool loadedReader;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (polityManager == null)
            {
                polityManager = Object.FindFirstObjectByType<PolityManager>();
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

            SerializedProperty structProp = property.FindPropertyRelative("Struct");
            SerializedProperty polityIndexProp = property.FindPropertyRelative("polityIndex");
            SerializedProperty classIndexProp = property.FindPropertyRelative("classIndex");
            SerializedProperty factionIndexProp = property.FindPropertyRelative("factionIndex");

            polityIndex = polityIndexProp.intValue;
            classIndex = classIndexProp.intValue;
            factionIndex = factionIndexProp.intValue;

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
                structProp.FindPropertyRelative("polityName").stringValue = polityNames[polityIndex];
                polityIndexProp.intValue = polityIndex;
                UpdateClassNames(polityIndex);
                classIndex = 0;
                classIndexProp.intValue = classIndex;
                structProp.FindPropertyRelative("className").stringValue = "";
                factionIndex = 0;
                factionIndexProp.intValue = factionIndex;
                structProp.FindPropertyRelative("factionName").stringValue = "";
            }

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Class dropdown
            if (classNames != null && classNames.Length > 1)
            {
                EditorGUI.BeginChangeCheck();
                classIndex = EditorGUI.Popup(rect, "Class", classIndex, classNames);
                if (EditorGUI.EndChangeCheck())
                {
                    structProp.FindPropertyRelative("className").stringValue = classNames[classIndex];
                    classIndexProp.intValue = classIndex;
                    UpdateFactionNames(polityIndex, classIndex);
                    factionIndex = 0;
                    factionIndexProp.intValue = factionIndex;
                    structProp.FindPropertyRelative("factionName").stringValue = "";
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
                    structProp.FindPropertyRelative("factionName").stringValue = factionNames[factionIndex];
                    factionIndexProp.intValue = factionIndex;
                }
            }

            EditorGUI.EndProperty();
            if (!loadedReader)
            {
                object targetObject = GetTargetObjectOfProperty();
                if (targetObject is PolityReader polityReader)
                {
                    if (string.IsNullOrEmpty(polityReader.Struct.polityName) &&
                        polityIndex >= 0 && polityIndex < polityNames.Length)
                        polityReader.Struct.polityName = polityNames[polityIndex];

                    if (string.IsNullOrEmpty(polityReader.Struct.className) &&
                        classIndex > 0 && classIndex < classNames.Length)
                        polityReader.Struct.className = classNames[classIndex];

                    if (string.IsNullOrEmpty(polityReader.Struct.factionName) &&
                        factionIndex > 0 && factionIndex < factionNames.Length)
                        polityReader.Struct.factionName = factionNames[factionIndex];
                }
                loadedReader = true;
            }
            object GetTargetObjectOfProperty()
            {
                object obj = property.serializedObject.targetObject;
                string[] pathParts = property.propertyPath.Split('.');

                foreach (var pathPart in pathParts)
                {
                    var fieldInfo = obj.GetType().GetField(pathPart, BindingFlags.Public
                                                            | BindingFlags.NonPublic
                                                            | BindingFlags.Instance);
                    if (fieldInfo == null)
                        return null;
                    obj = fieldInfo.GetValue(obj);
                }
                return obj;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty selectedClassIndexProp = property.FindPropertyRelative("classIndex");

            if (classNames != null && classNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (selectedClassIndexProp.intValue > 0 && factionNames != null && factionNames.Length > 1)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        void UpdateClassNames(int polityIndex)
        {
            if (polityManager.polities[polityIndex].classes != null &&
                polityManager.polities[polityIndex].classes.Length > 0)
            {
                classNames = new string[polityManager.polities[polityIndex].classes.Length + 1];
                classNames[0] = "\t";
                for (int i = 0; i < polityManager.polities[polityIndex].classes.Length; i++)
                    classNames[i + 1] = polityManager.polities[polityIndex].classes[i].name;
            }
            else
            { classNames = new string[1]; classNames[0] = "\t"; }
        }

        void UpdateFactionNames(int polityIndex, int classIndex)
        {
            int adjustedClassIndex = classIndex - 1;
            if (polityIndex >= 0 && polityIndex < polityManager.polities.Length &&
                adjustedClassIndex >= 0 &&
                adjustedClassIndex < polityManager.polities[polityIndex].classes.Length)
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
                { factionNames = new string[1]; factionNames[0] = "\t"; }
            }
        }
    }
}