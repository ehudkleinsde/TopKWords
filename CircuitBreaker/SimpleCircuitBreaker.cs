namespace CircuitBreaker
{
    public class SimpleCircuitBreaker : ICircuitBreaker
    {
        private volatile bool _open;

        public SimpleCircuitBreaker()
        {
            _open = false;
        }

        //TODO: make the interval configurable
        //TODO: consider linear/exponential backoff
        public async Task OpenForIntervalAsync(int interval = 60_000)
        {
            _open = true;
            await Task.Delay(interval);
            _open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }
    }
}
