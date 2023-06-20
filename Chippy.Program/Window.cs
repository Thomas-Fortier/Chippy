using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace Chippy.Program
{
  internal class Window : GameWindow
  {
    private int _nativeWidth = 64;
    private int _nativeHeight = 32;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, int nativeWidth, int nativeHeight)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      _nativeWidth = nativeWidth;
      _nativeHeight = nativeHeight;
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(Color.Black);
      GL.Color3(Color.White);
      GL.Ortho(0, 64, 32, 0, -1, 1);
      GL.Clear(ClearBufferMask.ColorBufferBit);

      SwapBuffers();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, e.Width, e.Height);
    }

    public void Render(byte[] buffer)
    {
      GL.Clear(ClearBufferMask.ColorBufferBit);

      for (int y = 0; y < _nativeHeight; y++)
      {
        for (int x = 0; x < _nativeWidth; x++)
        {
          var currentPixel = buffer[y * _nativeWidth + x];

          if (currentPixel > 0)
          {
            GL.Rect(x, y, x + 1, y + 1);
          }
        }
      }
    }
  }
}
