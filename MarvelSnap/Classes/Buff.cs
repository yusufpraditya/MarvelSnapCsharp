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
		if (Operation == BuffOperation.Add) 
		{
			return Value + value;
		} 
		else 
		{
			return Value * value;
		}
	}

    public int CompareTo(Buff? other)
    {
        if (other == null)
            return 1;
        else
            return (int)Operation.CompareTo((int)other.Operation);
    }
}