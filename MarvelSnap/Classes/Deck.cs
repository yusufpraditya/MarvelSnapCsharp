using System.Security.Cryptography;

namespace MarvelSnap;

public class Deck
{
	private List<CharacterCard> _cards = new();
	private CharacterCard? _cardToDraw;
	public const int MaxCardCount = 12;
	public int Id { get; set; }
	public string? Name { get; set; }

	public Deck(int id, string? name)
	{
		Id = id;
		Name = name;
	}

	public bool Add(CharacterCard? card)
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
		if (_cards.Count <= 0) return false;
		_cards.Remove(card);
		return true;
	}

	public bool Contains(CharacterCard card)
	{
		return _cards.Contains(card);
	}

	// https://code-maze.com/csharp-randomize-list/
	public void Shuffle()
	{
		for (int i = _cards.Count - 1; i > 0; i--)
		{
			int index = RandomNumberGenerator.GetInt32(i + 1);
			CharacterCard randomCard = _cards[index];
			_cards[index] = _cards[i];
			_cards[i] = randomCard;
		}
	}

	public CharacterCard? Draw()
	{
		if (_cards.Count <= 0) return null;
		_cardToDraw = _cards[_cards.Count - 1];
		_cards.RemoveAt(_cards.Count - 1);
		return _cardToDraw;
	}

	public bool IsFull()
	{
		return _cards.Count == MaxCardCount;
	}
}