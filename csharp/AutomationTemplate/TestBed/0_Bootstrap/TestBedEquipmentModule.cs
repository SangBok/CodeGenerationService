using Autofac;
using AutomationTemplate._0_System.Abstractions;
using AutomationTemplate._1_Hardware;
using AutomationTemplate._5_Utility.BtRuntime;
using TestBed._2_Mechanical;
using TestBed._3_Control;
using TestBed._4_Process;

namespace TestBed
{
    public sealed class TestBedEquipmentModule : IEquipmentModule
    {
        private readonly TestBedEquipmentProfile profile;

        public TestBedEquipmentModule(TestBedEquipmentProfile profile)
        {
            this.profile = profile ?? throw new System.ArgumentNullException(nameof(profile));
        }

        public void Register(ContainerBuilder builder)
        {
            builder.Register(ctx =>
                new MHandler(
                    ctx.ResolveNamed<HIAxisController>(profile.HandlerAxes.X),
                    ctx.ResolveNamed<HIAxisController>(profile.HandlerAxes.Y),
                    ctx.ResolveNamed<HIAxisController>(profile.HandlerAxes.Z),
                    ctx.ResolveNamed<HICylinder>(profile.Cylinders.Gripper)))
                .AsSelf()
                .As<IUnitMotion>()
                .SingleInstance();

            foreach (var alias in profile.UnitAliases)
            {
                var normalizedAlias = alias?.Trim();
                if (!string.IsNullOrWhiteSpace(normalizedAlias))
                    builder.Register(ctx => (object)ctx.Resolve<MHandler>()).Named<object>(normalizedAlias).SingleInstance();
            }

            builder.Register(ctx =>
                new MStage(
                    ctx.ResolveNamed<HIAxisController>(profile.StageAxes.X),
                    ctx.ResolveNamed<HIAxisController>(profile.StageAxes.Y),
                    ctx.ResolveNamed<HICylinder>(profile.Cylinders.SideAlign),
                    ctx.ResolveNamed<HICylinder>(profile.Cylinders.TopAlign),
                    ctx.ResolveNamed<HICylinder>(profile.Cylinders.BottomAlign)))
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CHandler>().AsSelf().SingleInstance();
            builder.RegisterType<PHandler>().AsSelf().SingleInstance();
            builder.RegisterType<PStage>().AsSelf().SingleInstance();
        }

        public void WarmUp(IContainer container)
        {
            container.Resolve<MHandler>();
            container.Resolve<MStage>();
            container.Resolve<CHandler>();
            container.Resolve<PHandler>();
            container.Resolve<PStage>();
        }
    }
}
