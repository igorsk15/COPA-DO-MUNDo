namespace JogoCopa2026.Services;

using JogoCopa2026.Models;
using System.Collections.Generic;
using System.Linq;

public class EstatisticaService
{
    private readonly List<Gol> _golsDaCopa = new();
    private readonly List<Partida> _partidasDaCopa = new();

    // =========================================================================
    // REGISTRO
    // =========================================================================

    /// <summary>
    /// Registra um gol ocorrido durante a copa.
    /// </summary>
    public void RegistrarGol(Gol gol)
    {
        _golsDaCopa.Add(gol);
    }

    /// <summary>
    /// Registra uma partida finalizada para estatísticas gerais.
    /// </summary>
    public void RegistrarPartida(Partida partida)
    {
        if (partida.Finalizada)
            _partidasDaCopa.Add(partida);
    }

    // =========================================================================
    // ARTILHARIA
    // =========================================================================

    /// <summary>
    /// Retorna a lista de artilheiros ordenada por gols (do mais ao menos).
    /// </summary>
    public List<(string Jogador, int Gols)> ObterArtilheiros(int top = 10)
    {
        return _golsDaCopa
            .GroupBy(g => g.Jogador)
            .Select(grupo => (Jogador: grupo.Key, Gols: grupo.Count()))
            .OrderByDescending(x => x.Gols)
            .Take(top)
            .ToList();
    }

    /// <summary>
    /// Retorna o artilheiro da copa (jogador com mais gols).
    /// </summary>
    public (string Jogador, int Gols)? ObterArtilheiro()
    {
        if (!_golsDaCopa.Any()) return null;

        return _golsDaCopa
            .GroupBy(g => g.Jogador)
            .Select(grupo => (Jogador: grupo.Key, Gols: grupo.Count()))
            .OrderByDescending(x => x.Gols)
            .FirstOrDefault();
    }

    // =========================================================================
    // GOLS POR PARTIDA
    // =========================================================================

    /// <summary>
    /// Retorna todos os gols de uma partida específica.
    /// </summary>
    public List<Gol> ObterGolsDaPartida(int partidaId)
    {
        return _golsDaCopa
            .Where(g => g.PartidaId == partidaId)
            .OrderBy(g => g.Minuto)
            .ToList();
    }

    /// <summary>
    /// Retorna os gols de um jogador específico ao longo da copa.
    /// </summary>
    public List<Gol> ObterGolsDoJogador(string nomeJogador)
    {
        return _golsDaCopa
            .Where(g => g.Jogador.Equals(nomeJogador, StringComparison.OrdinalIgnoreCase))
            .OrderBy(g => g.Minuto)
            .ToList();
    }

    // =========================================================================
    // ESTATÍSTICAS GERAIS DA COPA
    // =========================================================================

    /// <summary>
    /// Total de gols marcados na copa inteira.
    /// </summary>
    public int TotalDeGols() => _golsDaCopa.Count;

    /// <summary>
    /// Total de partidas registradas.
    /// </summary>
    public int TotalDePartidas() => _partidasDaCopa.Count;

    /// <summary>
    /// Média de gols por partida.
    /// </summary>
    public double MediaDeGolsPorPartida()
    {
        if (_partidasDaCopa.Count == 0) return 0;
        return Math.Round((double)_golsDaCopa.Count / _partidasDaCopa.Count, 2);
    }

    /// <summary>
    /// Partida com mais gols registrada na copa.
    /// </summary>
    public Partida? PartidaMaisGols()
    {
        return _partidasDaCopa
            .OrderByDescending(p => p.GolsCasa + p.GolsVisitante)
            .FirstOrDefault();
    }

    /// <summary>
    /// Retorna quantas partidas terminaram em vitória do mandante, visitante ou empate.
    /// </summary>
    public (int VitoriasMandante, int VitoriasVisitante, int Empates) ResultadosGerais()
    {
        int vitoriasMandante  = _partidasDaCopa.Count(p => p.GolsCasa > p.GolsVisitante);
        int vitoriasVisitante = _partidasDaCopa.Count(p => p.GolsVisitante > p.GolsCasa);
        int empates           = _partidasDaCopa.Count(p => p.GolsCasa == p.GolsVisitante);

        return (vitoriasMandante, vitoriasVisitante, empates);
    }

    /// <summary>
    /// Total de partidas que foram para pênaltis.
    /// </summary>
    public int TotalDePenaltis()
    {
        return _partidasDaCopa.Count(p => p.TevePenaltis);
    }

    // =========================================================================
    // EXIBIÇÃO
    // =========================================================================

    /// <summary>
    /// Exibe o quadro completo de artilharia no console.
    /// </summary>
    public void ExibirArtilharia(int top = 10)
    {
        var artilheiros = ObterArtilheiros(top);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║             ⚽  ARTILHARIA DA COPA                  ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {"#",-4} {"Jogador",-25} {"Gols",5}");
        Console.ResetColor();
        Console.WriteLine($"  {new string('-', 36)}");

        for (int i = 0; i < artilheiros.Count; i++)
        {
            var (jogador, gols) = artilheiros[i];
            Console.ForegroundColor = i == 0 ? ConsoleColor.Yellow : ConsoleColor.Gray;
            Console.WriteLine($"  {i + 1,-4} {jogador,-25} {gols,5}");
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Exibe um resumo geral das estatísticas da copa.
    /// </summary>
    public void ExibirResumoGeral()
    {
        var (vm, vv, emp) = ResultadosGerais();
        var partidaMaisGols = PartidaMaisGols();
        var artilheiro = ObterArtilheiro();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║           📊  ESTATÍSTICAS GERAIS DA COPA           ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        ExibirLinha("Total de partidas",          TotalDePartidas().ToString());
        ExibirLinha("Total de gols",              TotalDeGols().ToString());
        ExibirLinha("Média de gols por partida",  MediaDeGolsPorPartida().ToString("F2"));
        ExibirLinha("Vitórias do mandante",       vm.ToString());
        ExibirLinha("Vitórias do visitante",      vv.ToString());
        ExibirLinha("Empates",                    emp.ToString());
        ExibirLinha("Decisões nos pênaltis",      TotalDePenaltis().ToString());

        if (artilheiro.HasValue)
            ExibirLinha("Artilheiro",             $"{artilheiro.Value.Jogador} ({artilheiro.Value.Gols} gols)");

        if (partidaMaisGols != null)
        {
            int totalGols = partidaMaisGols.GolsCasa + partidaMaisGols.GolsVisitante;
            ExibirLinha("Jogo com mais gols",
                $"{partidaMaisGols.TimeCasa.Nome} {partidaMaisGols.GolsCasa}x{partidaMaisGols.GolsVisitante} {partidaMaisGols.TimeVisitante.Nome} ({totalGols} gols)");
        }

        Console.WriteLine();
    }

    // =========================================================================
    // HELPER PRIVADO
    // =========================================================================

    private static void ExibirLinha(string label, string valor)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  {label,-35}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(valor);
        Console.ResetColor();
    }
}
