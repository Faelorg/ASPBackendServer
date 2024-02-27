using AutoMapper;
using InterfaceServer.Containers;
using InterfaceServer.Helpers;
using InterfaceServer.Modal;
using InterfaceServer.Repos;
using InterfaceServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddRateLimiter(_=>
_.AddFixedWindowLimiter(policyName:"fixedwindow", options =>
{
    options.Window = TimeSpan.FromSeconds(5);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder =
    System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode=401);

builder.Services.AddTransient<IUserService, UserClass>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();

var _key = builder.Configuration.GetValue<string>("JwtSettings:securitykey");

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item=>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ValidateIssuer = false,
        ValidateAudience =  false,
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddDbContext<FileTestContext>(o => o.UseMySQL(builder.Configuration.GetConnectionString("dbconn")!));

var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));

IMapper mapper = autoMapper.CreateMapper();

builder.Services.AddSingleton(mapper);

string logpath = builder.Configuration.GetSection("Logging:Logpath").Value;

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logpath!)
    .CreateLogger();
builder.Logging.AddSerilog(logger);

var _jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtSettings);

var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
