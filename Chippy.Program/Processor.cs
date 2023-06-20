namespace Chippy.Program
{
  internal class Processor
  {
    public byte[] VRegisters { get; private set; }
    public ushort IndexRegister { get; private set; }

    private ushort _programCounter;
    private byte _delayTimer;
    private byte _soundTimer;

    private readonly Memory _memory;
    private readonly Window _window;

    public Processor(Memory memory, Window window, byte[] vRegisters)
    {
      _memory = memory;
      _window = window;
      VRegisters = vRegisters;

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

      for (int index = 0; index < VRegisters.Length; index++)
      {
        VRegisters[index] = 0;
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
        case 0x0000:
          return InstructionFactory.IgnoreInstruction();
        case 0x1000:
          return InstructionFactory.JumpInstruction(NNN, this);
        case 0x6000:
          return InstructionFactory.SetVXInstruction(X, NN, this);
        case 0x7000:
          return InstructionFactory.AddToVXInstruction(X, NN, this);
        case 0xA000:
          return InstructionFactory.SetIInstruction(NNN, this);
        case 0xD000:
          return InstructionFactory.DrawInstruction(X, Y, N, this, _memory, _window);
        default:
          throw new NotSupportedException();
      }
    }

    public void SetProgramCounter(ushort data)
    {
      _programCounter = data;
    }

    public void SetVRegister(int index, byte value)
    {
      VRegisters[index] = value;
    }

    public void AddToVRegister(int index, byte value)
    {
      VRegisters[index] += value;
    }

    public void SetIRegister(ushort value)
    {
      IndexRegister = value;
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
