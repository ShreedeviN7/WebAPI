using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Microsoft.OpenApi.Models;
using WebAPITemplate.Services;
using WebAPITemplate.MiddleWare;
using WebAPITemplate.BusinessModel;
using WebAPITemplate.Helpers;
using WebAPITemplate.DataDBContext;
using WebAPITemplate.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cors
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    _ = builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

//Web DBContext Intiation with Services
builder.Services.AddDbContext<APIDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

//Swagger registration with Web API
builder.Services.AddSwaggerGen(c =>
{
    //Swagger Documentation
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "V1",
        Title = "Web-API Project Template",
        Description = "Web-API Project Template",
        Contact = new OpenApiContact
        {
            Name = "Web-API Project Template",
            Email = "support@itplusi.com"
        }
    });

    // To Enable authorization using Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",
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
                        new string[] {}
                    }
                });

    //ToDo: Documentation Path
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

//Redis Cache
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var redisOptions = configuration.GetSection("RedisOptions").Get<RedisOptions>();
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
});

//Distributed Cache
builder.Services.AddSingleton<IDistributedCache>(provider =>
{
    var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
    return new RedisCache(new RedisCacheOptions
    {
        Configuration = connectionMultiplexer.Configuration,
        //TO-DO : need to change instance name 
        InstanceName = "CoffeeWeb-RC-ORG" // Set a unique instance name for your cache
    });
});

//SeriLog Logger using MSSQL Server
string SeriLogConnectionStrings = configuration.GetValue<string>("SeriLogConnectionStrings:DbConnection");
string SeriLogInstanceName = configuration.GetValue<string>("SeriLogConnectionStrings:InstanceName");
string SeriLogTableName = configuration.GetValue<string>("SeriLogConnectionStrings:TableName");
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.MSSqlServer(
        connectionString: SeriLogConnectionStrings,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = SeriLogTableName,
            AutoCreateSqlTable = true
        })
    .CreateLogger();

// - Singleton
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<WebAPIMiddleware>();

// - Scopped
builder.Services.AddSingleton<ISalesService, SalesService>();
builder.Services.AddControllers();


var app = builder.Build();
var connectionMultiplexer = app.Services.GetRequiredService<IConnectionMultiplexer>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });

    // Enable Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coffee WebAPI V1");
        c.DefaultModelsExpandDepth(-1);
    });
}

//Enable CORS
app.UseCors("MyPolicy");

// custom middleware
app.UseMiddleware<WebAPIMiddleware>();

// Logging Trace, Debug, Information, Warning, Error, Critical, None
app.UseHttpLogging();

//JWT Auth
app.UseAuthentication();

//.Net Core
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
}); 
app.UseHttpsRedirection();
app.MapControllers();
app.Run();