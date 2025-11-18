using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MindMap_MicroProject.Middlewares;
using Model;
using Service;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MindMapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
// Add services to the container.
builder.Services.AddScoped<IMindMapService, MindMapService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<INodeService, NodeService>();
builder.Services.AddScoped<IMindMapReportService, MindMapReportService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MindMap_MicroProject",
        Version = "v1"
    });

    // ?O?N NÀY LÀM HI?N NÚT AUTHORIZE – PH?I CÓ ?ÚNG NH? V?Y
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nh?p token JWT theo ??nh d?ng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// Trong Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireRole("ROLE_CUSTOMER"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("ROLE_ADMIN"));

    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("ROLE_CUSTOMER", "ROLE_ADMIN"));
});
var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.


    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MindMap API v1");
    c.RoutePrefix = ""; // m? swagger tr?c ti?p t?i "/"
});



app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
