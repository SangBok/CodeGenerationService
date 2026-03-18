using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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

            foreach (var axisGroup in axes)
            {
                foreach (var axisName in axisGroup.AxisNames)
                {
                    var axisId = Enum.GetName(typeof(AxisName), axisName);
                    builder.RegisterInstance(new MockAxisController(axisId))
                        .As<HIAxisController>()
                        .Named<HIAxisController>(axisId)
                        .SingleInstance();
                }
            }

            foreach (var cylinder in cylinders)
            {
                var cylinderId = cylinder.Name;
                builder.Register(ctx =>
                    new MockCylinder(
                        ctx.Resolve<HIIoController>(),
                        cylinder.OnSensor,
                        cylinder.OffSensor,
                        cylinder.OnSol,
                        cylinder.OffSol))
                    .As<HICylinder>()
                    .Named<HICylinder>(cylinderId)
                    .SingleInstance();
            }

            builder.Register(ctx =>
                new MHandler(
                    ctx.ResolveNamed<HIAxisController>(Enum.GetName(typeof(AxisName), AxisName.HandlerX)),
                    ctx.ResolveNamed<HIAxisController>(Enum.GetName(typeof(AxisName), AxisName.HandlerY)),
                    ctx.ResolveNamed<HIAxisController>(Enum.GetName(typeof(AxisName), AxisName.HandlerZ)),
                    ctx.ResolveNamed<HICylinder>(Enum.GetName(typeof(CylinderName), CylinderName.Gripper))))
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx =>
                new MStage(
                    ctx.ResolveNamed<HIAxisController>(Enum.GetName(typeof(AxisName), AxisName.StageX)),
                    ctx.ResolveNamed<HIAxisController>(Enum.GetName(typeof(AxisName), AxisName.StageY)),
                    ctx.ResolveNamed<HICylinder>(Enum.GetName(typeof(CylinderName), CylinderName.SideAlignCylinder)),
                    ctx.ResolveNamed<HICylinder>(Enum.GetName(typeof(CylinderName), CylinderName.TopAlignCylinder)),
                    ctx.ResolveNamed<HICylinder>(Enum.GetName(typeof(CylinderName), CylinderName.BotAlignCylinder))))
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
