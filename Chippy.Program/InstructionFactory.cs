namespace Chippy.Program
{
  internal class InstructionFactory
  {
    private readonly Processor _processor;
    private readonly Memory _memory;
    private readonly Window _window;

    public InstructionFactory(Processor processor, Memory memory, Window window)
    {
      _processor = processor;
      _memory = memory;
      _window = window;
    }

    public Instruction Instruction0NNN(ushort nnn)
    {
      return new Instruction("0NNN", "(Deprecated) Jump to a machine code routine at NNN", () => { });
    }

    public Instruction Instruction00E0()
    {
      return new Instruction("00E0", "Clear Display", () =>
      {
        _window.Clear();
      });
    }

    public Instruction Instruction00EE()
    {
      return new Instruction("00EE", "Return", () =>
      {
        _processor.SetProgramCounter(_memory.PopStack());
      });
    }

    public Instruction Instruction1NNN(ushort nnn)
    {
      return new Instruction("1NNN", "Jump to location", () =>
      {
        _processor.SetProgramCounter(nnn);
      });
    }

    public Instruction Instruction2NNN(ushort nnn)
    {
      return new Instruction("2NNN", "Call subroutine at NNN", () =>
      {
        _memory.PushStack(_processor.GetProgramCounter());
        _processor.SetProgramCounter(nnn);
      });
    }

    public Instruction Instruction3XNN(byte x, byte nn)
    {
      return new Instruction("3XNN", "Skip next instruction if Vx = NN", () =>
      {
        if (_processor.GetVRegister(x) == nn)
        {
          _processor.IncrementProgramCounter(2);
        }
      });
    }

    public Instruction Instruction4XNN(byte x, byte nn)
    {
      return new Instruction("4XNN", "Skip next instruction if Vx != NN", () =>
      {
        if (_processor.GetVRegister(x) != nn)
        {
          _processor.IncrementProgramCounter(2);
        }
      });
    }

    public Instruction Instruction6XNN(byte x, byte nn)
    {
      return new Instruction("6XNN", "Put NN into Vx", () =>
      {
        _processor.SetVRegister(x, nn);
      });
    }

    public Instruction Instruction7XNN(byte x, byte nn)
    {
      return new Instruction("7XNN", "Set Vx = Vx + nn", () =>
      {
        var result = (byte) (_processor.GetVRegister(x) + nn);
        _processor.SetVRegister(x, result);
      });
    }

    public Instruction Instruction8XY0(byte x, byte y)
    {
      return new Instruction("8XY0", "Set Vx = Vy", () =>
      {
        _processor.SetVRegister(x, _processor.GetVRegister(y));
      });
    }

    public Instruction Instruction8XY2(byte x, byte y)
    {
      return new Instruction("8XY2", "Set Vx = Vx AND Vy", () =>
      {
        byte result = (byte)(_processor.GetVRegister(x) & _processor.GetVRegister(y));
        _processor.SetVRegister(x, result);
      });
    }

    public Instruction Instruction8XY4(byte x, byte y)
    {
      return new Instruction("8XY4", "Set Vx = Vx + Vy, set VF = carry", () =>
      {
        int result = (byte)(_processor.GetVRegister(x) + _processor.GetVRegister(y));
        _processor.SetVRegister(0xF, result > byte.MaxValue ? (byte) 1 : (byte) 0); 
      });
    }

    public Instruction Instruction8XYE(byte x, byte y)
    {
      return new Instruction("8XYE", "Set Vx = Vx SHL 1", () =>
      {
        _processor.SetVRegister(0xF, (x & 0xF0000000) == 1 ? (byte)1 : (byte)0);
        _processor.SetVRegister(x, (byte)(_processor.GetVRegister(x) * 2));
      });
    }

    public Instruction InstructionANNN(ushort nnn)
    {
      return new Instruction("ANNN", "Put NNN into I", () =>
      {
        _processor.SetIndexRegister(nnn);
      });
    }

    public Instruction InstructionCXNN(byte x, byte nn)
    {
      return new Instruction("CXNN", "Set Vx = random byte AND NN", () =>
      {
        var randomByte = (byte)new Random().Next(255);
        _processor.SetVRegister(x, (byte)(randomByte & nn));
      });
    }

    public Instruction InstructionDXYN(byte x, byte y, byte n)
    {
      return new Instruction("DXYN", "Draw", () =>
      {
        for (int line = 0; line < n; line++)
        {
          // starting line = Y + current line. If y is larger than the total width of the screen then wrap around (this is the modulo operation).
          var startingLine = (_processor.GetVRegister(y) + line) % _window.NativeHeight;

          // The current sprite being drawn, each line is a new sprite.
          byte sprite = _memory.Read(_processor.GetIndexRegister() + line);

          // Each bit in the sprite is a pixel on or off.
          for (int column = 0; column < 8; column++)
          {
            // Start with the current most significant bit. The next bit will be left shifted in from the right.
            if ((sprite & 0x80) != 0)
            {
              // Get the current x position and wrap around if needed.
              var xPosition = (_processor.GetVRegister(x) + column) % 64;
              var location = startingLine * 64 + xPosition;

              // Collision detection: If the target pixel is already set then set the collision detection flag in register VF.
              if (_window.IsPixelOn(location))
              {
                _processor.SetVRegister(0xF, 1);
              }

              // Enable or disable the pixel (XOR operation).
              _window.TogglePixel(location);
            }

            // Shift the next bit in from the right.
            sprite <<= 0x1;
          }
        }
      });
    }

    public Instruction InstructionFX1E(byte x)
    {
      return new Instruction("FX1E", "Set I = I + Vx", () =>
      {
        var result = _processor.GetIndexRegister() + _processor.GetVRegister(x);
        _processor.SetIndexRegister((ushort) result);
      });
    }

    public Instruction InstructionFX65(byte x)
    {
      return new Instruction("FX65", "Read registers V0 through Vx from memory starting at location I", () =>
      {
        for (int index = 0; index <= x; index++)
        {
          _processor.SetVRegister(index, _memory.Read(_processor.GetIndexRegister() + index));
        }
      });
    }
  }
}
