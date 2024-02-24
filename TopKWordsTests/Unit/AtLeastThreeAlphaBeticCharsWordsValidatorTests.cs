using FluentAssertions;
using NSubstitute;
using WordValidation;

namespace TopKWordsTests.Unit
{
    public class AtLeastThreeAlphaBeticCharsWordsValidatorTests
    {
        [Fact]
        public async Task IsValid_InBankButTooShort_ShouldReturnFalse()
        {
            IWordsBank wordsBank = MockWordsBank();
            AtLeastThreeAlphaBeticCharsWordsValidator atLeastThreeAlphaBeticCharsWordsValidator = new(wordsBank);
            await atLeastThreeAlphaBeticCharsWordsValidator.InitAsync();

            (await atLeastThreeAlphaBeticCharsWordsValidator.IsValidAsync("x")).Should().Be(false);
        }

        [Fact]
        public async Task IsValid_InBankAndMoreThan3CharsButNotOnlyAlphaBetic_ShouldReturnFalse()
        {
            IWordsBank wordsBank = MockWordsBank();
            AtLeastThreeAlphaBeticCharsWordsValidator atLeastThreeAlphaBeticCharsWordsValidator = new(wordsBank);
            await atLeastThreeAlphaBeticCharsWordsValidator.InitAsync();

            (await atLeastThreeAlphaBeticCharsWordsValidator.IsValidAsync("xyz1")).Should().Be(false);
        }

        [Fact]
        public async Task IsValid_NotInBank_ShouldReturnFalse()
        {
            IWordsBank wordsBank = MockWordsBank();
            AtLeastThreeAlphaBeticCharsWordsValidator atLeastThreeAlphaBeticCharsWordsValidator = new(wordsBank);
            await atLeastThreeAlphaBeticCharsWordsValidator.InitAsync();

            (await atLeastThreeAlphaBeticCharsWordsValidator.IsValidAsync("notInBank")).Should().Be(false);
        }

        private IWordsBank MockWordsBank()
        {
            var sub = Substitute.For<IWordsBank>();
            sub.IsWordInBank(Arg.Is<string>(input => input == "x")).Returns(true);//in bank, but less than 3 chars
            sub.IsWordInBank(Arg.Is<string>(input => input == "xyz1")).Returns(true);//in bank, but 
            return sub;
        }
    }
}
