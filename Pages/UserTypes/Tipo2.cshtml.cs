using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;

namespace PainelDTI.Pages.UserTypes;

[UserTypeAuthorize(2)]
public class Tipo2Model : PageModel
{
    public void OnGet()
    {
    }
}
