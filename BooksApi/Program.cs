using BooksApi.Infrastructure.DbService;
using BooksApi.ExceptionMiddleware;
using BooksApi.Infrastructure.Services;
using BooksApi.Application.Services;
using BooksApi.Application.Mapper;
using BooksApi.Application.Validators;
using BooksApi.Domain.Repositories;
using BooksApi.Infrastructure.SqliteRepositoryServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using BooksApi.Application.Interfaces;
using BooksApi.Application.UseCases.UserUseCases;
using BooksApi.Application.UseCases.BookUseCases;
using BooksApi.Application.UseCases.AuthorUseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<UserLoginDataValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookDataValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookUpdateDataValidator>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var config = builder.Configuration;

builder.Services.AddSingleton<IConfiguration>(config);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepositorySqlite>();

builder.Services.AddScoped<IUserRepository, UserRepositorySqlite>();

builder.Services.AddScoped<IBookRepository, BookRepositorySqlite>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepositorySqlite>();

builder.Services.AddTransient<ITokenGenerationService, TokenGenerationService>();
builder.Services.AddTransient<ITokenValidationService, TokenValidationService>();

builder.Services.AddTransient<IFileService, FileService>();

builder.Services.AddTransient<IEncryptService, EncryptService>();

builder.Services.AddTransient<IFileService, FileService>();

builder.Services.AddScoped<LoginUserUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<RefreshUserTokensUseCase>();

builder.Services.AddScoped<AddBookUseCase>();
builder.Services.AddScoped<ChangeBookUseCase>();
builder.Services.AddScoped<DeleteBookUseCase>();
builder.Services.AddScoped<FindBooksByAuthorUseCase>();
builder.Services.AddScoped<FindBooksByNameUseCase>();
builder.Services.AddScoped<GetAllBooksOnHandsUseCase>();
builder.Services.AddScoped<GetBookByIdUseCase>();
builder.Services.AddScoped<GetBookByIsbnUseCase>();
builder.Services.AddScoped<GetBooksPaginatedListUseCase>();
builder.Services.AddScoped<GiveBookOnHandsUseCase>();

builder.Services.AddScoped<AddAuthorUseCase>();
builder.Services.AddScoped<ChangeAuthorUseCase>();
builder.Services.AddScoped<DeleteAuthorUseCase>();
builder.Services.AddScoped<GetAllAuthorsUseCase>();
builder.Services.AddScoped<GetOneAuthorByIdUseCase>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseMiddleware<ExMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.WebRootPath, "Images")),
    RequestPath = "/Resources"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
