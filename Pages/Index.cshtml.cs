using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Services;

namespace PainelDTI.Pages;

public class IndexModel(IMockDashboardService service) : PageModel
{
    public IReadOnlyList<(string Id, string Nome)> Areas { get; private set; } = [];

    public void OnGet()
    {
        Areas = service.GetAreas();
    }
}
