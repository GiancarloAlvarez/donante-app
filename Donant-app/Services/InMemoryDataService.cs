using Donant_app.Models;
using System.Security.Cryptography;

namespace Donant_app.Services;

internal sealed class InMemoryDataService
{
    private const int PasswordIterations = 100_000;
    private static readonly Lazy<InMemoryDataService> LazyInstance = new(() => new InMemoryDataService());

    public static InMemoryDataService Instance => LazyInstance.Value;

    public Usuario? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser is not null;

    // Cuentas de demostración; las contraseñas no se almacenan en texto plano.
    private readonly List<Usuario> _users =
    [
        new()
        {
            Id = 1,
            Username = "admin",
            PasswordSalt = "N9b43p8vp2aHd8KfH4DYLg==",
            PasswordHash = "dqun7wzpiup3D+MC40cdjauXBfwn8MogZEsJ6vX78ak=",
            Role = "admin"
        },
        new()
        {
            Id = 2,
            Username = "donante",
            PasswordSalt = "xeo7daQwOt0fmhvD9N/uiA==",
            PasswordHash = "AJsNkGMy4I41aYH+iSNMsD5gBvjL7jb2GUg0kEKeV74=",
            Role = "donor"
        }
    ];

    private InMemoryDataService()
    {
    }

    public bool Login(string username, string password)
    {
        Usuario? user = _users.FirstOrDefault(candidate =>
            string.Equals(candidate.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));

        if (user is null || !VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
        {
            CurrentUser = null;
            return false;
        }

        CurrentUser = user;
        return true;
    }

    public void Logout() => CurrentUser = null;

    private static bool VerifyPassword(string password, string salt, string expectedHash)
    {
        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromBase64String(salt),
            PasswordIterations,
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            Convert.FromBase64String(expectedHash));
    }
}
