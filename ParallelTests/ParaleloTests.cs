namespace ParallelTests
{
    public class ParaleloTests
    {
        [Fact]
        public async Task Test_ForEach_1()
        {
            var numbers = new[] { 1, 2, 3, 4 , 5,6,7,8,9,10 };

            var results = new List<int>();
            Parallel.ForEach(numbers, (number) =>
            {
                lock(results)
                {
                    results.Add(number * 2);
                }
            });

            Assert.Equal(numbers.Length, results.Count);
        }
        [Fact]
        public async Task Test_ForEach_2()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var results = new List<int>();
            await Parallel.ForEachAsync(numbers, async (number,ct) =>
            {
                await Task.Delay(1000);
                lock(results)
                {
                    results.Add(number * 2);
                }
            });

            Assert.Equal(numbers.Length, results.Count);
        }
        [Fact]
        public async Task Test_MapAsync_1()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var results = await ParallelMapper.MapAsync(numbers, async (int number, CancellationToken cancellationToken) =>
            {
                await Task.Delay(1000, cancellationToken);
                return number * 2;
            }, new MapOptions() { 
                CancellationToken = CancellationToken.None,
                MaxChunks = 10
            });

            Assert.Equal(numbers.Length, results.Count());
        }
        [Fact]
        public async Task Test_MapAsync_2()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var results = await ParallelMapper.MapAsync(numbers.Select(n=> new ItemCancelavel<int>(n,CancellationToken.None)), async (ICancelable<int> item) =>
            {
                await Task.Delay(1000, item.CancellationToken);
                return item.Value * 2;
            }, new MapOptions()
            {
                CancellationToken = CancellationToken.None,
                MaxChunks = 10
            });

            Assert.Equal(numbers.Length, results.Count());
        }
    }
}