
namespace ParallelTests
{
    public interface ICancelable<T>
    {
        CancellationToken CancellationToken { get; set; }
        T? Value { get; set; }
    }
}