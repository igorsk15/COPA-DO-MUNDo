namespace JogoCopa2026.Services;

using JogoCopa2026.Models;

public class NarracaoService
{
    private readonly Random _random = new Random();

    // -------------------------------------------------------------------------
    // NARRAÇÃO DE INÍCIO
    // -------------------------------------------------------------------------

    public string NarrarInicioDaPartida(Partida partida)
    {
        var frases = new List<string>
        {
            $"⚽ A bola rola! {partida.TimeCasa.Nome} recebe {partida.TimeVisitante.Nome} em grande confronto!",
            $"🎙️ Começa o jogo entre {partida.TimeCasa.Nome} e {partida.TimeVisitante.Nome}!",
            $"🔥 Tudo pronto para {partida.TimeCasa.Nome} x {partida.TimeVisitante.Nome}! Que comecem os jogos!",
            $"📣 O árbitro apita e começa a partida: {partida.TimeCasa.Nome} contra {partida.TimeVisitante.Nome}!",
        };

        return frases[_random.Next(frases.Count)];
    }

    // -------------------------------------------------------------------------
    // NARRAÇÃO DE GOL
    // -------------------------------------------------------------------------

    public string NarrarGol(Gol gol)
    {
        var frases = new List<string>
        {
            $"⚽ GOOOOL! {gol.Jogador} balança as redes no minuto {gol.Minuto}!",
            $"🥅 QUE GOLAÇO! {gol.Jogador} marca aos {gol.Minuto} minutos!",
            $"📣 É GOOOL DE {gol.Jogador?.ToUpper()}! Minuto {gol.Minuto} — a torcida vai à loucura!",
            $"🎯 Que finalização! {gol.Jogador} não perdoa e faz o gol aos {gol.Minuto}'!",
            $"💥 GOOOL! Aos {gol.Minuto} minutos, {gol.Jogador} coloca a bola no fundo da rede!",
        };

        return frases[_random.Next(frases.Count)];
    }

    // -------------------------------------------------------------------------
    // NARRAÇÃO DE RESULTADO
    // -------------------------------------------------------------------------

    public string NarrarResultado(Partida partida)
    {
        string placar = $"{partida.TimeCasa.Nome} {partida.GolsCasa} x {partida.GolsVisitante} {partida.TimeVisitante.Nome}";

        if (partida.TevePenaltis)
        {
            string placaPen = $"(Pên: {partida.PenaltisCasa} x {partida.PenaltisVisitante})";
            string vencedor = partida.PenaltisCasa > partida.PenaltisVisitante
                ? partida.TimeCasa.Nome
                : partida.TimeVisitante.Nome;

            return $"🏁 Fim de jogo! {placar} {placaPen}\n" +
                   $"   🎉 {vencedor} avança nos pênaltis!";
        }

        if (partida.GolsCasa > partida.GolsVisitante)
        {
            return NarrarVitoria(partida.TimeCasa, partida.TimeVisitante, placar);
        }
        else if (partida.GolsVisitante > partida.GolsCasa)
        {
            return NarrarVitoria(partida.TimeVisitante, partida.TimeCasa, placar);
        }
        else
        {
            return NarrarEmpate(placar);
        }
    }

    // -------------------------------------------------------------------------
    // NARRAÇÃO DE PÊNALTIS
    // -------------------------------------------------------------------------

    public string NarrarPenaltis(Partida partida)
    {
        if (!partida.TevePenaltis)
            return string.Empty;

        string vencedor = partida.PenaltisCasa > partida.PenaltisVisitante
            ? partida.TimeCasa.Nome
            : partida.TimeVisitante.Nome;

        var frases = new List<string>
        {
            $"😱 A decisão vai para os pênaltis! {partida.PenaltisCasa} x {partida.PenaltisVisitante} — {vencedor} passa de fase!",
            $"🧤 Que tensão! Nos pênaltis, {vencedor} se classifica por {partida.PenaltisCasa} x {partida.PenaltisVisitante}!",
            $"💔 Pênaltis! {vencedor} vence a disputa e avança na competição!",
        };

        return frases[_random.Next(frases.Count)];
    }

    // -------------------------------------------------------------------------
    // HELPERS PRIVADOS
    // -------------------------------------------------------------------------

    private string NarrarVitoria(Selecao vencedor, Selecao perdedor, string placar)
    {
        var frases = new List<string>
        {
            $"🏁 Fim de jogo! {placar}\n   🎉 {vencedor.Nome} vence e soma mais pontos!",
            $"🏁 Apito final! {placar}\n   💪 {vencedor.Nome} dominou e leva os três pontos!",
            $"🏁 Acabou! {placar}\n   ✅ Vitória de {vencedor.Nome} sobre {perdedor.Nome}!",
        };

        return frases[_random.Next(frases.Count)];
    }

    private string NarrarEmpate(string placar)
    {
        var frases = new List<string>
        {
            $"🏁 Fim de jogo! {placar}\n   🤝 Empate! Um ponto para cada lado.",
            $"🏁 Apito final! {placar}\n   ⚖️ Ninguém conseguiu a vitória — empate justo!",
            $"🏁 Acabou! {placar}\n   😐 As seleções dividem os pontos.",
        };

        return frases[_random.Next(frases.Count)];
    }
}
