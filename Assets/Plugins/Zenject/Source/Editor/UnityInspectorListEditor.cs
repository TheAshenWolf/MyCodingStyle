using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Zenject
{
    public abstract class UnityInspectorListEditor : Editor
    {
        List<ReorderableList> _installersLists;
        List<SerializedProperty> _installersProperties;

        protected abstract string[] PropertyDisplayNames
        {
            get;
        }

        protected abstract string[] PropertyNames
        {
            get;
        }

        protected abstract string[] PropertyDescriptions
        {
            get;
        }

        public virtual void OnEnable()
        {
            _installersProperties = new List<SerializedProperty>();
            _installersLists = new List<ReorderableList>();

            string[] descriptions = PropertyDescriptions;
            string[] names = PropertyNames;
            string[] displayNames = PropertyDisplayNames;

            Assert.IsEqual(descriptions.Length, names.Length);

            var infos = Enumerable.Range(0, names.Length).Select(i => new { Name = names[i], DisplayName = displayNames[i], Description = descriptions[i] }).ToList();

            foreach (var info in infos)
            {
                SerializedProperty installersProperty = serializedObject.FindProperty(info.Name);
                _installersProperties.Add(installersProperty);

                ReorderableList installersList = new ReorderableList(serializedObject, installersProperty, true, true, true, true);
                _installersLists.Add(installersList);

                string closedName = info.DisplayName;
                string closedDesc = info.Description;

                installersList.drawHeaderCallback += rect =>
                {
                    GUI.Label(rect,
                    new GUIContent(closedName, closedDesc));
                };
                installersList.drawElementCallback += (rect, index, active, focused) =>
                {
                    rect.width -= 40;
                    rect.x += 20;
                    EditorGUI.PropertyField(rect, installersProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
                };
            }
        }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnGui();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnGui()
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }

            foreach (ReorderableList list in _installersLists)
            {
                list.DoLayoutList();
            }

            GUI.enabled = true;
        }
    }
}

