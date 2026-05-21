using JogoCopa2026.Models;

namespace JogoCopa2026.Services;

public class CopaService
{
    private readonly GrupoService _grupoService;
    private readonly SimuladorPartidaService _simuladorPartidaService;
    private readonly ClassificacaoService _classificacaoService;
    private readonly MataMataService _mataMataService;
    private readonly NarracaoService _narracaoService;

    private Selecao _selecaoDoUsuario;

    public CopaService(
        GrupoService grupoService,
        SimuladorPartidaService simuladorPartidaService,
        ClassificacaoService classificacaoService,
        MataMataService mataMataService,
        NarracaoService narracaoService)
    {
        _grupoService = grupoService;
        _simuladorPartidaService = simuladorPartidaService;
        _classificacaoService = classificacaoService;
        _mataMataService = mataMataService;
        _narracaoService = narracaoService;
    }

    // Exibe a lista de seleções e aguarda o usuário escolher a sua
    public void EscolherSelecao(List<Selecao> selecoes)
    {
        Console.WriteLine("=== 🌍 COPA DO MUNDO 2026 — MODO CARREIRA ===\n");
        Console.WriteLine("Escolha a sua seleção:\n");

        for (int i = 0; i < selecoes.Count; i++)
            Console.WriteLine($"{i + 1}. {selecoes[i].Nome} (Força: {selecoes[i].Forca} | Ranking FIFA: {selecoes[i].RankingFifa})");

        Console.Write("\nDigite o número da sua seleção: ");
        int escolha = int.Parse(Console.ReadLine()) - 1;

        _selecaoDoUsuario = selecoes[escolha];
        Console.WriteLine($"\n✅ Você escolheu: {_selecaoDoUsuario.Nome}. Boa sorte!\n");
    }

    // Ponto de entrada: orquestra todo o fluxo da copa em sequência
    public void IniciarCopa()
    {
        SimularFaseDeGrupos();
        SimularMataMata();
        ExibirResultadoFinal();
    }

    // Simula todos os grupos — narra com detalhes só as partidas da seleção do usuário
    public void SimularFaseDeGrupos()
    {
        Console.WriteLine("=== ⚽ FASE DE GRUPOS ===\n");

        var grupos = _grupoService.GerarGrupos();

        foreach (var grupo in grupos)
        {
            var partidas = _simuladorPartidaService.SimularGrupo(grupo);

            foreach (var partida in partidas)
            {
                bool ehPartidaDoUsuario = partida.SelecaoCasa.Id == _selecaoDoUsuario.Id
                                       || partida.SelecaoVisitante.Id == _selecaoDoUsuario.Id;

                if (ehPartidaDoUsuario)
                {
                    // Narração completa para as partidas do usuário
                    Console.WriteLine(_narracaoService.NarrarInicioDaPartida(partida));

                    foreach (var gol in partida.Gols)
                        Console.WriteLine(_narracaoService.NarrarGol(gol));

                    Console.WriteLine(_narracaoService.NarrarResultado(partida));
                    Console.WriteLine();
                }

                _classificacaoService.AtualizarClassificacao(grupo, partida);
            }
        }

        // Ao final dos grupos, exibe só a classificação do grupo do usuário
        var grupoDoUsuario = grupos.First(g => g.Nome == _selecaoDoUsuario.Grupo);
        Console.WriteLine($"--- Classificação do Grupo {grupoDoUsuario.Nome} ---");
        _classificacaoService.ExibirClassificacao(grupoDoUsuario);
        Console.WriteLine();
    }

    // Simula as fases eliminatórias focando na jornada da seleção do usuário
    public void SimularMataMata()
    {
        Console.WriteLine("=== 🏆 FASE ELIMINATÓRIA ===\n");

        var classificados = _classificacaoService.ObterClassificados();

        SimularFase("Dezesseis avos de final", () => _mataMataService.SimularDezesseisAvosDeFinais(classificados));
        SimularFase("Oitavas de final",        () => _mataMataService.SimularOitavasDeFinais());
        SimularFase("Quartas de final",        () => _mataMataService.SimularQuartasDeFinais());
        SimularFase("Semifinais",              () => _mataMataService.SimularSemifinais());
        SimularFase("Disputa de 3º lugar",     () => _mataMataService.SimularTerceiroLugar());
        SimularFase("Final",                   () => _mataMataService.SimularFinal());
    }

    // Roda uma fase e narra só a partida do usuário; para as demais exibe só o placar
    private void SimularFase(string nomeFase, Action simular)
    {
        Console.WriteLine($"--- {nomeFase} ---");

        var partidas = _mataMataService.ObterPartidasDaFaseAtual();
        simular();

        foreach (var partida in partidas)
        {
            bool ehPartidaDoUsuario = partida.SelecaoCasa.Id == _selecaoDoUsuario.Id
                                   || partida.SelecaoVisitante.Id == _selecaoDoUsuario.Id;

            if (ehPartidaDoUsuario)
            {
                Console.WriteLine(_narracaoService.NarrarInicioDaPartida(partida));

                foreach (var gol in partida.Gols)
                    Console.WriteLine(_narracaoService.NarrarGol(gol));

                Console.WriteLine(_narracaoService.NarrarResultado(partida));

                // Informa se a seleção do usuário foi eliminada
                if (partida.Eliminado?.Id == _selecaoDoUsuario.Id)
                {
                    Console.WriteLine($"\n😢 {_selecaoDoUsuario.Nome} foi eliminada na fase de {nomeFase}.");
                    return;
                }
            }
            else
            {
                // Outros jogos aparecem só como placar rápido
                Console.WriteLine($"  {partida.SelecaoCasa.Nome} {partida.GolsCasa} x {partida.GolsVisitante} {partida.SelecaoVisitante.Nome}");
            }
        }

        Console.WriteLine();
    }

    // Exibe o resultado final com foco na seleção do usuário
    public void ExibirResultadoFinal()
    {
        Console.WriteLine("=== 🎉 RESULTADO FINAL ===\n");

        var campeao = _mataMataService.ObterCampeao();
        var artilheiro = _classificacaoService.ObterArtilheiro();

        Console.WriteLine($"🏆 Campeão da Copa do Mundo 2026: {campeao.Nome}");
        Console.WriteLine($"⚽ Artilheiro: {artilheiro.Nome} ({artilheiro.Selecao.Nome}) — {artilheiro.Gols} gols");

        Console.WriteLine();

        if (campeao.Id == _selecaoDoUsuario.Id)
            Console.WriteLine($"🥇 PARABÉNS! Você venceu a Copa do Mundo com {_selecaoDoUsuario.Nome}!");
        else
            Console.WriteLine($"Sua seleção ({_selecaoDoUsuario.Nome}) não conquistou o título dessa vez. Tente novamente!");
    }
}
