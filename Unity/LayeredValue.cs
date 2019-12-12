using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace AsgUtils.Unity
{
    /// <summary>
    ///     A value that can have various layers of overrides to set a final value. Adding a layer allows you to
    ///     temporarily shadow this value with a different value. Other systems can modify lower layers without touching the
    ///     current value. This allows us to easily layer changes of the value from different, independent systems.
    /// </summary>
    /// <remarks>
    ///     Override layers on top of the BaseValue are stored in a stack. The most recent override is used as the
    ///     'perceived' value.
    /// </remarks>
    public class LayeredValue<T>
    {
        /// <summary>
        ///     Single layer that overrides all values in the layers below. The value of this object is the value of the
        ///     highest layer override.
        /// </summary>
        public class Layer
        {
            /// <summary>Set or get the value in the override layer itself.</summary>
            public T Value
            {
                get => value;
                set
                {
                    this.value = value;
                    parent.OnValuePossiblyChanged();
                }
            }

            private readonly LayeredValue<T> parent;
            private T value;

            /// <summary>Create a new layer.</summary>
            public Layer(LayeredValue<T> parent, T defaultValue)
            {
                value = defaultValue;
                this.parent = parent;
            }
        }

        /// <summary>
        ///     The base value. This value is never removed. If you want to make permanent changes to a value (rather than
        ///     shadowing it with a new value temporarily), change this value.
        /// </summary>
        public T BaseValue
        {
            set => values[0].Value = value;
            get => values[0].Value;
        }

        private readonly List<Layer> values = new List<Layer>();
        private readonly Action<T> onValueUpdate;

        /// <param name="defaultValue">The value this object starts with.</param>
        /// <param name="onValueUpdate">A callback that is run whenever the perceived value of this object may have changed.</param>
        public LayeredValue(T defaultValue, Action<T> onValueUpdate = null)
        {
            this.onValueUpdate = onValueUpdate;
            values.Add(new Layer(this, defaultValue));
        }

        /// <summary>Gets the 'perceived' value, which is the value after all overrides have been applied.</summary>
        public T Get()
        {
            return values[values.Count - 1].Value;
        }

        /// <summary>
        ///     Adds a new override to this value. Until this layer is removed or another one is added on top, this layer
        ///     represents the "current perceived" value for this object.
        /// </summary>
        public Layer AddOverrideLayer(T newValue)
        {
            Layer l = new Layer(this, newValue);

            values.Add(l);

            OnValuePossiblyChanged();
            return l;
        }

        /// <summary>Removes the given layer from the override system, returning the perceived value to the override below.</summary>
        public void RemoveOverrideLayer(Layer toRemove)
        {
            Assert.IsTrue(values.Remove(toRemove));
            OnValuePossiblyChanged();
        }

        /// <summary>Checks to see if a layer is at the top of the override stack, and therefore acting as the 'perceived' value.</summary>
        public bool IsHighestOverride(Layer layer)
        {
            Assert.IsTrue(values.Contains(layer));
            return values.IndexOf(layer) == values.Count - 1;
        }

        /// <summary>Called when the 'perceived' value may have changed.</summary>
        private void OnValuePossiblyChanged()
        {
            // Currently does not deduplicate changes, so this may be called even if the actual value does not change, just the underlying layers.
            onValueUpdate?.Invoke(Get());
        }
    }
}
