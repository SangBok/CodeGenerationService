using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._1_Hardware
{
    public interface ICylinder
    {
        int Forward(bool isAsync = true);
        int Backward(bool isAsync = true);

        bool IsForward();
        bool IsBackward();

    }
}
