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
    private string BaseURL = "https://hacker-news.firebaseio.com/v0/"; //uri del api de Hacker News
    private readonly HttpClient _httpClient; //clase para realizar solicitudes HTTP
    private readonly IMemoryCache _cache;

    //constructor del HackerNewsService
    public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    //metodo que regresara las mejores n historias que recibe como parametro la n en este caso count
    public async Task<List<Story>> GetTopStoriesAsync(int count)
    {
        //obtenemos la lista de los id de las principales historias
        var topStoryIDURL = $"{BaseURL}topstories.json";
        var response = await _httpClient.GetAsync(topStoryIDURL);
        response.EnsureSuccessStatusCode();

        var storyIds = JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsStringAsync());
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

    //metodo que devuelve una historia correspondiente al id que reciba
    private async Task<Story?> GetStoryByIdAsync(int storyID)
    {
        //construimos la url para alguna historia en especifico
        var storyURL = $"{BaseURL}item/{storyID}.json";
        var response = await _httpClient.GetAsync(storyURL);
        response.EnsureSuccessStatusCode();

        var storyData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

        if (storyData.ValueKind == JsonValueKind.Null) return null;

        //mapeamos los datos JSON a un objeto Story verificando la existencia de cada propiedad
        var story = new Story();

        if (storyData.TryGetProperty("title", out var titleProp))
            story.Title = titleProp.GetString();

        if (storyData.TryGetProperty("url", out var urlProp))
            story.URI = urlProp.GetString();

        if (storyData.TryGetProperty("by", out var byProp))
            story.PostedBy = byProp.GetString();

        if (storyData.TryGetProperty("time", out var timeProp) && timeProp.TryGetInt64(out var timeValue))
            story.Time = DateTimeOffset.FromUnixTimeSeconds(timeValue).UtcDateTime;

        if (storyData.TryGetProperty("score", out var scoreProp) && scoreProp.TryGetInt32(out var scoreValue))
            story.Score = scoreValue;

        if (storyData.TryGetProperty("descendants", out var descendantsProp) && descendantsProp.TryGetInt32(out var commentCountValue))
            story.CommentCount = commentCountValue;

        return story;
    }

    //metodo para evitar sobrecargar el API de Hacker News 
    //obteniendo las mejores n historias usando un sistema cache
    //este sistema evita hacer multiples solicitudes a la API
    public async Task<List<Story>> GetTopStoryesAsync(int count)
    {
        if (_cache.TryGetValue("TopStories", out List<Story> cachedStories))
            return cachedStories.Take(count).ToList();

        var stories = await FetchTopStoriesFromApiAsync(count);
        _cache.Set("TopStories", stories, TimeSpan.FromMinutes(5));
        return stories;
    }

    private async Task<List<Story>> FetchTopStoriesFromApiAsync(int count)
    {
        // URL de la API de Hacker News para obtener las IDs de las historias principales
        var topStoryIdsUrl = $"{BaseURL}topstories.json";

        // Realizar la solicitud HTTP GET a la API
        var response = await _httpClient.GetAsync(topStoryIdsUrl);
        response.EnsureSuccessStatusCode(); // Verifica si la solicitud fue exitosa

        // Leer la respuesta como un JSON y deserializarla en una lista de IDs de historias
        var storyIds = JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsStringAsync());

        // Obtiene las historias completas a partir de los IDs obtenidos
        var topStoriesTasks = storyIds.Take(count).Select(GetStoryByIdAsync);
        var topStories = await Task.WhenAll(topStoriesTasks);

        // Filtra historias nulas y las ordena por puntaje
        return topStories.Where(s => s != null).OrderByDescending(s => s.Score).ToList();
    }
}
