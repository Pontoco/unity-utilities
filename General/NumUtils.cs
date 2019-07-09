using System;

namespace Utilities
{
    /// <summary>
    /// A collection of utilities for working with numbers, integers, modulos, etc.
    /// </summary>
    public class NumUtils
    {
        /// <summary>Returns the nearest float to value that is a multiplier of factor.</summary>
        public static int NearestMultiple(int value, int factor)
        {
            return ((int) Math.Round(value / (double) factor, MidpointRounding.AwayFromZero)) * factor;
        }

        /// <summary>Returns the nearest float to value that is a multiplier of factor.</summary>
        public static float NearestMultiple(float value, float factor)
        {
            return (float) Math.Round(value / (double) factor, MidpointRounding.AwayFromZero) * factor;
        }

        /// <summary>Returns the first multiple of factor greater than value.</summary>
        public static int NextHighestMultiple(int value, int factor)
        {
            return (int)Math.Ceiling((double)value / factor) * factor;
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
        ///     Whether to force the final value to be within start/end or to allow the linear
        ///     interpolation to extend outside (if value > 1, for instance).
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
        ///     Whether to force the final value to be within start/end or to allow the linear
        ///     interpolation to extend outside (if value > 1, for instance).
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
    }
}
