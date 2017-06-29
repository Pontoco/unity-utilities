using System;

namespace Utilities {
  public class NumUtils {

    /// <summary>
    ///   Returns the nearest float to value that is a multiplier of factor.
    /// </summary>
    public static float NearestMultiple(float value, float factor) {
      return (float) System.Math.Round(value / (double) factor, MidpointRounding.AwayFromZero) * factor;
    }

    /// <summary>
    ///   Returns the first multiple of factor greater than value.
    /// </summary>
    public static float NextHighestMultiple(float value, float factor) {
      return (float) NextHighestMultiple(value, (double) factor);
    }

    /// <summary>
    ///   Returns the first multiple of factor greater than value.
    /// </summary>
    public static double NextHighestMultiple(double value, double factor) {
      return System.Math.Ceiling(value / factor) * factor;
    }

    /// <summary>
    ///   Returns the distance between two values in modulo space.
    /// </summary>
    public static float DistanceInModulo(float value, float target, float modulo) {
      var diff = Math.Abs(value - target);
      return Math.Min(diff, modulo - diff);
    }
  }
}
