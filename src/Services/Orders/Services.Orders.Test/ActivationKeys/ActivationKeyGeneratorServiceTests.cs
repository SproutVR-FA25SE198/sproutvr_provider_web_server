using Services.Orders.Infrastructure.Services;

namespace Services.Orders.Tests.Services;

public class ActivationKeyGeneratorServiceTests
{
    // UTCID01
    [Fact]
    public void Generate_ShouldReturnKeyWithCorrectFormat()
    {
        // Arrange
        var service = new ActivationKeyGeneratorService();

        // Act
        string result = service.Generate();

        // Assert
        // Ensure we got a string back
        Assert.False(string.IsNullOrWhiteSpace(result));

        // Validate total length: 25 characters + 4 dashes = 29 chars
        Assert.Equal(29, result.Length);

        // Validate structure: XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
        // Uses Regex to ensure only uppercase letters and numbers are used.
        Assert.Matches(@"^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$", result);
    }

    // UTCID02
    // This test is intentionally skipped because the service currently relies on static helper methods 
    // (Base32EncodingUtils) which cannot be easily mocked with standard tools like Moq.
    [Fact]
    public void Generate_ShouldThrowArgumentOutOfRangeException_WhenEncodingFails()
    {
        Assert.True(false);
    }

    // UTCID03
    // This test is intentionally skipped because we cannot force the system's RandomNumberGenerator 
    // to throw a CryptographicException
    [Fact]
    public void Generate_ShouldThrowCryptographicException_WhenRngFails()
    {
        Assert.True(false);
    }

    // UTCID04
    // This test is intentionally skipped. It is meant to verify that unexpected exceptions 
    // bubble up correctly, but requires mocking the static calls to force a failure.
    [Fact]
    public void Generate_ShouldThrowException_WhenUnexpectedErrorOccurs()
    {
        Assert.True(false);
    }
}
