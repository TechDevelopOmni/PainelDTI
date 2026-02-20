using PainelDTI.Models;

namespace PainelDTI.Services;

public interface IMockDashboardService
{
    DashboardData GetByArea(string area);
    IReadOnlyList<(string Id, string Nome)> GetAreas();
}
