using Microsoft.EntityFrameworkCore;
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
});

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())

    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dish API v1");
    c.RoutePrefix = ""; // m? swagger tr?c ti?p t?i "/"
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
