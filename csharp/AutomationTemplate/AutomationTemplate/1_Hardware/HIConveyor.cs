using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._1_Hardware
{
    public interface HIConveyor
    {

        int Stop();
        int RunForward();
        int RunBackward();

        bool IsStop();
        bool IsRunForward();
        bool IsRunBackward();


    }
}
