using System;
using UnityEngine;

namespace Utilities
{
    /// <summary>A collection of utilities for working with numbers, integers, modulos, etc.</summary>
    public static class NumUtils
    {
        /// <summary>Returns the nearest float to value that is a multiplier of factor.</summary>
        public static int NearestMultiple(int value, int factor)
        {
            return (int) Math.Round(value / (double) factor, MidpointRounding.AwayFromZero) * factor;
        }

        /// <summary>Returns the nearest float to value that is a multiplier of factor.</summary>
        public static float NearestMultiple(float value, float factor)
        {
            return (float) Math.Round(value / (double) factor, MidpointRounding.AwayFromZero) * factor;
        }

        /// <summary>Returns the first multiple of factor greater than value.</summary>
        public static int NextHighestMultiple(int value, int factor)
        {
            return (int) Math.Ceiling((double) value / factor) * factor;
        }

        /// <summary>Returns the first multiple of factor greater than value.</summary>
        public static float NextHighestMultiple(float value, float factor)
        {
            return (float) NextHighestMultiple(value, (double) factor);
        }

        /// <summary>Returns the first multiple of factor greater than value.</summary>
        public static double NextHighestMultiple(double value, double factor)
        {
            return Math.Ceiling(value / factor) * factor;
        }

        /// <summary>Clamps a value to be between the given boundaries, inclusive.</summary>
        /// <param name="value">Some value (-inf, +inf).</param>
        /// <param name="min">(-inf, +inf)</param>
        /// <param name="max">(-inf, +inf)</param>
        /// <returns>A value [min, max].</returns>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        /// <summary>Returns the distance between two values in modulo space.</summary>
        public static float DistanceInModulo(float value, float target, float modulo)
        {
            var diff = Math.Abs(value - target);
            return Math.Min(diff, modulo - diff);
        }

        /// <summary>Linearly maps a given value [0,1] to the range start..end.</summary>
        /// <param name="clamp">
        ///     Whether to force the final value to be within start/end or to allow the linear interpolation to
        ///     extend outside (if value > 1, for instance).
        /// </param>
        /// <returns></returns>
        public static float MapUnitToRange(float unitValue, float start, float end, bool clamp = false)
        {
            if (clamp)
            {
                unitValue = Clamp(unitValue, 0, 1);
            }

            return start + (end - start) * unitValue;
        }

        /// <summary>Takes a number (-inf..+inf) and two bounds and maps to 0..1 inside a given bounds.</summary>
        /// <param name="clamp">
        ///     Whether to force the final value to be within start/end or to allow the linear interpolation to
        ///     extend outside (if value > 1, for instance).
        /// </param>
        /// <returns></returns>
        public static float MapValueToUnit(float fullValue, float start, float end, bool clamp = false)
        {
            if (clamp)
            {
                fullValue = Clamp(fullValue, start, end);
            }

            return (fullValue - start) / (end - start);
        }

        /// <summary>Maps a value linearly from one range to another. Example: 5, (0, 10), (4, 8) Result: 6</summary>
        /// <param name="clamp">Whether to clamp the values between the ranges.</param>
        public static float MapBetweenRanges(float value, float sourceRangeStart, float sourceRangeEnd,
                                             float destinationRangeStart, float destinationRangeEnd, bool clamp = true)
        {
            var unitValue = MapValueToUnit(value, sourceRangeStart, sourceRangeEnd, clamp);
            return MapUnitToRange(unitValue, destinationRangeStart, destinationRangeEnd, clamp);
        }

        /// <summary>Whether the given number is a power of 2.</summary>
        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        /// <summary>Performs a canonical Modulus operation, where the output is on the range [0, b).</summary>
        public static int Mod(int value, int modulo)
        {
            int c = value % modulo;
            if (c < 0 && modulo > 0 || c > 0 && modulo < 0)
            {
                c += modulo;
            }

            return c;
        }

        /// <summary>
        ///     Returns the nearest value X to TargetValue, such that X = Value + M*Increment, for some number M. Equivalent
        ///     to adding/subtracting multiples of Increment from Value, until as close as possible to TargetValue.
        /// </summary>
        public static float NearestValueByIncrement(float value, float targetValue, float increment)
        {
            // Slightly modified version of this equation: https://stackoverflow.com/questions/29557459/round-to-nearest-multiple-of-a-number
            float number = targetValue - value;
            float normalized = Mathf.Floor((number + increment / 2) / increment) * increment;
            float result = normalized + value;
            return result;
        }

        /// <summary>The square magnitude of a quaternion.</summary>
        public static float SquareMagnitude(this Quaternion quat)
        {
            return quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w;
        }

        /// <summary>
        ///     Returns the Swing/Twist decomposition of a Quaternion. Useful for smooth lerping. See google for more
        ///     information.
        /// </summary>
        /// <param name="twistAxis">The starting forward direction that the rotation q is being applied to.</param>
        /// <param name="q">A rotation to decompose</param>
        /// <param name="swing">The swing component of q.</param>
        /// <param name="twist">The twist component of q.</param>
        public static void SwingTwistDecomposition(this Quaternion q, Vector3 twistAxis, out Quaternion swing,
                                                   out Quaternion twist)
        {
            // Vector part projected onto twist axis
            Vector3 projection = Vector3.Dot(twistAxis, new Vector3(q.x, q.y, q.z)) * twistAxis;

            // Twist quaternion
            twist = new Quaternion(projection.x, projection.y, projection.z, q.w);

            // Singularity close to 180deg
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (twist.SquareMagnitude() == 0.0f)
            {
                twist = Quaternion.identity;
            }
            else
            {
                twist = Quaternion.Normalize(twist);
            }

            // Set swing
            swing = q * Quaternion.Inverse(twist);
        }

        /// <summary>Calculates the distance from this point to a given ray. </summary>
        /// <remarks>Uses the technique from this video: https://youtu.be/tYUtWYGUqgw </remarks>
        public static float DistanceToRay(this Vector3 point, Ray ray)
        {
            return Vector3.Cross(point - ray.origin, ray.direction).magnitude;
        }

        /// <summary>
        ///     Calculates a vector offset of this point from the given ray. Subtracting this value from the point will place
        ///     it on the ray. This vector represents the shortest distance from the ray to the point.
        /// </summary>
        public static Vector3 OffsetFromRay(this Vector3 point, Ray ray)
        {
            Vector3 relativePoint = point - ray.origin; // A vector from the ray origin to the point
            Vector3 projection =
                Vector3.Project(relativePoint, ray.direction); // Vector from the ray origin to the projected point.
            Vector3
                orthogonalProjection =
                    relativePoint - projection; // A vector from the projected point to the original point.
            return orthogonalProjection; // Invert the vector to give the offset
        }
    }
}
