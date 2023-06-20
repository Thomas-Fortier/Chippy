using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;

namespace Chippy.Program
{
  internal class Window : GameWindow
  {
    public int NativeWidth { get; } = 64;
    public int NativeHeight { get; } = 32;

    public byte[] Buffer { get; set; }

    private Emulator? _emulator;
    private byte[] _data;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, int nativeWidth, int nativeHeight, byte[] data)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      NativeWidth = nativeWidth;
      NativeHeight = nativeHeight;
      _data = data;
      Buffer = new byte[NativeWidth * NativeHeight];
    }

    public void AddEmulator(Emulator emulator)
    {
      _emulator = emulator;
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

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);
      Render();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);

      if (_emulator != null)
      {
        _emulator.ExecuteCycle();
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, e.Width, e.Height);
    }

    public void Clear()
    {
      for (int index = 0; index < Buffer.Length; index++)
      {
        Buffer[index] = 0;
      }
    }

    private void Render()
    {
      GL.Clear(ClearBufferMask.ColorBufferBit);

      for (int y = 0; y < NativeHeight; y++)
      {
        for (int x = 0; x < NativeWidth; x++)
        {
          var currentPixel = Buffer[y * NativeWidth + x];

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
