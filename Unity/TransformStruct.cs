using UnityEngine;

namespace Global.Utilities.Unity
{
    /// <summary>
    ///     Represents a general transformation in 3 dimensions. Depending on context, this could be an absolute world
    ///     orientation (world position, etc), or it could be a relative difference (local transformation). This isn't in any
    ///     specific space.
    /// </summary>
    /// <remarks>
    ///     The default Unity <see cref="Transform" /> is not a struct, and cannot actually be instantiated. Instead we
    ///     use a helper struct whenever we want to pass around these three values as a group.
    /// </remarks>
    public struct TransformStruct
    {
        /// <summary>The position value of the transform.</summary>
        public Vector3 Position;

        /// <summary>The rotation value of the transform.</summary>
        public Quaternion Rotation;

        /// <summary>The scale value of the transform.</summary>
        public Vector3 Scale;

        /// <summary>Creates a default transform with only a position. Other values are set to identities.</summary>
        public TransformStruct(Vector3 pos)
        {
            Position = pos;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }

        /// <summary>Creates a default transform with only a position and rotation. Other values are set to identities.</summary>
        public TransformStruct(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
            Scale = Vector3.one;
        }

        /// <summary>Creates a default transform with a position and rotation and scale..</summary>
        public TransformStruct(Vector3 pos, Quaternion rot, Vector3 scl)
        {
            Position = pos;
            Rotation = rot;
            Scale = scl;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"(Position: {Position.ToString("R")}, Rotation: {Rotation.ToString("R")}, Scale: {Scale.ToString("R")})";
        }
    }

    /// <summary>A bunch of helpers for TransformStruct.</summary>
    public static class TransformStructHelpers
    {
        /// <summary>Returns the world transform as a <see cref="TransformStruct" />.</summary>
        public static TransformStruct ToWorldStruct(this Transform transform)
        {
            return new TransformStruct(transform.position, transform.rotation, transform.lossyScale);
        }
    }
}
