#nullable enable
using System;
using System.Globalization;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;




namespace NoticiasRSSComFiltroTermo.Services
{
    public class NoticiaDTO
    {
        public string Titulo { get; set; } = "Sem título";
        public string Link { get; set; } = "#";
        public string Fonte { get; set; } = "Fonte desconhecida";
        public string DataPublicacao { get; set; } = "Sem data";
    }

    public class RssAggregatorService
    {
        private readonly HttpClient _http;

        public RssAggregatorService(HttpClient http)
        {
            _http = http;
        }

        private readonly string[] fontesRss = new[]
        {
            "https://g1.globo.com/rss/g1/",
            "https://agenciabrasil.ebc.com.br/rss/agenciabrasil.xml",
            "https://www.estadao.com.br/rss/ultimas.xml",
            "https://exame.com/feed/",
            "https://extra.globo.com/rss.xml",
            "https://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml",
            "https://news.google.com/rss?hl=pt-BR&gl=BR&ceid=BR:pt-419"
        };

        private string Normalizar(string texto)
        {
            return new string(texto.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray()).ToLowerInvariant();
        }

        public async Task<List<NoticiaDTO>> BuscarNoticiasAsync(
    string? termo, string? dataInicio, string? dataFim,
    string? fontesSelecionadas = "", string? idioma = "")
{
    var resultado = new List<NoticiaDTO>();

    DateTime.TryParse(dataInicio, out var inicio);
    DateTime.TryParse(dataFim, out var fim);
    bool usarFiltroData = inicio != default && fim != default;

    var termosBusca = string.IsNullOrWhiteSpace(termo)
        ? new[] { "museu", "exposição", "galeria", "história", "arte", "patrimônio", "científico", "cultura" }
        : termo.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Select(t => t.ToLowerInvariant())
               .ToArray();

    var fontesFiltro = string.IsNullOrWhiteSpace(fontesSelecionadas)
        ? Array.Empty<string>()
        : fontesSelecionadas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(f => f.ToLowerInvariant()).ToArray();

    var regex = new Regex(@"\b(" + string.Join("|", termosBusca) + @")\b", RegexOptions.IgnoreCase);

    var todasFontes = new (string url, string label)[]
    {
        ("https://g1.globo.com/rss/g1/", "g1"),
        ("https://agenciabrasil.ebc.com.br/rss/agenciabrasil.xml", "agência brasil"),
        ("https://www.estadao.com.br/rss/ultimas.xml", "estadão"),
        ("https://exame.com/feed/", "exame"),
        ("https://extra.globo.com/rss.xml", "extra"),
        ("https://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml", "bbc"),
        ("https://news.google.com/rss?hl=pt-BR&gl=BR&ceid=BR:pt-419", "google news")
    };

    // filtrar por idioma
    var fontesFiltradas = todasFontes.Where(f =>
        idioma switch
        {
            "pt" => f.label is not ("bbc" or "google news"),
            "en" => f.label is ("bbc" or "google news"),
            _ => true
        }).ToList();

    foreach (var (url, label) in fontesFiltradas)
    {
        try
        {
            using var stream = await _http.GetStreamAsync(url);
            using var reader = XmlReader.Create(stream);
            var feed = SyndicationFeed.Load(reader);

            var fonteAtual = (feed.Title?.Text ?? "Fonte desconhecida").ToLowerInvariant();
            if (fontesFiltro.Length > 0 && !fontesFiltro.Any(f => fonteAtual.Contains(f))) continue;

            foreach (var item in feed.Items)
            {
                var dataPub = item.PublishDate.DateTime;
                if (usarFiltroData && (dataPub < inicio || dataPub > fim)) continue;

                var texto = (item.Title?.Text + " " +
                             item.Summary?.Text + " " +
                             string.Join(" ", item.Categories.Select(c => c.Name)))
                             .ToLowerInvariant();

                if (!regex.IsMatch(texto)) continue;

                resultado.Add(new NoticiaDTO
                {
                    Titulo = item.Title?.Text ?? "Sem título",
                    Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? "#",
                    Fonte = feed.Title?.Text ?? "Fonte desconhecida",
                    DataPublicacao = dataPub.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
                });
            }
        }
        catch
        {
            continue;
        }
    }

    return resultado
        .OrderByDescending(n => DateTime.Parse(n.DataPublicacao))
        .ToList();
}

    }
}
