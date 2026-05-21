
namespace MeuJogoCopa2026.Models;
public class EstatisticaGrupo
{
    public Selecao Selecao { get; set; }
    public int Pontos { get; set; }
    public int JogosJogados { get; set; }
    public int Vitorias { get; set; }
    public int Empates { get; set; }
    public int Derrotas { get; set; }
    public int GolsPro { get; set; }
    public int GolsContra { get; set; }
    
    // O Saldo de Gols pode ser calculado automaticamente
    public int SaldoGols => GolsPro - GolsContra; 
}