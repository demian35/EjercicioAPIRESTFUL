using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class StoriesController : ControllerBase{
    private readonly HackerNewsService _hackerNewsService;
}