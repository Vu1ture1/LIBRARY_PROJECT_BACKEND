using BooksApi.DbService;
using BooksApi.ExceptionMiddleware;
using BooksApi.FileService;
using BooksApi.Helpers;
using BooksApi.Mapper;
using BooksApi.PostEntities.EntitiesValidators;
using BooksApi.Repositories;
using BooksApi.SqliteRepositoryServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

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

builder.Services.AddScoped<TokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000")  // Разрешить только с этого домена
               .AllowAnyMethod()  // Разрешить любые HTTP-методы
               .AllowAnyHeader()  // Разрешить любые заголовки
               .AllowCredentials();  // Разрешить использование учетных данных
    });
});


builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepositorySqlite>();

builder.Services.AddScoped<IUserRepository, UserRepositorySqlite>();

builder.Services.AddScoped<IBookRepository, BookRepositorySqlite>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepositorySqlite>();

builder.Services.AddTransient<IFileService, FileService>();


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

var app = builder.Build();



app.UseCors("AllowSpecificOrigin");

app.UseMiddleware<ExMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.WebRootPath, "Images")),
    RequestPath = "/Resources"
});


//app.UseCors("AllowAll");

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
