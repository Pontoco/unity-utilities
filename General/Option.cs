/// Modified from https://github.com/nlkl/Optional/blob/master/LICENSE

namespace Optional
{
    public static class Option
    {
        /// <summary>Creates an empty Option&lt;T&gt; instance.</summary>
        /// <returns>An empty optional.</returns>
        public static Option<T> None<T>()
        {
            return new Option<T>(default(T), false);
        }

        /// <summary>Wraps an existing value in an Option&lt;T&gt; instance.</summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value, true);
        }
    }

    /// <summary>Provides a set of functions for creating optional values.</summary>
    public static class Optional
    {
        /// <summary>Wraps an existing value in an Option&lt;T&gt; instance.</summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<T> Some<T>(this T value)
        {
            return new Option<T>(value, true);
        }

        /// <summary>
        ///     Creates an Option&lt;T&gt; instance from a specified value. If the value is null, an empty
        ///     optional is returned.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<T> NullToOption<T>(this T value) where T : class
        {
            if (value == null)
            {
                return Option.None<T>();
            }

            return value.Some();
        }

        /// <summary>Converts a Nullable&lt;T&gt; to an Option&lt;T&gt; instance.</summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <returns>The Option&lt;T&gt; instance.</returns>
        public static Option<T> ToOption<T>(this T? value) where T : struct
        {
            return value.HasValue ? Option.Some(value.Value) : Option.None<T>();
        }
    }
}
