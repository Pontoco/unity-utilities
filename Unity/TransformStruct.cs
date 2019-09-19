using Optional;
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
            return
                $"(Position: {Position.ToString("R")}, Rotation: {Rotation.ToString("R")}, Scale: {Scale.ToString("R")})";
        }
    }

    /// <summary>A bunch of helpers for TransformStruct.</summary>
    public static class TransformHelpers
    {
        /// <summary>Returns the world transform as a <see cref="TransformStruct" />.</summary>
        public static TransformStruct ToWorldStruct(this Transform transform)
        {
            return new TransformStruct(transform.position, transform.rotation, transform.lossyScale);
        }

        /// <summary>Returns the local transform as a <see cref="TransformStruct" />.</summary>
        public static TransformStruct ToLocalStruct(this Transform transform)
        {
            return new TransformStruct(transform.localPosition, transform.localRotation, transform.localScale);
        }

        /// <summary>Returns the transformation matrix that takes a point from the parent local space to the child space local.</summary>
        public static Matrix4x4 GetTransformBetween(Transform transformParent, Transform transformChild)
        {
            return transformChild.worldToLocalMatrix * transformParent.localToWorldMatrix;
        }

        /// <summary>
        ///     Returns the Translation, Rotation, Scaling decomposition of this 4x4 matrix. Not all 4x4 matrices can be
        ///     represented by a combination of TRS (ie. rotating around a point), so this will return None in these cases.
        /// </summary>
        /// <param name="matrix">The input transformation matrix.</param>
        /// <remarks>
        ///     Often, we use 4x4 matrices to represent affine transformations. Transforms between arbitrary coordinate spaces
        ///     in the unity scene graph are affine transforms and are not necessarily possible to decompose into a TRS
        ///     representation. This function is most useful when you have
        /// </remarks>
        public static Option<TransformStruct> GetTRS(this Matrix4x4 matrix)
        {
            if (!matrix.ValidTRS())
            {
                return Option<TransformStruct>.None();
            }

            return new TransformStruct(
                ExtractTranslation(matrix),
                ExtractRotation(matrix),
                ExtractScale(matrix)
            ).Some();
        }

        /// <summary>Extracts a rotation from a transformation matrix. Matrix must be a valid TRS transformation matrix.</summary>
        private static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>Extracts a translation from a transformation matrix. Matrix must be a valid TRS transformation matrix.</summary>
        private static Vector3 ExtractTranslation(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        /// <summary>Extracts a scaling from a transformation matrix. Matrix must be a valid TRS transformation matrix.</summary>
        private static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}
