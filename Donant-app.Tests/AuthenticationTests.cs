using Donant_app.Services;

namespace Donant_app.Tests;

public class AuthenticationTests
{
    [Theory]
    [InlineData("admin", "admin123")]
    [InlineData("ADMIN", "admin123")]
    [InlineData("donante", "dona123")]
    public void Login_AcceptsValidDemoCredentials(string username, string password)
    {
        InMemoryDataService service = InMemoryDataService.Instance;
        service.Logout();

        Assert.True(service.Login(username, password));
        Assert.True(service.IsLoggedIn);
    }

    [Theory]
    [InlineData("admin", "wrong")]
    [InlineData("unknown", "admin123")]
    public void Login_RejectsInvalidCredentials(string username, string password)
    {
        InMemoryDataService service = InMemoryDataService.Instance;
        service.Logout();

        Assert.False(service.Login(username, password));
        Assert.False(service.IsLoggedIn);
    }
}
