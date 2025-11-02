using Fuse.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSpaStaticFiles(opt => opt.RootPath = "Fuse.Web/dist");

FuseDataModule.RegisterServices(builder.Services);

var app = builder.Build();

app.UseRouting();
app.UseSpaStaticFiles();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "Web"; // your Vue app folder

    if (app.Environment.IsDevelopment())
    {
        // proxy all non-/api routes to Vite dev server
        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
    }
    else
    {
        // in prod, UseSpa serves index.html from Web/dist automatically
    }
});

app.Run();
