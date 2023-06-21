namespace Chippy.Program
{
  internal class Instruction
  {
    private readonly string _description;
    private readonly string _id;
    private readonly Action _logic;

    public Instruction(string id, string description, Action logic)
    {
      _description = description;
      _id = id;
      _logic = logic;
    }

    public void Execute()
    {
      _logic.Invoke();
    }

    public override string ToString()
    {
      return $"{_id}: {_description}";
    }
  }
}
