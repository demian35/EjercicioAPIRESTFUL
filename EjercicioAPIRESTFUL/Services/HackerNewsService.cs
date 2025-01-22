using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
//clase HackerNewsService esta clase nos brindara el servicio de conectarnos al API de Hacker News para 
//extraer las historias para nuestra API
public class HackerNewsService
{
    private string BaseURL = "https://hacker-news.firebaseio.com/v0/";//uri del api de Hacker News
    private readonly HttpClient _httpClient;//clase para realizas solicitudes HTTP

    private readonly IMemoryCache _cache;

    //constructor del HackerNewsService
    public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    //metodo que regresara las mejores n historias que recibe como parametro la n en este caso count
    public async Task<List<Story>> GetTopStoriesAs(int count)
    {
        //obtenemos la lista de los id de las principales historias
        var topStoryIDURL = $"{BaseURL}topstories.json";
        var response = await _httpClient.GetAsync(topStoryIDURL);
        response.EnsureSuccessStatusCode();

        var storyIds = JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsByteArrayAsync());
        //tomamos los primeros n ids para regresar sus respectivas historias
        var topStoriesTasks = storyIds.Take(count).Select(GetStoryByIdAsync);
        var topStories = await Task.WhenAll(topStoriesTasks);

        //filtramos las historias para separar los valores que son nulos de las historias
        //que no son nulas y las regresamos en orden decreciente de acuerdo a su score
        return topStories
            .Where(s => s != null)
            .OrderByDescending(s => s.Score)
            .ToList();
    }

    private async Task<List<Story>> FetchTopStoriesFromApiAsync(int count)
    {
        // URL de la API de Hacker News para obtener las IDs de las historias principales
        var topStoryIdsUrl = "https://hacker-news.firebaseio.com/v0/topstories.json";

        // Realizar la solicitud HTTP GET a la API
        var response = await _httpClient.GetAsync(topStoryIdsUrl);
        response.EnsureSuccessStatusCode(); // Verifica si la solicitud fue exitosa

        // Leer la respuesta como un JSON y deserializarla en una lista de IDs de historias
        var storyIds = JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsStringAsync());

        // Obtiene las historias completas a partir de los IDs obtenidos
        var topStoriesTasks = storyIds.Take(count).Select(GetStoryByIdAsync);
        var topStories = await Task.WhenAll(topStoriesTasks);

        // Filtra historias nulas y las ordena por puntaje (si es necesario)
        return topStories.Where(s => s != null).OrderByDescending(s => s.Score).ToList();
    }

    //metodo que devuelve una historia correpondiente al id que reciba
    private async Task<Story?> GetStoryByIdAsync(int storyID)
    {
        //construimos la url para alguna historia en especifico
        var storyURL = $"{BaseURL}item/{storyID}.json";
        var response = await _httpClient.GetAsync(storyURL);
        response.EnsureSuccessStatusCode();

        var storyData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

        if (storyData.ValueKind == JsonValueKind.Null) return null;

        //mapeamos los datos JSON a un objeto Story
        return new Story
        {
            Title = storyData.GetProperty("title").GetString(),
            URI = storyData.TryGetProperty("url", out var uriProp) ? uriProp.GetString() : null,
            PostedBy = storyData.GetProperty("by").GetString(),
            Time = DateTimeOffset.FromUnixTimeSeconds(storyData.GetProperty("time").GetInt64()).UtcDateTime,
            Score = storyData.GetProperty("score").GetInt32(),
            CommentCount = storyData.GetProperty("descendants").GetInt32()
        };

    }

    //metodo para evitar sobrecargar el API de Hacker News 
    //obteniendo las mejores n historias usando un sistema cache
    //para evitar sobrecargar el API este sistema evita el hacer multiples solicitudes a esta
    public async Task<List<Story>> GetTopStoryesAsync(int count)
    {
        if (_cache.TryGetValue("TopStories", out List<Story> cachedStories))
            return cachedStories.Take(count).ToList();

        var stories = await FetchTopStoriesFromApiAsync(count);
        _cache.Set("TopStories", stories, TimeSpan.FromMinutes(5));
        return stories;
    }



}