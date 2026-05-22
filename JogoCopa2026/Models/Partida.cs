namespace JogoCopa2026.Models;

public class Partida
{
    public int Id { get; set; }

    public int TimeCasaId { get; set; }
    public Selecao TimeCasa { get; set; } = null!;

    public int TimeVisitanteId { get; set; }
    public Selecao TimeVisitante { get; set; } = null!;

    // Aliases
    public Selecao Mandante
    {
        get => TimeCasa;
        set { TimeCasa = value; TimeCasaId = value?.Id ?? 0; }
    }
    public Selecao Visitante
    {
        get => TimeVisitante;
        set { TimeVisitante = value; TimeVisitanteId = value?.Id ?? 0; }
    }

    public int GolsCasa { get; set; }
    public int GolsVisitante { get; set; }
    public int GolsMandante
    {
        get => GolsCasa;
        set => GolsCasa = value;
    }

    public bool TevePenaltis { get; set; }
    public int PenaltisCasa { get; set; }
    public int PenaltisVisitante { get; set; }

    public Selecao? Eliminado { get; set; }

    public List<Gol> Gols { get; set; } = new();

    public bool Finalizada { get; set; }
    public string Fase { get; set; } = string.Empty;
    public DateTime Data { get; set; } = DateTime.Now;

    public Selecao SelecaoCasa => TimeCasa;
    public Selecao SelecaoVisitante => TimeVisitante;
}
