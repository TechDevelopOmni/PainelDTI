using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace PainelDTI.Services;

public interface IAuthApiClient
{
    Task<LoginApiResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
}

public sealed class AuthApiClient(HttpClient httpClient, IOptions<AuthApiOptions> options) : IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AuthApiOptions authApiOptions = options.Value;

    public async Task<LoginApiResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var endpoint = $"{authApiOptions.BaseUrl.TrimEnd('/')}/api/users/login";

        using var response = await httpClient.PostAsJsonAsync(endpoint, new
        {
            Email = email,
            Password = password
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorContent)
                ? "Não foi possível autenticar com a API."
                : errorContent.Trim('"');

            return LoginApiResult.Fail(message);
        }

        LoginApiResponse? payload;
        try
        {
            payload = await response.Content.ReadFromJsonAsync<LoginApiResponse>(JsonOptions, cancellationToken);
        }
        catch (JsonException)
        {
            return LoginApiResult.Fail("Resposta inválida da API de autenticação.");
        }

        if (payload is null || payload.User is null || string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return LoginApiResult.Fail("Resposta inválida da API de autenticação.");
        }

        return LoginApiResult.Success(
            payload.User.FullName,
            payload.User.Email,
            payload.User.GetUserTypeValue(),
            payload.AccessToken,
            payload.ExpiresAtUtc);
    }
}

public sealed class AuthApiOptions
{
    public string BaseUrl { get; set; } = "https://hubdaytrade.omnisaas.com.br";
}

public sealed record LoginApiResult(
    bool Succeeded,
    string? Error,
    string? FullName,
    string? Email,
    string? UserType,
    string? AccessToken,
    DateTime? ExpiresAtUtc)
{
    public static LoginApiResult Fail(string error) => new(false, error, null, null, null, null, null);

    public static LoginApiResult Success(string fullName, string email, string userType, string accessToken, DateTime expiresAtUtc) =>
        new(true, null, fullName, email, userType, accessToken, expiresAtUtc);
}

public sealed class LoginApiResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public LoginApiUserResponse? User { get; init; }
}

public sealed class LoginApiUserResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public JsonElement UserType { get; init; }

    public string GetUserTypeValue()
    {
        return UserType.ValueKind switch
        {
            JsonValueKind.String => UserType.GetString() ?? string.Empty,
            JsonValueKind.Number => UserType.GetRawText(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            _ => string.Empty
        };
    }
}
