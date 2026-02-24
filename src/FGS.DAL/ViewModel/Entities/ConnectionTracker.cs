using FGS.Domain.FgsLobby.Enums;

namespace FGS.DAL.ViewModel.Entities;

public record ConnectionTracker(Guid UserId, Guid LobbyId, PlayerRole PlayerRole);