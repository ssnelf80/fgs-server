using FGS.Domain.Base;

namespace FGS.Domain.FgsLobby.Exceptions;

public class LobbyGameManagerException : DomainException
{
    public LobbyGameManagerException(string message) : base(message)
    {
    }
}

