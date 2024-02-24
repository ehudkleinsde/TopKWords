namespace WordValidation
{
    public interface IWordsValidator
    {
        Task<bool> IsValidAsync(string str);
        Task InitAsync();
        bool IsInit();
    }
}
