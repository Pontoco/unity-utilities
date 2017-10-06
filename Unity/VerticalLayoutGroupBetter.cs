// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.VerticalLayoutGroup
// Assembly: UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A87F162D-47B8-4BE7-B6EA-E656C9C5AA2B
// Assembly location: /Applications/Unity/Unity.app/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll

using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity
{
    /// <summary>
    ///   <para>Layout child layout elements below each other.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class VerticalLayoutGroupBetter : HorizontalOrVerticalLayoutGroupBetter
    {

        /// <summary>
        ///   <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            this.SetChildrenAlongAxis(0, true);
        }

        /// <summary>
        ///   <para>Called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            this.SetChildrenAlongAxis(1, true);
        }
    }
}
