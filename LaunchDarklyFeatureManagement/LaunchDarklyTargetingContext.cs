using Microsoft.FeatureManagement.FeatureFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopUp.FeatureManagement.LaunchDarkly
{
    public class LaunchDarklyTargetingContext: TargetingContext
    {
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
