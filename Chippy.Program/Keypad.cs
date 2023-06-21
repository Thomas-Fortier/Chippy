using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Chippy.Program
{
  internal class Keypad
  {
    private Dictionary<byte, bool> _keys;

    public Keypad(byte[] keys, Window window)
    {
      _keys = new Dictionary<byte, bool>();
      
      foreach (byte key in keys)
      {
        _keys.Add(key, false);
      }

      window.OnKeyPressed += EnableKey;
      window.OnKeyReleased += DisableKey;
    }

    public void EnableKey(object? sender, byte key)
    {
      _keys[key] = true;
    }

    public void DisableKey(object? sender, byte key)
    {
      _keys[key] = false;
    }

    public bool IsKeyPressed(byte key)
    {
      return _keys.GetValueOrDefault(key);
    }

    public static byte GetKeyCode(KeyboardKeyEventArgs e)
    {
      switch (e.Key)
      {
        case Keys.D0:
          return 0x0;
        case Keys.D1:
          return 0x1;
        case Keys.D2:
          return 0x2;
        case Keys.D3:
          return 0x3;
        case Keys.D4:
          return 0x4;
        case Keys.D5:
          return 0x5;
        case Keys.D6:
          return 0x6;
        case Keys.D7:
          return 0x7;
        case Keys.D8:
          return 0x8;
        case Keys.D9:
          return 0x9;
        case Keys.A:
          return 0xA;
        case Keys.B:
          return 0xB;
        case Keys.C:
          return 0xC;
        case Keys.D:
          return 0xD;
        case Keys.E:
          return 0xE;
        case Keys.F:
          return 0xF;
        default:
          throw new NotImplementedException();
      }
    }
  }
}
