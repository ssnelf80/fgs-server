using FGS.Domain.FgsLobby.Enums;

namespace FGS.Domain.FgsLobby.Entities;

public record Player(Guid UserId, long Balance, PlayerRole Role, bool IsBot);