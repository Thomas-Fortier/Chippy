using Chippy.Program;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Text;

namespace Chippy.Programe
{
  internal class Program
  {
    private static void Main()
    {
      byte[] fileBytes = File.ReadAllBytes(@"C:\Users\Thomas\Desktop\IBM Logo.ch8");
      StringBuilder sb = new StringBuilder();
      List<byte> bytes = new List<byte>();

      foreach (byte b in fileBytes)
      {
        bytes.Add(b);
        sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
      }

      var result = sb.ToString();

      var window = CreateWindow(bytes.ToArray());
      window.Run();
    }

    private static Window CreateWindow(byte[] data)
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

      const int NATIVE_WIDTH = 64;
      const int NATIVE_HEIGHT = 32;

      var window = new Window(gameSettings, nativeSettings, NATIVE_WIDTH, NATIVE_HEIGHT, data);
      var memory = new Memory(new byte[4096], new Stack<byte>());
      var processor = new Processor(memory, window, new byte[16]);
      var emulator = new Emulator(processor, data);

      window.AddEmulator(emulator);

      return window;
    }
  }
}