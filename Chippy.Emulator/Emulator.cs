using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Chippy.Emulator
{
  internal class Emulator : GameWindow
  {
    private readonly Processor _processor;
    private readonly Keypad _keypad;
    private readonly Display _display;

    private bool _pause;
    private bool _step;

    public Emulator(WindowSettings windowSettings, Hardware hardware, byte[] rom)
      : base(windowSettings.GameWindowSettings, windowSettings.NativeWindowSettings)
    {
      _processor = hardware.Processor;
      _keypad = hardware.Keypad;
      _display = hardware.Display;

      hardware.Memory.LoadRom(rom, 0x200);

      _processor.WaitingForKeyPress += (_, _) =>
      {
        ProcessWindowEvents(true);
      };
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(Color.Black);
      GL.Color3(Color.White);
      GL.Ortho(0, Display.NativeWidth, Display.NativeHeight, 0, -1, 1);
      GL.Clear(ClearBufferMask.ColorBufferBit);

      SwapBuffers();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
      base.OnKeyDown(e);

      switch (e.Key)
      {
        case Keys.P:
          _pause = !_pause;
          Title = _pause ? "Chippy (PAUSED)" : "Chippy";
          return;
        case Keys.Space when _pause:
          _step = true;
          return;
      }

      var keyCode = Keypad.GetKeyCode(e.Key);

      if (keyCode != null)
      {
        _keypad.EnableKey(keyCode.Value);
      }
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
      base.OnKeyUp(e);

      var keyCode = Keypad.GetKeyCode(e.Key);

      if (keyCode != null)
      {
        _keypad.DisableKey(keyCode.Value);
      }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      switch (_pause)
      {
        case true when !_step:
          return;
        case true when _step:
          _step = false;
          break;
      }

      GL.Clear(ClearBufferMask.ColorBufferBit);

      Render();
      _processor.ExecuteCycle();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, e.Width, e.Height);
    }

    private void Render()
    {
      GL.Clear(ClearBufferMask.ColorBufferBit);

      for (var y = 0; y < Display.NativeHeight; y++)
      {
        for (var x = 0; x < Display.NativeWidth; x++)
        {
          if (_display.IsPixelOn(y * Display.NativeWidth + x))
          {
            GL.Rect(x, y, x + 1, y + 1);
          }
        }
      }

      SwapBuffers();
    }
  }
}
