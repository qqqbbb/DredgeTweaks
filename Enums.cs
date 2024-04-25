using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweaks
{
    public enum FishingSpots
    {
        [Description("vanilla")]
        Vanilla ,
        [Description("never deplete")]
        NeverDeplete,
        [Description("never restock")]
        NeverRestock,
    }

    public enum DredgeSpots
    {
        [Description("vanilla")]
        Vanilla,
        [Description("never deplete")]
        NeverDeplete,
        [Description("never restock")]
        NeverRestock,
    }
}
