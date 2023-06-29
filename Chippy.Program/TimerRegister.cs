namespace Chippy.Program;

public class TimerRegister
{
  public byte Value { get; set; }

  private readonly int _delay;

  public TimerRegister(int delay)
  {
    _delay = delay;
  }

  public void Start()
  {
    Task.Factory.StartNew(() =>
    {
      while (true)
      {
        while (Value != 0)
        {
          Thread.Sleep(_delay);
          Value--;
        }
      }
      
      // ReSharper disable once FunctionNeverReturns
    });
  }
}