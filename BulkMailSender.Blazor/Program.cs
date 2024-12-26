using BulkMailSender.Blazor.Components;
using BulkMailSender.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen;
using EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen;
using BulkMailSender.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCascadingAuthenticationState();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddScoped<AuthenticationStateProvider, YourCustomAuthenticationStateProvider>();

//infrasturcture
builder.Services.AddScoped<IReadFileAsBytesService, ReadFileAsBytesService>();
builder.Services.AddScoped<IExcelFileProcessingService, ExcelFileProcessingService>();


// import excel file usecase
builder.Services.AddScoped<IReadFileAsBytesUseCase, ReadFileAsBytesUseCase>();
builder.Services.AddScoped<IExcelReaderUseCase, ExcelReaderUseCase>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
