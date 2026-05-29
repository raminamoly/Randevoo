using Randevoo.Application;
using Randevoo.Infrastructure;
using Randevoo.WebApi.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Server=DESKTOP-5QNHMHJ\\SQL2019;Database=Randevoo;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddRandevooInfrastructure(connectionString);
builder.Services.AddRandevooApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapDatingProfileEndpoints();

app.Run();

public partial class Program;
