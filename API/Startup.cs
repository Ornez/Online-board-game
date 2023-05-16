using API.Data;
using API.Extensions;
using API.Hubs;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Startup
{
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment _env;
    private readonly bool useDatabase = false;

    public Startup(IConfiguration config, IWebHostEnvironment env)
    {
        this.config = config;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(config);
        services.AddGameServices(config);
        services.AddCors();
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = false);
        services.AddSignalR();
        services.AddIdentityServices(config);

        
        var connString = "";
        if (useDatabase && _env.IsDevelopment()) 
            connString = config.GetConnectionString("DefaultConnection");
        else if (useDatabase)
        {
            // Use connection string provided at runtime by FlyIO.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            // Parse connection URL to connection string for Npgsql
            connUrl = connUrl.Replace("postgres://", string.Empty);
            var pgUserPass = connUrl.Split("@")[0];
            var pgHostPortDb = connUrl.Split("@")[1];
            var pgHostPort = pgHostPortDb.Split("/")[0];
            var pgDb = pgHostPortDb.Split("/")[1];
            var pgUser = pgUserPass.Split(":")[0];
            var pgPass = pgUserPass.Split(":")[1];
            var pgHost = pgHostPort.Split(":")[0];
            var pgPort = pgHostPort.Split(":")[1];
            var updatedHost = pgHost.Replace("flycast", "internal");
    
            connString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
        }

        if (useDatabase)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(connString);
            });
        }
        else
        {
            services.AddDbContext<DataContext>();
        }
    }

    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<LobbiesHub>("/lobbies");
            endpoints.MapHub<LobbyHub>("/lobby");
            endpoints.MapHub<ChatHub>("/chat");
            endpoints.MapHub<GameHub>("/game");
            endpoints.MapControllers();
        });

        if (useDatabase)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
        }
    }
}
