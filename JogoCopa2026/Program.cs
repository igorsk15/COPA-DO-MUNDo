// ╔══════════════════════════════════════════════════════════╗
// ║           COPA DO MUNDO 2026 — JogoCopa2026              ║
// ╚══════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JogoCopa2026.Models;
using JogoCopa2026.Services;

// ── Serviços ──────────────────────────────────────────────
var simulador        = new SimuladorPartidaService();
var grupoService     = new GrupoService();
var classificacaoSvc = new ClassificacaoService();
var mataMataService  = new MataMataService(simulador);
var narracaoService  = new NarracaoService();
var estatisticaSvc   = new EstatisticaService();

// ── Dados: 48 seleções da Copa 2026 ───────────────────────
var selecoes = CriarSelecoes();

// ── Fluxo principal ───────────────────────────────────────
TelaInicial();
var selecaoUsuario = EscolherSelecao(selecoes);
LimparTela();

var grupos = grupoService.CriarGrupos(selecoes);
classificacaoSvc.InicializarClassificacao(grupos);

SimularFaseDeGrupos(grupos, selecaoUsuario);
LimparTela();

var (primeiros, segundos) = classificacaoSvc.ObterClassificadosPorPosicao();
var campeao = mataMataService.SimularMataMataCompleto(primeiros, segundos);

ExibirResultadoFinal(campeao, selecaoUsuario, classificacaoSvc);

// ══════════════════════════════════════════════════════════
//  FUNÇÕES
// ══════════════════════════════════════════════════════════

static void LimparTela()
{
    try { Console.Clear(); }
    catch { Console.WriteLine("\n\n\n\n\n"); }
}

static void TelaInicial()
{
    LimparTela();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine();
    Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
    Console.WriteLine("  ║                                                      ║");
    Console.WriteLine("  ║        🏆  COPA DO MUNDO 2026  🏆                   ║");
    Console.WriteLine("  ║        Estados Unidos  •  Canadá  •  México          ║");
    Console.WriteLine("  ║                                                      ║");
    Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  Bem-vindo ao simulador da Copa do Mundo 2026!");
    Console.ResetColor();
    Console.WriteLine();
    Thread.Sleep(1000);
}

static Selecao EscolherSelecao(List<Selecao> selecoes)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
    Console.WriteLine("  ║              ESCOLHA A SUA SELEÇÃO                  ║");
    Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();

    for (int i = 0; i < selecoes.Count; i++)
    {
        string entrada = $"  {i + 1,2}. {selecoes[i].Nome,-22} (Força: {selecoes[i].ForcaGeral,3} | FIFA: #{selecoes[i].RankingFifa})";
        Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.Gray : ConsoleColor.DarkGray;
        Console.WriteLine(entrada);
    }

    Console.ResetColor();
    Console.WriteLine();

    int escolha = -1;
    while (escolha < 1 || escolha > selecoes.Count)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  ► Digite o número da sua seleção: ");
        Console.ResetColor();

        string? input = Console.ReadLine();
        if (!int.TryParse(input, out escolha) || escolha < 1 || escolha > selecoes.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✗ Número inválido. Tente novamente.");
            Console.ResetColor();
            escolha = -1;
        }
    }

    var selecionada = selecoes[escolha - 1];
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  ✅ Você escolheu: {selecionada.Nome}! Boa sorte na Copa!");
    Console.ResetColor();
    Console.WriteLine();

    return selecionada;
}

void SimularFaseDeGrupos(List<Grupo> grupos, Selecao selecaoUsuario)
{
    LimparTela();
    Titulo("⚽  FASE DE GRUPOS");

    foreach (var grupo in grupos)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  ── {grupo.Nome} ──────────────────────────────────────");
        Console.ResetColor();

        foreach (var partida in grupo.Partidas)
        {
            simulador.SimularPartida(partida);
            classificacaoSvc.AtualizarClassificacao(partida);
            RegistrarGolsAleatorios(partida, estatisticaSvc);

            bool ehDoUsuario = partida.TimeCasaId == selecaoUsuario.Id
                            || partida.TimeVisitanteId == selecaoUsuario.Id;

            if (ehDoUsuario)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  {narracaoService.NarrarInicioDaPartida(partida)}");
                Console.ResetColor();
                Console.WriteLine($"  {narracaoService.NarrarResultado(partida)}");
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"    {partida.TimeCasa.Nome,-22} {partida.GolsCasa} x {partida.GolsVisitante}  {partida.TimeVisitante.Nome}");
                Console.ResetColor();
            }
        }

        if (grupo.Selecoes.Any(s => s.Id == selecaoUsuario.Id))
        {
            Console.WriteLine();
            classificacaoSvc.ExibirTabelaDoGrupo(grupo.Nome);
        }
    }

    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("  Deseja ver a classificação de todos os grupos? (S/N)");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("  ► ");
    Console.ResetColor();
    var resp = Console.ReadLine()?.Trim().ToUpper();
    if (resp == "S")
    {
        LimparTela();
        Titulo("📊  CLASSIFICAÇÃO GERAL — FASE DE GRUPOS");
        classificacaoSvc.ExibirTodasAsTabelas();
        Console.WriteLine("\n  Pressione ENTER para continuar...");
        Console.ReadLine();
    }
}

