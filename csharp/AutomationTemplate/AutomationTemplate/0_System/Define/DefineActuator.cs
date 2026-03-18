using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutomationTemplate._0_System.Define.DefineIO;

namespace AutomationTemplate._0_System.Define
{
    public class DefineActuator
    {
        public enum CylinderName
        {
            SideAlignCylinder,
            TopAlignCylinder,
            BotAlignCylinder,
            Gripper,
        }

        public class Cylinder
        {
            public Cylinder(CylinderName name, IN_MAP onSensor, IN_MAP offSensor, OUT_MAP onSol, OUT_MAP offSol)
            {
                Name = Enum.GetName(typeof(CylinderName), name);
                OnSensor = (int)onSensor;
                OffSensor = (int)offSensor;
                OnSol = (int)onSol;
                OffSol = (int)offSol;
            }

            public string Name { get; set; }
            public int OnSensor;
            public int OffSensor;
            public int OnSol;
            public int OffSol;
        }

    }
}

namespace AutomationTemplate._0_System
{
    using System.Collections.Generic;
    using AutomationTemplate._0_System.Define;

    public partial class MSystem
    {
        public static List<DefineActuator.Cylinder> cylinders = new List<DefineActuator.Cylinder> {
            new DefineActuator.Cylinder(DefineActuator.CylinderName.SideAlignCylinder, DefineIO.IN_MAP.SIDE_CYL_FWD_SENSOR, DefineIO.IN_MAP.SIDE_CYL_BWD_SENSOR, DefineIO.OUT_MAP.SIDE_CYL_FWD_SOL, DefineIO.OUT_MAP.SIDE_CYL_BWD_SOL),
            new DefineActuator.Cylinder(DefineActuator.CylinderName.TopAlignCylinder, DefineIO.IN_MAP.TOP_CYL_FWD_SENSOR, DefineIO.IN_MAP.TOP_CYL_BWD_SENSOR, DefineIO.OUT_MAP.TOP_CYL_FWD_SOL, DefineIO.OUT_MAP.TOP_CYL_BWD_SOL),
            new DefineActuator.Cylinder(DefineActuator.CylinderName.BotAlignCylinder, DefineIO.IN_MAP.BOT_CYL_FWD_SENSOR, DefineIO.IN_MAP.BOT_CYL_BWD_SENSOR, DefineIO.OUT_MAP.BOT_CYL_FWD_SOL, DefineIO.OUT_MAP.BOT_CYL_BWD_SOL),
            new DefineActuator.Cylinder(DefineActuator.CylinderName.Gripper, DefineIO.IN_MAP.GRIPPER_FWD_SENSOR, DefineIO.IN_MAP.GRIPPER_BWD_SENSOR, DefineIO.OUT_MAP.GRIPPER_FWD_SOL, DefineIO.OUT_MAP.GRIPPER_BWD_SOL),
        };
    }
}
