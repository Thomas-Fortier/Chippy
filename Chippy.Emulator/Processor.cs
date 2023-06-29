namespace Chippy.Emulator
{
  public sealed partial class Processor
  {
    public event EventHandler? WaitingForKeyPress;

    private ushort _programCounter;
    private ushort _index;
    private readonly TimerRegister _delayTimer;
    private readonly TimerRegister _soundTimer;
    private readonly byte[] _vRegisters;

    private readonly Memory _memory;
    private readonly Display _display;
    private readonly Keypad _keypad;

    public Processor(Memory memory, Display display, Keypad keypad, byte[] vRegisters)
    {
      _memory = memory;
      _display = display;
      _keypad = keypad;
      _vRegisters = vRegisters;
      
      const int DELAY = 17; // 60 Hz
      _delayTimer = new TimerRegister(DELAY);
      _soundTimer = new TimerRegister(DELAY);

      Reset();
      MakeSound();
    }

    // TODO: Move
    private void MakeSound()
    {
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          if (_soundTimer.Value == 0)
            continue;
          
          Console.Beep();
          _soundTimer.Value = 0;
        }
        
        // ReSharper disable once FunctionNeverReturns
      });
    }

    private void Reset()
    {
      _memory.Reset();
      _display.Clear();

      _programCounter = 0x200;
      _index = 0;
      _delayTimer.Value = 0;
      _soundTimer.Value = 0;

      _delayTimer.Start();
      _soundTimer.Start();
      
      for (var index = 0; index < _vRegisters.Length; index++)
      {
        _vRegisters[index] = 0;
      }
    }
    
    private byte Fetch()
    {
      var result = _memory.Read(_programCounter);
      _programCounter++;

      return result;
    }

    public void ExecuteCycle()
    {
      var firstNibble = Fetch();
      var secondNibble = Fetch();
      
      ExecuteInstruction(firstNibble, secondNibble);

      Thread.Sleep(2); // TODO: Move to emulator ?
    }
    
    private void ExecuteInstruction(byte firstInstruction, byte secondInstruction)
    {
      var opcode = (ushort)(firstInstruction << 8 | secondInstruction);

      var nnn = (ushort)(opcode & 0x0FFF);
      var nn = (byte)(opcode & 0x00FF);
      var n = (byte)(opcode & 0x000F);
      var x = (byte)((opcode & 0x0F00) >> 8);
      var y = (byte)((opcode & 0x00F0) >> 4);

      switch (opcode & 0xF000)
      {
        case 0x0000 when (opcode & 0x00FF) == 0x00:
          Instruction0NNN(nnn);
          break;
        case 0x0000 when (opcode & 0x00FF) == 0xE0:
          Instruction00E0();
          break;
        case 0x0000 when (opcode & 0x00FF) == 0xEE:
          Instruction00EE();
          break;
        case 0x1000:
          Instruction1NNN(nnn);
          break;
        case 0x2000:
          Instruction2NNN(nnn);
          break;
        case 0x3000:
          Instruction3XNN(x, nn);
          break;
        case 0x4000:
          Instruction4XNN(x, nn);
          break;
        case 0x5000:
          Instruction5XY0(x, y);
          break;
        case 0x6000:
          Instruction6XNN(x, nn);
          break;
        case 0x7000:
          Instruction7XNN(x, nn);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x0:
          Instruction8XY0(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x1:
          Instruction8XY1(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x2:
          Instruction8XY2(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x3:
          Instruction8XY3(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x4:
          Instruction8XY4(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x5:
          Instruction8XY5(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x6:
          Instruction8XY6(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0x7:
          Instruction8XY7(x, y);
          break;
        case 0x8000 when (opcode & 0x000F) == 0xE:
          Instruction8XYE(x, y);
          break;
        case 0x9000:
          Instruction9XY0(x, y);
          break;
        case 0xA000:
          InstructionANNN(nnn);
          break;
        case 0xB000:
          InstructionBNNN(nnn);
          break;
        case 0xC000:
          InstructionCXNN(x, nn);
          break;
        case 0xD000:
          InstructionDXYN(x, y, n);
          break;
        case 0xE000 when (opcode & 0x00FF) == 0x9E:
          InstructionEX9E(x);
          break;
        case 0xE000 when (opcode & 0x00FF) == 0xA1:
          InstructionEXA1(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x1E:
          InstructionFX1E(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x07:
          InstructionFX07(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x0A:
          InstructionFX0A(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x15:
          InstructionFX15(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x18:
          InstructionFX18(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x29:
          InstructionFX29(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x33:
          InstructionFX33(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x55:
          InstructionFX55(x);
          break;
        case 0xF000 when (opcode & 0x00FF) == 0x65:
          InstructionFX65(x);
          break;
        default:
          throw new NotSupportedException();
      }
    }

    private void OnWaitingForKeyPress(EventArgs e)
    {
      WaitingForKeyPress?.Invoke(this, e);
    }
  }
}
