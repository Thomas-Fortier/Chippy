using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Text;

namespace Chippy.Program
{
  internal abstract class Program
  {
    private static void Main()
    {
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Chip8 emulator Logo [Garstyciuks].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\IBM Logo.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Chip8 Picture.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Particle Demo [zeroZshadow, 2008].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Random Number Test [Matthew Mikolay, 2010].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Zero Demo [zeroZshadow, 2007].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Clock Program [Bill Fisher, 1981].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Brix [Andreas Gustafsson, 1990].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Tetris [Fran Dachille, 1991].ch8");
      byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Space Invaders [David Winter].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Trip8 Demo (2008) [Revival Studios].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Maze [David Winter, 199x].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Stars [Sergey Naydenov, 2010].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Breakout [Carmelo Cortez, 1979].ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\Pong (1 player).ch8");

      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\1-chip8-logo (1).8o");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\2-ibm-logo.8o");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\test_opcode.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\c8_test.c8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\chip8-test-rom.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\SCTEST.CH8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\1-chip8-logo.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\2-ibm-logo.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\4-flags.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\3-corax+.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\5-quirks.ch8");
      //byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\6-keypad.ch8");
      StringBuilder sb = new StringBuilder();
      List<byte> bytes = new List<byte>();

      foreach (byte b in fileBytes)
      {
        bytes.Add(b);
        sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
      }

      var emulator = CreateEmulator(bytes.ToArray());
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