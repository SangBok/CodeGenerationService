using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._1_Hardware
{
    public interface IAxisController
    {
        string AxisId { get; }

        int SeroOn();
        int SeroOff();
        bool isServoOn();
        bool isServoOff();

        int Stop();

        int MoveHome();
        bool IsMoveHomeDone();

        int MoveAbsolute(double position);
        int MoveRelative(double position);
        double GetActualPosition();
    }
}
