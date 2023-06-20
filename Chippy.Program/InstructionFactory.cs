namespace Chippy.Program
{
  internal class InstructionFactory
  {
    public static Instruction IgnoreInstruction()
    {
      return new Instruction("Ignore", "0NNN", () => { });
    }

    public static Instruction ClearScreenInstruction(Window window)
    {
      return new Instruction("Clear Screen", "00E0", () =>
      {
        window.Clear();
      });
    }

    public static Instruction JumpInstruction(ushort nnn, Processor processor)
    {
      return new Instruction("Jump", "1NNN", () =>
      {
        processor.SetProgramCounter(nnn);
      });
    }

    public static Instruction SetVXInstruction(byte x, byte nn, Processor processor)
    {
      return new Instruction("Set Register VX", "6XNN", () =>
      {
        processor.SetVRegister(x, nn);
      });
    }

    public static Instruction AddToVXInstruction(byte x, byte nn, Processor processor)
    {
      return new Instruction("Add to Register VX", "7XNN", () =>
      {
        processor.AddToVRegister(x, nn);
      });
    }

    public static Instruction SetIInstruction(ushort nnn, Processor processor)
    {
      return new Instruction("Set Register I", "ANNN", () =>
      {
        processor.SetIRegister(nnn);
      });
    }

    public static Instruction DrawInstruction(byte x, byte y, byte n, Processor processor, Memory memory, Window window)
    {
      return new Instruction("Draw", "DXYN", () =>
      {
        for (int line = 0; line < n; line++)
        {
          // starting line = Y + current line. If y is larger than the total width of the screen then wrap around (this is the modulo operation).
          var startingLine = (processor.VRegisters[y] + line) % window.NativeHeight;

          // The current sprite being drawn, each line is a new sprite.
          byte sprite = memory.Read(processor.IndexRegister + line);

          // Each bit in the sprite is a pixel on or off.
          for (int column = 0; column < 8; column++)
          {
            // Start with the current most significant bit. The next bit will be left shifted in from the right.
            if ((sprite & 0x80) != 0)
            {
              // Get the current x position and wrap around if needed.
              var xPosition = (processor.VRegisters[x] + column) % 64;

              // Collision detection: If the target pixel is already set then set the collision detection flag in register VF.
              if (window.Buffer[startingLine * 64 + xPosition] == 1)
              {
                processor.VRegisters[0xF] = 1;
              }

              // Enable or disable the pixel (XOR operation).
              window.Buffer[startingLine * 64 + xPosition] ^= 1;
            }

            // Shift the next bit in from the right.
            sprite <<= 0x1;
          }
        }
      });
    }
  }
}
