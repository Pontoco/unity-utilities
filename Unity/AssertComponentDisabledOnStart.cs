using System;
using Conditions;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// Verifies that a gameobject is disabled when this gameobject is instantiated or turned on. 
    /// </summary>
    public class AssertComponentDisabledOnStart : MonoBehaviour
    {
        [Tooltip("The gameobject that should be disabled when this component is loaded. ")]
        public GameObject TargetObject;

        public void Start()
        {
            if (TargetObject.activeInHierarchy)
            {
                throw new ArgumentException("Gameobject [" + TargetObject.name +
                                            "] should be disabled when the gameobject ["+gameObject.name+"] is turned on.");
            }
        }
    }
}
