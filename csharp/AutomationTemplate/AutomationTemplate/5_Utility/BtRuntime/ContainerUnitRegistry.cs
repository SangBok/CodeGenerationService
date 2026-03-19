using AutomationTemplate._0_System;

namespace AutomationTemplate._5_Utility.BtRuntime
{
    public sealed class ContainerUnitRegistry : IUnitRegistry
    {
        public object ResolveUnit(string unitName)
        {
            return MSystem.GetInstance().ResolveNamed<object>(unitName);
        }
    }
}
