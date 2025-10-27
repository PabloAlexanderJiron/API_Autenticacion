using Aplicacion.Helper.Comunes.Interfaces;
using Aplicacion.Infraestructura.Persistencia;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace Aplicacion.Integracion.Test.Comun
{
    [CollectionDefinition(nameof(SliceFixture))]
    public class SliceFixtureCollection : ICollectionFixture<SliceFixture>
    {
    }

    public class SliceFixture : IAsyncLifetime
    {
        private readonly IConfiguration configuration;
        private readonly WebApplicationFactory<Program> factory;

        private readonly IServiceScopeFactory scopeFactory;

        private readonly string dbconnection;

        public SliceFixture()
        {
            Environment.SetEnvironmentVariable("SECRETO_JWT","NoLeDiganANadieEsteSecretoSinoTendranMalaSuerteParaSiempreSaludos");
            factory = new AplicacionFactory();
          

            configuration = factory.Services.GetRequiredService<IConfiguration>();
            scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            dbconnection = Environment.GetEnvironmentVariable("URI_DB_TEST") ?? 
                "Server=localhost;Database=autenticacion_test;Port=5432;User Id=postgres;Password=admin";
        }

        public async Task InitializeAsync()
        {
            await using var connection = new NpgsqlConnection(dbconnection);
            await connection.OpenAsync();
            var checkpoint = await Respawner.CreateAsync(connection!, new RespawnerOptions
            {
                SchemasToInclude = new[]
                {
                    "public"
                },
                DbAdapter = DbAdapter.Postgres
            });
            await checkpoint.ResetAsync(connection);
        }

        public async Task DisposeAsync()
        {
            //await ResetCheckpoint();
            factory?.Dispose();
        }

        public async Task ResetCheckpoint()
        {
            await using var connection = new NpgsqlConnection(dbconnection);
            await connection.OpenAsync();
            var checkpoint = await Respawner.CreateAsync(connection!, new RespawnerOptions
            {
                SchemasToInclude = new[]
                {
                    "public"
                },
                DbAdapter = DbAdapter.Postgres
            });
            await checkpoint.ResetAsync(connection);
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ContextoDB>();

            //await dbContext.BeginTransactionAsync().ConfigureAwait(false);

            var result = await action(scope.ServiceProvider).ConfigureAwait(false);

            //await dbContext.CommitTransactionAsync().ConfigureAwait(false);

            return result;
        }

        public Task<T> ExecuteDbContextAsync<T>(Func<ContextoDB, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ContextoDB>()));

        public Task<T> ExecuteDbContextAsync<T>(Func<ContextoDB, IMediator, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<ContextoDB>(), sp.GetService<IMediator>()));

        public Task InsertAsync<T>(params T[] entities) where T : class => ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities) db.Set<T>().Add(entity);
            return db.SaveChangesAsync();
        });

        public Task InsertAsync<T>(List<T> entities) where T : class => ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities) db.Set<T>().Add(entity);
            return db.SaveChangesAsync();
        });

        public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class => ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });

        public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
            where TEntity : class
            where TEntity2 : class => ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);

                return db.SaveChangesAsync();
            });

        public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class => ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);

                return db.SaveChangesAsync();
            });

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2,
            TEntity3 entity3, TEntity4 entity4)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class => ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);

                return db.SaveChangesAsync();
            });

        public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>(TEntity entity, TEntity2 entity2,
            TEntity3 entity3, TEntity4 entity4, TEntity5 entity5)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class
            where TEntity5 : class => ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);
                db.Set<TEntity5>().Add(entity5);

                return db.SaveChangesAsync();
            });

        public Task<T> FindAsync<T>(int id)
            where T : class, IEntidad => ExecuteDbContextAsync(db => db.Set<T>().SingleAsync(x => x.Id == id));

        public Task<T> FindOrDefaultAsync<T>(int id)
            where T : class, IEntidad => ExecuteDbContextAsync(db => db.Set<T>().SingleOrDefaultAsync(x => x.Id == id));

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request) => ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetService<IMediator>();
            return mediator?.Send(request);
        });

        public Task<TResult> MapReverseAsync<TFrom, TResult>(TFrom request)
            where TResult : class, IEntidad, IEntidadAuditable => ExecuteScopeAsync(sp =>
            {
                var mapper = sp.GetService<IMapper>();
                return Task.FromResult(mapper?.Map<TResult>(request));
            });

        public Task<TResult> MapAsync<TFrom, TResult>(TFrom request)
            where TFrom : class, IEntidad, IEntidadAuditable => ExecuteScopeAsync(sp =>
            {
                var mapper = sp.GetService<IMapper>();
                return Task.FromResult(mapper!.Map<TResult>(request));
            });

        //public Task SendAsync(IRequest request) => ExecuteScopeAsync(sp =>
        //{
        //    var mediator = sp.GetService<IMediator>();
        //    return mediator.Send(request);
        //});

        public Task<ContextoDB> Context() => ExecuteScopeAsync(sp => Task.FromResult(sp.GetService<ContextoDB>()));
    }
}
