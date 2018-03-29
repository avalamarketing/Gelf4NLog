using Gelf4NLog.Target;
using Gelf4NLog.Target.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gelf4NLog.UnitTest
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Truncate_MultibyteCharactersWithSuffix_ReturnsTruncatedStringWithSuffix()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab...";

            var result = sut.Truncate(5, "...");

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_AfterMultibyteCharactersWithSuffix_ReturnsTruncatedStringWithSuffix()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab...";

            var result = sut.Truncate(7, "...");

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_MultibyteCharacterOnFirstByte_ReturnsCharactersBeforeMultibyte()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab";

            var result = sut.Truncate(3);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_MultibyteCharacterOnSecondByte_ReturnsCharactersBeforeMultibyte()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab";

            var result = sut.Truncate(4);

            Assert.Equal(expectedResult, result);
        }


        [Fact]
        public void Truncate_MultibyteCharacterOnThirdByte_ReturnsCharactersBeforeMultibyte()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab";

            var result = sut.Truncate(5);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_MultibyteCharacterOnFourthByte_ReturnsCharactersBeforeMultibyte()
        {
            var sut = "ab👊cd";
            var expectedResult = "ab";

            var result = sut.Truncate(6);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_StringWithAllMultibyteCharactersOnFirstByte_ReturnsFirstFullCharacterBeforeTruncate()
        {
            var sut = "▨👊👍";
            var expectedResult = "▨";

            var result = sut.Truncate(4);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_StringWithAllMultibyteCharactersOnSecondByte_ReturnsFirstFullCharacterBeforeTruncate()
        {
            var sut = "▨👊👍";
            var expectedResult = "▨";

            var result = sut.Truncate(5);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_StringWithAllMultibyteCharactersOnThridByte_ReturnsFirstFullCharacterBeforeTruncate()
        {
            var sut = "▨👊👍";
            var expectedResult = "▨";

            var result = sut.Truncate(6);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_StringWithAllMultibyteCharactersOnFourthByte_ReturnsFirstFullCharacterBeforeTruncate()
        {
            var sut = "▨👊👍";
            var expectedResult = "▨";

            var result = sut.Truncate(7);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Truncate_SingleByteCharacter_ReturnsStringWithSizeOfSizeInBytes()
        {
            var sut = new string('a', 20);

            var result = sut.Truncate(10);

            Assert.Equal(10, Encoding.UTF8.GetByteCount(result));
        }
    }
}
