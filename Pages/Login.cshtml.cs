using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PainelDTI.Services;

namespace PainelDTI.Pages;

[AllowAnonymous]
public class LoginModel(IAuthApiClient authApiClient) : PageModel
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

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var loginResult = await authApiClient.LoginAsync(Input.Email, Input.Senha, cancellationToken);
        if (!loginResult.Succeeded)
        {
            Erro = loginResult.Error ?? "Usuário ou senha inválidos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, loginResult.FullName ?? Input.Email),
            new(ClaimTypes.Email, loginResult.Email ?? Input.Email)
        };

        if (!string.IsNullOrWhiteSpace(loginResult.UserType))
        {
            claims.Add(new Claim(ClaimTypes.Role, loginResult.UserType));
        }

        if (!string.IsNullOrWhiteSpace(loginResult.AccessToken))
        {
            claims.Add(new Claim("access_token", loginResult.AccessToken));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties();
        if (loginResult.ExpiresAtUtc.HasValue)
        {
            authProperties.ExpiresUtc = new DateTimeOffset(loginResult.ExpiresAtUtc.Value, TimeSpan.Zero);
            authProperties.IsPersistent = true;
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        return RedirectToPage("/Index");
    }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;
    }
}
