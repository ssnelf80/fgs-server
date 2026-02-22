using FGS.Domain.FgsLobby.Entities;
using FGS.Domain.FgsLobby.Events;

namespace FGS.Domain.Services;

public interface IFgsViewModelRepository : ILobbyEventVisitor<Task>
{
    Task<IReadOnlyCollection<LobbyEntity>> GetLobbyEntitiesListAsync(
        LobbyEntitySearchFilter searchFilter, CancellationToken cancellationToken);
    
    // todo возможно имеет смысл вынести из домена
    Task<(ulong CommitPosition, ulong PreparePosition)?> GetCurrentLobbyStreamPositionAsync(CancellationToken cancellationToken);
    
    Task SetCurrentLobbyStreamPositionAsync(ulong commitPosition, ulong preparePosition, CancellationToken cancellationToken);
}