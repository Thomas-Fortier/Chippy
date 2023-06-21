namespace Chippy.Program
{
  internal class Instruction
  {
    public string Id { get; }

    private readonly string _description;
    private readonly Action _logic;

    public Instruction(string id, string description, Action logic)
    {
      _description = description;
      Id = id;
      _logic = logic;
    }

    public void Execute()
    {
      _logic.Invoke();
    }

    public override string ToString()
    {
      return $"{Id}: {_description}";
    }
  }
}
