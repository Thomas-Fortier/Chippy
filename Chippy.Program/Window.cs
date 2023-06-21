using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Chippy.Program
{
  internal class Window : GameWindow
  {
    public delegate void WindowEvent();
    public event WindowEvent? OnFrameRendered;
    public event EventHandler<byte> OnKeyPressed;
    public event EventHandler<byte> OnKeyReleased;

    public int NativeWidth { get; } = 64;
    public int NativeHeight { get; } = 32;

    private byte[] _displayBuffer;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, int nativeWidth, int nativeHeight)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      NativeWidth = nativeWidth;
      NativeHeight = nativeHeight;
      _displayBuffer = new byte[NativeWidth * NativeHeight];
    }

    public void Clear()
    {
      for (int index = 0; index < _displayBuffer.Length; index++)
      {
        _displayBuffer[index] = 0;
      }
    }

    public void TogglePixel(int location)
    {
      _displayBuffer[location] ^= 1;
    }

    public bool IsPixelOn(int location)
    {
      return _displayBuffer[location] == 1;
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(Color.Black);
      GL.Color3(Color.White);
      GL.Ortho(0, NativeWidth, NativeHeight, 0, -1, 1);
      GL.Clear(ClearBufferMask.ColorBufferBit);

      SwapBuffers();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
      base.OnKeyDown(e);
      byte keyCode;

      try
      {
        keyCode = Keypad.GetKeyCode(e);
      }
      catch (NotImplementedException)
      {
        return;
      }

      OnKeyPressed?.Invoke(this, keyCode);
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
      base.OnKeyUp(e);
      byte keyCode;

      try
      {
        keyCode = Keypad.GetKeyCode(e);
      }
      catch (NotImplementedException)
      {
        return;
      }

      OnKeyReleased?.Invoke(this, keyCode);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      Render();

      if (OnFrameRendered != null)
      {
        OnFrameRendered();
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, e.Width, e.Height);
    }

    private void Render()
    {
      GL.Clear(ClearBufferMask.ColorBufferBit);

      for (int y = 0; y < NativeHeight; y++)
      {
        for (int x = 0; x < NativeWidth; x++)
        {
          var currentPixel = _displayBuffer[y * NativeWidth + x];

          if (currentPixel > 0)
          {
            GL.Rect(x, y, x + 1, y + 1);
          }
        }
      }

      SwapBuffers();
    }
  }
}
