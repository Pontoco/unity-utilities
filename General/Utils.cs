using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Conditions;
using Optional;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

public static class Utils
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    /// <summary>Prints out a set of items, one on each line.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    public static void PrintOnLines<T>(this IEnumerable<T> source)
    {
        foreach (T e in source)
        {
            Debug.Log(e);
        }
    }

    /// <summary>Selects consecutive pairs of elements from an iterable list.</summary>
    public static IEnumerable<TResult> SelectTwo<TSource, TResult>(this IEnumerable<TSource> source,
                                                                   Func<TSource, TSource, TResult> selector)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (selector == null)
        {
            throw new ArgumentNullException("selector");
        }

        return SelectTwoImpl(source, selector);
    }

    private static IEnumerable<TResult> SelectTwoImpl<TSource, TResult>(this IEnumerable<TSource> source,
                                                                        Func<TSource, TSource, TResult> selector)
    {
        using (IEnumerator<TSource> iterator = source.GetEnumerator())
        {
            TSource item2 = default;
            int i = 0;
            while (iterator.MoveNext())
            {
                TSource item1 = item2;
                item2 = iterator.Current;
                i++;

                if (i >= 2)
                {
                    yield return selector(item1, item2);
                }
            }
        }
    }

    /// <summary>Converts processor ticks to time in ms.</summary>
    public static double TicksToMs(long ticks)
    {
        System.Diagnostics.Debug.Assert(Stopwatch.IsHighResolution);
        return (double) ticks / Stopwatch.Frequency * 1000.0;
    }

    /// <summary>
    ///     Unwraps the given option. If the option is a None, returns the given default value instead. Additionally
    ///     applies a mapping function to the option.
    /// </summary>
    public static T GetOrDefault<T, V>(this Option<V> option, Func<V, T> map, T defaultValue)
    {
        if (option.HasValue)
        {
            return map(option.Value);
        }

        return defaultValue;
    }

    /// <summary>Performs a canonical Modulus operation, where the output is on the range [0, b).</summary>
    private static int Mod(int value, int modulo)
    {
        int c = value % modulo;
        if (c < 0 && modulo > 0 || c > 0 && modulo < 0)
        {
            c += modulo;
        }

        return c;
    }

    /// <summary>
    ///     A <see cref="Condition" /> validator that ensures a value is equal to an expected value, modulo some number.
    ///     Both the tested value and the expected value have the modulus operation applied.
    /// </summary>
    /// <param name="validator">Validator to build on.</param>
    /// <param name="expected">The expected value. Must be >0 and <modulo.</param>
    /// <param name="modulo">The modulus value to apply to the tested value.</param>
    public static ConditionValidator<int> IsModuloEqualTo(this ConditionValidator<int> validator, int expected,
                                                          int modulo)
    {
        if (Mod(validator.Value, modulo) != Mod(expected, modulo))
        {
            throw new AssertionException(
                $"{validator.ArgumentName}: {validator.Value} mod {modulo} is not equal to {expected}", "");
        }

        return validator;
    }

    /// <summary>A <see cref="Condition" /> validator to ensure that an integer is divisible by some modulo.</summary>
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
    ///     Looks for a key in the dictionary, and returns its value. If it is not found, returns the provided default
    ///     value.
    /// </summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                         TValue defaultValue)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
    }

    /// <summary>
    ///     Looks for a key in the dictionary, and returns its value. If it is not found, returns the provided default
    ///     value, using the provider function.
    /// </summary>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                         Func<TValue> defaultValueProvider)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : defaultValueProvider();
    }

    /// <summary>
    ///     Gets the value associated with the given key. If not found, inserts the provided value, using the provider
    ///     function, then returns the value.
    /// </summary>
    public static TValue GetOrInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                   Func<TValue> valueToInsert)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        TValue value = valueToInsert();
        dictionary[key] = value;
        return value;
    }

    /// <summary>Scans the given directory and deletes all files within it, except the most recent N.</summary>
    /// <param name="directoryPath">The directory to scan.</param>
    /// <param name="numberToKeep">The number of files to keep.</param>
    /// <param name="searchPattern">This operation will only look at the files that match this search pattern.</param>
    public static void KeepLatestFiles(string directoryPath, int numberToKeep, string searchPattern = "*")
    {
        DirectoryInfo directory = new DirectoryInfo(directoryPath);
        FileInfo[] fileInfos = directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

        // Keep the most recent recordings, but delete the rest.
        IEnumerable<FileInfo> filesToDelete =
            fileInfos.OrderByDescending(f => f.CreationTime).Skip(numberToKeep);

        foreach (FileInfo fileInfo in filesToDelete)
        {
            fileInfo.Delete();
        }
    }

    /// <summary>Scans the given directory and deletes all directories within it, except the most recent N.</summary>
    /// <param name="directoryPath">The directory to scan.</param>
    /// <param name="numberToKeep">The number of directories to keep.</param>
    /// <param name="searchPattern">This operation will only look at the directories that match this search pattern.</param>
    /// <param name="fullPathsToIgnore">Any full file paths included in this array won't be deleted.</param>
    /// <param name="deleteRecursively">
    ///     This function will only delete empty folders by default. Set this to true to delete all
    ///     files within.    ///
    /// </param>
    public static void KeepLatestDirectories(string directoryPath, int numberToKeep, string searchPattern = "*",
                                             string[] fullPathsToIgnore = null, bool deleteRecursively = false)
    {
        DirectoryInfo directory = new DirectoryInfo(directoryPath);
        DirectoryInfo[] directoryInfos = directory.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);

        // Keep the most recent recordings, but delete the rest.
        IEnumerable<DirectoryInfo> directoriesToDelete =
            directoryInfos.OrderByDescending(f => f.CreationTime).Skip(numberToKeep);

        foreach (DirectoryInfo directoryInfo in directoriesToDelete)
        {
            if (fullPathsToIgnore != null && !fullPathsToIgnore.Contains(directoryInfo.FullName))
            {
                directoryInfo.Delete(deleteRecursively);
            }
        }
    }
}
