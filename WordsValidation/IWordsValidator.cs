namespace WordValidation
{
    public interface IWordsValidator
    {
        Task<bool> IsValid(string str);
        Task Init();
    }
}
