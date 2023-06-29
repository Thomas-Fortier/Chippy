using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Text;

namespace Chippy.Emulator
{
  internal abstract class Program
  {
    private static void Main(string[] args)
    {
      if (args.Length < 1)
      {
        throw new ArgumentException("Must specify a file path for the ROM to run.");
      }

      var romFilePath = args[0];

      if (!File.Exists(romFilePath))
      {
        throw new FileNotFoundException("Could not find specified ROM file.", romFilePath);
      }
      
      var stringBuilder = new StringBuilder();
      var rom = new List<byte>();

      foreach (var romByte in File.ReadAllBytes(romFilePath))
      {
        rom.Add(romByte);
        stringBuilder.Append(Convert.ToString(romByte, 2).PadLeft(8, '0'));
      }

      var emulator = CreateEmulator(rom.ToArray());
      emulator.Run();
    }

    private static Emulator CreateEmulator(byte[] rom)
    {
      var gameSettings = new GameWindowSettings
      {
        UpdateFrequency = 600
      };

      var nativeSettings = new NativeWindowSettings
      {
        Size = new Vector2i(1024, 512),
        Profile = ContextProfile.Compatability,
        Title = "Chippy"
      };

      var windowSettings = new WindowSettings(gameSettings, nativeSettings);
      var memory = new Memory(new byte[4096], new Stack<ushort>());
      var keypad = CreateKeypad();
      var display = new Display(new byte[Display.NativeWidth * Display.NativeHeight]);
      var hardware = new Hardware(display, keypad, memory);
      var emulator = new Emulator(windowSettings, hardware, rom);

      return emulator;
    }

    private static Keypad CreateKeypad()
    {
      var keys = new byte[]
      {
        0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,
        0xA, 0xB, 0xC, 0xD, 0xE, 0xF
      };

      return new Keypad(keys);
    }
  }
}