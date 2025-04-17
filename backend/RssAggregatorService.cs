#nullable enable


using System.Globalization;
using System.ServiceModel.Syndication;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;


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

        private readonly string[] fontes = new[]
        {
            "https://g1.globo.com/rss/g1/",
            "https://agenciabrasil.ebc.com.br/rss/agenciabrasil.xml",
            "https://www.estadao.com.br/rss/ultimas.xml",
            "https://exame.com/feed/",
            "https://extra.globo.com/rss.xml",
            "https://news.google.com/rss?hl=pt-BR&gl=BR&ceid=BR:pt-419",
            "https://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml"
        };

        public async Task<List<NoticiaDTO>> BuscarNoticiasAsync(string? termo, string? dataInicio, string? dataFim)
        {
            var resultado = new List<NoticiaDTO>();
            DateTime.TryParse(dataInicio, out var inicio);
            DateTime.TryParse(dataFim, out var fim);
            bool usarFiltro = inicio != default && fim != default;

            var termosBusca = string.IsNullOrWhiteSpace(termo)
                ? new[] { "museu", "exposição", "galeria", "história", "arte", "patrimônio", "científico", "cultura" }
                : termo.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(t => t.ToLower())
                    .ToArray();

            foreach (var url in fontes)
            {
                try
                {
                    using var stream = await _http.GetStreamAsync(url);
                    using var reader = XmlReader.Create(stream);
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        var dataPub = item.PublishDate.DateTime;
                        if (usarFiltro && (dataPub < inicio || dataPub > fim)) continue;

                        var texto = (item.Title?.Text + " " + item.Summary?.Text + " " + string.Join(" ", item.Categories.Select(c => c.Name))).ToLower();
                        if (!termosBusca.Any(p => texto.Contains(p))) continue;

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
                .OrderByDescending(x => DateTime.Parse(x.DataPublicacao))
                .ToList();
        }
    }
}