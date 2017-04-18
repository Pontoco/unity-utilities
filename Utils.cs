using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public static class Utils {
  public static class EnumUtil {
    public static IEnumerable<T> GetEnumValues<T>() {
      return Enum.GetValues(typeof(T)).Cast<T>();
    }
  }

  public static T UnwrapOrDefault<T, V>(this Option<V> option, Func<V,T> map, T default_value) {
    return option.Match(some: map, none: () => default_value);
  }
}
