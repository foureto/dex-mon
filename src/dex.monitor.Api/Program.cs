using dex.monitor.Business;
using Wolverine;

string[] nonWeb = ["/api", "/docs"];
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWolverine(config => config.ApplicationAssembly = typeof(BusinessInjections).Assembly);

builder.Services
    .AddControllers().Services
    .AddBusiness(builder.Configuration)
    .AddSpaStaticFiles(e => e.RootPath = "dist");

var app = builder.Build();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(_ => { });
#pragma warning restore ASP0014
app.UseSpaStaticFiles();
app.MapWhen(
    ctx => !nonWeb.All(e => ctx.Request.Path.StartsWithSegments(e)),
    next => next.UseSpa(spa =>
    {
        spa.Options.SourcePath = "webapp";
        if (builder.Environment.IsDevelopment())
        {
            spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4031");
        }
    }));

app.MapGet("/api/healthz", ctx => ctx.Response.WriteAsJsonAsync(new { msg = "ok!" }));
app.MapControllers();

app.Run();