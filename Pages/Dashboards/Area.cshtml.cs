using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;
using PainelDTI.Models;
using PainelDTI.Services;

namespace PainelDTI.Pages.Dashboards;

public class AreaModel(IMockDashboardService service) : PageModel
{
    public DashboardData Dados { get; private set; } = default!;

    public IActionResult OnGet(string? id)
    {
        var areaId = id ?? "financeiro";
        if (!AccessRules.PodeAcessarTela(User, areaId))
        {
            return Forbid();
        }

        Dados = service.GetByArea(areaId);
        return Page();
    }
}
