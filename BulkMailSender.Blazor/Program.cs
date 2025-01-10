using BulkMailSender.Application.Interfaces.CommonService;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.Mappings;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.UseCases.Identity;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Blazor.Components;
using BulkMailSender.Blazor.Components.Account;
using BulkMailSender.Blazor.Hubs;
using BulkMailSender.Blazor.Mappings;
using BulkMailSender.Blazor.Services;
using BulkMailSender.Infrastructure.Common.Entities.Identity;
using BulkMailSender.Infrastructure.Mappings;
using BulkMailSender.Infrastructure.Services;
using BulkMailSender.Infrastructure.SQLServerPersistence.Contexts;
using BulkMailSender.Infrastructure.SQLServerPersistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireClaim("Permission", "Admin"));
    options.AddPolicy("CanAccessEmailSending", policy => policy.RequireClaim("Permission", "CanAccessEmailSending"));
});
//builder.Services.AddAuthentication(options => {
//    options.DefaultScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//})
//    .AddIdentityCookies();
//builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationUserDbContext>()
//    .AddSignInManager()
//    .AddDefaultTokenProviders();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(BlazorMappingProfile));
builder.Services.AddAutoMapper(typeof(InfrastructureMappingProfile));
builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));

builder.Services.AddDbContext<ApplicationUserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));


builder.Services.AddDbContextFactory<SqlServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmailConnection")), ServiceLifetime.Scoped);

builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmailConnection")));

builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireClaim("Permission", "Admin"));
    options.AddPolicy("CanAccessEmailSending", policy => policy.RequireClaim("Permission", "CanAccessEmailSending"));
});
// Add repository

builder.Services.AddTransient<IEmailRepository, SqlServerEmailRepository>();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
}).AddIdentityCookies(); 
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationUserDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders(); //these two are essetial for signin and user manager


builder.Services.AddTransient<IUserRepository, SqlServerUserRepository>();
builder.Services.AddTransient<IAuthRepository, SqlServerAuthRepository>();


builder.Services.AddTransient<IGetRequesterByNameUseCase, GetRequesterByNameUseCase>();
builder.Services.AddTransient<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddTransient<IConfirmEmailUserCase, ConfirmEmailUserCase>();
builder.Services.AddTransient<ILoginUseCase, LoginUseCase>();
builder.Services.AddTransient<ILogoutUseCase, LogoutUseCase>();
builder.Services.AddTransient<IResetPasswordUseCase, ResetPasswordUseCase>();
builder.Services.AddTransient<IFindUserByEmailUseCase, FindUserByEmailUseCase>();
builder.Services.AddTransient<IRequestPasswordResetUseCase, RequestPasswordResetUseCase>();
builder.Services.AddTransient<IResendEmailConfirmationUseCase, ResendEmailConfirmationUseCase>();
builder.Services.AddTransient<IChangePasswordUseCase, ChangePasswordUseCase>();
builder.Services.AddTransient<ICheckUserHasPasswordUseCase, CheckUserHasPasswordUseCase>();
builder.Services.AddTransient<ISetPasswordUseCase, SetPasswordUseCase>();
builder.Services.AddTransient<IGetUsersUseCase, GetUsersUseCase>();
builder.Services.AddTransient<IUpdateUserClaimsUseCase, UpdateUserClaimsUseCase>();
builder.Services.AddTransient<IToggleUserStatusUseCase, ToggleUserStatusUseCase>();
builder.Services.AddTransient<IGetUserWithClaimsUseCase, GetUserWithClaimsUseCase>();






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
builder.Services.AddSingleton<EmailProcessingService>();
builder.Services.AddSingleton<FileHelperService>();
builder.Services.AddScoped<StatusMessageViewModelService>();


builder.Services.AddSignalR(options => {
    options.MaximumReceiveMessageSize = 2 * 1024 * 1024; // 2 MB
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});
builder.Services.AddServerSideBlazor(options => {
    options.DetailedErrors = true;
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

app.MapAdditionalIdentityEndpoints();
app.Run();












/*
 
 Add-Migration CreateIdentitySchema -Context ApplicationUserDbContext
Update-Database -Context ApplicationUserDbContext


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