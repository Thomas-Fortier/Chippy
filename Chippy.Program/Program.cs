using Chippy.Program;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Chippy.Programe
{
  internal class Program
  {
    private static void Main()
    {
      var window = CreateWindow();
      window.Run();
    }

    private static Window CreateWindow()
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

      return new Window(gameSettings, nativeSettings, NATIVE_WIDTH, NATIVE_HEIGHT);
    }
  }
}