using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CustomerOnboarding.Data;
using CustomerOnboarding.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        if (hostingContext.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        }
    })
    .ConfigureServices((hostingContext, services) =>
    {
        var dbConnectionString = hostingContext.Configuration.GetConnectionString("CustomerOnboardingDbConnection");

        services.Configure<WorkerOptions>(workerOptions =>
        {
            var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;

            workerOptions.Serializer = new NewtonsoftJsonObjectSerializer(settings);
        });

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<CustomerOnboardingContext>((options) => options.UseSqlServer(dbConnectionString));

        services.AddScoped<CustomerCredentialService>();
        services.AddScoped<EmailService>();
        services.AddScoped<ManagerAssignerService>();
        services.AddScoped<SwagOrderingService>();
    }).Build();

host.Run();
