namespace MarvelSnap;

public class Deck
{
	private List<CharacterCard> _cards = new();
	public const int MaxCardCount = 20;
	public int Id { get; set; }
	public string Name { get; set; }
	
	public Deck(int id, string name) 
	{
		Id = id;
		Name = name;
	}
	
	public bool Add(CharacterCard card) 
	{
		if (card != null && _cards.Count < MaxCardCount) 
		{
			_cards.Add(card);
			return true;
		}
		return false;
	}
	
	public bool Remove(CharacterCard card) 
	{
		if (_cards.Count > 0) 
		{
			_cards.Remove(card);
			return true;
		}
		else 
		{
			return false;
		}
	}
	
	public bool Contains(CharacterCard card) 
	{
		if (_cards.Contains(card)) return true;
		else return false;
	}
	
	// https://code-maze.com/csharp-randomize-list/
	public void Shuffle() 
	{
		Random random = new();
		for (int i = _cards.Count - 1; i > 0; i--) 
		{
			int randomIndex = random.Next(i + 1);
			CharacterCard randomCard = _cards[randomIndex];
			_cards[randomIndex] = _cards[i];
			_cards[i] = randomCard;
		}
	}
	
	public Card? Draw() 
	{
		Card cardToDraw;
		if (!IsEmpty()) 
		{
			cardToDraw = _cards[0];	
		}
		// if (_cards.Count > 0) 
		// {
		// 	return cardToDraw;
		// }
		// else 
		// {
		// 	return null;
		// }
	}
	
	private bool IsEmpty() 
	{
		if (_cards.Count <= 0) return true;
		else return false;
	}
}