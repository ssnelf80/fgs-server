using FGS.App;
using FGS.App.Models;
using FGS.Domain.Base;
using FGS.Domain.FgsLobby.Aggregate;
using FGS.Domain.FgsLobby.Enums;
using FGS.Domain.FgsLobby.Events;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FGS.Test.Integration;

public class LobbyStateTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LobbyStateTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task LobbyCreateTest_ShouldSuccess()
    { 
       // Arrange
       var ct = TestContext.Current.CancellationToken;
       using var scope = _factory.Services.CreateScope();
       var lobbyAppService = scope.ServiceProvider.GetService<LobbyAppService>()!;
       var lobbyRepository = scope.ServiceProvider.GetService<IAggregateRepository<Lobby, LobbyEvent>>()!;
       var user1 = Guid.NewGuid();
       var user2 = Guid.NewGuid();
       var masterUser = Guid.NewGuid();
       var lobbyId = await lobbyAppService.CreateLobbyAsync(new CreateLobbyRequest(masterUser, "test"), 
           ct);

       await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.DisconnectUserFromLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.DisconnectUserFromLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, user1, ct);
       // connect 2
       await lobbyAppService.ConnectUserToLobbyAsync(lobbyId, user2, ct);
       
       // connect bot
       await lobbyAppService.ConnectBotToLobbyAsync(lobbyId, ct);
       await lobbyAppService.ConnectBotToLobbyAsync(lobbyId, ct);
       var lobby = await lobbyRepository.GetAsync(lobbyId, ct);
      
       // подтверждение начала игры
       await lobbyAppService.SendUserChoiceToLobbyAsync(lobbyId, user1, ["y"], ct);
       await lobbyAppService.SendRandomUserChoiceToLobbyAsync(lobbyId, user2, ct);
       
       lobby = await lobbyRepository.GetAsync(lobbyId, ct);
       // голосование
       await lobbyAppService.SendRandomUserChoiceToLobbyAsync(lobbyId, user1, ct);
       // todo не применяется в esdb ?!
       await lobbyAppService.SendRandomUserChoiceToLobbyAsync(lobbyId, user2, ct);
       lobby = await lobbyRepository.GetAsync(lobbyId, ct);
       // подтверждение результатов
       await lobbyAppService.SendRandomUserChoiceToLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.SendRandomUserChoiceToLobbyAsync(lobbyId, user2, ct);
       lobby = await lobbyRepository.GetAsync(lobbyId, ct);
       Assert.True(lobby.Status == LobbyStatus.Closed);
    }
}