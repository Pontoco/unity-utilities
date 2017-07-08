using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Conditions;
using NUnit.Framework;
using Optional;

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

    /// <summary>
    /// Converts processor ticks to time in ms.
    /// </summary>
    public static double TicksToMs(long ticks)
    {
        Debug.Assert(Stopwatch.IsHighResolution);
        return (double)ticks / Stopwatch.Frequency * 1000.0;
    }


    public static class EnumUtil
    {
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public static T UnwrapOrDefault<T, V>(this Option<V> option, Func<V, T> map, T defaultValue)
    {
        return option.Match(map, () => defaultValue);
    }

    public static ConditionValidator<int> IsDivisibleBy(this ConditionValidator<int> validator, int modulo)
    {
        if (validator.Value % modulo != 0)
        {
            throw new AssertionException(validator.ArgumentName + ": " + validator.Value + " is not divisible by " + modulo);
        }
        return validator;
    }
}
