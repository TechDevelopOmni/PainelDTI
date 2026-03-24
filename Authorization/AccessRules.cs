using System.Security.Claims;

namespace PainelDTI.Authorization;

public static class AccessRules
{
    public const string RegraClaimType = "regra";

    public const int Administrador = 1;
    public const int Operacoes = 2;
    public const int Comercial = 3;
    public const int Financeiro = 4;
    public const int Juridico = 5;
    public const int Marketing = 6;

    private static readonly Dictionary<string, int[]> RegrasPorTela = new(StringComparer.OrdinalIgnoreCase)
    {
        ["index"] = [Administrador],
        ["financeiro"] = [Administrador, Financeiro],
        ["comercial"] = [Administrador, Comercial],
        ["operacoes"] = [Administrador, Operacoes],
        ["rh"] = [Administrador],
        ["juridico"] = [Administrador, Juridico],
        ["marketing"] = [Administrador, Marketing]
    };

    public static bool PodeAcessarTela(ClaimsPrincipal user, string telaId)
    {
        if (!RegrasPorTela.TryGetValue(telaId, out var regrasPermitidas))
        {
            return false;
        }

        var regrasUsuario = ObterRegrasUsuario(user);
        return regrasUsuario.Any(regra => regrasPermitidas.Contains(regra));
    }

    public static HashSet<int> ObterRegrasUsuario(ClaimsPrincipal user)
    {
        var regras = user.FindAll(RegraClaimType)
            .Select(claim => int.TryParse(claim.Value, out var regra) ? regra : (int?)null)
            .Where(regra => regra.HasValue)
            .Select(regra => regra!.Value)
            .ToHashSet();

        return regras;
    }
}
