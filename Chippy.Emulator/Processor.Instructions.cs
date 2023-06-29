namespace Chippy.Emulator;

public sealed partial class Processor
{
  private void Instruction0NNN(ushort _) { }

  private void Instruction00E0()
  {
    _display.Clear();
  }

  private void Instruction00EE()
  {
    _programCounter = _memory.PopStack();
  }

  private void Instruction1NNN(ushort nnn)
  {
    _programCounter = nnn;
  }

  private void Instruction2NNN(ushort nnn)
  {
    _memory.PushStack(_programCounter);
    _programCounter = nnn;
  }

  private void Instruction3XNN(byte x, byte nn)
  {
    if (_vRegisters[x] == nn)
    {
      _programCounter += 2;
    }
  }

  private void Instruction4XNN(byte x, byte nn)
  {
    if (_vRegisters[x] != nn)
    {
      _programCounter += 2;
    }
  }

  private void Instruction5XY0(byte x, byte y)
  {
    if (_vRegisters[x] == _vRegisters[y])
    {
      _programCounter += 2;
    }
  }

  private void Instruction6XNN(byte x, byte nn)
  {
    _vRegisters[x] = nn;
  }

  private void Instruction7XNN(byte x, byte nn)
  {
    var result = (byte)(_vRegisters[x] + nn);
    _vRegisters[x] = result;
  }

  private void Instruction8XY0(byte x, byte y)
  {
    _vRegisters[x] = _vRegisters[y];
  }

  private void Instruction8XY1(byte x, byte y)
  {
    _vRegisters[x] = (byte)(_vRegisters[x] | _vRegisters[y]);
    _vRegisters[0xF] = 0x0;
  }

  private void Instruction8XY2(byte x, byte y)
  {
    var result = (byte)(_vRegisters[x] & _vRegisters[y]);
    _vRegisters[x] = result;
    _vRegisters[0xF] = 0x0;
  }

  private void Instruction8XY3(byte x, byte y)
  {
    var result = (byte)(_vRegisters[x] ^ _vRegisters[y]);
    _vRegisters[x] = result;
    _vRegisters[0xF] = 0x0;
  }

  private void Instruction8XY4(byte x, byte y)
  {
    var result = _vRegisters[x] + _vRegisters[y];
      
    _vRegisters[x] = (byte)result;
    _vRegisters[0xF] = result > byte.MaxValue ? (byte)1 : (byte)0;
  }

  private void Instruction8XY5(byte x, byte y)
  {
    var xRegisterValue = _vRegisters[x];
    var yRegisterValue = _vRegisters[y];
    var result = (byte)(xRegisterValue - yRegisterValue);
    
    _vRegisters[x] = result;
    _vRegisters[0xF] = xRegisterValue > yRegisterValue ? (byte)1 : (byte)0;
  }

  private void Instruction8XY6(byte x, byte y)
  {
    var xRegisterValue = _vRegisters[x];
    var yRegisterValue = _vRegisters[y];

    _vRegisters[x] = (byte)(yRegisterValue >> 0x1);
    _vRegisters[0xF] = (byte)(xRegisterValue & 0x1);
  }

  private void Instruction8XY7(byte x, byte y)
  {
    var difference = _vRegisters[y] - _vRegisters[x];
    
    _vRegisters[x] = (byte)(difference & 0xFF);
    _vRegisters[0xF] = (byte)(difference > 0 ? 1 : 0);
  }

  private void Instruction8XYE(byte x, byte y)
  {
    var xRegisterValue = _vRegisters[x];
    var yRegisterValue = _vRegisters[y];

    _vRegisters[x] = (byte)(yRegisterValue << 0x1);
    _vRegisters[0xF] = (byte)((xRegisterValue & 0x80) >> 0x7);
  }

  private void Instruction9XY0(byte x, byte y)
  {
    if (_vRegisters[x] != _vRegisters[y])
    {
      _programCounter += 2;
    }
  }

  private void InstructionANNN(ushort nnn)
  {
    _index = nnn;
  }

  private void InstructionBNNN(ushort nnn)
  {
    var jumpLocation = (ushort)(_vRegisters[0x0] + nnn);
    _programCounter = jumpLocation;
  }

  private void InstructionCXNN(byte x, byte nn)
  {
    var randomByte = (byte)new Random().Next(255);
    _vRegisters[x] = (byte)(randomByte & nn);
  }

  private void InstructionDXYN(byte x, byte y, byte n)
  {
    for (var line = 0; line < n; line++)
    {
      // starting line = Y + current line. If y is larger than the total width of the screen then wrap around (this is the modulo operation).
      var startingLine = (_vRegisters[y] + line) % Display.NativeHeight;

      // The current sprite being drawn, each line is a new sprite.
      var sprite = _memory.Read(_index + line);

      // Each bit in the sprite is a pixel on or off.
      for (var column = 0; column < 8; column++)
      {
        // Start with the current most significant bit. The next bit will be left shifted in from the right.
        if ((sprite & 0x80) != 0)
        {
          // Get the current x position and wrap around if needed.
          var xPosition = (_vRegisters[x] + column) % 64;
          var location = startingLine * 64 + xPosition;

          // Collision detection: If the target pixel is already set then set the collision detection flag in register VF.
          if (_display.IsPixelOn(location))
          {
            _vRegisters[0xF] = 1;
          }

          // Enable or disable the pixel (XOR operation).
          _display.TogglePixel(location);
        }

        // Shift the next bit in from the right.
        sprite <<= 0x1;
      }
    }
  }

  private void InstructionEX9E(byte x)
  {
    if (_keypad.IsKeyPressed(_vRegisters[x]))
    {
      _programCounter += 2;
    }
  }

  private void InstructionEXA1(byte x)
  {
    if (!_keypad.IsKeyPressed(_vRegisters[x]))
    {
      _programCounter += 2;
    }
  }

  private void InstructionFX07(byte x)
  {
    _vRegisters[x] = _delayTimer.Value;
  }

  private void InstructionFX0A(byte x)
  {
    while (true)
    {
      if (_keypad.IsAnyKeyPressed())
      {
        var pressedKey = _keypad.GetPressedKeyCode();

        if (pressedKey == null)
        {
          continue;
        }
        
        _vRegisters[x] = pressedKey.Value;
        
        break;
      }

      OnWaitingForKeyPress(EventArgs.Empty);
    }
  }

  private void InstructionFX15(byte x)
  {
    _delayTimer.Value = _vRegisters[x];
  }

  private void InstructionFX18(byte x)
  {
    _soundTimer.Value = _vRegisters[x];
  }

  private void InstructionFX29(byte x)
  {
    _index = (ushort)(_vRegisters[x] * 5);
  }

  private void InstructionFX1E(byte x)
  {
    var result = _index + _vRegisters[x];
    _index = (ushort)result;
  }

  private void InstructionFX33(byte x)
  {
    var number = _vRegisters[x];

    _memory.Write(_index, (byte)(number / 100));
    _memory.Write(_index + 1, (byte)((number / 10) % 10));
    _memory.Write(_index + 2, (byte)((number % 100) % 10));
  }

  private void InstructionFX55(byte x)
  {
    for (var index = 0; index <= x; index++)
    {
      _memory.Write(_index + index, _vRegisters[index]);
    }

    _index = (ushort)(_index + 1);
  }

  private void InstructionFX65(byte x)
  {
    for (var index = 0; index <= x; index++)
    {
      _vRegisters[index] = _memory.Read(_index + index);
    }

    _index = (ushort)(_index + 1);
  }
}