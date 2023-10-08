using Microsoft.EntityFrameworkCore;

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

// Add services to the container.
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(connection));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.Run();
