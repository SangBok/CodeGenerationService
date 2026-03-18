using AutomationTemplate._1_Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._2_Mechanical
{
    public class MStage
    {
        private HIAxisController stageX, stageY;
        private HICylinder sideClamp, topClamp, botClamp;

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
