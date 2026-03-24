using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Authorization;

namespace PainelDTI.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private static readonly Dictionary<string, UsuarioSistema> Usuarios = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = new("123456", "Administrador", [AccessRules.Administrador]),
        ["operacoes"] = new("123456", "Usuário Operações", [AccessRules.Operacoes]),
        ["comercial"] = new("123456", "Usuário Comercial", [AccessRules.Comercial]),
        ["financeiro"] = new("123456", "Usuário Financeiro", [AccessRules.Financeiro]),
        ["juridico"] = new("123456", "Usuário Jurídico", [AccessRules.Juridico]),
        ["marketing"] = new("123456", "Usuário Marketing", [AccessRules.Marketing]),
        ["admincomercial"] = new("123456", "Administrador + Comercial", [AccessRules.Administrador, AccessRules.Comercial])
    };

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

        if (!Usuarios.TryGetValue(Input.Usuario, out var usuario) || usuario.Senha != Input.Senha)
        {
            Erro = "Usuário ou senha inválidos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuario.Nome)
        };
        claims.AddRange(usuario.Regras.Select(regra => new Claim(AccessRules.RegraClaimType, regra.ToString())));

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

    private sealed record UsuarioSistema(string Senha, string Nome, int[] Regras);
}
