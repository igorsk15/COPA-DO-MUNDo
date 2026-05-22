namespace JogoCopa2026.Services;

using JogoCopa2026.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class ClassificacaoService
{
    private readonly List<ClassificacaoGrupo> _classificacoes = new();

    // -------------------------------------------------------------------------
    // INICIALIZAÇÃO
    // -------------------------------------------------------------------------

    public void InicializarClassificacao(List<Grupo> grupos)
    {
        _classificacoes.Clear();

        foreach (var grupo in grupos)
        {
            foreach (var selecao in grupo.Selecoes)
            {
                _classificacoes.Add(new ClassificacaoGrupo
                {
                    Grupo       = grupo.Nome,
                    SelecaoId   = selecao.Id,
                    Selecao     = selecao,
                    Pontos      = 0,
                    Jogos       = 0,
                    Vitorias    = 0,
                    Empates     = 0,
                    Derrotas    = 0,
                    GolsPro     = 0,
                    GolsContra  = 0,
                    SaldoDeGols = 0
                });
            }
        }
    }

    // -------------------------------------------------------------------------
    // ATUALIZAÇÃO
    // -------------------------------------------------------------------------

    public void AtualizarClassificacao(Partida partida)
    {
        if (!partida.Finalizada)
            throw new InvalidOperationException("A partida ainda não foi finalizada.");

        var classCasa      = ObterClassificacaoPorSelecao(partida.TimeCasaId);
        var classVisitante = ObterClassificacaoPorSelecao(partida.TimeVisitanteId);

        classCasa.Jogos++;
        classVisitante.Jogos++;

        classCasa.GolsPro         += partida.GolsCasa;
        classCasa.GolsContra      += partida.GolsVisitante;
        classVisitante.GolsPro    += partida.GolsVisitante;
        classVisitante.GolsContra += partida.GolsCasa;

        if (partida.GolsCasa > partida.GolsVisitante)
        {
            classCasa.Vitorias++;
            classCasa.Pontos += 3;
            classVisitante.Derrotas++;
        }
        else if (partida.GolsCasa < partida.GolsVisitante)
        {
            classVisitante.Vitorias++;
            classVisitante.Pontos += 3;
            classCasa.Derrotas++;
        }
        else
        {
            classCasa.Empates++;
            classVisitante.Empates++;
            classCasa.Pontos++;
            classVisitante.Pontos++;
        }

        classCasa.SaldoDeGols      = classCasa.GolsPro      - classCasa.GolsContra;
        classVisitante.SaldoDeGols = classVisitante.GolsPro - classVisitante.GolsContra;

        SincronizarEstatisticasNaSelecao(classCasa);
        SincronizarEstatisticasNaSelecao(classVisitante);
    }

    // -------------------------------------------------------------------------
    // CONSULTA
    // -------------------------------------------------------------------------

    public List<ClassificacaoGrupo> ObterClassificacaoDoGrupo(string nomeGrupo)
    {
        var classificacaoDoGrupo = _classificacoes
            .Where(c => c.Grupo.Equals(nomeGrupo, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return OrdenarGrupo(classificacaoDoGrupo);
    }

    public (List<Selecao> primeiros, List<Selecao> segundos) ObterClassificadosPorPosicao()
    {
        var primeiros = new List<Selecao>();
        var segundos  = new List<Selecao>();

        var grupos = _classificacoes
            .Select(c => c.Grupo)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        foreach (var nomeGrupo in grupos)
        {
            var ordenado = ObterClassificacaoDoGrupo(nomeGrupo);

            if (ordenado.Count >= 1) primeiros.Add(ordenado[0].Selecao);
            if (ordenado.Count >= 2) segundos.Add(ordenado[1].Selecao);
        }

        return (primeiros, segundos);
    }

    // NOVO MÉTODO: Pega os 8 melhores terceiros colocados de toda a copa
    public List<Selecao> ObterMelhoresTerceiros(int quantidade = 8)
    {
        var todosOsTerceiros = new List<ClassificacaoGrupo>();

        var grupos = _classificacoes
            .Select(c => c.Grupo)
            .Distinct()
            .ToList();

        foreach (var nomeGrupo in grupos)
        {
            var ordenado = ObterClassificacaoDoGrupo(nomeGrupo);
            
            // O índice 2 representa o 3º colocado (pois a lista começa no 0)
            if (ordenado.Count >= 3) 
            {
                todosOsTerceiros.Add(ordenado[2]);
            }
        }

        // Usa o seu próprio método OrdenarGrupo para ver quem foi melhor
        // Depois pega apenas a 'quantidade' pedida (8 times)
        return OrdenarGrupo(todosOsTerceiros)
            .Take(quantidade)
            .Select(c => c.Selecao)
            .ToList();
    }

    /// <summary>
    /// Retorna o artilheiro com mais gols registrados nas seleções.
    /// </summary>
    public Jogador ObterArtilheiro()
    {
        return _classificacoes
            .SelectMany(c => c.Selecao.Jogadores)
            .OrderByDescending(j => j.Gols)
            .FirstOrDefault();
    }

    public List<ClassificacaoGrupo> ObterClassificacaoGeral()
    {
        return _classificacoes
            .GroupBy(c => c.Grupo)
            .SelectMany(g => OrdenarGrupo(g.ToList()))
            .ToList();
    }

    // -------------------------------------------------------------------------
    // ORDENAÇÃO
    // -------------------------------------------------------------------------

    public List<ClassificacaoGrupo> OrdenarGrupo(List<ClassificacaoGrupo> grupo)
    {
        return grupo
            .OrderByDescending(c => c.Pontos)
            .ThenByDescending(c => c.SaldoDeGols)
            .ThenByDescending(c => c.GolsPro)
            .ThenBy(c => c.Selecao.Nome)
            .ToList();
    }

    // -------------------------------------------------------------------------
    // EXIBIÇÃO
    // -------------------------------------------------------------------------

    public void ExibirTabelaDoGrupo(string nomeGrupo)
    {
        var classificacao = ObterClassificacaoDoGrupo(nomeGrupo);

        Console.WriteLine($"\n{"=",50}");
        Console.WriteLine($"  GRUPO {nomeGrupo.ToUpper()}");
        Console.WriteLine($"{"=",50}");
        Console.WriteLine($"{"Seleção",-20} {"Pts",3} {"J",3} {"V",3} {"E",3} {"D",3} {"GP",4} {"GC",4} {"SG",4}");
        Console.WriteLine(new string('-', 50));

        foreach (var c in classificacao)
        {
            Console.WriteLine(
                $"{c.Selecao.Nome,-20} {c.Pontos,3} {c.Jogos,3} {c.Vitorias,3} " +
                $"{c.Empates,3} {c.Derrotas,3} {c.GolsPro,4} {c.GolsContra,4} {c.SaldoDeGols,4}");
        }

        Console.WriteLine($"{"=",50}\n");
    }

    public void ExibirTodasAsTabelas()
    {
        var grupos = _classificacoes
            .Select(c => c.Grupo)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        foreach (var grupo in grupos)
            ExibirTabelaDoGrupo(grupo);
    }

    // -------------------------------------------------------------------------
    // HELPERS PRIVADOS
    // -------------------------------------------------------------------------

    private ClassificacaoGrupo ObterClassificacaoPorSelecao(int selecaoId)
    {
        return _classificacoes.FirstOrDefault(c => c.SelecaoId == selecaoId)
               ?? throw new Exception($"Seleção com Id {selecaoId} não encontrada na classificação.");
    }

    private void SincronizarEstatisticasNaSelecao(ClassificacaoGrupo c)
    {
        c.Selecao.Pontos      = c.Pontos;
        c.Selecao.Vitorias    = c.Vitorias;
        c.Selecao.Empates     = c.Empates;
        c.Selecao.Derrotas    = c.Derrotas;
        c.Selecao.GolsPro     = c.GolsPro;
        c.Selecao.GolsContra  = c.GolsContra;
        c.Selecao.SaldoDeGols = c.SaldoDeGols;
    }
}