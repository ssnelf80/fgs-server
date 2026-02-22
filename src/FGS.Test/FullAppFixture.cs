using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FGS.Test;

public class FullAppFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}