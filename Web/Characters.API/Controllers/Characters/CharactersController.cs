using Characters.Application.Characters.AddCharacter;
using Characters.Application.Characters.GetCharacters;
using Characters.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Characters.API.Controllers.Characters;

[ApiController]
[Route("api/characters")]
public class CharactersController : ControllerBase
{
    private readonly ISender _sender;

    public CharactersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetCharactersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCharactersResponse>> GetCharacters([FromQuery]PaginationRequest request)
    {
        var result = await _sender.Send(new GetCharactersQuery(request));
        var response = result.Adapt<GetCharactersResponse>();
        HttpContext.Response.Headers.Append("from-database", "true");
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Character), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Character>> AddCharacter([FromBody] CharacterDto request)
    {
        var rq = new AddCharacterRequest(request);
        var command = rq.Adapt<AddCharacterCommand>();
        var result = await _sender.Send(command);
        var response = result.Adapt<Character>();

        return Created($"/characters/{response.Id}", response);
    }
}