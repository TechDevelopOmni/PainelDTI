using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;

namespace PainelDTI.Pages.UserTypes;

[UserTypeAuthorize(3)]
public class Tipo3Model : PageModel
{
    public void OnGet()
    {
    }
}
