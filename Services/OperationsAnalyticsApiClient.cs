using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PainelDTI.Services;

public interface IOperationsAnalyticsApiClient
{
    Task<IReadOnlyList<DailyAnalyticsDto>> GetDailyAsync(DateOnly startDate, DateOnly endDate, string? accessToken, CancellationToken cancellationToken);
    Task<IReadOnlyList<MonthlyAnalyticsDto>> GetMonthlyAsync(int year, string? accessToken, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClientPerformanceDto>> GetClientsPerformanceAsync(string? accessToken, CancellationToken cancellationToken);
}

public sealed class OperationsAnalyticsApiClient(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<AuthApiOptions> authOptions) : IOperationsAnalyticsApiClient
{
    private readonly string baseUrl = authOptions.Value.BaseUrl.TrimEnd('/');

    public Task<IReadOnlyList<DailyAnalyticsDto>> GetDailyAsync(DateOnly startDate, DateOnly endDate, string? accessToken, CancellationToken cancellationToken)
    {
        var endpoint = $"{baseUrl}/api/analytics/daily?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        return GetAsync<DailyAnalyticsDto>(endpoint, accessToken, cancellationToken);
    }

    public Task<IReadOnlyList<MonthlyAnalyticsDto>> GetMonthlyAsync(int year, string? accessToken, CancellationToken cancellationToken)
    {
        var endpoint = $"{baseUrl}/api/analytics/monthly?year={year}";
        return GetAsync<MonthlyAnalyticsDto>(endpoint, accessToken, cancellationToken);
    }

    public Task<IReadOnlyList<ClientPerformanceDto>> GetClientsPerformanceAsync(string? accessToken, CancellationToken cancellationToken)
    {
        var endpoint = $"{baseUrl}/api/analytics/clients-performance";
        return GetAsync<ClientPerformanceDto>(endpoint, accessToken, cancellationToken);
    }

    private async Task<IReadOnlyList<T>> GetAsync<T>(string endpoint, string? accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var data = await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken: cancellationToken);
        return data ?? [];
    }
}

public sealed class DailyAnalyticsDto
{
    public DateTime Date { get; init; }
    public int OperationsCount { get; init; }
    public decimal TotalFaceValue { get; init; }
    public decimal TotalDiscountValue { get; init; }
    public decimal EffectiveAverageRate { get; init; }
}

public sealed class MonthlyAnalyticsDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int OperationsCount { get; init; }
    public decimal TotalFaceValue { get; init; }
    public decimal TotalDiscountValue { get; init; }
    public decimal EffectiveAverageRate { get; init; }
}

public sealed class ClientPerformanceDto
{
    public string ClientName { get; init; } = string.Empty;
    public decimal CurrentMonthVolume { get; init; }
    public decimal AverageLast3Months { get; init; }
    public bool IsBelowPattern { get; init; }
    public bool IsAboveLimit { get; init; }
    public bool HasOpenDebt { get; init; }
}
