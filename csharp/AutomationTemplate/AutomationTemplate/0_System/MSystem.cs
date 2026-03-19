using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutomationTemplate._0_System.Abstractions;
using AutomationTemplate._0_System.Config;
using AutomationTemplate._0_System.Define;
using AutomationTemplate._1_Hardware;
using AutomationTemplate._1_Hardware.Mock;
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

        public int Initialize(IEquipmentModule equipmentModule = null)
        {
            if (container != null)
                return 0;

            CreateHardware(equipmentModule);
            CreateMechanical();
            CreateControl();
            CreateProcess();

            return 0;
        }

        public int CreateHardware(IEquipmentModule equipmentModule = null)
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

            equipmentModule?.Register(builder);

            container = builder.Build();
            equipmentModule?.WarmUp(container);

            return 0;
        }

        public int CreateMechanical()
        {
            return 0;
        }

        public int CreateControl()
        {
            return 0;
        }

        public int CreateProcess()
        {
            //PHandler 생성
            //PStage 생성
            return 0;
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public HIAxisController ResolveAxis(string axisId)
        {
            if (string.IsNullOrWhiteSpace(axisId))
                throw new ArgumentException("axisId is required", nameof(axisId));
            return container.ResolveNamed<HIAxisController>(axisId);
        }

        public T ResolveNamed<T>(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("serviceName is required", nameof(serviceName));
            return container.ResolveNamed<T>(serviceName);
        }
    }
}
