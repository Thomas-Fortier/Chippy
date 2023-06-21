namespace Chippy.Program
{
  internal class Processor
  {
    private byte[] _vRegisters;
    private ushort IndexRegister;

    private ushort _programCounter;
    private byte _delayTimer;
    private byte _soundTimer;

    private readonly Memory _memory;
    private readonly Window _window;
    private readonly InstructionFactory _instructionFactory;

    public Processor(Memory memory, Window window, byte[] vRegisters)
    {
      _memory = memory;
      _window = window;
      _vRegisters = vRegisters;
      _instructionFactory = new InstructionFactory(this, memory, window);

      Reset();
    }

    public void Reset()
    {
      _memory.Reset();
      _window.Clear();

      _programCounter = 0x200;
      IndexRegister = 0;
      _delayTimer = 0;
      _soundTimer = 0;

      for (int index = 0; index < _vRegisters.Length; index++)
      {
        _vRegisters[index] = 0;
      }
    }

    public byte Fetch()
    {
      var result = _memory.Read(_programCounter);
      _programCounter++;

      return result;
    }

    public Instruction Decode(byte firstInstruction, byte secondInstruction)
    {
      var opcode = (ushort)(firstInstruction << 8 | secondInstruction);

      ushort nnn = (ushort)(opcode & 0x0FFF);
      byte nn = (byte)(opcode & 0x00FF);
      byte n = (byte)(opcode & 0x000F);
      byte x = (byte)((opcode & 0x0F00) >> 8);
      byte y = (byte)((opcode & 0x00F0) >> 4);

      switch (opcode & 0xF000)
      {
        case 0x0000 when (opcode & 0x00FF) == 0x00:
          return _instructionFactory.Instruction0NNN(nnn);
        case 0x0000 when (opcode & 0x00FF) == 0xE0:
          return _instructionFactory.Instruction00E0();
        case 0x0000 when (opcode & 0x00FF) == 0xEE:
          return _instructionFactory.Instruction00EE();
        case 0x1000:
          return _instructionFactory.Instruction1NNN(nnn);
        case 0x2000:
          return _instructionFactory.Instruction2NNN(nnn);
        case 0x3000:
          return _instructionFactory.Instruction3XNN(x, nn);
        case 0x4000:
          return _instructionFactory.Instruction4XNN(x, nn);
        case 0x5000:
          return _instructionFactory.Instruction5XY0(x, y);
        case 0x6000:
          return _instructionFactory.Instruction6XNN(x, nn);
        case 0x7000:
          return _instructionFactory.Instruction7XNN(x, nn);
        case 0x8000 when (opcode & 0x000F) == 0x0:
          return _instructionFactory.Instruction8XY0(x, y);
        case 0x8000 when (opcode & 0x000F) == 0x2:
          return _instructionFactory.Instruction8XY2(x, y);
        case 0x8000 when (opcode & 0x000F) == 0x4:
          return _instructionFactory.Instruction8XY4(x, y);
        case 0x8000 when (opcode & 0x000F) == 0x5:
          return _instructionFactory.Instruction8XY5(x, y);
        case 0x8000 when (opcode & 0x000F) == 0x6:
          return _instructionFactory.Instruction8XY6(x, y);
        case 0x8000 when (opcode & 0x000F) == 0xE:
          return _instructionFactory.Instruction8XYE(x, y);
        case 0x9000:
          return _instructionFactory.Instruction9XY0(x, y);
        case 0xA000:
          return _instructionFactory.InstructionANNN(nnn);
        case 0xC000:
          return _instructionFactory.InstructionCXNN(x, nn);
        case 0xD000:
          return _instructionFactory.InstructionDXYN(x, y, n);
        case 0xE000 when (opcode & 0x00FF) == 0x9E:
          return _instructionFactory.InstructionEX9E(x);
        case 0xE000 when (opcode & 0x00FF) == 0xA1:
          return _instructionFactory.InstructionEXA1(x);
        case 0xF000 when (opcode & 0x00FF) == 0x1E:
          return _instructionFactory.InstructionFX1E(x);
        case 0xF000 when (opcode & 0x00FF) == 0x07:
          return _instructionFactory.InstructionFX07(x);
        case 0xF000 when (opcode & 0x00FF) == 0x0A:
          return _instructionFactory.InstructionFX0A(x);
        case 0xF000 when (opcode & 0x00FF) == 0x15:
          return _instructionFactory.InstructionFX15(x);
        case 0xF000 when (opcode & 0x00FF) == 0x29:
          return _instructionFactory.InstructionFX29(x);
        case 0xF000 when (opcode & 0x00FF) == 0x33:
          return _instructionFactory.InstructionFX33(x);
        case 0xF000 when (opcode & 0x00FF) == 0x55:
          return _instructionFactory.InstructionFX55(x);
        case 0xF000 when (opcode & 0x00FF) == 0x65:
          return _instructionFactory.InstructionFX65(x);
        default:
          throw new NotSupportedException();
      }
    }

    public void IncrementProgramCounter(ushort amount)
    {
      _programCounter += amount;
    }

    public void SetProgramCounter(ushort value)
    {
      _programCounter = value;
    }

    public ushort GetProgramCounter()
    {
      return _programCounter;
    }

    public byte GetVRegister(int index)
    {
      return _vRegisters[index];
    }

    public void SetVRegister(int index, byte value)
    {
      _vRegisters[index] = value;
    }

    public void AddToVRegister(int index, byte value)
    {
      _vRegisters[index] += value;
    }

    public void SetIndexRegister(ushort value)
    {
      IndexRegister = value;
    }

    public ushort GetIndexRegister()
    {
      return IndexRegister;
    }

    public void SetDelayTimer(byte delay)
    {
      _delayTimer = delay;
    }

    public byte GetDelayTimer()
    {
      return _delayTimer;
    }

    public void Execute(Instruction instruction)
    {
      instruction.Execute();
    }

    public void LoadRom(byte[] rom)
    {
      _memory.LoadRom(rom, _programCounter);
    }
  }
}
