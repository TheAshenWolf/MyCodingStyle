//-----------------------------------------------------------------------
// <copyright file="FixBrokenUnityObjectWrapperDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
#pragma warning disable

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using OdinInspector;
    using Editor;
    using Utilities;
    using Sirenix.Utilities.Editor;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(0.001, 0, 0)]
    public class FixBrokenUnityObjectWrapperDrawer<T> : OdinValueDrawer<T>, IDefinesGenericMenuItems
        where T : Component
    {
        private const string AUTO_FIX_PREFS_KEY = "TemporarilyBrokenUnityObjectWrapperDrawer.autoFix";

        private bool isBroken = false;
        private T realWrapperInstance;
        private bool allowSceneViewObjects;
        private static bool autoFix;

        protected override void Initialize()
        {
            allowSceneViewObjects = ValueEntry.Property.GetAttribute<AssetsOnlyAttribute>() == null;
            autoFix = EditorPrefs.HasKey(AUTO_FIX_PREFS_KEY);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (!(ValueEntry.ValueState == PropertyValueState.NullReference || ValueEntry.ValueState == PropertyValueState.ReferenceValueConflict))
            {
                CallNextDrawer(label);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                isBroken = false;
                int count = ValueEntry.ValueCount;
                for (int i = 0; i < count; i++)
                {
                    T component = ValueEntry.Values[i];

                    if (ComponentIsBroken(component, ref realWrapperInstance))
                    {
                        isBroken = true;
                        break;
                    }
                }

                if (isBroken && autoFix)
                {
                    isBroken = false;

                    for (int i = 0; i < ValueEntry.ValueCount; i++)
                    {
                        T fixedComponent = null;
                        if (ComponentIsBroken(ValueEntry.Values[i], ref fixedComponent) && fixedComponent)
                        {
                            (ValueEntry as IValueEntryActualValueSetter<T>).SetActualValue(i, fixedComponent);
                        }
                    }

                    ValueEntry.Update();
                }
            }

            if (!isBroken)
            {
                CallNextDrawer(label);
                return;
            }

            Rect rect = EditorGUILayout.GetControlRect(label != null);
            Rect btnRect = rect.AlignRight(20);
            Rect controlRect = rect.SetXMax(btnRect.xMin - 5);

            object newInstance = null;

            EditorGUI.BeginChangeCheck();
            {
                if (ValueEntry.BaseValueType.IsInterface)
                {
                    newInstance = SirenixEditorFields.PolymorphicObjectField(controlRect,
                        label,
                        realWrapperInstance,
                        ValueEntry.BaseValueType,
                        allowSceneViewObjects);
                }
                else
                {
                    newInstance = SirenixEditorFields.UnityObjectField(
                        controlRect,
                        label,
                        realWrapperInstance,
                        ValueEntry.BaseValueType,
                        allowSceneViewObjects) as Component;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.WeakSmartValue = newInstance;
            }

            if (GUI.Button(btnRect, " ", EditorStyles.miniButton))
            {
                FixBrokenUnityObjectWrapperPopup popup = new FixBrokenUnityObjectWrapperPopup(ValueEntry);
                OdinEditorWindow.InspectObjectInDropDown(popup, 300);
            }

            if (Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(btnRect, EditorIcons.ConsoleWarnicon, ScaleMode.ScaleToFit);
            }
        }

        private static bool ComponentIsBroken(T component, ref T realInstance)
        {
            T uObj = component;
            object oObj = (object)uObj;

            if (oObj != null && uObj == null)
            {
                int instanceId = uObj.GetInstanceID();
                if (AssetDatabase.Contains(instanceId))
                {
                    string path = AssetDatabase.GetAssetPath(instanceId);
                    T realWrapper = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(n => n.GetInstanceID() == instanceId) as T;
                    if (realWrapper)
                    {
                        realInstance = realWrapper;
                        return true;
                    }
                }
            }

            return false;
        }

        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            if (EditorPrefs.HasKey(AUTO_FIX_PREFS_KEY))
            {
                genericMenu.AddItem(new GUIContent("Disable auto-fix of broken prefab instance references"), false, (x) =>
                {
                    EditorPrefs.DeleteKey(AUTO_FIX_PREFS_KEY);
                    autoFix = false;
                }, null);
            }
        }

        [TypeInfoBox("This asset reference is temporarily broken until the next reload, because of an error in Unity where the C# wrapper object of a prefab asset is destroyed when changes are made to that prefab asset. This error has been reported to Unity.\n\nMeanwhile, Odin can fix this for you by getting a new, valid wrapper object from the asset database and replacing the broken wrapper instance with the new one.")]
        private class FixBrokenUnityObjectWrapperPopup
        {
            private IPropertyValueEntry<T> valueEntry;

            public FixBrokenUnityObjectWrapperPopup(IPropertyValueEntry<T> valueEntry)
            {
                this.valueEntry = valueEntry;
            }

            [HorizontalGroup, Button(ButtonSizes.Large)]
            public void FixItThisTime()
            {
                for (int i = 0; i < valueEntry.ValueCount; i++)
                {
                    int localI = i;
                    T fixedComponent = null;
                    if (ComponentIsBroken(valueEntry.Values[i], ref fixedComponent) && fixedComponent)
                    {
                        valueEntry.Property.Tree.DelayActionUntilRepaint(() =>
                        {
                            (valueEntry as IValueEntryActualValueSetter<T>).SetActualValue(localI, fixedComponent);
                        });
                    }
                }

                if (GUIHelper.CurrentWindow) 
                {
                    EditorApplication.delayCall += GUIHelper.CurrentWindow.Close;
                }
            }

            [HorizontalGroup, Button(ButtonSizes.Large)]
            public void FixItAlways()
            {
                EditorPrefs.SetBool(AUTO_FIX_PREFS_KEY, true);
                autoFix = true;

                if (GUIHelper.CurrentWindow) 
                {
                    EditorApplication.delayCall += GUIHelper.CurrentWindow.Close;
                }
            }
        }
    }
}

#endif