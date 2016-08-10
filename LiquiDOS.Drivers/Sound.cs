using Cosmos.HAL;

namespace LiquiDOS.Drivers
{
    public class Sound
    {
        PCSpeaker s;
        public Sound() { s = new PCSpeaker(); }
        public void beep() { s.beep(); }
        public void playSound(uint freq) { s.playSound(freq); }
        public void muteSound() { s.nosound(); }
    }
}
