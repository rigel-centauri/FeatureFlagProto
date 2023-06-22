using LoopUp.FeatureManagement.LaunchDarkly;
using Microsoft.FeatureManagement.FeatureFilters;

namespace FeatureFlagProto
{
    public class ProtoTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string TargetingContextLookup = "TargetingContext";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProtoTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext? httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext != null)
            {
                var contextFound = httpContext.Items.TryGetValue(TargetingContextLookup, out object? value);
                if (contextFound && value is TargetingContext)
                {
                    return new ValueTask<TargetingContext>((TargetingContext)value);
                }

                TargetingContext targetingContext = new LaunchDarklyTargetingContext
                {
                    UserId = Guid.NewGuid().ToString(),
                    Attributes = new Dictionary<string, string>()
                    {
                        ["domain"] = "loopup.co",
                        ["country"] = "UK",
                    }
                };

                if (httpContext != null)
                    httpContext.Items[TargetingContextLookup] = targetingContext;

                return new ValueTask<TargetingContext>(targetingContext);
            }
            else
            {
                TargetingContext targetingContext = new LaunchDarklyTargetingContext
                {
                    UserId = Guid.NewGuid().ToString(),
                };

                return new ValueTask<TargetingContext>(targetingContext);
            }
        }
    }
}
