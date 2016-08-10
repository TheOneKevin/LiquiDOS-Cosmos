using System;
using Ho = Cosmos.HAL;

namespace LiquiDOS.HAL
{
    public class PIT
    {
        Ho.PIT pit;

        public PIT() { pit = new Ho.PIT(); }

        public void timer(Ho.PIT.PITTimer.dOnTrigger trigger, int msTime, bool recuring)
        {
            Ho.PIT.PITTimer timerz = new Ho.PIT.PITTimer(trigger, msTime, recuring);
            pit.RegisterTimer(timerz);
        }

        public void waitMS(uint msDelay) { pit.Wait(msDelay); }
        public void muteSound() { pit.MuteSound(); }
        public void playSound(int frequency) { pit.PlaySound(frequency); }
        public void handleInt() { pit.HandleInterrupt(); }
    }
}
