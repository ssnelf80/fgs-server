namespace FGS.Domain.FgsLobby.Exceptions;

public class InvalidOperationLobbyStateException : LobbyStateException
{
    public InvalidOperationLobbyStateException(string message) : base(message)
    {
    }
}