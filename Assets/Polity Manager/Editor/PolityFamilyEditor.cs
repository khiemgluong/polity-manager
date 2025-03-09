using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace KL
{
    [CustomEditor(typeof(PolityFamily))]
    public class PolityFamilyEditor : Editor
    {
        private PolityFamily polityFamily;
        private bool showDictionary = true;

        private void OnEnable()
        {
            polityFamily = (PolityFamily)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display the default inspector
            DrawDefaultInspector();

            // Display the dictionary
            // showDictionary = EditorGUILayout.Foldout(showDictionary, "Family Dictionary");
            // if (showDictionary)
            // {
            //     EditorGUI.indentLevel++;
            //     foreach (var key in polityFamily.family.Keys)
            //     {
            //         EditorGUILayout.BeginHorizontal();
            //         EditorGUILayout.LabelField("Key: " + key.name, GUILayout.Width(200));
            //         if (GUILayout.Button("Edit", GUILayout.Width(50)))
            //         {
            //             // Implement your custom edit logic here
            //         }
            //         EditorGUILayout.EndHorizontal();

            //         var familyStruct = polityFamily.family[key];
            //         EditorGUILayout.LabelField("Parents:");
            //         EditorGUI.indentLevel++;
            //         foreach (var parent in familyStruct.parents)
            //         {
            //             EditorGUILayout.LabelField(parent.name);
            //         }
            //         EditorGUI.indentLevel--;

            //         EditorGUILayout.LabelField("Partners:");
            //         EditorGUI.indentLevel++;
            //         foreach (var partner in familyStruct.partners.Keys)
            //         {
            //             EditorGUILayout.LabelField(partner.name);
            //         }
            //         EditorGUI.indentLevel--;
            //     }
            //     EditorGUI.indentLevel--;
            // }
            if (!Application.isPlaying) if (GUILayout.Button("Member Family Graph"))
                    EditorWindow.GetWindow<PolityFamilyGraph>("Polity Manager");
            serializedObject.ApplyModifiedProperties();
        }
    }
}