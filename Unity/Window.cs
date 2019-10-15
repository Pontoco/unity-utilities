using System.Collections.Generic;
using Utilities;

namespace AsgUtils.Unity
{
    /// <summary>
    ///     A circular(ring) buffer holding the last N elements added to it. This is useful to keep a window of N size
    ///     over changes to a value. The window will always have less than N elements in it. It has no elements when it is
    ///     first created. See <see cref="CurrentSize" />.
    /// </summary>
    public class Window<T>
    {
        /// <summary>The current number of elements in the window. Starts at 0 and grows until equal to size.</summary>
        public int CurrentSize { get; private set; }

        private readonly T[] array;

        // The next index to write into
        private int nextIndex;

        /// <summary>Creates a new window.</summary>
        /// <param name="size">The number of elements. The size of the window.</param>
        public Window(int size)
        {
            array = new T[size];
        }

        /// <summary>
        ///     Adds a new element, either growing the size within the window, or kicking out the oldest element if the window
        ///     is already full.
        /// </summary>
        public void Add(T value)
        {
            array[nextIndex] = value;
            nextIndex++;
            nextIndex = NumUtils.Mod(nextIndex, array.Length);

            if (CurrentSize < array.Length)
            {
                CurrentSize++;
            }
        }

        /// <summary>An enumerator of the values in the window. Returns the oldest values first.</summary>
        /// <remarks>
        ///     This doesn't seem to allocate, but I'm a bit suspicious of C# generators. Might be worth being careful when
        ///     using this.
        /// </remarks>
        public IEnumerable<T> GetValues()
        {
            // If the window is full, the first element is actually the "next writable" element.
            int startIndex;
            if (CurrentSize == array.Length)
            {
                startIndex = nextIndex;
                startIndex = NumUtils.Mod(startIndex, array.Length);
            }
            else
            {
                // otherwise, the first element is just 0
                startIndex = 0;
            }

            // March through the buffer and return each element
            for (int i = 0; i < CurrentSize; i++)
            {
                yield return array[startIndex];
                startIndex++;
                startIndex = NumUtils.Mod(startIndex, array.Length);
            }
        }
    }
}
