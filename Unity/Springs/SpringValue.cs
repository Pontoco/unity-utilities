using UnityEngine;

namespace Global.Utilities.Unity.Springs
{
    /// <summary>
    ///     Contains and controls a single value parameter on a spring. This makes it easy to spring different properties
    ///     without having to setup all of the boilerplate.  The overall formula is F = -kx - cv.  Where k is the spring
    ///     constant, x is the value different, c is the dampening and v is the velocity.
    /// </summary>
    /// <remarks>Must be stepped manually. Does not update itself.</remarks>
    public abstract class SpringValue<T> where T : struct
    {
        /// <summary>The target value this spring is attempting to move towards.</summary>
        public T TargetValue;

        /// <summary>The current value for this spring.</summary>
        public T CurrentValue;

        /// <summary>The velocity of the spring.</summary>
        public T CurrentVelocity;

        /// <summary>
        ///     The "Spring Constant" represented as K in most equations. Assumes a mass of 1.   A higher value makes the
        ///     spring 'stronger' or 'more taught'. The force applied scales  directly with this value.  ///
        /// </summary>
        public float SpringConstantK;

        /// <summary>
        ///     The dampening to apply each frame. This is in multiples of velocity. A value of 1 will apply a force of -1 on
        ///     the spring if the velocity is 1.
        /// </summary>
        public float SpringConstantDampening;

        /// <summary>
        ///     Creates a spring, with the given spring constants. Defaults are suitable for small scale springs between 1..10
        ///     ish.
        /// </summary>
        public SpringValue(float k = 10, float dampening = 1)
        {
            TargetValue = default;
            CurrentValue = default;
            CurrentVelocity = default;
            SpringConstantK = k;
            SpringConstantDampening = dampening;
        }

        /// <summary>
        ///     The implemented integration operation for each value type. This is generally like the float formula F = -kx -
        ///     cv, but can be different for more complex types such as Quaternions, or colors.
        /// </summary>
        /// <param name="dt"></param>
        protected abstract void StepInternal(float dt);

        /// <summary>Step this spring forward by some number of seconds.</summary>
        /// <param name="deltaTime">The amount of time in seconds to step by.</param>
        /// <param name="capDeltaTime">
        ///     If true, the deltaTime parameter is capped to be within a sensible range. Otherwise the
        ///     spring can explode.
        /// </param>
        public void Step(float deltaTime, bool capDeltaTime = true)
        {
            // When the window loses focus and returns, the time step can be very large. Cap it to avoid integrating the spring and 
            // getting ridiculous values.
            float dt = capDeltaTime ? Mathf.Min(deltaTime, .1f) : deltaTime;
            StepInternal(dt);
        }
    }

    /// <summary>
    ///     Contains and controls a single float parameter on a spring. This makes it easy to spring different properties
    ///     without having to setup all of the boilerplate.  The overall formula is F = -kx - cv.  Where k is the spring
    ///     constant, x is the value different, c is the dampening and v is the velocity.
    /// </summary>
    public class SpringFloat : SpringValue<float>
    {
        /// <inheritdoc />
        protected override void StepInternal(float dt)
        {
            // Integrate the spring.
            float difference = TargetValue - CurrentValue;
            float force = SpringConstantK * difference - SpringConstantDampening * CurrentVelocity;
            CurrentVelocity += force * dt;
            CurrentValue += CurrentVelocity * dt;
        }
    }

    /// <summary>
    ///     Contains and controls a single vector3 parameter on a spring. This makes it easy to spring different
    ///     properties without having to setup all of the boilerplate.  The overall formula is F = -kx - cv.  Where k is the
    ///     spring constant, x is the value different, c is the dampening and v is the velocity.
    /// </summary>
    public class SpringVector3 : SpringValue<Vector3>
    {
        /// <inheritdoc />
        protected override void StepInternal(float dt)
        {
            // Integrate the spring.
            Vector3 difference = TargetValue - CurrentValue;
            Vector3 force = SpringConstantK * difference - SpringConstantDampening * CurrentVelocity;
            CurrentVelocity += force * dt;
            CurrentValue += CurrentVelocity * dt;
        }
    }
}
