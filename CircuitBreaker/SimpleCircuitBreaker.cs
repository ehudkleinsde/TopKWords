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
        public async Task OpenForIntervalAsync()
        {
            _open = true;
            await Task.Delay(60000);
            _open = false;
        }

        public bool IsOpen()
        {
            return _open;
        }
    }
}
