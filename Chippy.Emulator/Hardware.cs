namespace Chippy.Emulator;

public class Hardware
{
  public Processor Processor { get; private set; }
  public Display Display { get; }
  public Keypad Keypad { get; }
  public Memory Memory { get; }

  public Hardware(Display display, Keypad keypad, Memory memory)
  {
    Display = display;
    Keypad = keypad;
    Memory = memory;
    Processor = new Processor(Memory, Display, Keypad, new byte[16]);
  }
}