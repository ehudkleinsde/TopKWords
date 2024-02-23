namespace WordValidation
{
    public interface IWordsBank
    {
        Task<bool> IsWordInBank(string word);
        Task Init();
    }
}
