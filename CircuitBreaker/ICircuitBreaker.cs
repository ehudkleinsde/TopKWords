namespace CircuitBreaker
{
    public interface ICircuitBreaker
    {
        bool IsOpen();
        Task OpenForIntervalAsync();
    }
}
