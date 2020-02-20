using System;

namespace AsgUtils.General
{
    /// <summary>Integer sampler with an upper bound on the number of samples between repeated values.</summary>
    /// <remarks>
    ///     <para>
    ///         Let R = the exclusive upper bound of the sampling range and let D = the number of 'decks' in the sampler. In
    ///         each (R * D) samples, every value in the sampling range will appear D times. The upper bound on the number of
    ///         samples between repeated values is therefore ((R - 1) * D).
    ///     </para>
    ///     <para>
    ///         The sampler works like a shuffled deck of cards. Imagine a deck with a card for every value in the sampling
    ///         range. Then take D of these decks, shuffle them together. The shuffled cards are in an unpredictable order, but
    ///         there are exactly D occurrences of each value.
    ///     </para>
    ///     <para>
    ///         This sampler is NOT suitable for very large ranges because it requires storage proportional to (R * D). Large
    ///         deck numbers (D > 10) are also not recommended, as this will make the sampler nearly indistinguishable from a
    ///         uniform random sampler.
    ///     </para>
    /// </remarks>
    public class ShuffleRandom
    {
        private readonly Random random;

        private readonly int range;

        private int nextValue;

        private readonly int[] values;

        private readonly double[] keys;

        /// <summary>Create a new sampler.</summary>
        /// <param name="random">Pseudo-random number generator.</param>
        /// <param name="range">Exclusive upper bound of the sampling range. Must be greater than 0.</param>
        /// <param name="decks">
        ///     Number of 'decks' to use in the sampler. Must be greater than 0.
        ///     <remarks>
        ///         A smaller number of 'decks' will ensure that there aren't a large number of samples between repeated
        ///         values. A larger number of 'decks' will more closely approximate a uniform random sampler.
        ///     </remarks>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="range"/> is less than or equal to 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="decks"/> is less than or equal to 0.</exception>
        public ShuffleRandom(Random random, int range, int decks)
        {
            if (range <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }

            if (decks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(decks));
            }

            this.random = random;
            this.range = range;

            int valueCount = decks * range;
            values = new int[valueCount];
            keys = new double[valueCount];
            nextValue = valueCount;
        }

        /// <summary>
        /// Returns a non-negative integer less than the sampler's upper bound.
        /// </summary>
        public int Next()
        {
            if (nextValue == values.Length)
            {
                Shuffle();
            }

            return values[nextValue++];
        }

        private void Shuffle()
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = i % range;
                keys[i] = random.NextDouble();
            }

            Array.Sort(keys, values);

            nextValue = 0;
        }
    }
}
