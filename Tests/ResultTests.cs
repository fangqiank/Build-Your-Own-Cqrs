using BuildYourOwnCqrs;

namespace Tests
{
    public class ResultTests
    {
        [Fact]
        public void Success_Result_HasIsSuccessTrue()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Error);
        }

        [Fact]
        public void Failure_Result_HasError()
        {
            // Act
            var result = Result.Failure("Test error");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test error", result.Error);
        }

        [Fact]
        public void SuccessT_Result_HasValue()
        {
            // Act
            var result = Result<string>.Success("Test value");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test value", result.Value);
        }

        [Fact]
        public void FailureT_Result_HasError()
        {
            // Act
            var result = Result<string>.Failure("Test error");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test error", result.Error);
            Assert.Null(result.Value);
        }
    }

}
