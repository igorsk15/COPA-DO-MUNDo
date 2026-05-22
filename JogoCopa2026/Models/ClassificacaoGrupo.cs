namespace JogoCopa2026.Models;

public class ClassificacaoGrupo
{
    public int Id { get; set; }
    public int SelecaoId { get; set; }
    public Selecao Selecao { get; set; } = null!;
    public string Grupo { get; set; } = string.Empty;

    public int Pontos { get; set; }
    public int Jogos { get; set; }
    public int Vitorias { get; set; }
    public int Empates { get; set; }
    public int Derrotas { get; set; }
    public int GolsPro { get; set; }
    public int GolsContra { get; set; }
    public int SaldoDeGols { get; set; }
}
