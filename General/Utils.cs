using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Conditions;
using Optional;
using UnityEngine.Assertions;

public static class Utils
{
    /// <summary>
    ///   Prints out a set of items, one on each line.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    public static void PrintOnLines<T>(this IEnumerable<T> source)
    {
        foreach (T e in source)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    /// <summary>
    /// Selects consecutive pairs of elements from an iterable list.
    /// </summary>
    public static IEnumerable<TResult> SelectTwo<TSource, TResult>(this IEnumerable<TSource> source,
                                                                   Func<TSource, TSource, TResult> selector)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (selector == null) throw new ArgumentNullException("selector");

        return SelectTwoImpl(source, selector);
    }

    private static IEnumerable<TResult> SelectTwoImpl<TSource, TResult>(this IEnumerable<TSource> source,
                                                                        Func<TSource, TSource, TResult> selector)
    {
        using (var iterator = source.GetEnumerator())
        {
            var item2 = default(TSource);
            var i = 0;
            while (iterator.MoveNext())
            {
                var item1 = item2;
                item2 = iterator.Current;
                i++;

                if (i >= 2)
                {
                    yield return selector(item1, item2);
                }
            }
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>
    /// Converts processor ticks to time in ms.
    /// </summary>
    public static double TicksToMs(long ticks)
    {
        Debug.Assert(Stopwatch.IsHighResolution);
        return (double) ticks / Stopwatch.Frequency * 1000.0;
    }

    /// <summary>
    /// Unwraps the given option. If the option is a None, returns the given default value instead.
    /// </summary>
    public static T UnwrapOrDefault<T, V>(this Option<V> option, Func<V, T> map, T defaultValue)
    {
        return option.Match(map, () => defaultValue);
    }


    /// <summary>
    /// A <see cref="Condition"/> validator to ensure that an integer is divisible by some modulo.
    /// </summary>
    public static ConditionValidator<int> IsDivisibleBy(this ConditionValidator<int> validator, int modulo)
    {
        if (validator.Value % modulo != 0)
        {
            throw new AssertionException(
                $"{validator.ArgumentName}: {validator.Value} is not divisible by {modulo}", "");
        }

        return validator;
    }

    /// <summary>
    /// Looks for a key in the dictionary, and returns its value. If it is not found, returns the provided default value.
    /// </summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                         TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    /// <summary>
    /// Looks for a key in the dictionary, and returns its value. If it is not found, returns the provided default value, using the provider function.
    /// </summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                         Func<TValue> defaultValueProvider)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValueProvider();
    }

    /// <summary>
    /// Gets the value associated with the given key. If not found, inserts the provided value, using the provider function, then returns the value.
    /// </summary>
    public static TValue GetOrInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                   Func<TValue> valueToInsert)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        var value = valueToInsert();
        dictionary[key] = value;
        return value;
    }
}
