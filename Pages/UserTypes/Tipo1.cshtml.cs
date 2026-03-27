using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;
using PainelDTI.Services;

namespace PainelDTI.Pages.UserTypes;

[UserTypeAuthorize(1)]
public class Tipo1Model(IOperationsAnalyticsApiClient analyticsApiClient) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string ViewType { get; set; } = "daily";

    [BindProperty(SupportsGet = true)]
    public DateOnly? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Year { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ClientSearch { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Metric { get; set; } = "operations";

    public IReadOnlyList<DailyAnalyticsDto> DailyData { get; private set; } = [];
    public IReadOnlyList<MonthlyAnalyticsDto> MonthlyData { get; private set; } = [];
    public IReadOnlyList<ClientPerformanceDto> ClientsPerformance { get; private set; } = [];

    public IReadOnlyList<ClientPerformanceDto> BelowPatternAlerts => ClientsPerformance.Where(client => client.IsBelowPattern).ToList();
    public IReadOnlyList<ClientPerformanceDto> AboveLimitAlerts => ClientsPerformance.Where(client => client.IsAboveLimit).ToList();
    public IReadOnlyList<ClientPerformanceDto> OpenDebtAlerts => ClientsPerformance.Where(client => client.HasOpenDebt).ToList();

    public IReadOnlyList<ClientPerformanceDto> FilteredClients => string.IsNullOrWhiteSpace(ClientSearch)
        ? ClientsPerformance
        : ClientsPerformance.Where(client => client.ClientName.Contains(ClientSearch, StringComparison.OrdinalIgnoreCase)).ToList();

    public DashboardSummary Summary { get; private set; } = DashboardSummary.Empty;

    public IReadOnlyList<ChartBarPoint> ChartData { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        ApplyDefaults();

        var accessToken = User.FindFirst("access_token")?.Value;

        if (ViewType == "monthly")
        {
            MonthlyData = await analyticsApiClient.GetMonthlyAsync(Year!.Value, accessToken, cancellationToken);
            Summary = DashboardSummary.FromMonthly(MonthlyData);
            ChartData = BuildMonthlyChart(MonthlyData, Metric);
        }
        else if (ViewType == "clients")
        {
            ClientsPerformance = await analyticsApiClient.GetClientsPerformanceAsync(accessToken, cancellationToken);
            Summary = DashboardSummary.FromClients(ClientsPerformance);
        }
        else
        {
            DailyData = await analyticsApiClient.GetDailyAsync(StartDate!.Value, EndDate!.Value, accessToken, cancellationToken);
            Summary = DashboardSummary.FromDaily(DailyData);
            ChartData = BuildDailyChart(DailyData, Metric);
        }

        if (ViewType != "clients")
        {
            ClientsPerformance = await analyticsApiClient.GetClientsPerformanceAsync(accessToken, cancellationToken);
        }
    }

    private void ApplyDefaults()
    {
        if (string.IsNullOrWhiteSpace(ViewType) || (ViewType != "daily" && ViewType != "monthly" && ViewType != "clients"))
        {
            ViewType = "daily";
        }

        if (!StartDate.HasValue)
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7));
        }

        if (!EndDate.HasValue)
        {
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        }

        if (!Year.HasValue)
        {
            Year = DateTime.UtcNow.Year;
        }

        if (string.IsNullOrWhiteSpace(Metric) || !AllowedMetrics.Contains(Metric))
        {
            Metric = "operations";
        }
    }

    private static readonly HashSet<string> AllowedMetrics =
    [
        "operations",
        "faceValue",
        "discount",
        "effectiveRate"
    ];

    private static IReadOnlyList<ChartBarPoint> BuildDailyChart(IEnumerable<DailyAnalyticsDto> data, string metric)
    {
        return data
            .OrderBy(item => item.Date)
            .Select(item => new ChartBarPoint(item.Date.ToString("dd/MM"), ReadMetric(item.OperationsCount, item.TotalFaceValue, item.TotalDiscountValue, item.EffectiveAverageRate, metric)))
            .ToList();
    }

    private static IReadOnlyList<ChartBarPoint> BuildMonthlyChart(IEnumerable<MonthlyAnalyticsDto> data, string metric)
    {
        return data
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .Select(item => new ChartBarPoint($"{item.Month:00}/{item.Year}", ReadMetric(item.OperationsCount, item.TotalFaceValue, item.TotalDiscountValue, item.EffectiveAverageRate, metric)))
            .ToList();
    }

    private static decimal ReadMetric(int operationsCount, decimal totalFaceValue, decimal totalDiscountValue, decimal effectiveAverageRate, string metric)
    {
        return metric switch
        {
            "faceValue" => totalFaceValue,
            "discount" => totalDiscountValue,
            "effectiveRate" => effectiveAverageRate,
            _ => operationsCount
        };
    }

    public sealed record ChartBarPoint(string Label, decimal Value);

    public sealed record DashboardSummary(int TotalOperations, decimal TotalFaceValue, decimal TotalDiscount, decimal AverageRate)
    {
        public static DashboardSummary Empty => new(0, 0m, 0m, 0m);

        public static DashboardSummary FromDaily(IEnumerable<DailyAnalyticsDto> data)
        {
            var list = data.ToList();
            return new DashboardSummary(
                list.Sum(item => item.OperationsCount),
                list.Sum(item => item.TotalFaceValue),
                list.Sum(item => item.TotalDiscountValue),
                list.Count == 0 ? 0m : list.Average(item => item.EffectiveAverageRate));
        }

        public static DashboardSummary FromMonthly(IEnumerable<MonthlyAnalyticsDto> data)
        {
            var list = data.ToList();
            return new DashboardSummary(
                list.Sum(item => item.OperationsCount),
                list.Sum(item => item.TotalFaceValue),
                list.Sum(item => item.TotalDiscountValue),
                list.Count == 0 ? 0m : list.Average(item => item.EffectiveAverageRate));
        }

        public static DashboardSummary FromClients(IEnumerable<ClientPerformanceDto> data)
        {
            var list = data.ToList();
            return new DashboardSummary(
                list.Count,
                list.Sum(item => item.CurrentMonthVolume),
                0m,
                0m);
        }
    }
}
