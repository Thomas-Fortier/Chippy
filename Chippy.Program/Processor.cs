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

    public Processor(Memory memory, Window window, byte[] vRegisters)
    {
      _memory = memory;
      _window = window;
      _vRegisters = vRegisters;

      Reset();
    }

    public void Reset()
    {
      _memory.Clear();
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

      ushort NNN = (ushort)(opcode & 0x0FFF);
      byte NN = (byte)(opcode & 0x00FF);
      byte N = (byte)(opcode & 0x000F);
      byte X = (byte)((opcode & 0x0F00) >> 8);
      byte Y = (byte)((opcode & 0x00F0) >> 4);

      switch (opcode & 0xF000)
      {
        case 0x0000 when opcode == 0x00E0:
          return InstructionFactory.ClearScreenInstruction(_window);
        case 0x0000 when opcode == 0x00EE:
          return InstructionFactory.ReturnInstruction(this, _memory);
        case 0x0000:
          return InstructionFactory.IgnoreInstruction();
        case 0x1000:
          return InstructionFactory.JumpInstruction(NNN, this);
        case 0x2000:
          return InstructionFactory.CallInstruction(NNN, this, _memory);
        case 0x3000:
          return InstructionFactory.SkipIfEqualToByteInstruction(X, NN, this);
        case 0x4000:
          return InstructionFactory.SkipIfNotEqualToByteInstruction(X, NN, this);
        case 0x5000:
          return InstructionFactory.SkipIfEqualToVInstruction(X, Y, this);
        case 0x6000:
          return InstructionFactory.SetVRegtisterInstruction(X, NN, this);
        case 0x7000:
          return InstructionFactory.AddToVXInstruction(X, NN, this);
        case 0x8000 when (opcode & 0x000F) == 0:
          return InstructionFactory.SetVXRegisterToVYRegisterInstruction(X, Y, this);
        case 0x8000 when (opcode & 0x000F) == 2:
          return InstructionFactory.AndInstruction(X, Y, this);
        case 0x8000 when (opcode & 0x000F) == 4:
          return InstructionFactory.AddInstruction(X, Y, this);
        case 0x8000 when (opcode & 0x000F) == 6:
          return InstructionFactory.ShrInstruction(X, this);
        case 0x8000 when (opcode & 0x000F) == 0xE:
          return InstructionFactory.ShlInstruction(X, this);
        case 0xA000:
          return InstructionFactory.SetIInstruction(NNN, this);
        case 0xC000:
          return InstructionFactory.JumpWithOffsetInstruction(X, NN, this);
        case 0xD000:
          return InstructionFactory.DrawInstruction(X, Y, N, this, _memory, _window);
        case 0xF000 when (opcode & 0x00FF) == 0x7:
          return InstructionFactory.StoreDelayTimerInstruction(X, this);
        case 0xF000 when (opcode & 0x00FF) == 0xA:
          return InstructionFactory.WaitForKey(X, this);
        case 0xF000 when (opcode & 0x00FF) == 0x15:
          return InstructionFactory.SetDelayTimerInstruction(X, this);
        case 0xF000 when (opcode & 0x00FF) == 0x1E:
          return InstructionFactory.AddIndexAndVRegisters(X, this);
        case 0xF000 when (opcode & 0x00FF) == 0x29:
          return InstructionFactory.SetIToDigitSprite(X, this, _memory);
        case 0xF000 when (opcode & 0x00FF) == 0x33:
          return InstructionFactory.StoreBcd(X, this, _memory);
        case 0xF000 when (opcode & 0x00FF) == 0x55:
          return InstructionFactory.CopyVRegistersInstruction(X, this, _memory);
        case 0xF000 when (opcode & 0x00FF) == 0x65:
          return InstructionFactory.ReadAllVRegisters(X, this, _memory);
        default:
          throw new NotSupportedException();
      }
    }

    public void SetProgramCounter(ushort data)
    {
      _programCounter = data;
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
