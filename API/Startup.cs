using API.Extensions;
using API.Hubs;
using API.Middleware;

namespace API;

public class Startup
{
    private readonly IConfiguration config;

    public Startup(IConfiguration config)
    {
        this.config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(config);
        services.AddGameServices(config);
        services.AddCors();
        services.AddIdentityServices(config);
        services.AddSignalR();
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = false);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<LobbiesHub>("/lobbies");
            endpoints.MapHub<LobbyHub>("/lobby");
            endpoints.MapHub<ChatHub>("/chat");
            endpoints.MapHub<GameHub>("/game");
            endpoints.MapControllers();
        });
    }
}
