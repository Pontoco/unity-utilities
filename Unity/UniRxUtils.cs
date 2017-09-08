using UniRx;

namespace Utilities
{
    public static class UniRxUtils
    {
        /// <summary>
        ///     Returns a new Observable that is the same as the self stream, but only emits items when
        ///     the second observable is currently true (ie. it's last value was true). This essentially
        ///     filters the first stream based on the current state of the second.
        /// </summary>
        public static IObservable<T> FilterWithLatestFrom<T>(this IObservable<T> self, IObservable<bool> other)
        {
            return self.WithLatestFrom(other, (exit, pressed) => new Tuple<T, bool>(exit, pressed))
                       .Where(tuple => tuple.Item2)
                       .Select(t => t.Item1);
        }

        /// <summary>
        ///     A gate that allows a single item of source through, and resets whenever gatingStream emits
        ///     an item. Multiple consecutive resets aren't additive, ie. emitting multiple items from gating
        ///     stream only resets the gate once.
        /// </summary>
        /// <param name="source">The source to filter.</param>
        /// <param name="gatingStream">The stream used to gate the source.</param>
        public static IObservable<T> GatedOn<T, V>(this IObservable<T> source, IObservable<V> gatingStream)
        {
            var stamps = gatingStream.Timestamp();
            return source.WithLatestFrom(stamps, (value, stamp) => new Tuple<T, Timestamped<V>>(value, stamp))
                         .DistinctUntilChanged(pair => pair.Item2.Timestamp)
                         .Select(pair => pair.Item1);
        }
    }
}
