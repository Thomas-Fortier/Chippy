namespace Chippy.Program
{
  internal class Instruction
  {
    public string Id { get; }
    public string Description { get; }

    private readonly Action _logic;

    public Instruction(string id, string description, Action logic)
    {
      Description = description;
      Id = id;
      _logic = logic;
    }

    public void Execute()
    {
      _logic.Invoke();
    }

    public override string ToString()
    {
      return $"{Id}: {Description}";
    }
  }
}
