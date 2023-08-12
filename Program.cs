using Issue49329Example;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

var services = new ServiceCollection();

services
    .AddDbContext<IMyDbContext, MyDbContext>(options => options.UseSqlServer(
        "Data Source=localhost;Initial Catalog=issue-49329;Integrated Security=true;Trust Server Certificate=true"));

services
    .AddDataProtection()
    .PersistKeysToDbContext<MyDbContext>();

services.AddScoped<TestClass>();

var provider = services.BuildServiceProvider();

// Ensuring there is no double-dispose calls when retrieving a DbContext instance
// through the `GetRequiredService<TContextService>()` call.
var scope = provider.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<IMyDbContext>();
await context.Database.MigrateAsync();
await scope.DisposeAsync();

// When running an ASP.NET Core Web API application, somewhere under the hood of
// `WebApplication.RunAsync()` method, basically application startup - a new instance
// of `EntityFrameworkCoreXmlRepository` is being created to check the keys in the
// database. Since it is a Console application - we are just doing it here manually to
// mimic the behavior.
var repo = new EntityFrameworkCoreXmlRepository<MyDbContext>(provider, NullLoggerFactory.Instance);
var elements = repo.GetAllElements();

var instance = provider.GetRequiredService<TestClass>();