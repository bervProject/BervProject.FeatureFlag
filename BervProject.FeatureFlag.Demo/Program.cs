// See https://aka.ms/new-console-template for more information
using Amazon.AppConfigData;
using BervProject.FeatureFlag;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddAWSService<IAmazonAppConfigData>();
        services.AddSingleton<FeatureFlag>();
    })
    .Build();

CheckFeatureFlag(host.Services);

await host.RunAsync();

static void CheckFeatureFlag(IServiceProvider services)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var featureFlag = provider.GetRequiredService<FeatureFlag>();
    var ffTask = featureFlag.GetFeatureFlag("try_feature_flag");
    ffTask.Wait();
    var featureFlagResult = ffTask.Result;
    Console.WriteLine($"try_feature_flag result: {featureFlagResult}");
}