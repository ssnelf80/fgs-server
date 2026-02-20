using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Exceptions;

namespace FGS.Domain.FgsLobby.Services;

public partial class LobbyGameManager
{
    private void ValidateLobbySettingsOrThrow(LobbySettings s)
    {
        if (s.PlayersCount < 2)
            throw new LobbyGameManagerException($"Минимальное кол-во игроков для создания лобби: 2. Указано: {s.PlayersCount}");
        if (s.TraitorsCount > s.PlayersCount)
            throw new LobbyGameManagerException($"Кол-во предателей меньше кол-ва игроков. Указано {s.TraitorsCount}/{s.PlayersCount} ");
        if (s.StartBalance < 0)
            throw new LobbyGameManagerException($"Стартовый баланс игроков < 0 {s.StartBalance} ");
    }
}