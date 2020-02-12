using UnityEngine;

namespace AsgUtils.Unity
{
    public static class LayerMaskExtensions
    {
        /// <summary>Returns true if the bit for the given layer is set in this mask, otherwise returns false.</summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) != 0;
        }
    }
}
