using FluentAssertions;
using Logger;
using NSubstitute;
using TopKWordsConfigProvider;
using WordValidation;

namespace TopKWordsTests.Unit
{
    public class WordsBankTests
    {
        [Fact]
        public async Task IsWordInBank()
        {
            TopKWordsConfigFileConfigProvider topKWordsConfigFileConfigProvider = new("../../../../TopKWords/Config.json");
            WordsBank wordsBank = new(topKWordsConfigFileConfigProvider, Substitute.For<ILogger>());

            await wordsBank.InitAsync();

            (await wordsBank.IsWordInBank("aberrational")).Should().Be(true);
            (await wordsBank.IsWordInBank("youtube")).Should().Be(false);
        }        
    }
}
