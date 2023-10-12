using System.Text.Json;

namespace MarvelSnap;

public class Elektra : CharacterCard
{
	public Elektra(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}
	
	public Elektra() 
	{
		
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
			
			Player opponent = controller.GetOpponent(player);
			Dictionary<Player, List<CharacterCard>> arenaCards = controller.GetArenaCardsForEachPlayer();
			
			List<CharacterCard> targetCards = new();
			Random random = new();

			foreach (var card in arenaCards[opponent]) 
			{
				if (card.IsRevealed && card.BaseEnergyCost == 1) 
				{
					targetCards.Add(card);
				}
			}
			
			int target = random.Next(0, targetCards.Count - 1);
			controller.DestroyCard(opponent, targetCards[target]);
			controller.NotifyCardDestroyed(player, this, targetCards[target]);
		}
	}
	
	public override Elektra? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Elektra? card = JsonSerializer.Deserialize<Elektra>(json);
		return card;
	}
}
