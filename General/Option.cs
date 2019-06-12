/// Modified from https://github.com/nlkl/Optional/blob/master/LICENSE

namespace Optional
{
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
                return Option<T>.None();
            }

            return value.Some();
        }

        /// <summary>Converts a Nullable&lt;T&gt; to an Option&lt;T&gt; instance.</summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <returns>The Option&lt;T&gt; instance.</returns>
        public static Option<T> ToOption<T>(this T? value) where T : struct
        {
            return value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None();
        }
    }
}
