using Flagsmith;
using Microsoft.FeatureManagement;


namespace FeatureFlagProto
{
    public class FlagsmithFeatureManagement : IFeatureDefinitionProvider
    {
        private readonly IFlagsmithClient _flagsmithClient;

        public FlagsmithFeatureManagement(IFlagsmithClient flagsmithClient)
        {
            this._flagsmithClient = flagsmithClient;
        }

        public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
        {
            var flags = await _flagsmithClient.GetEnvironmentFlags();

            foreach(var flag in flags.AllFlags())
            {

                yield return CreateFeatureDefinition(flag.GetFeatureName(), await flags.IsFeatureEnabled(flag.GetFeatureName()));
            }

        }

        public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
        {
            var flags = await _flagsmithClient.GetEnvironmentFlags();

            try
            {
                return CreateFeatureDefinition(featureName, await flags.IsFeatureEnabled(featureName));
            }
            catch
            {
                //if failed, consider as false
                return CreateFeatureDefinition(featureName, false);
            }
        }

        private FeatureDefinition CreateFeatureDefinition(string featureName, bool enabled)
        {
            // NOTE: don't add any filter configuration as by default it is disabled
            var def = new FeatureDefinition
            {
                Name = featureName
            };

            if (enabled)
                def.EnabledFor = new[]
                {
                    new FeatureFilterConfiguration
                    {
                        Name = "AlwaysOn"
                    }
                };

            return def;
        }
    }
}