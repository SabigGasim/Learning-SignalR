using ChatApp.Hubs;
using ChatApp.InMemroy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Cookie")
    .AddCookie("Cookie");

builder.Services.AddSignalR();

builder.Services.AddSingleton<ChatRegistry>();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapHub<ChatHub>("/chat");

app.Run();
