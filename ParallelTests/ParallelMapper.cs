using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTests
{
    public class ParallelMapper
    {
        public static async Task<IEnumerable<TOut>> MapAsync<TIn,TOut>(IEnumerable<TIn> items, Func<TIn,CancellationToken,Task<TOut>> action, MapOptions options)
        {
            var chunks = items.Chunk(options.MaxChunks);
            var results = new List<TOut>();
            foreach (var chunk in chunks)
            {
                var tasks = chunk.Select(i => action(i, options.CancellationToken));
                results.AddRange(await Task.WhenAll(tasks));
            }
            return results;
        }
        public static async Task<IEnumerable<TOut>> MapAsync<TIn, TOut>(IEnumerable<ICancelable<TIn>> items, Func<ICancelable<TIn>, Task<TOut>> action, MapOptions options)
        {
            var chunks = items.Chunk(options.MaxChunks);
            var results = new List<TOut>();
            foreach (var chunk in chunks)
            {
                var tasks = chunk.Select(i => action(i));
                results.AddRange(await Task.WhenAll(tasks));
            }
            return results;
        }
    }
    public class ItemCancelavel<T> : ICancelable<T>
    {
        public T? Value { get; set; }
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        public ItemCancelavel()
        {

        }
        public ItemCancelavel(T value, CancellationToken cancellationToken)
        {
            Value = value;
            CancellationToken = cancellationToken;
        }
    }

    public class MapOptions
    {
        public int MaxChunks { get; set; } = 10;
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
    }
}
