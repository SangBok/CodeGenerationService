using TestBed._2_Mechanical;

namespace TestBed._3_Control
{
    public class CHandler
    {
        private readonly MHandler mHandler;

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
