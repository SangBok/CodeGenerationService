using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._1_Hardware
{
    public interface IIoController
    {
        //input check
        bool IsInputOn(int port);
        bool IsInputOff(int port);

        //output control
        int OutputOn(int port);
        int OutputOff(int port);

        //output check
        bool IsOutputOn(int port);
        bool IsOutputOff(int port);
    }
}
