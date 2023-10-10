namespace MarvelSnap;

public class Buff : IComparable<Buff>
{
	public int Id { get; set; }
	public int Value { get; set; }
	public BuffType Type { get; set; }
	public BuffOperation Operation { get; set; }
	
	public Buff(int id, int value, BuffType type, BuffOperation operation) 
	{
		Id = id;
		Value = value;
		Type = type;
		Operation = operation;
	}
	
	public int Apply(int value)
	{
		return Operation == BuffOperation.Add ? Value + value : Value * value;
	}
	
	public string GetSymbol() 
	{
		return Operation == BuffOperation.Add ? "+" : "x";
	}

	public int CompareTo(Buff? other)
	{
		if (other == null)
			return 1;
		else
			return Operation.CompareTo(other.Operation);
	}
}