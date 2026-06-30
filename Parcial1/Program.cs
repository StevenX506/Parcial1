using Parcial1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddHttpClient<TorneoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Supabase:Url"]!.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:ApiKey"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:ApiKey"]}");
});

builder.Services.AddHttpClient<EquipoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Supabase:Url"]!.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:ApiKey"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:ApiKey"]}");
});

builder.Services.AddHttpClient<PartidoService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Supabase:Url"]!.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:ApiKey"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:ApiKey"]}");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// EL ORDEN DE ESTAS LÍNEAS ES CRÍTICO PARA LOS BOTONES Y FORMULARIOS:
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Torneo}/{action=Index}/{id?}");

app.Run();