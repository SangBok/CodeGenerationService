using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutomationTemplate._0_System.Config;
using AutomationTemplate._0_System.Define;
using AutomationTemplate._1_Hardware;
using AutomationTemplate._1_Hardware.Mock;
using AutomationTemplate._2_Mechanical;
using AutomationTemplate._3_Control;
using static AutomationTemplate._0_System.Define.DefineActuator;
using static AutomationTemplate._0_System.Define.DefineAxis;

namespace AutomationTemplate._0_System
{
    public partial class MSystem
    {

        private static MSystem instance;
        private IContainer container;

        public static MSystem GetInstance()
        {
            if(instance == null)
                instance = new MSystem();
            return instance;
        }

        public int Initialize()
        {
            CreateHardware();
            CreateMechanical();
            CreateControl();
            CreateProcess();

            return 0;
        }

        public int CreateHardware()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MockIoController>()
                .As<HIIoController>()
                .SingleInstance();

            var config = HardwareConfigLoader.LoadFromConfigDirectory(AppDomain.CurrentDomain.BaseDirectory);

            foreach (var axisGroup in config.AxisGroups.AxisGroups)
            {
                foreach (var axisId in axisGroup.Axes)
                {
                    builder.RegisterInstance(new MockAxisController(axisId))
                        .As<HIAxisController>()
                        .Named<HIAxisController>(axisId)
                        .SingleInstance();
                }
            }

            foreach (var cylinder in config.Cylinders.Cylinders)
            {
                var cylinderId = cylinder.Name;
                var onSensorPort = config.ResolveInputPort(cylinder.OnSensor);
                var offSensorPort = config.ResolveInputPort(cylinder.OffSensor);
                var onSolPort = config.ResolveOutputPort(cylinder.OnSol);
                var offSolPort = config.ResolveOutputPort(cylinder.OffSol);

                builder.Register(ctx =>
                    new MockCylinder(
                        ctx.Resolve<HIIoController>(),
                        onSensorPort,
                        offSensorPort,
                        onSolPort,
                        offSolPort))
                    .As<HICylinder>()
                    .Named<HICylinder>(cylinderId)
                    .SingleInstance();
            }

            builder.Register(ctx =>
                new MHandler(
                    ctx.ResolveNamed<HIAxisController>("HandlerX"),
                    ctx.ResolveNamed<HIAxisController>("HandlerY"),
                    ctx.ResolveNamed<HIAxisController>("HandlerZ"),
                    ctx.ResolveNamed<HICylinder>("Gripper")))
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx =>
                new MStage(
                    ctx.ResolveNamed<HIAxisController>("StageX"),
                    ctx.ResolveNamed<HIAxisController>("StageY"),
                    ctx.ResolveNamed<HICylinder>("SideAlignCylinder"),
                    ctx.ResolveNamed<HICylinder>("TopAlignCylinder"),
                    ctx.ResolveNamed<HICylinder>("BotAlignCylinder")))
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CHandler>()
                .AsSelf()
                .SingleInstance();

            container = builder.Build();

            return 0;
        }

        public int CreateMechanical()
        {
            container.Resolve<MHandler>();
            container.Resolve<MStage>();
            return 0;
        }

        public int CreateControl()
        {
            container.Resolve<CHandler>();
            return 0;
        }

        public int CreateProcess()
        {
            //PHandler 생성
            //PStage 생성
            return 0;
        }
    }
}
