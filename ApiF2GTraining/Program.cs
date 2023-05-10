using ApiF2GTraining.Data;
using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Reflection.Metadata;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

//Conexion a BB.DD
string connectionString =
    builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddTransient<IRepositoryF2GTraining, RepositoryF2GTraining>();
builder.Services.AddDbContext<F2GDataBaseContext>(options => options.UseSqlServer(connectionString));

//Seguridad
builder.Services.AddSingleton<HelperOAuthToken>();
HelperOAuthToken helper = new HelperOAuthToken(builder.Configuration);
builder.Services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
/*builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api F2G Training App",
        Description = "Api realizada para el funcionamiento de la aplicación F2G Training.",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Fernando García Garrido",
            Email = "fernando.garciagarrido@tajamar365.com"
        }
    });
});*/

builder.Services.AddOpenApiDocument(document => {

    document.Title = "Api OAuth F2G Training";
    document.Description = "Api de F2G Training";
    
    document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
    });

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

app.UseOpenApi();
/*app.UseSwagger();*/
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/bootstrap.css");
    options.InjectStylesheet("/css/material3x.css");
    options.SwaggerEndpoint(
        url: "/swagger/v1/swagger.json", name: "Api v1");
    options.RoutePrefix = "";
    options.DocExpansion(DocExpansion.None);
});

app.UseCors("corsapp");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
