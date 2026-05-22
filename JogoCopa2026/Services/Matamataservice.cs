namespace JogoCopa2026.Services;

using JogoCopa2026.Models;

public class MataMataService
{
    private readonly SimuladorPartidaService _simulador;

    // Chaves do mata-mata em ordem
    private List<Partida> _oitavas     = new();
    private List<Partida> _quartas     = new();
    private List<Partida> _semifinais  = new();
    private Partida?      _final       = null;
    private Selecao?      _campeao     = null;

    public MataMataService(SimuladorPartidaService simulador)
    {
        _simulador = simulador;
    }

    // -------------------------------------------------------------------------
    // GERAÇÃO DE FASES
    // -------------------------------------------------------------------------

    /// <summary>
    /// Monta os confrontos das oitavas de final com base nos classificados.
    /// Copa 2026: 12 grupos → 24 classificados (1º e 2º de cada grupo).
    /// Critério FIFA: 1ºA x 2ºB, 1ºB x 2ºC, etc.
    /// </summary>
    public List<Partida> GerarOitavasDeFinais(List<Selecao> primeiros, List<Selecao> segundos)
    {
        if (primeiros.Count != segundos.Count)
            throw new ArgumentException("A quantidade de primeiros e segundos colocados deve ser igual.");

        _oitavas.Clear();
        int idPartida = 1000;

        // Confronto: 1º do grupo A x 2º do grupo B, etc.
        for (int i = 0; i < primeiros.Count; i++)
        {
            int j = (i + 1) % primeiros.Count; // próximo grupo (circular)

            _oitavas.Add(CriarPartida(idPartida++, primeiros[i], segundos[j], "Oitavas de Final"));
        }

        Console.WriteLine("\n=== OITAVAS DE FINAL ===");
        ExibirConfrontos(_oitavas);

        return _oitavas;
    }

    /// <summary>
    /// Monta confrontos genéricos para qualquer fase eliminatória.
    /// </summary>
    public List<Partida> GerarFase(List<Selecao> classificados, string nomeFase)
    {
        if (classificados.Count % 2 != 0)
            throw new ArgumentException("Número de classificados deve ser par.");

        var partidas  = new List<Partida>();
        int idPartida = nomeFase switch
        {
            "Quartas de Final" => 2000,
            "Semifinais"       => 3000,
            "Final"            => 4000,
            _                  => 5000
        };

        for (int i = 0; i < classificados.Count; i += 2)
            partidas.Add(CriarPartida(idPartida++, classificados[i], classificados[i + 1], nomeFase));

        Console.WriteLine($"\n=== {nomeFase.ToUpper()} ===");
        ExibirConfrontos(partidas);

        return partidas;
    }

    // -------------------------------------------------------------------------
    // SIMULAÇÃO DE FASES
    // -------------------------------------------------------------------------

    /// <summary>
    /// Simula todas as partidas de uma fase e retorna os vencedores.
    /// </summary>
    public List<Selecao> SimularFase(List<Partida> partidas)
    {
        var vencedores = new List<Selecao>();

        foreach (var partida in partidas)
        {
            _simulador.SimularPartidaMataMata(partida);
            var vencedor = ObterVencedor(partida);
            vencedores.Add(vencedor);

            ExibirResultadoPartida(partida, vencedor);
        }

        return vencedores;
    }

    /// <summary>
    /// Simula todo o mata-mata: oitavas → quartas → semi → final.
    /// </summary>
    public Selecao SimularMataMataCompleto(List<Selecao> primeiros, List<Selecao> segundos)
    {
        // Oitavas
        Console.WriteLine("\n╔══════════════════════════════╗");
        Console.WriteLine("║     FASE DE MATA-MATA        ║");
        Console.WriteLine("╚══════════════════════════════╝");

        _oitavas = GerarOitavasDeFinais(primeiros, segundos);
        var vencedoresOitavas = SimularFase(_oitavas);

        // Quartas
        _quartas = GerarFase(vencedoresOitavas, "Quartas de Final");
        var vencedoresQuartas = SimularFase(_quartas);

        // Semifinais
        _semifinais = GerarFase(vencedoresQuartas, "Semifinais");
        var vencedoresSemi = SimularFase(_semifinais);

        // Final
        _final = GerarFase(vencedoresSemi, "Final").First();
        var vencedoresFinal = SimularFase(new List<Partida> { _final });

        _campeao = vencedoresFinal.First();

        AnunciarCampeao(_campeao);

        return _campeao;
    }

    // -------------------------------------------------------------------------
    // CONSULTA
    // -------------------------------------------------------------------------

    public Selecao? ObterCampeao() => _campeao;

    public List<Partida> ObterPartidadasOitavas()    => _oitavas;
    public List<Partida> ObterPartidasQuartas()      => _quartas;
    public List<Partida> ObterPartidasSemifinais()   => _semifinais;
    public Partida?      ObterFinal()                => _final;

    // -------------------------------------------------------------------------
    // HELPERS PRIVADOS
    // -------------------------------------------------------------------------

    private Partida CriarPartida(int id, Selecao casa, Selecao visitante, string fase)
    {
        return new Partida
        {
            Id              = id,
            TimeCasaId      = casa.Id,
            TimeCasa        = casa,
            TimeVisitanteId = visitante.Id,
            TimeVisitante   = visitante,
            Fase            = fase,
            Data            = DateTime.Now,
            Finalizada      = false
        };
    }

    private Selecao ObterVencedor(Partida partida)
    {
        if (!partida.Finalizada)
            throw new InvalidOperationException("A partida não foi finalizada.");

        // Penaltis definem o vencedor se houver empate
        if (partida.TevePenaltis)
        {
            return partida.PenaltisCasa > partida.PenaltisVisitante
                ? partida.TimeCasa
                : partida.TimeVisitante;
        }

        return partida.GolsCasa > partida.GolsVisitante
            ? partida.TimeCasa
            : partida.TimeVisitante;
    }

    private void ExibirConfrontos(List<Partida> partidas)
    {
        foreach (var p in partidas)
            Console.WriteLine($"  {p.TimeCasa.Nome,-20} x  {p.TimeVisitante.Nome}");
    }

    private void ExibirResultadoPartida(Partida partida, Selecao vencedor)
    {
        string resultado = $"{partida.GolsCasa} x {partida.GolsVisitante}";

        if (partida.TevePenaltis)
            resultado += $"  (Pen: {partida.PenaltisCasa} x {partida.PenaltisVisitante})";

        Console.WriteLine(
            $"  {partida.TimeCasa.Nome,-20} {resultado,-15} {partida.TimeVisitante.Nome,-20}" +
            $"  → Avança: {vencedor.Nome}");
    }

    private void AnunciarCampeao(Selecao campeao)
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine($"║  🏆  CAMPEÃO DA COPA 2026: {campeao.Nome,-13}║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");
    }
}