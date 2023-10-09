namespace MarvelSnap;

public class OnslaughtsCitadel : LocationCard
{
	public OnslaughtsCitadel(LocationType id, string name, string description) : base(id, name, description)
	{
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(null, this);
		}
	}
	
	public override void Ongoing(Player? player, MarvelSnapGame controller)
	{
		List<Player> players = controller.GetPlayers();
		Dictionary<Player, List<CharacterCard>> playerCardsInArena = controller.GetArenaCardsForEachPlayer();
		
		foreach (var p in players) 
		{
			foreach (var card in playerCardsInArena[p]) 
			{
				card.Ongoing(p, controller);
			}
		}
	}
}