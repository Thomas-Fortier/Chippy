namespace Chippy.Program
{
  internal class Emulator
  {
    private Processor _processor;
    private Window _window;
    
    public Emulator(Processor processor, Window window, byte[] data)
    {
      _processor = processor;
      _processor.LoadRom(data);

      _window = window;
      _window.OnFrameRendered += new Window.WindowEvent(ExecuteCycle);
    }

    public void ExecuteCycle()
    {
      var firstNibble = _processor.Fetch();
      var secondNibble = _processor.Fetch();
      var instruction = _processor.Decode(firstNibble, secondNibble);

      _processor.Execute(instruction);

      Thread.Sleep(2);
    }

    public void Start()
    {
      _window.Run();
    }
  }
}
