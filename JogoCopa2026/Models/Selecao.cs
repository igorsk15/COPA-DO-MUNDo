namespace JogoCopa2026.Models;

public class Selecao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int ForcaGeral { get; set; }
    public int Forca => ForcaGeral;
    public int RankingFifa { get; set; }
    public string Grupo { get; set; } = string.Empty;

    // Estatísticas
    public int Pontos { get; set; }
    public int Vitorias { get; set; }
    public int Empates { get; set; }
    public int Derrotas { get; set; }
    public int GolsPro { get; set; }
    public int GolsContra { get; set; }
    public int SaldoDeGols { get; set; }

    public List<Jogador> Jogadores { get; set; } = new();
}
