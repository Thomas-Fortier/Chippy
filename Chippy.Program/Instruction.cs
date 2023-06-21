namespace Chippy.Program
{
  internal class Instruction
  {
    private readonly string _name;
    private readonly string _id;
    private readonly Action _logic;

    public Instruction(string name, string id, Action logic)
    {
      _name = name;
      _id = id;
      _logic = logic;
    }

    public void Execute()
    {
      _logic.Invoke();
    }

    public override string ToString()
    {
      return $"{_id}: {_name}";
    }
  }
}
