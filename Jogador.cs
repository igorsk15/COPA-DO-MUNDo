namespace JogoCopa2026.Models;

public class Jogador
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Overall { get; set; }
    public int Gols { get; set; }
    public Selecao Selecao { get; set; }
}
