using AutomationTemplate._2_Mechanical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._3_Control
{
    public class CHandler
    {
        private MHandler mHandler;

        public CHandler(MHandler mHandler)
        {
            this.mHandler = mHandler;
        }

        public int MoveSafePos(int positionidx)
        {
            mHandler.MovePositionZ(0);
            mHandler.MovePositionXY(positionidx);
            mHandler.MovePositionZ(positionidx);
            return 0;
        }
    }
}
