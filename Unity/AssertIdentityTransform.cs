using System;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// Logs an error if this object has anything other than the identity transform at Start time.
    /// </summary>
    public class AssertIdentityTransform : MonoBehaviour {
        public enum TransformType { LocalTransform, WorldTransform }

        [Tooltip("The space in which this gameobject should have an identity transform.")]
        public TransformType Type = TransformType.LocalTransform;

        private void Start () {

            switch (Type) { 
                case TransformType.LocalTransform:
                    if (transform.localPosition != Vector3.zero || transform.localRotation != Quaternion.identity || transform.localScale != Vector3.one)
                    {
                        LogError();   
                    }
                    break;
                case TransformType.WorldTransform:
                    if (transform.position != Vector3.zero || transform.rotation != Quaternion.identity || transform.lossyScale!= Vector3.one)
                    {
                        LogError();
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid value of parameter Type");
            }
        }

        private void LogError()
        {
            Debug.LogError("Gameobject [" + gameObject.name + "] does not have an identity transform in space ["+Type+"].");
        }
    }
}
