using ProductionSystem.Api.Services;

namespace ProductionSystem.Api.Tests.Unit;

public class CustomerPasswordRulesTests
{
    [Theory]
    [InlineData("Abc1")]
    [InlineData("Password1")]
    public void TryValidate_ValidPassword_ReturnsTrue(string password)
    {
        var ok = CustomerPasswordRules.TryValidate(password, out var error);

        Assert.True(ok);
        Assert.Null(error);
    }

    [Theory]
    [InlineData("abc1", "заглавную")]
    [InlineData("ABC", "цифру")]
    [InlineData("Ab1*", "*")]
    [InlineData("Ab", "4")]
    public void TryValidate_InvalidPassword_ReturnsFalse(string password, string _)
    {
        var ok = CustomerPasswordRules.TryValidate(password, out var error);

        Assert.False(ok);
        Assert.NotNull(error);
        Assert.NotEmpty(error);
    }
}
