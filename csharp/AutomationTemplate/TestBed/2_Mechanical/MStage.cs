using AutomationTemplate._1_Hardware;

namespace TestBed._2_Mechanical
{
    public class MStage
    {
        private readonly HIAxisController stageX;
        private readonly HIAxisController stageY;
        private readonly HICylinder sideClamp;
        private readonly HICylinder topClamp;
        private readonly HICylinder botClamp;

        public MStage(HIAxisController stageX, HIAxisController stageY, HICylinder sideClamp, HICylinder topClamp, HICylinder botClamp)
        {
            this.stageX = stageX;
            this.stageY = stageY;
            this.sideClamp = sideClamp;
            this.topClamp = topClamp;
            this.botClamp = botClamp;
        }

        public int MovePosition(int positionidx)
        {
            stageX.MoveAbsolute(positionidx);
            stageY.MoveAbsolute(positionidx);
            return 0;
        }

        public int MaterialGrip()
        {
            sideClamp.Forward();
            topClamp.Forward();
            botClamp.Forward();
            return 0;
        }

        public int MaterialUngrip()
        {
            sideClamp.Backward();
            topClamp.Backward();
            botClamp.Backward();
            return 0;
        }
    }
}
