using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PainelDTI.Pages;

public class IndexModel : PageModel
{
    public decimal MetaTrimestre { get; } = 1_200_000m;
    public IReadOnlyList<(string Dia, decimal Valor)> ProducaoPorDia { get; } =
    [
        ("01", 41_000m), ("02", 38_200m), ("03", 45_800m), ("04", 39_700m), ("05", 42_500m),
        ("06", 40_900m), ("07", 47_200m), ("08", 44_000m), ("09", 43_400m), ("10", 46_300m)
    ];

    public IReadOnlyList<(string Cedente, decimal Valor)> CedentesTop10 { get; } =
    [
        ("Cedente A", 360_000m), ("Cedente B", 250_000m), ("Cedente C", 180_000m), ("Cedente D", 140_000m),
        ("Cedente E", 95_000m), ("Cedente F", 70_000m), ("Cedente G", 55_000m), ("Cedente H", 42_000m),
        ("Cedente I", 36_000m), ("Cedente J", 30_000m)
    ];

    public decimal ProducaoMtd => ProducaoPorDia.Sum(item => item.Valor);

    public decimal AtingimentoMeta => MetaTrimestre == 0 ? 0 : Math.Round((ProducaoMtd / MetaTrimestre) * 100, 1);

    public decimal ProjecaoTrimestre
    {
        get
        {
            var mediaDiaria = MediaDiariaAtual;
            return Math.Round(mediaDiaria * 66, 2);
        }
    }

    public decimal GapMeta => Math.Max(0, MetaTrimestre - ProjecaoTrimestre);

    public decimal MediaDiariaAtual => Math.Round(ProducaoMtd / ProducaoPorDia.Count, 2);

    public decimal MediaNecessariaDiaria
    {
        get
        {
            const int diasUteisTrimestre = 66;
            var diasRestantes = Math.Max(1, diasUteisTrimestre - ProducaoPorDia.Count);
            return Math.Round(Math.Max(0, MetaTrimestre - ProducaoMtd) / diasRestantes, 2);
        }
    }

    public IReadOnlyList<(string Mes, decimal Acumulado)> ProducaoAcumuladaMensal =>
    [
        ("Mês 1", 365_000m),
        ("Mês 2", 792_000m),
        ("Mês 3 (proj.)", ProjecaoTrimestre)
    ];

    public IReadOnlyList<(string Nome, decimal Valor)> Carteira =>
    [
        ("À vencer", 890_000m),
        ("Atraso até 15 dias", 185_000m),
        ("Atraso > 15 dias", 132_000m)
    ];

    public decimal InadimplenciaPercentual
    {
        get
        {
            var total = Carteira.Sum(item => item.Valor);
            var atrasado = Carteira.Where(item => item.Nome != "À vencer").Sum(item => item.Valor);
            return total == 0 ? 0 : Math.Round((atrasado / total) * 100, 1);
        }
    }
}
