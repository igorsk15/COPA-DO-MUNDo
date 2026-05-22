namespace JogoCopa2026.Services;

using JogoCopa2026.Models;

public class SimuladorPartidaService
{
    private readonly Random _random = new Random();

    /// <summary>
    /// Simula uma partida da fase de grupos.
    /// </summary>
    public void SimularPartida(Partida partida)
    {
        int golsCasa = _random.Next(0, partida.TimeCasa.ForcaGeral / 10 + 1);
        int golsVisitante = _random.Next(0, partida.TimeVisitante.ForcaGeral / 10 + 1);

        partida.GolsCasa = golsCasa;
        partida.GolsVisitante = golsVisitante;
        partida.Finalizada = true;
    }

    /// <summary>
    /// Simula uma partida de mata-mata.
    /// Em caso de empate, vai para pênaltis e salva o resultado na partida.
    /// </summary>
    public void SimularPartidaMataMata(Partida partida)
    {
        SimularPartida(partida);

        // Empate vai para pênaltis
        if (partida.GolsCasa == partida.GolsVisitante)
        {
            partida.TevePenaltis = true;

            // Simula pênaltis levando em conta a força geral (ponderado)
            int forcaCasa = partida.TimeCasa.ForcaGeral;
            int forcaVisitante = partida.TimeVisitante.ForcaGeral;
            int total = forcaCasa + forcaVisitante;

            bool casaVenceu = _random.Next(0, total) < forcaCasa;

            if (casaVenceu)
            {
                partida.PenaltisCasa = _random.Next(4, 6);
                partida.PenaltisVisitante = _random.Next(2, partida.PenaltisCasa);
            }
            else
            {
                partida.PenaltisVisitante = _random.Next(4, 6);
                partida.PenaltisCasa = _random.Next(2, partida.PenaltisVisitante);
            }
        }

        // Define a seleção eliminada
        partida.Eliminado = ObterPerdedor(partida);
    }

    /// <summary>
    /// Retorna o perdedor de uma partida finalizada (null se ainda não finalizada).
    /// </summary>
    private Selecao ObterPerdedor(Partida partida)
    {
        if (partida.TevePenaltis)
            return partida.PenaltisCasa < partida.PenaltisVisitante
                ? partida.TimeCasa
                : partida.TimeVisitante;

        return partida.GolsCasa < partida.GolsVisitante
            ? partida.TimeCasa
            : partida.TimeVisitante;
    }
}
