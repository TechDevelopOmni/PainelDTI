namespace PainelDTI.Models;

public sealed class DashboardData
{
    public required string Nome { get; init; }
    public required string Descricao { get; init; }
    public required IReadOnlyList<KpiCard> Kpis { get; init; }
    public required IReadOnlyList<ChartPoint> Serie { get; init; }
}
