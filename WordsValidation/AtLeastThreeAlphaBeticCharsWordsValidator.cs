using WordsBank;

namespace WordValidation
{
    public class AtLeastThreeAlphaBeticCharsWordsValidator : IWordsValidator
    {
        private IWordsBank _wordsBank;
        public AtLeastThreeAlphaBeticCharsWordsValidator(IWordsBank wordsBank)
        {
            _wordsBank = wordsBank;
        }

        public bool IsValid(string str)
        {
            return str != null && str.Length >= 3 && _wordsBank.IsWordInBank(str);
        }
    }
}
