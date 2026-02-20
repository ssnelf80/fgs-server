using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record Player(Guid UserId, int Number, long Balance, PlayerRole Role, bool IsBot);