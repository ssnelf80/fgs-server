using FGS.App;
using FGS.App.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FGS.Test;

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
       var user1 = Guid.NewGuid();
       var user2 = Guid.NewGuid();
       var masterUser = Guid.NewGuid();
       var lobbyId = await lobbyAppService.CreateLobbyAsync(new CreateLobbyRequest(masterUser, "test"), 
           ct);

       await lobbyAppService.ConnectToLobbyAsync(lobbyId, user1, ct);
       await lobbyAppService.ConnectToLobbyAsync(lobbyId, user2, ct);
       
       Assert.True(true);
    }
}