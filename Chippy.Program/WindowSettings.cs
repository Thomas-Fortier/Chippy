using OpenTK.Windowing.Desktop;

namespace Chippy.Program;

public class WindowSettings
{
  public GameWindowSettings GameWindowSettings { get; private set; }
  public NativeWindowSettings NativeWindowSettings { get; private set; }

  public WindowSettings(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
  {
    GameWindowSettings = gameWindowSettings;
    NativeWindowSettings = nativeWindowSettings;
  }
}