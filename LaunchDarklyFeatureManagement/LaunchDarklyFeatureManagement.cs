using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using System.Runtime.CompilerServices;

namespace LoopUp.FeatureManagement.LaunchDarkly
{
    public class LaunchDarklyFeatureManagement : IFeatureDefinitionProvider
    {
        private readonly ILdClient _client;
        private readonly ITargetingContextAccessor? _targettingContextAccessor;
        
        private Context? _defaultContext;

        public LaunchDarklyFeatureManagement(ILdClient client, ITargetingContextAccessor contextAccessor)
        {
            this._client = client;
            this._targettingContextAccessor = contextAccessor;
        }

        public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
        {
            var flags = this._client.AllFlagsState(await GetContextAsync());
            var flagList = flags.ToValuesJsonMap().ToList();

            foreach(var flag in flagList)
            {
                yield return CreateFeatureDefinition(flag.Key, flag.Value.AsBool);
            }
        }

        public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
        {
            return await Task.Run(async () => CreateFeatureDefinition(featureName, this._client.BoolVariation(featureName, await GetContextAsync(), false)));
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

        private async ValueTask<Context> GetContextAsync()
        {
            Context? context = null;
            TargetingContext? targetingContext = this._targettingContextAccessor is not null ? await _targettingContextAccessor.GetContextAsync() : null;

            if (targetingContext == null)
            {
                this._defaultContext ??= Context.Builder("sample-context").Build();
                context = this._defaultContext;
            }
            else if (targetingContext is LaunchDarklyTargetingContext)
            {
                var builder = Context.Builder(targetingContext.UserId);
                foreach (var key in ((LaunchDarklyTargetingContext)targetingContext).Attributes.Keys)
                    builder.Set(key, ((LaunchDarklyTargetingContext)targetingContext).Attributes[key]);
                context = builder.Build();
            }
            else
            {
                var builder = Context.Builder(targetingContext.UserId);

                var groupIdx = 0;
                foreach (var group in targetingContext.Groups)
                    builder.Set(String.Concat("group", (++groupIdx).ToString()), group);

                context = builder.Build();
            }

            return context.GetValueOrDefault();
        }
    }
}