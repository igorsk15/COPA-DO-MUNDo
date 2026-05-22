namespace JogoCopa2026.Models;

public class Gol
{
    public int Id { get; set; }
    public int PartidaId { get; set; }
    public string Autor { get; set; } = string.Empty;
    public int Minuto { get; set; }
    public string Jogador { get; set; } = string.Empty;
}
