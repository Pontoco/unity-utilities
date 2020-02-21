using System;
using Conditions;
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

        /// <summary>The magnitude of a quaternion.</summary>
        public static float Magnitude(this Quaternion quat)
        {
            return Mathf.Sqrt(quat.SquareMagnitude());
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

        /// <summary>
        ///     Transforms a local-space position to world-space using this transform. Ignores any scale component of the
        ///     transform.
        /// </summary>
        /// <remarks>
        ///     Suitable for transforming to the Rigidbody coordinate space because that space is unscaled, but matches the
        ///     rotation and translation of the GameObject's transform.
        /// </remarks>
        public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
        {
            var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            return localToWorldMatrix.MultiplyPoint3x4(position);
        }

        /// <summary>Transforms a world-space position to local-space using this transform.</summary>
        /// <remarks>
        ///     Suitable for transforming from the Rigidbody coordinate space because that space is unscaled, but matches the
        ///     rotation and translation of the GameObject's transform.
        /// </remarks>
        public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(position);
        }

        /// <summary>Transforms a local-space vector to world-space using this transform.</summary>
        /// <remarks>
        ///     Suitable for transforming to the Rigidbody coordinate space because that space is unscaled, but matches the
        ///     rotation and translation of the GameObject's transform.
        /// </remarks>
        public static Vector3 TransformVectorUnscaled(this Transform transform, Vector3 vector)
        {
            var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            return localToWorldMatrix.MultiplyVector(vector);
        }

        /// <summary>Transforms a local-space vector to world-space using this transform.</summary>
        /// <remarks>
        ///     Suitable for transforming from the Rigidbody coordinate space because that space is unscaled, but matches the
        ///     rotation and translation of the GameObject's transform.
        /// </remarks>
        public static Vector3 InverseTransformVectorUnscaled(this Transform transform, Vector3 vector)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            return worldToLocalMatrix.MultiplyVector(vector);
        }

        /// <summary>
        ///     Intends to be the same as <see cref="Rigidbody.AddForce(Vector3,ForceMode)" />, except that the angular
        ///     acceleration caused by the force may be limited.
        ///     <para>The final, applied angular acceleration vector is magnitude-clamped. </para>
        /// </summary>
        /// <param name="body">The body to apply the force to.</param>
        /// <param name="force">The world space force to apply.</param>
        /// <param name="position">The world space position to apply the force.</param>
        /// <param name="maxAngularAcceleration">The maximum magnitude of the angular acceleration applied.</param>
        /// <param name="forceMode">
        ///     May be Force or Acceleration. In acceleration mode, we just multiply the mass into the force,
        ///     like Unity does.
        /// </param>
        /// <remarks>
        ///     This function is mathematically correct as far as I am aware. Note, though, that when using it in a spring
        ///     system, because we limit the angular acceleration, this can cause the spring to convert much more slowly (or
        ///     potentially spiral outwards). Capping angular acceleration can be helpful, but isn't a physical action. Because of
        ///     this, sometimes you can have strange behaviors.
        /// </remarks>
        public static void AddForceWithLimits(this Rigidbody body, Vector3 force, Vector3 position,
                                              float maxAngularAcceleration, ForceMode forceMode = ForceMode.Force)
        {
            Condition.Requires(maxAngularAcceleration).IsGreaterOrEqual(0);
            Condition.Requires(forceMode).IsNotEqualTo(ForceMode.VelocityChange);
            Condition.Requires(forceMode).IsNotEqualTo(ForceMode.Impulse);

            if (forceMode == ForceMode.Acceleration)
            {
                // We've passed in an acceleration, not a force, so multiply it by the mass to get
                // the linear force we need to apply. (F = m * a)

                // Note: it might seem like we should do something similar for the torque
                // (ie. multiplying the torque by the moment of inertia). In practice, this
                // doesn't work, and I believe it's because there's no clear formula for deriving
                // an angular acceleration from an acceleration at a point. It's more stable to just 
                // use this (mass including) value to calculate the torque. I believe this is what 
                // Unity does under the hood in Acceleration force mode.
                force *= body.mass;
            }

            // A force at a point can be decomposed into two components: the same force at the center of mass, and a torque.
            // We break the force into its component pieces, so we can limit the angular acceleration (torque):

            // Mass-local space is the the coordinate system centered at the center-of-mass of the rigid body
            // and oriented such that the XYZ unit vectors are aligned with the 3 primary axis of inertia.
            // We use this space for torque calculation because that's the coordinate space of the (diagonal) inertia tensor.
            Vector3 massLocalPosition = body.transform.InverseTransformPointUnscaled(position) - body.centerOfMass;

            // Note: this calculation requires that the transform is synced with the current transformation of the RigidBody
            // This is not *always* the case, but usually they are almost precisely the same
            Vector3 localForce = body.transform.InverseTransformVectorUnscaled(force);

            Vector3 massLocalForce = Quaternion.Inverse(body.inertiaTensorRotation) * localForce;
            Vector3 massLocalTorque = Vector3.Cross(massLocalPosition, massLocalForce);

            // The angular acceleration caused by this force, in mass-local space.
            Vector3 massLocalAngularAccel = new Vector3(
                massLocalTorque.x / body.inertiaTensor.x,
                massLocalTorque.y / body.inertiaTensor.y,
                massLocalTorque.z / body.inertiaTensor.z
            );

            Vector3 clampedMassLocalAngularAccel =
                Vector3.ClampMagnitude(massLocalAngularAccel, maxAngularAcceleration);

            // Transform the angular acceleration back to local space so we can apply it.
            Vector3 clampedMassLocalTorque = new Vector3(
                clampedMassLocalAngularAccel.x * body.inertiaTensor.x,
                clampedMassLocalAngularAccel.y * body.inertiaTensor.y,
                clampedMassLocalAngularAccel.z * body.inertiaTensor.z
            );

            Vector3 clampedLocalTorque = body.inertiaTensorRotation * clampedMassLocalTorque;

            // Apply the local, now clamped, torque to the rigidbody.
            body.AddRelativeTorque(clampedLocalTorque, ForceMode.Force);

            // Apply the local force to the center of mass.
            Vector3 linearForce = localForce;
            body.AddRelativeForce(linearForce, ForceMode.Force);
        }

        /// <summary>
        ///     A variation on "GetPointVelocity" that returns the velocity of a point, after a force has been applied. Note
        ///     that the position at which this function calculates the velocity is integrated forward. This returns the velocity
        ///     of this point after integration if no other forces were applied.
        /// </summary>
        /// <remarks>Additionally returns the (speculated) updated position of the point.</remarks>
        /// <param name="body">Body to access</param>
        /// <param name="force">The speculative world space force that would be applied.</param>
        /// <param name="forcePosition">The world space location of the force applied.</param>
        /// <param name="velReadPosition">The world space point to read the velocity of.</param>
        /// <param name="finalReadPos">The output location of the velReadPosition, after it has been integrated one timestep.</param>
        public static Vector3 GetPointVelocityAfterForce(this Rigidbody body, Vector3 force, Vector3 forcePosition,
                                                         Vector3 velReadPosition, out Vector3 finalReadPos)
        {
            Vector3 deltaLinearVelocity = force / body.mass * Time.deltaTime;

            Vector3 massLocalPosition = body.transform.InverseTransformPointUnscaled(forcePosition) - body.centerOfMass;

            Vector3 localForce = body.transform.InverseTransformVectorUnscaled(force);

            Vector3 massLocalForce = Quaternion.Inverse(body.inertiaTensorRotation) * localForce;
            Vector3 massLocalTorque = Vector3.Cross(massLocalPosition, massLocalForce);

            // The angular acceleration caused by this force, in mass-local space.
            Vector3 massLocalAngularAccel = new Vector3(
                massLocalTorque.x / body.inertiaTensor.x,
                massLocalTorque.y / body.inertiaTensor.y,
                massLocalTorque.z / body.inertiaTensor.z
            );

            Vector3 localAngularAccel = body.inertiaTensorRotation * massLocalAngularAccel;
            Vector3 deltaAngularVel = body.transform.TransformVectorUnscaled(localAngularAccel) * Time.deltaTime;

            Vector3 finalLinearVel = body.velocity + deltaLinearVelocity;
            Vector3 finalAngularVel = body.angularVelocity + deltaAngularVel;

            Vector3 radialToPoint = velReadPosition - body.worldCenterOfMass;
            Vector3 advectedPointDelta = finalLinearVel * Time.deltaTime +
                                         Vector3.Cross(finalAngularVel * Time.deltaTime, radialToPoint);

            Vector3 advectedPointPos = velReadPosition + advectedPointDelta;

            Vector3 advectedRadialToPoint = advectedPointPos - body.worldCenterOfMass;
            Vector3 velocityAtPoint = finalLinearVel + Vector3.Cross(finalAngularVel, advectedRadialToPoint);
            finalReadPos = advectedPointPos;

            return velocityAtPoint;
        }
    }
}
