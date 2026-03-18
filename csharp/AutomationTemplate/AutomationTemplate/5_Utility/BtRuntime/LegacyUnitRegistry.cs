using AutomationTemplate._0_System;
using AutomationTemplate._2_Mechanical;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class LegacyUnitRegistry : IUnitRegistry
    {
        public object ResolveUnit(string unitName)
        {
            // PoC mapping: StageHandler -> MHandler (legacy handler unit)
            if (string.Equals(unitName, "StageHandler", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unitName, "Handler", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(unitName, "MHandler", System.StringComparison.OrdinalIgnoreCase))
            {
                return MSystem.GetInstance().Resolve<MHandler>();
            }

            throw new System.InvalidOperationException("Unknown unit name: " + unitName);
        }
    }
}

