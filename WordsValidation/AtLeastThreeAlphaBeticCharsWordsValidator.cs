namespace WordValidation
{
    public class AtLeastThreeAlphaBeticCharsWordsValidator : IWordsValidator
    {
        private IWordsBank _wordsBank;
        private bool _isInit;

        public AtLeastThreeAlphaBeticCharsWordsValidator(IWordsBank wordsBank)
        {
            _wordsBank = wordsBank;
        }

        public async Task Init()
        {
            await _wordsBank.Init();
            _isInit = true;
        }

        //TODO: make validation logic configurable
        public async Task<bool> IsValid(string str)
        {
            //TODO: Log and implement new exception type
            if (!_isInit) { throw new Exception("Not initialized"); }
            return str != null && str.Length >= 3 && await _wordsBank.IsWordInBank(str);
        }
    }
}
