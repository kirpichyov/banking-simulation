using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microservices.Application.Contracts;
using Microservices.Application.Jobs;
using Microservices.Application.Mapping;
using Microservices.Application.Services;
using Microservices.Application.Validators;
using Microservices.Banking.Core.Options;
using Microservices.Banking.DataAccess.Connection;
using Microservices.Banking.DataAccess.Contracts;
using Microservices.Banking.DataAccess.Repositories;
using Microservices.Banking.Host.Authentication;
using Microservices.Banking.Host.Filters;
using Microservices.Banking.Host.Quartz;
using Microservices.Banking.Host.Routing;
using Microservices.Banking.Host.Swagger;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(nameof(MongoDbOptions)));
builder.Services.Configure<SimulationOptions>(builder.Configuration.GetSection(nameof(SimulationOptions)));

builder.Services.AddScoped<IMapper, Mapper>();

builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IWebhookConfigurationRepository, WebhookConfigurationRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ISimulationTaskRepository, SimulationTaskRepository>();

builder.Services.AddScoped<IWebhookConfigurationsService, WebhookConfigurationsService>();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<ISimulationTasksService, SimulationTasksService>();
builder.Services.AddScoped<IWebhookSender, WebhookSender>();

AddQuartz(builder);

builder.Services
    .AddAuthentication(options => options.DefaultScheme = ApiKeyAuthConstants.SchemePrefix)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeyAuthConstants.AuthenticationScheme, _ => { });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        options.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

ValidatorOptions.Global.LanguageManager.Enabled = false;
builder.Services.AddValidatorsFromAssemblyContaining<AddWebhookConfigurationRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Banking API"
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Description = "Obtained unique ApiKey."
    });

    options.OperationFilter<AuthOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

static void AddQuartz(WebApplicationBuilder builder)
{
    builder.Services.AddQuartz(quartz =>
    {
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
            .SetMinimumLevel(LogLevel.Information)
            .AddConsole());

        var logger = loggerFactory.CreateLogger<QuartzLogProvider>();
        
        LogProvider.SetCurrentLogProvider(new QuartzLogProvider(logger));
        quartz.SchedulerId = "Scheduler-Core";

        quartz.UseMicrosoftDependencyInjectionJobFactory();
        quartz.UseSimpleTypeLoader();
        quartz.UseInMemoryStore();
        quartz.UseDefaultThreadPool(tp =>
        {
            tp.MaxConcurrency = 5;
        });
                
        quartz.ScheduleJob<SimulationJob>(trigger =>
            {
                var cronExpression = builder.Configuration["BackgroundJobs:WebhookJobCronExpression"];

                trigger.WithIdentity(nameof(SimulationJob))
                    .StartNow()
                    .WithCronSchedule(CronScheduleBuilder.CronSchedule(cronExpression));
            }
        );
    });
            
    builder.Services.AddQuartzHostedService(options =>
    {
        options.WaitForJobsToComplete = true;
    });
}