using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Chippy.Program
{
  public class Keypad
  {
    private readonly Dictionary<byte, bool> _keys;

    public Keypad(IEnumerable<byte> keys)
    {
      _keys = new Dictionary<byte, bool>();
      
      foreach (var key in keys)
      {
        _keys.Add(key, false);
      }
    }

    public void EnableKey(byte key)
    {
      if (!_keys.ContainsKey(key))
      {
        return;
      }
      
      _keys[key] = true;
    }

    public void DisableKey(byte key)
    {
      if (!_keys.ContainsKey(key))
      {
        return;
      }
      
      _keys[key] = false;
    }

    public bool IsKeyPressed(byte key)
    {
      return _keys.GetValueOrDefault(key);
    }

    public bool IsAnyKeyPressed()
    {
      return _keys.Any(key => key.Value);
    }

    public byte? GetPressedKeyCode()
    {
      foreach (var key in _keys.Where(key => key.Value))
      {
        return key.Key;
      }

      return null;
    }

    public static byte? GetKeyCode(Keys key)
    {
      return key switch
      {
        Keys.D0 => 0x0,
        Keys.D1 => 0x1,
        Keys.D2 => 0x2,
        Keys.D3 => 0x3,
        Keys.D4 => 0x4,
        Keys.D5 => 0x5,
        Keys.D6 => 0x6,
        Keys.D7 => 0x7,
        Keys.D8 => 0x8,
        Keys.D9 => 0x9,
        Keys.A => 0xA,
        Keys.B => 0xB,
        Keys.C => 0xC,
        Keys.D => 0xD,
        Keys.E => 0xE,
        Keys.F => 0xF,
        _ => null
      };
    }
  }
}
