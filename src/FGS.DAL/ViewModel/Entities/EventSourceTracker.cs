namespace FGS.DAL.ViewModel.Entities;

public record EventSourceStreamTracker(int StreamTypeId, ulong CommitPosition, ulong PreparePosition)
{
    public enum StreamType{
        Lobby = 1
    }
}