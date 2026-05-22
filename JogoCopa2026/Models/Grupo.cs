namespace JogoCopa2026.Models;

public class Grupo
{
    public string Nome { get; set; } = string.Empty;

    public List<EstatisticaGrupo> Classificacao { get; set; } = new();
    public List<Partida> Partidas { get; set; } = new();
    public List<Selecao> Selecoes { get; set; } = new();

    // Alias para compatibilidade
    public List<Selecao> selecao
    {
        get => Selecoes;
        set => Selecoes = value;
    }
}
