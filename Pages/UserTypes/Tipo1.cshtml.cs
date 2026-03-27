using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;

namespace PainelDTI.Pages.UserTypes;

[UserTypeAuthorize(1)]
public class Tipo1Model : PageModel
{
    public void OnGet()
    {
    }
}
