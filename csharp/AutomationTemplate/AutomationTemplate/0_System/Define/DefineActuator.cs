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

        public enum CylinderName
        {
            SideAlignCylinder,
            TopAlignCylinder,
            BotAlignCylinder,
        }
    }
}
