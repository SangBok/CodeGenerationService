using AutomationTemplate._1_Hardware;
using AutomationTemplate._5_Utility.BtRuntime;

namespace TestBed._2_Mechanical
{
    public class MHandler : IUnitMotion
    {
        private readonly HIAxisController handlerX;
        private readonly HIAxisController handlerY;
        private readonly HIAxisController handlerZ;
        private readonly HICylinder gripper;

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
