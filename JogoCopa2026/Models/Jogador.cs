namespace JogoCopa2026.Models;

public class Jogador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Overall { get; set; }
    public int Gols { get; set; }
    public int Assistencias { get; set; }
    public int ParticipacoesEmGols => Gols + Assistencias;
    public Selecao? Selecao { get; set; }

    public override string ToString() =>
        $"{Nome} (Overall: {Overall} | Gols: {Gols} | Assistências: {Assistencias})";
}
