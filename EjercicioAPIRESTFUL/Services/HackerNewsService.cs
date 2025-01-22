using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
//clase HackerNewsService esta clase nos brindara el servicio de conectarnos al API de Hacker News para 
//extraer las historias para nuestra API
public class HackerNewsService{
    private string BaseURL="https://hacker-news.firebaseio.com/v0/";//uri del api de Hacker News
    private readonly HttpClient _httpClient;//clase para realizas solicitudes HTTP

    //constructor del HackerNewsService
    public HackerNewsService(HttpClient httpClient){
        _httpClient=httpClient;
    }

    //metodo que regresara las mejores n historias que recibe como parametro la n en este caso count
    public async Task<List<Story>> GetTopStoriesAs(int count){
        //obtenemos la lista de los id de las principales historias
        var topStoryIDURL= $"{BaseURL}topstories.json";
        var response = await _httpClient.GetAsync(topStoryIDURL);
        response.EnsureSuccessStatusCode();

        var storyIds=JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsByteArrayAsync());
        //tomamos los primeros n ids para regresar sus respectivas historias
        var topStoriesTasks=storyIds.Take(count).Select(GetStoryByIdAsync);
        var topStories= await Task.WhenAll(topStoriesTasks);

        //filtramos las historias para separar los valores que son nulos de las historias
        //que no son nulas y las regresamos en orden decreciente de acuerdo a su score
        return topStories
            .Where(s => s != null)
            .OrderByDescending(s => s.Score)
            .ToList();
    }

     //metodo que devuelve una historia correpondiente al id que reciba
    private async Task<Story?> GetStoryByIdAsync(int storyID){
        //construimos la url para alguna historia en especifico
        var storyURL=$"{BaseURL}item/{storyID}.json";
        var response= await _httpClient.GetAsync(storyURL);
        response.EnsureSuccessStatusCode();

        var storyData= JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());

        if(storyData.ValueKind==JsonValueKind.Null) return null;

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

   
}