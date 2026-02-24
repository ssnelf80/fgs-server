namespace FGS.Domain.FgsLobby.Exceptions;

public class InvalidInnerCallLobbyStateException : LobbyStateException
{
    public InvalidInnerCallLobbyStateException(string message) : base(message)
    {
    }
}