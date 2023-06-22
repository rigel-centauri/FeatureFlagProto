using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace FeatureFlagProto.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFeatureManager _featureManager;

        public IndexModel(ILogger<IndexModel> logger, IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        public string WelcomeMessage { get; set; } = "What";

        public async Task OnGetAsync()
        {
            WelcomeMessage = await Task.Run(() => _featureManager.IsEnabledAsync(FeatureFlags.NewWelcomeBanner)) ? "Welcome to the Beta": "Welcome";
        }
    }
}