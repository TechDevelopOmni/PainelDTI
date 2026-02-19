using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Models;
using PainelDTI.Services;

namespace PainelDTI.Pages.Dashboards;

public class AreaModel(IMockDashboardService service) : PageModel
{
    public DashboardData Dados { get; private set; } = default!;

    public void OnGet(string? id)
    {
        Dados = service.GetByArea(id ?? "financeiro");
    }
}
