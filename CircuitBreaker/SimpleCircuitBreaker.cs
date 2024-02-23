namespace CircuitBreaker
{
    public class SimpleCircuitBreaker : ICircuitBreaker
    {
        private bool _open;

        public SimpleCircuitBreaker()
        {
            _open = false;
        }

        //TODO: make the interval configurable
        //TODO: consider linear/exponential backoff
        public async Task OpenForIntervalAsync()
        {
            _open = true;
            await Task.Delay(120_000);
            _open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }
    }
}
