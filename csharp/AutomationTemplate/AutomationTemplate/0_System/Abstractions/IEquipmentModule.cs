using Autofac;

namespace AutomationTemplate._0_System.Abstractions
{
    public interface IEquipmentModule
    {
        void Register(ContainerBuilder builder);
        void WarmUp(IContainer container);
    }
}
