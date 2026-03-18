using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._0_System
{
    public class MSystem
    {
        private static MSystem instance;

        public static MSystem GetInstance()
        {
            if(instance == null)
                instance = new MSystem();
            return instance;
        }

        public int Initialize()
        {
            CreateHardware();
            CreateMechanical();
            CreateControl();
            CreateProcess();

            return 0;
        }

        public int CreateHardware()
        {

            return 0;
        }

        public int CreateMechanical()
        {
            return 0;
        }

        public int CreateControl()
        {
            return 0;
        }

        public int CreateProcess()
        {
            return 0;
        }
    }
}
