using LaunchDarkly.Sdk;
using LoopUp.FeatureManagement.LaunchDarkly;
using Microsoft.FeatureManagement.FeatureFilters;

namespace WebApplication1
{
    public class ProtoTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string TargetingContextLookup = "TargetingContext";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProtoTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

                var userId = httpContext?.User?.Identity?.Name ?? Guid.NewGuid().ToString();
                var userDomain = userId.Split("@", StringSplitOptions.None).Length > 1 ? userId.Split("@", StringSplitOptions.None)[1] : "default.com";

                TargetingContext targetingContext = new LaunchDarklyTargetingContext
                {
                    UserId = userId,
                    Attributes = new Dictionary<string, string>()
                    {
                        ["domain"] = userDomain,
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
