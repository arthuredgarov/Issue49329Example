using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Issue49329Example;

interface IMyDbContext : IDisposable, IDataProtectionKeyContext
{
    DatabaseFacade Database { get; }
}

class MyDbContext : DbContext, IMyDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        ChangeTracker.StateChanged += OnStateChanged;

        DataProtectionKeys = Set<DataProtectionKey>();
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; }

    public override void Dispose()
    {
        ChangeTracker.StateChanged -= OnStateChanged;

        base.Dispose();
    }

    private void OnStateChanged(object? sender, EntityStateChangedEventArgs e)
    {

    }
}