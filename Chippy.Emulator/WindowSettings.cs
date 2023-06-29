using OpenTK.Windowing.Desktop;

namespace Chippy.Emulator;

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