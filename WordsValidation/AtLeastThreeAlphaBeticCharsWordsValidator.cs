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

        public async Task InitAsync()
        {
            await _wordsBank.InitAsync();
            _isInit = true;
        }

        public bool IsInit()
        {
            return _isInit;
        }

        //TODO: make validation logic configurable
        public async Task<bool> IsValidAsync(string str)
        {
            //TODO: Log and implement new exception type
            if (!IsInit()) { throw new Exception("Not initialized"); }
            return str != null && str.Length >= 3 && OnlyAlphaBeticChars(str) && await _wordsBank.IsWordInBank(str);
        } 

        private bool OnlyAlphaBeticChars(string str)
        {
            foreach(char c in str)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
