namespace FGS.Domain.FgsLobby.Context.GameSettings;

public interface IGameSettings<out TGameSettings, out TPlayerSettings>
{
    public TGameSettings GlobalGameSettings { get; }
    public TPlayerSettings DefaultPlayerSettings { get; }
}