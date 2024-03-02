using Microsoft.EntityFrameworkCore;
using AccessElectionsService.api.Data;
using AccessElectionsService.api.Mappers;
using AccessElectionsService.api.Repositories;
using AccessElectionsService.api.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISurveyResponseService, SurveyResponseService>();
builder.Services.AddScoped<IExportRepository, ExportRepository>();
builder.Services.AddTransient<IProcessingSurveyRepository, ProcessingSurveyRepository>();
builder.Services.AddTransient<IResponseResultRepository, ResponseResultRepository>();

builder.Services.AddAutoMapper(x => x.AddProfile(new EntityMapper()));

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

builder.Services.AddDbContext<AccessElectionsDbContext>(options =>
       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
   );

var app = builder.Build();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<AccessElectionsDbContext>();
    context?.Database.Migrate();
}

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
