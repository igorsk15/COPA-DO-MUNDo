namespace JogoCopa2026.Models;

public class EstatisticaGrupo
{
    public Selecao Selecao { get; set; } = null!;
    public int Pontos { get; set; }
    public int JogosJogados { get; set; }
    public int Vitorias { get; set; }
    public int Empates { get; set; }
    public int Derrotas { get; set; }
    public int GolsPro { get; set; }
    public int GolsContra { get; set; }
    public int SaldoGols => GolsPro - GolsContra;
}
