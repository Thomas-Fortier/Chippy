namespace Chippy.Program
{
  public class Display
  {
    public const int NativeWidth = 64;
    public const int NativeHeight = 32;

    private readonly byte[] _buffer;

    public Display(byte[] buffer)
    {
      _buffer = buffer;
      _buffer = new byte[NativeWidth * NativeHeight];
    }

    public void TogglePixel(int location)
    {
      _buffer[location] ^= 1;
    }

    public bool IsPixelOn(int location)
    {
      return _buffer[location] == 1;
    }

    public void Clear()
    {
      for (var index = 0; index < _buffer.Length; index++)
      {
        _buffer[index] = 0;
      }
    }
  }
}