static void ExibirResultadoFinal(Selecao campeao, Selecao selecaoUsuario, ClassificacaoService classificacaoSvc)
{
    LimparTela();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
    Console.WriteLine("  ║      🏆  CAMPEÃO DA COPA DO MUNDO 2026  🏆          ║");
    Console.WriteLine($"  ║          {campeao.Nome,-44}║");
    Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();

    var artilheiro = classificacaoSvc.ObterArtilheiro();
    if (artilheiro != null)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  ⚽ Artilheiro: {artilheiro.Nome} ({artilheiro.Selecao?.Nome}) — {artilheiro.Gols} gols");
        Console.ResetColor();
    }

    Console.WriteLine();

    if (campeao.Id == selecaoUsuario.Id)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  🥇 PARABÉNS!! Você venceu a Copa do Mundo!");
        Console.WriteLine($"     {selecaoUsuario.Nome} é campeã do mundo! 🎉🎉🎉");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"  😔 Sua seleção ({selecaoUsuario.Nome}) não conquistou o título desta vez.");
        Console.WriteLine($"     O campeão foi {campeao.Nome}. Tente novamente!");
    }

    Console.ResetColor();
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("  Pressione qualquer tecla para sair...");
    Console.ResetColor();
    Console.ReadKey();
}

static void Titulo(string texto)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine();
    Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
    Console.WriteLine($"  ║  {texto,-52}║");
    Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();
}

static void RegistrarGolsAleatorios(Partida partida, EstatisticaService estatisticaSvc)
{
    var random = new Random();

    void AtribuirGols(Selecao selecao, int qtdGols)
    {
        if (selecao.Jogadores == null || selecao.Jogadores.Count == 0) return;

        for (int i = 0; i < qtdGols; i++)
        {
            var jogador = selecao.Jogadores
                .OrderByDescending(_ => random.Next(0, 100) + _.Overall)
                .First();

            jogador.Gols++;

            var gol = new Gol
            {
                PartidaId = partida.Id,
                Jogador   = jogador.Nome,
                Autor     = jogador.Nome,
                Minuto    = random.Next(1, 90)
            };

            partida.Gols.Add(gol);
            estatisticaSvc.RegistrarGol(gol);
        }
    }

    AtribuirGols(partida.TimeCasa, partida.GolsCasa);
    AtribuirGols(partida.TimeVisitante, partida.GolsVisitante);
}

// ── Dados das 48 seleções ─────────────────────────────────

