#if UNITY_EDITOR
using UnityEditor;

namespace Polity
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Member))]
    public class PolityMemberEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                // Skip the script field
                if (prop.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif