using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.FeatureManagement;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFeatureManager _featureManager;

        public string WelcomeMessage { get; set; } = "What";

        public IndexModel(ILogger<IndexModel> logger, IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        public async Task OnGetAsync()
        {
            WelcomeMessage = await Task.Run(() => _featureManager.IsEnabledAsync(FeatureFlags.NewWelcomeBanner)) ? "Welcome to the Beta" : "Welcome";
        }

        public async Task OnGetAsync_B()
        {
            if (await _featureManager.IsEnabledAsync(FeatureFlags.Beta)) 
                methodBetaOn(); 
            else 
                methodBetaOff();
        }

        private void methodBetaOn()
        {
            //step 1
            //step 2
            WelcomeMessage = "Welcome to the Beta";
            //step 3
            //step 4
        }

        private void methodBetaOff()
        {
            //step 1
            //step 2
            WelcomeMessage = "Welcome";
            //step 3
            //step 4
        }

    }
}