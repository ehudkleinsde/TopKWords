using CircuitBreaker;
using EssaysProvider.EssaysList;
using EssaysProvider.SingleEssay;
using Logger;
using NSubstitute;
using TopKWords;
using TopKWordsConfigProvider;
using WordValidation;
using FluentAssertions;
using TopKWords.Contracts;

namespace TopKWordsTests.E2E
{
    public class E2E
    {
        [Fact]
        public async Task TopKWords_E2E()
        {
            ILogger logger = Substitute.For<ILogger>();
            ITopKWordsConfigProvider configProvider = MockConfigProvider();
            IEssaysListProvider essaysProvider = MockEssaysListProvider();
            ISingleEssayProvider singleEssayProvider = MockSingleEssayProvider();
            IWordsValidator wordsValidator = MockWordsValidator();
            ICircuitBreaker circuitBreaker = MockCircuitBreaker();
            TopKWordsFinder finder = new(logger, configProvider, essaysProvider, singleEssayProvider, wordsValidator, circuitBreaker);


            List<WordCount> result = await finder.ExecuteAsync();

            //Lorem ipsum dolor sit amet Lorem
            result[0].Word.Should().BeEquivalentTo("Lorem");
            result[0].Count.Should().Be(3);

            result[1].Word.Should().BeEquivalentTo("ipsum");
            result[1].Count.Should().Be(2);

            result[2].Word.Should().BeEquivalentTo("dolor");
            result[2].Count.Should().Be(1);

            result.Count.Should().Be(3);
        }

        private ITopKWordsConfigProvider MockConfigProvider()
        {
            var sub = Substitute.For<ITopKWordsConfigProvider>();
            sub.GetMaxRequestsPerMinute().Returns(100);
            sub.GetMaxRetriesForFetchingEssayContent().Returns(5);
            sub.GetTopKWordsToFind().Returns(10);

            return sub;
        }

        private IEssaysListProvider MockEssaysListProvider()
        {
            var sub = Substitute.For<IEssaysListProvider>();
            sub.GetEssaysListAsync().Returns(new List<Uri>() { new Uri("https://www.engadget.com") });
            return sub;
        }

        private ISingleEssayProvider MockSingleEssayProvider()
        {
            var sub = Substitute.For<ISingleEssayProvider>();
            sub.GetEssayContentAsync(Arg.Any<Uri>()).ReturnsForAnyArgs("Lorem ipsum dolor sit amet Lorem Lorem ipsum");
            return sub;
        }

        private IWordsValidator MockWordsValidator()
        {
            var sub = Substitute.For<IWordsValidator>();
            sub.IsValidAsync(Arg.Is<string>(input => input == "Lorem")).Returns(true);
            sub.IsValidAsync(Arg.Is<string>(input => input == "ipsum")).Returns(true);
            sub.IsValidAsync(Arg.Is<string>(input => input == "dolor")).Returns(true);

            sub.IsValidAsync(Arg.Is<string>(input => input == "sit")).Returns(false);
            sub.IsValidAsync(Arg.Is<string>(input => input == "amet")).Returns(false);
            return sub;
        }

        private ICircuitBreaker MockCircuitBreaker()
        {
            var sub = Substitute.For<ICircuitBreaker>();
            sub.IsOpen().Returns(false);
            return sub;
        }
    }
}