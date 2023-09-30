using Microsoft.EntityFrameworkCore;
using quizapp.api.Data;
using quizapp.api.Mappers;
using quizapp.api.Repositories;
using quizapp.api.Services;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

builder.Services.AddAutoMapper(x => x.AddProfile(new EntityMapper()));


builder.Services.AddDbContext<QuizDbContext>(options =>
       options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
   );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
