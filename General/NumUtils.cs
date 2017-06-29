using System;
using UnityEngine;

namespace Utilities {
  public class NumUtils {
    /// <summary>
    ///   Determine the signed angle between two vectors, with normal 'n'
    ///   as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
      return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

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
      var diff = Mathf.Abs(value - target);
      return Mathf.Min(diff, modulo - diff);
    }
  }
}
