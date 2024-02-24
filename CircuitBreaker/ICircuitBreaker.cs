namespace CircuitBreaker
{
    public interface ICircuitBreaker
    {
        bool IsOpen();
        Task OpenForIntervalAsync(int interval = 60_000);
    }
}
