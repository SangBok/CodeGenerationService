namespace TestBed._4_Process
{
    public class PStage
    {
        public enum StageStep
        {
            CHECK_INIT,
            CHECK_SENSOR,
            MOVE_GRIP,
            GRIP,
            MOVE_PLACE,
            PLACE,
            DONE,
        }

        private StageStep currentStep;

        public void Run()
        {
            switch (currentStep)
            {
                case StageStep.CHECK_INIT:
                    currentStep = StageStep.CHECK_SENSOR;
                    break;
                case StageStep.CHECK_SENSOR:
                    currentStep = StageStep.MOVE_GRIP;
                    break;
                case StageStep.MOVE_GRIP:
                    currentStep = StageStep.GRIP;
                    break;
                case StageStep.GRIP:
                    currentStep = StageStep.MOVE_PLACE;
                    break;
                case StageStep.MOVE_PLACE:
                    currentStep = StageStep.PLACE;
                    break;
                case StageStep.PLACE:
                    currentStep = StageStep.DONE;
                    break;
                case StageStep.DONE:
                    currentStep = StageStep.CHECK_INIT;
                    break;
            }
        }
    }
}
