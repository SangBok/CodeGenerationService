using AutomationTemplate._1_Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._2_Mechanical
{
    public class MHandler
    {
        private HIAxisController handlerX, handlerY, handlerZ;
        private HICylinder gripper;

        public MHandler(HIAxisController handlerX, HIAxisController handlerY, HIAxisController handlerZ, HICylinder gripper)
        {
            this.handlerX = handlerX;
            this.handlerY = handlerY;
            this.handlerZ = handlerZ;
            this.gripper = gripper;
        }

        public int MovePositionZ(int positionidx)
        {
            handlerZ.MoveAbsolute(positionidx);
            return 0;
        }

        public int MovePositionXY(int positionidx)
        {
            handlerX.MoveAbsolute(positionidx);
            handlerY.MoveAbsolute(positionidx);
            return 0;
        }

        public int MaterialGrip()
        {
            gripper.Forward();
            return 0;
        }

        public int MaterialUngrip()
        {
            gripper.Backward();
            return 0;
        }
    }
}
