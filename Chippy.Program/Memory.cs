namespace Chippy.Program
{
  internal class Memory
  {
    private readonly byte[] _data;
    private readonly Stack<ushort> _stack;

    public Memory(byte[] data, Stack<ushort> stack)
    {
      _data = data;
      _stack = stack;
    }

    public byte Read(int location)
    {
      return _data[location];
    }

    public ushort PopStack()
    {
      return _stack.Pop();
    }

    public void PushStack(ushort value)
    {
      _stack.Push(value);
    }

    public void Write(int location, byte value)
    {
      _data[location] = value;
    }

    public void Clear()
    {
      _stack.Clear();

      for (int index = 0; index < _data.Length; index++)
      {
        _data[index] = 0;
      }
    }

    public void LoadRom(byte[] rom, int index)
    {
      rom.CopyTo(_data, index);
    }
  }
}
