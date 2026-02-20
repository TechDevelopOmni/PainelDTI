using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PainelDTI.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Erro { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Usuario != "admin" || Input.Senha != "123456")
        {
            Erro = "Usuário ou senha inválidos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Administrador")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("/Index");
    }

    public sealed class InputModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;
    }
}
