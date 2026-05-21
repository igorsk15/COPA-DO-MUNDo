
namespace MeuJogoCopa2026.Models;

public class Partida
{
    public int Id { get; set; }
    public Selecao Mandante { get; set; }
    public Selecao Visitante { get; set; }
    
    public int GolsMandante { get; set; }
    public int GolsVisitante { get; set; }
    
    // Controla se o jogo já foi simulado/jogado
    public bool Finalizada { get; set; } 

    // Opcional: Fase da Copa (ex: "Fase de Grupos", "Oitavas")
    public string Fase { get; set; } 
}