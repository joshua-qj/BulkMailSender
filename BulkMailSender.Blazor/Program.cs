using BulkMailSender.Blazor.Components;
using BulkMailSender.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen;
using EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen;
using BulkMailSender.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using BulkMailSender.Infrastructure.SQLServerPersistence.Contexts;
using BulkMailSender.Blazor.Mappings;
using BulkMailSender.Infrastructure.Mappings;
using BulkMailSender.Infrastructure.SQLServerPersistence.Repositories;
using BulkMailSender.Application.Mappings;
using BulkMailSender.Blazor.Hubs;
using BulkMailSender.Blazor.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCascadingAuthenticationState();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(EmailMappingProfile));
builder.Services.AddAutoMapper(typeof(InfrastructureMappingProfile));
builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));


builder.Services.AddDbContextFactory<SqlServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmailConnection")), ServiceLifetime.Scoped);

builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmailConnection")));

// Add repository

builder.Services.AddTransient<IEmailRepository, SqlServerEmailRepository>();



builder.Services.AddTransient<IGetRequesterByNameUseCase, GetRequesterByNameUseCase>();






//builder.Services.AddScoped<AuthenticationStateProvider, YourCustomAuthenticationStateProvider>();

//infrasturcture

builder.Services.AddScoped<IExcelFileProcessingService, ExcelFileProcessingService>();


// import excel file usecase

builder.Services.AddScoped<IExcelReaderUseCase, ExcelReaderUseCase>();
builder.Services.AddTransient<IReadFileAsBytesUseCase, ReadFileAsBytesUseCase>();
builder.Services.AddTransient<IReadFileAsBytesService, ReadFileAsBytesService>();

// send email
builder.Services.AddTransient<IEmailSenderService, SmtpEmailSenderService>();
builder.Services.AddTransient<ISendEmailsUseCase, SendEmailsUseCase>();
builder.Services.AddTransient<ISaveEmailUseCase, SaveEmailUseCase>();
builder.Services.AddTransient<IUpdateEmailStatusUseCase, UpdateEmailStatusUseCase>();

builder.Services.AddSingleton<ISignalRNotificationService, SignalRNotificationService>();


builder.Services.AddSignalR(options => {
    options.MaximumReceiveMessageSize = 2 * 1024 * 1024; // 2 MB
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

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
app.MapHub<EmailStatusHub>("/emailStatus");
app.Run();












/*
 
 Add-Migration CreateIdentitySchema -Context ApplicationDbContext
Update-Database -Context ApplicationDbContext


select * from Requester
select * from Host


delete from EmailInlineResource;
delete from InlineResource;
delete from EmailAttachment;
delete from Attachment;
delete from Email


 Add-Migration CreateEmailTable -Context SqlServerDbContext
Update-Database -Context SqlServerDbContext
run at plugins project
 */