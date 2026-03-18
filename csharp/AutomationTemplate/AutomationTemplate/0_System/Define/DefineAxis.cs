using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._0_System.Define
{
    public class DefineAxis
    {
        public enum AxisName
        {
            HandlerX,
            HandlerY,
            HandlerZ,

            StageX,
            StageY,
        }

        public enum AxisGroup
        {
            HandlerXY,
            HandlerZ,
            StageXY
        }

        public enum TeachingPosHandlerXY
        {
            Grip,
            UnGrip
        }

        public enum TeachingPosHandlerZ
        {
            Safe,
            GripUngrip
        }

        public enum TeachingPosStageXY
        {
            Receive,
            Work,
            Send
        }


        public class Axis
        {
            public Axis(AxisGroup groupName, List<AxisName> axisNames)
            {
                GroupName = Enum.GetName(typeof(AxisGroup), groupName);
                AxisNames = axisNames ?? new List<AxisName>();
            }

            public string GroupName { get; set; }
            public List<AxisName> AxisNames { get; private set; }
        }

    }
}

namespace AutomationTemplate._0_System
{
    using System.Collections.Generic;
    using AutomationTemplate._0_System.Define;

    public partial class MSystem
    {
        public static List<DefineAxis.Axis> axes = new List<DefineAxis.Axis> {
            new DefineAxis.Axis(DefineAxis.AxisGroup.HandlerXY, new List<DefineAxis.AxisName>{ DefineAxis.AxisName.HandlerX, DefineAxis.AxisName.HandlerY }),
            new DefineAxis.Axis(DefineAxis.AxisGroup.HandlerZ, new List<DefineAxis.AxisName>{ DefineAxis.AxisName.HandlerZ}),
            new DefineAxis.Axis(DefineAxis.AxisGroup.StageXY, new List<DefineAxis.AxisName>{ DefineAxis.AxisName.StageX, DefineAxis.AxisName.StageY }),
        };
    }
}
