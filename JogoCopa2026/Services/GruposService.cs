namespace JogoCopa2026.Services;

using JogoCopa2026.Models;
using System.Collections.Generic;
using System.Linq;

public class GrupoService
{
    private List<Grupo> _gruposDaCopa = new List<Grupo>();

    /// <summary>
    /// Cria os grupos da copa dividindo as seleções em grupos de 4.
    /// </summary>
    public List<Grupo> CriarGrupos(List<Selecao> selecoes)
    {
        _gruposDaCopa.Clear();
        char nomeGrupo = 'A';

        for (int i = 0; i < selecoes.Count; i += 4)
        {
            var selecoesDessGrupo = selecoes.Skip(i).Take(4).ToList();

            var grupo = new Grupo
            {
                Nome = $"Grupo {nomeGrupo}",
                Selecoes = selecoesDessGrupo,
                Classificacao = selecoesDessGrupo
                    .Select(s => new EstatisticaGrupo { Selecao = s })
                    .ToList()
            };

            // Define o grupo na seleção
            foreach (var s in selecoesDessGrupo)
                s.Grupo = grupo.Nome;

            GerarPartidasDoGrupo(grupo);
            _gruposDaCopa.Add(grupo);
            nomeGrupo++;
        }

        return _gruposDaCopa;
    }

    public Grupo ObterGrupo(string nome)
    {
        return _gruposDaCopa.FirstOrDefault(g => g.Nome == nome);
    }

    /// <summary>
    /// Gera todos os confrontos dentro do grupo (todos contra todos = 6 partidas por grupo de 4).
    /// </summary>
    public void GerarPartidasDoGrupo(Grupo grupo)
    {
        var times = grupo.Selecoes;

        for (int i = 0; i < times.Count; i++)
        {
            for (int j = i + 1; j < times.Count; j++)
            {
                grupo.Partidas.Add(new Partida
                {
                    TimeCasa = times[i],
                    TimeCasaId = times[i].Id,
                    TimeVisitante = times[j],
                    TimeVisitanteId = times[j].Id,
                    Fase = "Fase de Grupos",
                    Finalizada = false
                });
            }
        }
    }

    public List<Grupo> ObterTodosOsGrupos() => _gruposDaCopa;
}
