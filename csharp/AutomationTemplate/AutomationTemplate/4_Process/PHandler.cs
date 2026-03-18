using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTemplate._4_Process
{
    public class PHandler
    {
        public enum HandlerStep
        {
            CHECK_INIT,
            CHECK_SENSOR,
            MOVE_GRIP,
            GRIP,
            MOVE_PLACE,
            PLACE,
            DONE,
        }

        private HandlerStep currentStep;

        public void Run()
        {

            switch (currentStep)
            {
                case HandlerStep.CHECK_INIT:
                    currentStep = HandlerStep.CHECK_SENSOR;
                    break;
                case HandlerStep.CHECK_SENSOR:
                    currentStep = HandlerStep.MOVE_GRIP;
                    break;
                case HandlerStep.MOVE_GRIP:
                    currentStep = HandlerStep.GRIP;
                    break;
                case HandlerStep.GRIP:
                    currentStep = HandlerStep.MOVE_PLACE;
                    break;
                case HandlerStep.MOVE_PLACE:
                    currentStep = HandlerStep.PLACE;
                    break;
                case HandlerStep.PLACE:
                    currentStep = HandlerStep.DONE;
                    break;
                case HandlerStep.DONE:
                    currentStep = HandlerStep.CHECK_INIT;
                    break;
            }
        }
    }
}
