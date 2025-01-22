using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class StoriesController : ControllerBase{
    private readonly HackerNewsService _hackerNewsService;

    //constructor del controlador
    public StoriesController(HackerNewsService hackerNewsService){
        _hackerNewsService= hackerNewsService;
    }

    //Metodo que recibe peticiones get para obtener las historias
    [HttpGet]
    public async Task<IActionResult> GetTopStories([FromQuery] int n=10){
        if(n<=0){
            return BadRequest("The number of stories must be better than 0");
        }
        var stories= await _hackerNewsService.GetTopStoriesAs(n);
        return Ok(stories);
    }


}