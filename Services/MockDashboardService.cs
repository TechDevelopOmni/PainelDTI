using PainelDTI.Models;

namespace PainelDTI.Services;

public sealed class MockDashboardService : IMockDashboardService
{
    private readonly Dictionary<string, DashboardData> _dados = new(StringComparer.OrdinalIgnoreCase)
    {
        ["financeiro"] = new DashboardData
        {
            Nome = "Financeiro",
            Descricao = "Acompanhamento de receita, despesas e margem.",
            Kpis =
            [
                new("Receita mensal", "R$ 1.240.000", "+7,8%", "success"),
                new("Despesas", "R$ 840.000", "-1,9%", "success"),
                new("Margem líquida", "32,3%", "+2,1 p.p.", "success")
            ],
            Serie =
            [
                new("Jan", 920), new("Fev", 980), new("Mar", 1020), new("Abr", 1120),
                new("Mai", 1210), new("Jun", 1240)
            ]
        },
        ["comercial"] = new DashboardData
        {
            Nome = "Comercial",
            Descricao = "Pipeline de vendas e taxa de conversão por etapa.",
            Kpis =
            [
                new("Leads qualificados", "356", "+12%", "success"),
                new("Taxa de conversão", "18,4%", "+1,2 p.p.", "success"),
                new("Ticket médio", "R$ 14.800", "-0,5%", "warning")
            ],
            Serie =
            [
                new("Prospecção", 420), new("Diagnóstico", 268), new("Proposta", 130), new("Fechamento", 66)
            ]
        },
        ["operacoes"] = new DashboardData
        {
            Nome = "Operações",
            Descricao = "Eficiência operacional e cumprimento de SLA.",
            Kpis =
            [
                new("Ordens concluídas", "1.482", "+6,2%", "success"),
                new("SLA no prazo", "94,1%", "+0,9 p.p.", "success"),
                new("Retrabalho", "3,8%", "-1,1 p.p.", "success")
            ],
            Serie =
            [
                new("Semana 1", 91), new("Semana 2", 93), new("Semana 3", 95), new("Semana 4", 94)
            ]
        },
        ["rh"] = new DashboardData
        {
            Nome = "Recursos Humanos",
            Descricao = "Indicadores de pessoas, engajamento e turnover.",
            Kpis =
            [
                new("Turnover", "2,4%", "-0,3 p.p.", "success"),
                new("eNPS", "71", "+5 pts", "success"),
                new("Horas de treinamento", "412 h", "+9,7%", "success")
            ],
            Serie =
            [
                new("Q1", 63), new("Q2", 68), new("Q3", 70), new("Q4", 71)
            ]
        }
    };

    public DashboardData GetByArea(string area)
    {
        return _dados.TryGetValue(area, out var dashboard)
            ? dashboard
            : _dados["financeiro"];
    }

    public IReadOnlyList<(string Id, string Nome)> GetAreas()
    {
        return _dados.Select(item => (item.Key, item.Value.Nome)).ToList();
    }
}
