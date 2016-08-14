namespace LiquiDOS.HAL
{
    public class CPU
    {
        public static uint getRam() { return Cosmos.Core.CPU.GetAmountOfRAM(); }
    }
}
