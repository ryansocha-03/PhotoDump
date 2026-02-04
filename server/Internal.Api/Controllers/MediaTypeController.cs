using Infrastructure.EntityFramework.Models;
using Infrastructure.EntityFramework.Repositories.Interfaces;
using Internal.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaTypeController(IRepository<MediaType> repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await repository.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewMediaType([FromBody] NewMediaTypeRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.MediaTypeText))
            return BadRequest("come on now.");

        var newMediaType = new MediaType
        {
            FileExtension = request.MediaTypeText
        };
        
        await repository.AddAsync(newMediaType);
        
        return Ok(newMediaType);
    }
}