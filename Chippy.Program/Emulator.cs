namespace Chippy.Program
{
  internal class Emulator
  {
    private Processor _processor;
    
    public Emulator(Processor processor, byte[] data)
    {
      _processor = processor;
      _processor.LoadRom(data);
    }

    public void ExecuteCycle()
    {
      var firstNibble = _processor.Fetch();
      var secondNibble = _processor.Fetch();
      var instruction = _processor.Decode(firstNibble, secondNibble);

      _processor.Execute(instruction);
    }
  }
}
