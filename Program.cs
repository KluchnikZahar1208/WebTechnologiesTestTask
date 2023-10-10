using WebTechnologiesTestTask.Data;
using Microsoft.EntityFrameworkCore;
using System;
using WebTechnologiesTestTask.Services.IServices;
using WebTechnologiesTestTask.Services;
using AutoMapper;
using WebTechnologiesTestTask;
using WebTechnologiesTestTask.Model;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//add serilog
builder.Services.AddLogging(loggingBuilder =>
{
	loggingBuilder.ClearProviders();
	loggingBuilder.AddSerilog();
});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

//add dbContext
builder.Services.AddDbContext<AppDbContext>(option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//configure JwtToken
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

//add Automapper
IMapper mapper = MappingProfile.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

//add DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//configure Swagger
builder.Services.AddSwaggerGen(option =>
{
	option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference= new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id=JwtBearerDefaults.AuthenticationScheme
				}
			}, new string[]{}
		}
	});
});

//add JwtToken
var settingsSection = builder.Configuration.GetSection("ApiSettings:JwtOptions");
var secret = settingsSection.GetValue<string>("Secret");
var issuer = settingsSection.GetValue<string>("Issuer");
var audience = settingsSection.GetValue<string>("Audience");
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
	x.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = true,
		ValidIssuer = issuer,
		ValidAudience = audience,
		ValidateAudience = true
	};
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
