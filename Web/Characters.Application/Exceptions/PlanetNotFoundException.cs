using Characters.Commons.Exceptions;

namespace Characters.Application.Exceptions;

public class PlanetNotFoundException : NotFoundException
{
    public PlanetNotFoundException(string message) : base(message)
    {
    }

}