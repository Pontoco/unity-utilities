// Modified from https://github.com/nlkl/Optional/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace Optional
{
    /// <summary>Represents an optional value. It contains either a value of type T, or None. </summary>
    /// <typeparam name="T">The type of the value to be wrapped.</typeparam>
    [Serializable]
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T value;

        /// <summary>Checks if a value is present.</summary>
        public bool HasValue { get; }

        /// <summary>The value currently in the Option. Throws if the option is none.</summary>
        /// <exception cref="InvalidOperationException"></exception>
        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Called option.Value on empty option of type " +
                                                        typeof(T).Name + ".");
                }

                return value;
            }
        }

        internal Option(T value, bool hasValue)
        {
            this.value = value;
            HasValue = hasValue;
        }

        /// <summary>Determines whether two optionals are equal.</summary>
        /// <param name="other">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public bool Equals(Option<T> other)
        {
            if (!HasValue && !other.HasValue)
            {
                return true;
            }

            if (HasValue && other.HasValue)
            {
                return EqualityComparer<T>.Default.Equals(value, other.value);
            }

            return false;
        }

        /// <summary>Determines whether two optionals are equal.</summary>
        /// <param name="obj">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Option<T>))
            {
                return false;
            }

            return Equals((Option<T>) obj);
        }

        /// <summary>Determines whether two optionals are equal.</summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether two optionals are unequal.</summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>Generates a hash code for the current optional.</summary>
        /// <returns>A hash code for the current optional.</returns>
        public override int GetHashCode()
        {
            if (!HasValue)
            {
                return 0;
            }

            if (value == null)
            {
                return 1;
            }

            return value.GetHashCode();
        }

        /// <summary>Returns a string that represents the current optional.</summary>
        /// <returns>A string that represents the current optional.</returns>
        public override string ToString()
        {
            if (!HasValue)
            {
                return "None";
            }

            if (value == null)
            {
                return "Some(null)";
            }

            return $"Some({value})";
        }

        /// <summary>Determines if the current optional contains a specified value.</summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>A boolean indicating whether or not the value was found.</returns>
        public bool Contains(T value)
        {
            if (!HasValue)
            {
                return false;
            }

            if (this.value == null)
            {
                return value == null;
            }

            return this.value.Equals(value);
        }

        /// <summary>Determines if the current optional contains a value satisfying a specified predicate.</summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public bool Exists(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (HasValue)
            {
                return predicate(value);
            }

            return false;
        }

        /// <summary>Returns the existing value if present, and otherwise an alternative value.</summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public T ValueOr(T alternative)
        {
            if (!HasValue)
            {
                return alternative;
            }

            return value;
        }

        /// <summary>Uses an alternative value, if no existing value is present.</summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Option<T> Or(T alternative)
        {
            if (!HasValue)
            {
                return Some(alternative);
            }

            return this;
        }

        /// <summary>Uses an alternative optional, if no existing value is present.</summary>
        /// <param name="alternativeOption">The alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option<T> Else(Option<T> alternativeOption)
        {
            if (!HasValue)
            {
                return alternativeOption;
            }

            return this;
        }

        /// <summary>Uses an alternative optional, if no existing value is present.</summary>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Option<T> Else(Func<Option<T>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null)
            {
                throw new ArgumentNullException(nameof(alternativeOptionFactory));
            }

            if (!HasValue)
            {
                return alternativeOptionFactory();
            }

            return this;
        }

        /// <summary>Empties an optional if the value is null.</summary>
        /// <returns>The filtered optional.</returns>
        public Option<T> NotNull()
        {
            if (!HasValue || value != null)
            {
                return this;
            }

            return None();
        }

        /// <summary>Returns the value if it exists, or throw the custom error message if it doesn't.</summary>
        public T ValueOrFailure(string failureMessage)
        {
            if (!HasValue)
            {
                throw new InvalidOperationException(failureMessage);
            }

            return Value;
        }

        /// <summary>Creates an empty Option&lt;T&gt; instance.</summary>
        /// <returns>An empty optional.</returns>
        public static Option<T> None()
        {
            return new Option<T>(default, false);
        }

        /// <summary>Wraps an existing value in an Option&lt;T&gt; instance.</summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Option<T> Some(T value)
        {
            return new Option<T>(value, true);
        }
    }
}
