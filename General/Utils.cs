using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conditions;
using NUnit.Framework;
using Optional;

public static class Utils
{
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
