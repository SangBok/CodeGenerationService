using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._0_System.Define
{
    public class DefineIO
    {
        public enum IN_MAP
        {
            START_BUTTON = 0,
            STOP_BUTTON = 1,
            RESET_BUTTON = 2,
            E_STOP_BUTTON = 3,
            SPARE_4,
            SPARE_5,
            SPARE_6,
            SPARE_7,
            SPARE_8,

            SIDE_CYL_FWD_SENSOR = 9,
            SIDE_CYL_BWD_SENSOR = 10,
            TOP_CYL_FWD_SENSOR = 11,
            TOP_CYL_BWD_SENSOR = 12,
            BOT_CYL_FWD_SENSOR = 13,
            BOT_CYL_BWD_SENSOR = 14,

            GRIPPER_FWD_SENSOR = 15,
            GRIPPER_BWD_SENSOR = 16,
        }

        public enum OUT_MAP
        {
            START_BUTTON_LED = 0,
            STOP_BUTTON_LED = 1,
            RESET_BUTTON_LED = 2,
            TOWER_LAMP_RED = 3,     
            TOWER_LAMP_YELLOW = 4,  
            TOWER_LAMP_GREEN = 5,   
            BUZZER = 6,             
            SPARE_7 = 7,             
            SPARE_8 = 8,             

            SIDE_CYL_FWD_SOL = 9,
            SIDE_CYL_BWD_SOL = 10,
            TOP_CYL_FWD_SOL = 11,
            TOP_CYL_BWD_SOL = 12,
            BOT_CYL_FWD_SOL = 13,
            BOT_CYL_BWD_SOL = 14,

            GRIPPER_FWD_SOL = 15,
            GRIPPER_BWD_SOL = 16,

        }
    }
}
