using System.Collections.Generic;


namespace MeuJogoCopa2026.Models;

public class Grupo
{
    public string Nome { get; set; } // Ex: "Grupo A", "Grupo B"
    
    // A tabela de classificação do grupo
    public List<EstatisticaGrupo> Classificacao { get; set; } = new List<EstatisticaGrupo>();
    
    // Os confrontos desse grupo
    public List<Partida> Partidas { get; set; } = new List<Partida>();
}