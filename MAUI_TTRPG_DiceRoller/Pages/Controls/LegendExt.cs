using Syncfusion.Maui.Toolkit.Charts;

namespace MAUI_TTRPG_DiceRoller.Pages.Controls
{
    public class LegendExt : ChartLegend
    {
        protected override double GetMaximumSizeCoefficient()
        {
            return 0.5;
        }
    }
}
