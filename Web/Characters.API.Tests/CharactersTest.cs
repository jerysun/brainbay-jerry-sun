using Characters.API.Controllers.Characters;
using Characters.Application.Characters.AddCharacter;
using Characters.Application.Characters.GetCharacters;
using Characters.Application.Dtos;
using Characters.Commons.Pagination;
using Characters.Domain.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Characters.API.Tests;

public class CharactersTest
{
    private readonly Mock<ISender> _mockSender;
    private readonly CharactersController _controller;

    public CharactersTest()
    {
        _mockSender = new Mock<ISender>();
        _controller = new CharactersController(_mockSender.Object);
        
        // Initialize HttpContext with mocks
        var mockHttpContext = new Mock<HttpContext>();
        var mockResponse = new Mock<HttpResponse>();
        var headers = new HeaderDictionary();
        
        mockResponse.Setup(r => r.Headers).Returns(headers);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };
    }

    #region GetCharacters Tests

    [Fact]
    public async Task GetCharacters_WithValidRequest_ReturnsOkResultWithCharacters()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(PageIndex: 0, PageSize: 10);
        var characters = new List<Character>
        {
            new Character {
                Id = 1,
                Name = "Jerry Sun",
                Status = "Alive",
                Species = "Human",
                Type = "Human with ant in eyes",
                Gender = "Male",
                Origin = new Origin { Name = "Earth (C-137)", Url = "https://rickandmortyapi.com/api/location/1" },
                Location = new Location { Name = "Citadel of Ricks", Url = "https://rickandmortyapi.com/api/location/3" },
                Image = "https://rickandmortyapi.com/api/character/avatar/1.jpeg",
                Episode = new[] { "https://rickandmortyapi.com/api/episode/1", "https://rickandmortyapi.com/api/episode/2" },
                Url = "https://rickandmortyapi.com/api/character/1",
                Created = DateTime.UtcNow
            },
            new Character {
                Id = 2,
                Name = "Peter pan",
                Status = "Alive",
                Species = "Human",
                Type = "Human with tattoo",
                Gender = "Male",
                Origin = new Origin { Name = "Earth (C-137)", Url = "https://rickandmortyapi.com/api/location/1" },
                Location = new Location { Name = "Citadel of Ricks", Url = "https://rickandmortyapi.com/api/location/3" },
                Image = "https://rickandmortyapi.com/api/character/avatar/1.jpeg",
                Episode = new[] { "https://rickandmortyapi.com/api/episode/1", "https://rickandmortyapi.com/api/episode/2" },
                Url = "https://rickandmortyapi.com/api/character/1",
                Created = DateTime.UtcNow
            }
        };
        var paginatedResult = new PaginatedResult<Character>(0, 10, 2, characters);
        var getCharactersResult = new GetCharactersResult(paginatedResult);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetCharactersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getCharactersResult);

        // Act
        var result = await _controller.GetCharacters(paginationRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        var response = okResult.Value as GetCharactersResponse;
        Assert.NotNull(response);
        Assert.Equal(2, response.Characters.Data.Count());
        _mockSender.Verify(
            x => x.Send(It.IsAny<GetCharactersQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCharacters_WithValidRequest_SetsFromDatabaseHeader()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(PageIndex: 0, PageSize: 10);
        var paginatedResult = new PaginatedResult<Character>(0, 10, 0, new List<Character>());
        var getCharactersResult = new GetCharactersResult(paginatedResult);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetCharactersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getCharactersResult);

        // Act
        await _controller.GetCharacters(paginationRequest);

        // Assert
        Assert.True(_controller.HttpContext.Response.Headers.ContainsKey("from-database"));
        Assert.Equal("true", _controller.HttpContext.Response.Headers["from-database"].ToString());
    }

    [Fact]
    public async Task GetCharacters_WithEmptyResult_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        var paginationRequest = new PaginationRequest(PageIndex: 0, PageSize: 10);
        var paginatedResult = new PaginatedResult<Character>(0, 10, 0, new List<Character>());
        var getCharactersResult = new GetCharactersResult(paginatedResult);

        _mockSender
            .Setup(x => x.Send(It.IsAny<GetCharactersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getCharactersResult);

        // Act
        var result = await _controller.GetCharacters(paginationRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = okResult.Value as GetCharactersResponse;
        Assert.NotNull(response);
        Assert.Empty(response.Characters.Data);
    }

    #endregion

    #region AddCharacter Tests

    [Fact]
    public async Task AddCharacter_WithValidRequest_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var characterDto = new CharacterDto
        {
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };
        var characterId = 1;
        var character = new Character
        {
            Id = characterId,
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };

        _mockSender
            .Setup(x => x.Send(It.IsAny<AddCharacterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var result = await _controller.AddCharacter(characterDto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal($"/characters/{characterId}", createdResult.Location);
        Assert.NotNull(createdResult.Value);
        var returnedCharacter = createdResult.Value as Character;
        Assert.NotNull(returnedCharacter);
        Assert.Equal("NewCharacter", returnedCharacter.Name);
    }

    [Fact]
    public async Task AddCharacter_WithValidRequest_CallsSenderWithMappedCommand()
    {
        // Arrange
        var characterDto = new CharacterDto
        {
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };
        var character = new Character
        {
            Id = 1,
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };

        _mockSender
            .Setup(x => x.Send(It.IsAny<AddCharacterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        await _controller.AddCharacter(characterDto);

        // Assert
        _mockSender.Verify(
            x => x.Send(It.IsAny<AddCharacterCommand>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddCharacter_WithValidRequest_ReturnsStatusCode201()
    {
        // Arrange
        var characterDto = new CharacterDto
        {
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };
        var character = new Character
        {
            Id = 1,
            Name = "NewCharacter",
            Status = "Alive",
            Species = "Human",
            Type = "Human type",
            Gender = "Male",
            Origin = new Origin { Name = "Earth", Url = "https://example.com/location/1" },
            Location = new Location { Name = "Citadel", Url = "https://example.com/location/2" },
            Image = "https://example.com/image.jpg",
            Episode = new[] { "https://example.com/episode/1" },
            Url = "https://example.com/character/1",
            Created = DateTime.UtcNow
        };

        _mockSender
            .Setup(x => x.Send(It.IsAny<AddCharacterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(character);

        // Act
        var result = await _controller.AddCharacter(characterDto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    #endregion
}