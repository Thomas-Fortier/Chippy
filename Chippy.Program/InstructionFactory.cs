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

    public static Instruction SetVRegtisterInstruction(byte x, byte nn, Processor processor)
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
        processor.SetIndexRegister(nnn);
      });
    }

    public static Instruction DrawInstruction(byte x, byte y, byte n, Processor processor, Memory memory, Window window)
    {
      return new Instruction("Draw", "DXYN", () =>
      {
        for (int line = 0; line < n; line++)
        {
          // starting line = Y + current line. If y is larger than the total width of the screen then wrap around (this is the modulo operation).
          var startingLine = (processor.GetVRegister(y) + line) % window.NativeHeight;

          // The current sprite being drawn, each line is a new sprite.
          byte sprite = memory.Read(processor.GetIndexRegister() + line);

          // Each bit in the sprite is a pixel on or off.
          for (int column = 0; column < 8; column++)
          {
            // Start with the current most significant bit. The next bit will be left shifted in from the right.
            if ((sprite & 0x80) != 0)
            {
              // Get the current x position and wrap around if needed.
              var xPosition = (processor.GetVRegister(x) + column) % 64;
              var location = startingLine * 64 + xPosition;

              // Collision detection: If the target pixel is already set then set the collision detection flag in register VF.
              if (window.IsPixelOn(location))
              {
                processor.SetVRegister(0xF, 1);
              }

              // Enable or disable the pixel (XOR operation).
              window.TogglePixel(location);
            }

            // Shift the next bit in from the right.
            sprite <<= 0x1;
          }
        }
      });
    }

    public static Instruction ReturnInstruction(Processor processor, Memory memory)
    {
      return new Instruction("Return", "00EE", () =>
      {
        processor.SetProgramCounter(memory.PopStack());
      });
    }

    public static Instruction CallInstruction(ushort nnn, Processor processor, Memory memory)
    {
      return new Instruction("Call", "2NNN", () =>
      {
        memory.PushStack(processor.GetProgramCounter());
        processor.SetProgramCounter(nnn);
      });
    }

    public static Instruction SkipIfEqualToByteInstruction(byte x, byte nn, Processor processor)
    {
      return new Instruction("Skip Next If Byte Equal", "3XNN", () =>
      {
        if (x == nn)
        {
          processor.SetProgramCounter((ushort)(processor.GetProgramCounter() + 2));
        }
      });
    }

    public static Instruction SkipIfNotEqualToByteInstruction(byte x, byte nn, Processor processor)
    {
      return new Instruction("Skip Next If Byte Not Equal", "4XNN", () =>
      {
        if (x != nn)
        {
          processor.SetProgramCounter((ushort)(processor.GetProgramCounter() + 2));
        }
      });
    }

    public static Instruction SkipIfEqualToVInstruction(byte x, byte y, Processor processor)
    {
      return new Instruction("Skip Next If V Register Equal", "5XY0", () =>
      {
        if (x == y)
        {
          processor.SetProgramCounter((ushort)(processor.GetProgramCounter() + 2));
        }
      });
    }

    public static Instruction SetVXRegisterToVYRegisterInstruction(byte x, byte y, Processor processor)
    {
      return new Instruction("Set VX to VY", "8XY0", () =>
      {
        processor.SetVRegister(x, y);
      });
    }

    public static Instruction OrInstruction(byte xLocation, byte y, Processor processor)
    {
      return new Instruction("OR", "8XY1", () =>
      {
        processor.SetVRegister(xLocation, (byte)(processor.GetVRegister(xLocation) | y));
      });
    }

    public static Instruction AndInstruction(byte x, byte y, Processor processor)
    {
      return new Instruction("AND", "8XY2", () =>
      {
        processor.SetVRegister(x, (byte)(processor.GetVRegister(x) & processor.GetVRegister(y)));
      });
    }

    public static Instruction XorInstruction(byte xLocation, byte y, Processor processor)
    {
      return new Instruction("XOR", "8XY3", () =>
      {
        processor.SetVRegister(xLocation, (byte)(processor.GetVRegister(xLocation) ^ y));
      });
    }

    public static Instruction AddInstruction(byte x, byte y, Processor processor)
    {
      return new Instruction("ADD", "8XY4", () =>
      {
        int result = processor.GetVRegister(x) + processor.GetVRegister(y);

        if (result > byte.MaxValue)
        {
          processor.SetVRegister(0xF, 1);
        }
        else
        {
          processor.SetVRegister(0xF, 0);
        }

        processor.SetVRegister(x, (byte)(result & 0xFF));
      });
    }

    public static Instruction SubtractInstruction(byte xLocation, byte y, Processor processor)
    {
      return new Instruction("SUB", "8XY5", () =>
      {
        if (processor.GetVRegister(xLocation) > y)
        {
          processor.SetVRegister(0xF, 1);
        }
        else
        {
          processor.SetVRegister(0xF, 0);
        }

        processor.SetVRegister(xLocation, (byte)(processor.GetVRegister(xLocation) - y));
      });
    }

    public static Instruction ShrInstruction(byte x, Processor processor)
    {
      return new Instruction("SHR", "8XY6", () =>
      {
        var leastSignificantBit = processor.GetVRegister(x) & 0x0000000F;

        if (leastSignificantBit == 1)
        {
          processor.SetVRegister(0xF, 1);
        }
        else
        {
          processor.SetVRegister(0xF, 0);
        }

        processor.SetVRegister(x, (byte)(processor.GetVRegister(x) / 2));
      });
    }

    public static Instruction ShlInstruction(byte x, Processor processor)
    {
      return new Instruction("SHL", "8XYE", () =>
      {
        var mostSignificantBit = processor.GetVRegister(x) & 0xF0000000;

        if (mostSignificantBit == 1)
        {
          processor.SetVRegister(0xF, 1);
        }
        else
        {
          processor.SetVRegister(0xF, 0);
        }

        processor.SetVRegister(x, (byte)(processor.GetVRegister(x) * 2));
      });
    }

    public static Instruction SkipIfNotEqualToVInstruction(byte x, byte y, Processor processor)
    {
      return new Instruction("Skip If V Not Equal", "9XY0", () =>
      {
        if (x != y)
        {
          processor.SetProgramCounter((ushort)(processor.GetProgramCounter() + 2));
        }
      });
    }

    public static Instruction JumpWithOffsetInstruction(ushort nnn, Processor processor)
    {
      return new Instruction("Jump With Offset", "BNNN", () =>
      {
        processor.SetProgramCounter((ushort)(nnn + processor.GetVRegister(0x0)));
      });
    }

    public static Instruction JumpWithOffsetInstruction(byte xLocation, byte nn, Processor processor)
    {
      return new Instruction("Jump With Offset", "CXNN", () =>
      {
        byte randomByte = (byte)new Random().Next(255);
        byte result = (byte)(randomByte & nn);

        processor.SetVRegister(xLocation, result);
      });
    }

    public static Instruction WaitForKey(byte x, Processor processor)
    {
      return new Instruction("Wait for Key", "FX0A", () =>
      {
        processor.SetVRegister(x, 0x5);
      });
    }

    public static Instruction StoreDelayTimerInstruction(byte x, Processor processor)
    {
      return new Instruction("Store Delay", "FX07", () =>
      {
        processor.SetVRegister(x, processor.GetDelayTimer());
      });
    }

    public static Instruction SetDelayTimerInstruction(byte x, Processor processor)
    {
      return new Instruction("Set Delay", "FX15", () =>
      {
        processor.SetDelayTimer(processor.GetVRegister(x));
      });
    }

    public static Instruction AddIndexAndVRegisters(byte x, Processor processor)
    {
      return new Instruction("Add Index and V Registers", "FX1E", () =>
      {
        processor.SetIndexRegister((ushort) (processor.GetIndexRegister() + processor.GetVRegister(x)));
      });
    }

    public static Instruction SetIToDigitSprite(byte x, Processor processor, Memory memory)
    {
      return new Instruction("Set I to Digit Sprite", "FX29", () =>
      {
        processor.SetIndexRegister((ushort) (processor.GetVRegister(x) * 5));
      });
    }

    public static Instruction StoreBcd(byte x, Processor processor, Memory memory)
    {
      return new Instruction("Store BCD", "FX33", () =>
      {
        var value = processor.GetVRegister(x);
        memory.Write(processor.GetIndexRegister(), (byte) (value / 100));
        memory.Write(processor.GetIndexRegister() + 1, (byte)((value / 10) % 10));
        memory.Write(processor.GetIndexRegister() + 2, (byte)((value % 100) % 10));
      });
    }

    public static Instruction CopyVRegistersInstruction(byte x, Processor processor, Memory memory)
    {
      return new Instruction("Copy V Registers", "FX55", () =>
      {
        for (int index = processor.GetIndexRegister(); index <= x; index++)
        {
          memory.Write(processor.GetIndexRegister() + index, processor.GetVRegister(index));
        }
      });
    }

    public static Instruction ReadAllVRegisters(byte x, Processor processor, Memory memory)
    {
      return new Instruction("Read All Registers", "FX65", () =>
      {
        for (int index = 0; index <= x; index++)
        {
          processor.SetVRegister(index, memory.Read(processor.GetIndexRegister() + index));
        }
      });
    }
  }
}
