using System.Net;
using Microsoft.EntityFrameworkCore;

// Set up the web application and configure the database connection string based on the application's environment
var builder = WebApplication.CreateBuilder(args);

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}
else
{
    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}


builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");

// Add services to the container
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(connection));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//specify CORS policy
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/Student", (StudentDbContext context) =>
{
    return context.Students.ToList();
})
.WithName("GetStudents")
.WithOpenApi();

app.MapPost("/Student", (Student student, StudentDbContext context) =>
{
    context.Add(student);
    context.SaveChanges();
})
.WithName("CreateStudent")
.WithOpenApi();

app.MapPut("/Student", (Student student, StudentDbContext context) =>
{
    context.Update(student);
    context.SaveChanges();
})
.WithName("UpdateStudent");

app.MapDelete("/Student/{id}", async (HttpContext context, StudentDbContext dbContext, int id) =>
{
    Student student = dbContext.Students.Find(id) ?? null;

    if (student is null)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        await context.Response.WriteAsync("Student was not found.");
        return;
    }

    dbContext.Remove(student);
    dbContext.SaveChanges();

    context.Response.StatusCode = (int)HttpStatusCode.OK;
    await context.Response.WriteAsync("Successfully deleted student.");
}).WithName("DeleteStudent");

app.Run();
