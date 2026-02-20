using FGS.Domain.Base;

namespace FGS.Domain.FgsLobby.Exceptions;

public class LobbyException : DomainException
{
    public LobbyException(string message) : base(message)
    {
    }
}