static List<Selecao> CriarSelecoes()
{
    return new List<Selecao>
    {
        // Grupo A
        CriarSelecao(1,  "Brasil",          95, 1,  "Grupo A", new[] { ("Vinicius Jr", 92), ("Rodrygo", 87),      ("Raphinha", 86),      ("Endrick", 82) }),
        CriarSelecao(2,  "Argentina",       93, 2,  "Grupo A", new[] { ("Messi", 91),        ("Di María", 84),     ("Lautaro", 88),       ("Mac Allister", 85) }),
        CriarSelecao(3,  "Marrocos",        80, 14, "Grupo A", new[] { ("Hakimi", 85),        ("En-Nesyri", 82),    ("Ounahi", 80),        ("Amrabat", 81) }),
        CriarSelecao(4,  "Canadá",          75, 20, "Grupo A", new[] { ("Davies", 88),        ("David", 84),        ("Buchanan", 78),      ("Larin", 75) }),

        // Grupo B
        CriarSelecao(5,  "França",          92, 3,  "Grupo B", new[] { ("Mbappé", 93),        ("Griezmann", 87),    ("Camavinga", 85),     ("Tchouaméni", 84) }),
        CriarSelecao(6,  "Espanha",         91, 4,  "Grupo B", new[] { ("Pedri", 89),          ("Yamal", 88),        ("Morata", 84),        ("Rodri", 90) }),
        CriarSelecao(7,  "Alemanha",        88, 5,  "Grupo B", new[] { ("Musiala", 88),        ("Wirtz", 87),        ("Havertz", 85),       ("Kimmich", 88) }),
        CriarSelecao(8,  "Portugal",        87, 6,  "Grupo B", new[] { ("Cristiano", 88),      ("Félix", 86),        ("Bernardo", 89),      ("Leão", 87) }),

        // Grupo C
        CriarSelecao(9,  "Inglaterra",      88, 7,  "Grupo C", new[] { ("Bellingham", 91),     ("Saka", 88),         ("Kane", 90),          ("Foden", 89) }),
        CriarSelecao(10, "Holanda",         85, 8,  "Grupo C", new[] { ("Van Dijk", 88),        ("Gakpo", 85),        ("Depay", 84),         ("Reijnders", 83) }),
        CriarSelecao(11, "Bélgica",         84, 10, "Grupo C", new[] { ("De Bruyne", 91),       ("Lukaku", 85),       ("Carrasco", 82),      ("Tielemans", 83) }),
        CriarSelecao(12, "Croácia",         83, 11, "Grupo C", new[] { ("Modric", 87),           ("Gvardiol", 86),     ("Kovacic", 85),       ("Perisic", 83) }),

        // Grupo D
        CriarSelecao(13, "Itália",          84, 9,  "Grupo D", new[] { ("Barella", 87),         ("Chiesa", 85),       ("Donnarumma", 89),    ("Pellegrini", 84) }),
        CriarSelecao(14, "Uruguay",         82, 12, "Grupo D", new[] { ("Valverde", 88),         ("Núñez", 86),        ("Bentancur", 84),     ("Araújo", 85) }),
        CriarSelecao(15, "Colombia",        81, 13, "Grupo D", new[] { ("J. Díaz", 86),          ("J. Cuadrado", 82),  ("Arias", 80),         ("Córdoba", 79) }),
        CriarSelecao(16, "México",          79, 15, "Grupo D", new[] { ("Chucky Lozano", 84),    ("H. Herrera", 82),   ("Corona", 81),        ("Lainez", 78) }),

        // Grupo E
        CriarSelecao(17, "Estados Unidos",  80, 16, "Grupo E", new[] { ("Pulisic", 86),          ("McKennie", 82),     ("Reyna", 81),         ("Adams", 83) }),
        CriarSelecao(18, "Senegal",         79, 17, "Grupo E", new[] { ("Mané", 87),              ("Diatta", 80),       ("Sarr", 82),          ("Mendy", 81) }),
        CriarSelecao(19, "Suíça",           78, 18, "Grupo E", new[] { ("Shaqiri", 83),           ("Sow", 80),          ("Embolo", 81),        ("Akanji", 84) }),
        CriarSelecao(20, "Japão",           77, 19, "Grupo E", new[] { ("Mitoma", 84),             ("Kubo", 83),         ("Doan", 80),          ("Ito", 81) }),

        // Grupo F
        CriarSelecao(21, "Portugal B",      76, 21, "Grupo F", new[] { ("Trincão", 82),           ("Horta", 80),        ("Vitinha", 83),       ("Dalot", 81) }),
        CriarSelecao(22, "Coreia do Sul",   76, 22, "Grupo F", new[] { ("Son", 88),                ("Lee K.", 82),       ("Hwang", 81),         ("Kim M.", 80) }),
        CriarSelecao(23, "Austrália",       74, 23, "Grupo F", new[] { ("Hrustic", 80),            ("Leckie", 79),       ("Irvine", 78),        ("Rowles", 77) }),
        CriarSelecao(24, "Polônia",         74, 24, "Grupo F", new[] { ("Lewandowski", 90),        ("Zielinski", 83),    ("Szymanski", 80),     ("Bednarek", 79) }),

        // Grupo G
        CriarSelecao(25, "Dinamarca",       77, 25, "Grupo G", new[] { ("Eriksen", 86),            ("Hojlund", 84),      ("Christensen", 83),   ("Maehle", 80) }),
        CriarSelecao(26, "Egito",           73, 26, "Grupo G", new[] { ("Salah", 90),              ("Trezeguet", 80),    ("El-Nenny", 79),      ("Omar", 76) }),
        CriarSelecao(27, "Sérvia",          75, 27, "Grupo G", new[] { ("Vlahovic", 86),           ("Milinkovic", 86),   ("Kostic", 82),        ("Tadic", 83) }),
        CriarSelecao(28, "Nigéria",         73, 28, "Grupo G", new[] { ("Osimhen", 87),            ("Lookman", 83),      ("Aribo", 79),         ("Simon", 80) }),

        // Grupo H
        CriarSelecao(29, "Chile",           74, 29, "Grupo H", new[] { ("Vidal", 82),              ("Alexis", 84),       ("Pulgar", 80),        ("Medel", 78) }),
        CriarSelecao(30, "Turquia",         75, 30, "Grupo H", new[] { ("Çalhanoglu", 85),         ("Yazici", 82),       ("Yildiz", 83),        ("Demiral", 81) }),
        CriarSelecao(31, "Ucrânia",         74, 31, "Grupo H", new[] { ("Mudryk", 84),             ("Zinchenko", 83),    ("Dovbyk", 82),        ("Shaparenko", 80) }),
        CriarSelecao(32, "Arábia Saudita",  70, 32, "Grupo H", new[] { ("Al-Dawsari", 80),         ("Al-Shahrani", 77),  ("Al-Burayk", 76),     ("Kanno", 75) }),

        // Grupo I
        CriarSelecao(33, "Equador",         72, 33, "Grupo I", new[] { ("Valencia", 82),           ("Plata", 79),        ("Estupiñán", 81),     ("Caicedo", 83) }),
        CriarSelecao(34, "Ghana",           70, 34, "Grupo I", new[] { ("Kudus", 83),              ("Partey", 82),       ("Ayew", 79),          ("Amartey", 76) }),
        CriarSelecao(35, "Camarões",        70, 35, "Grupo I", new[] { ("Aboubakar", 82),          ("Anguissa", 84),     ("Toko Ekambi", 80),   ("Choupo", 79) }),
        CriarSelecao(36, "Costa Rica",      69, 36, "Grupo I", new[] { ("Navas", 84),              ("Campbell", 78),     ("Tejeda", 76),        ("Torres", 75) }),

        // Grupo J
        CriarSelecao(37, "Iran",            70, 37, "Grupo J", new[] { ("Taremi", 83),             ("Jahanbakhsh", 80),  ("Ghoddos", 78),       ("Rezaeian", 76) }),
        CriarSelecao(38, "Tunísia",         69, 38, "Grupo J", new[] { ("Msakni", 79),             ("Slimane", 77),      ("Ben Slimane", 76),   ("Bronn", 78) }),
        CriarSelecao(39, "Paraguai",        70, 39, "Grupo J", new[] { ("Almirón", 83),            ("Sanabria", 80),     ("Enciso", 79),        ("Villasanti", 76) }),
        CriarSelecao(40, "Venezuela",       70, 40, "Grupo J", new[] { ("Soteldo", 81),            ("Machís", 79),       ("Herrera", 77),       ("Peñaranda", 76) }),

        // Grupo K
        CriarSelecao(41, "Bolivia",         64, 41, "Grupo K", new[] { ("Marcelo Martins", 78),    ("Arce", 74),         ("Saucedo", 73),       ("Justiniano", 71) }),
        CriarSelecao(42, "Nova Zelândia",   65, 42, "Grupo K", new[] { ("Wood", 78),               ("McGlinchey", 74),   ("Fyfe", 72),          ("Thomas", 70) }),
        CriarSelecao(43, "Panamá",          66, 43, "Grupo K", new[] { ("Davis", 79),              ("Murillo", 77),      ("Fajardo", 74),       ("Godoy", 72) }),
        CriarSelecao(44, "Jamaica",         65, 44, "Grupo K", new[] { ("Bailey", 82),             ("Nicholson", 77),    ("Shaw", 75),          ("Reid", 74) }),

        // Grupo L
        CriarSelecao(45, "Costa do Marfim", 72, 45, "Grupo L", new[] { ("Haller", 82),            ("Pepe", 81),         ("Zaha", 83),          ("Kessie", 82) }),
        CriarSelecao(46, "Argélia",         71, 46, "Grupo L", new[] { ("Mahrez", 86),             ("Bennacer", 83),     ("Slimani", 78),       ("Belaili", 79) }),
        CriarSelecao(47, "Peru",            71, 47, "Grupo L", new[] { ("Cueva", 80),              ("Lapadula", 79),     ("Tapia", 78),         ("Yotún", 77) }),
        CriarSelecao(48, "Qatar",           67, 48, "Grupo L", new[] { ("Al Moez", 79),            ("Boudiaf", 76),      ("Hatem", 74),         ("Al-Haydos", 77) }),
    };
}

static Selecao CriarSelecao(int id, string nome, int forca, int ranking, string grupo,
    (string nome, int overall)[] jogadores)
{
    var selecao = new Selecao
    {
        Id          = id,
        Nome        = nome,
        ForcaGeral  = forca,
        RankingFifa = ranking,
        Grupo       = grupo,
        Jogadores   = jogadores.Select((j, i) => new Jogador
        {
            Id      = id * 100 + i,
            Nome    = j.nome,
            Overall = j.overall,
            Selecao = null
        }).ToList()
    };

    foreach (var j in selecao.Jogadores)
        j.Selecao = selecao;

    return selecao;
}
