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
    public async Task<List<Story>> GetTopStories(int count){
        //obtenemos la lista de los id de las principales historias
        var topStoryIDURL= $"{BaseURL}topstories.json";
        var response = await _httpClient.GetAsync(topStoryIDURL);
        response.EnsureSuccessStatusCode();

        var storyIds=JsonSerializer.Deserialize<List<int>>(await response.Content.ReadAsByteArrayAsync());
        //tomamos los primeros n ids para regresar sus respectivas historias
        var topStoriesTasks=storyIds.Take(count).Select(GetStoryByIdAsync);
        var topStories= await Task.WhenAll(topStoriesTasks);

        //regresamos las historias que son nulas 
        return topStories
            .Where(s => s != null)
            .OrderByDescending(s => s.Score)
            .ToList();
    }

   
}