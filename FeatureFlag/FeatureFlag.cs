using Amazon.AppConfigData;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BervProject.FeatureFlag
{
    public class FeatureFlag
    {
        private readonly IAmazonAppConfigData _amazonAppConfigDataClient;
        private string _token;
        public FeatureFlag(IAmazonAppConfigData amazonAppConfigData)
        {
            _amazonAppConfigDataClient = amazonAppConfigData;
        }
        public async Task<bool> GetFeatureFlag(string feature)
        {
            if (string.IsNullOrEmpty(_token))
            {
                var response = await _amazonAppConfigDataClient.StartConfigurationSessionAsync(new Amazon.AppConfigData.Model.StartConfigurationSessionRequest
                {
                    ApplicationIdentifier = Environment.GetEnvironmentVariable("APPCONFIG_APPLICATION_IDENTIFIER"),
                    ConfigurationProfileIdentifier = Environment.GetEnvironmentVariable("APPCONFIG_CONFIGURATION_PROFILE_IDENTIFIER"),
                    EnvironmentIdentifier = Environment.GetEnvironmentVariable("APPCONFIG_ENVIRONMENT_IDENTIFIER")
                });
                _token = response.InitialConfigurationToken;
            }
            var latestConfig = await _amazonAppConfigDataClient.GetLatestConfigurationAsync(new Amazon.AppConfigData.Model.GetLatestConfigurationRequest
            {
                ConfigurationToken = _token
            });
            if (latestConfig.ContentLength > 0)
            {
                using (var jsonDocument = JsonDocument.Parse(latestConfig.Configuration))
                {
                    if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        return jsonDocument.RootElement.GetProperty(feature).GetBoolean();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
