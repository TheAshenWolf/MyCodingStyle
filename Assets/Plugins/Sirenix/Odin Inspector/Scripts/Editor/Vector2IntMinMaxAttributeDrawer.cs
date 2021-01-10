//-----------------------------------------------------------------------
// <copyright file="Vector2IntMinMaxAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using OdinInspector;
    using Editor;
    using ValueResolvers;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws Vector2Int properties marked with <see cref="MinMaxSliderAttribute"/>.
    /// </summary>
    public class Vector2IntMinMaxAttributeDrawer : OdinAttributeDrawer<MinMaxSliderAttribute, Vector2Int>
    {
        private ValueResolver<float> minGetter;
        private ValueResolver<float> maxGetter;
        private ValueResolver<Vector2Int> vector2IntMinMaxGetter;

        /// <summary>
        /// Initializes the drawer by resolving any optional references to members for min/max value.
        /// </summary>
        protected override void Initialize()
        {
            // Min member reference.
            minGetter = ValueResolver.Get<float>(Property, Attribute.MinValueGetter, Attribute.MinValue);
            maxGetter = ValueResolver.Get<float>(Property, Attribute.MaxValueGetter, Attribute.MaxValue);

            // Min max member reference.
            if (Attribute.MinMaxValueGetter != null)
            {
                vector2IntMinMaxGetter = ValueResolver.Get<Vector2Int>(Property, Attribute.MinMaxValueGetter);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueResolver.DrawErrors(minGetter, maxGetter, vector2IntMinMaxGetter);

            // Get the range of the slider from the attribute or from member references.
            Vector2 range;
            if (vector2IntMinMaxGetter != null && !vector2IntMinMaxGetter.HasError)
            {
                range = (Vector2)vector2IntMinMaxGetter.GetValue();
            }
            else
            {
                range.x = minGetter.GetValue();
                range.y = maxGetter.GetValue();
            }

            EditorGUI.BeginChangeCheck();
            Vector2 value = SirenixEditorFields.MinMaxSlider(label, (Vector2)ValueEntry.SmartValue, range, Attribute.ShowFields);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = new Vector2Int((int)value.x, (int)value.y);
            }
        }
    }
}
#endif // UNITY_EDITOR && UNITY_2017_2_OR_NEWER