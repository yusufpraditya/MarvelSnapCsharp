using System.Text.Json;

namespace MarvelSnap;

public class DreamDimension : LocationCard
{
	private const int _FutureTurn1 = 5;
	private const int _FutureTurn2 = 6;
	private const int _BuffValue = 1;
	private const BuffType _BuffType = BuffType.Energy;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	private Dictionary<CharacterCard, Buff> _cardBuffs = new();
	private MarvelSnapGame? _controller;
	
	public DreamDimension(LocationType id, string name, string description) : base(id, name, description)
	{
		
	}
	
	public DreamDimension()
	{
		
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
		_controller = controller;
		List<Player> players = controller.GetPlayers();
		if (!IsRevealed) 
		{
			IsRevealed = true;

			foreach (var p in players) 
			{
				int taskId = controller.GetLatestTaskId(p.Id) + 1;
				controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn1) { Action = () => { FutureTask(p.Id); } });
				controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn2) { Action = () => { FutureTask(p.Id); } });
			}
			
			controller.NotifyCardRevealed(null, this);
		}
	}
	
	private void FutureTask(int ownerId) 
	{
		if (_controller != null) 
		{
			List<Player> players = _controller.GetPlayers();
			
			foreach (var player in players) 
			{
				if (player.Id == ownerId) 
				{
					List<CharacterCard?> handCards = _controller.GetHandCards(player);
					foreach (var card in handCards) 
					{
						if (_controller.Turn == _FutureTurn1) 
						{
							int id = card.GetLatestBuffId(player) + 1;
							Buff buff = new Buff(id, _BuffValue, _BuffType, _BuffOperation);
							_cardBuffs.TryAdd(card, buff);
							card.AddBuff(player.Id, buff);
							_controller.NotifyEnergyCostChanged(player, card);
						}
						if (_controller.Turn == _FutureTurn2) 
						{
							if (_cardBuffs.ContainsKey(card)) 
							{
								card.RemoveBuff(player.Id, _cardBuffs[card].Id);
								_controller.NotifyEnergyCostChanged(player, card);
							}
								
						}
					}
				}
			}
		}
	}
	
	public override DreamDimension? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		DreamDimension? card = JsonSerializer.Deserialize<DreamDimension>(json);
		return card;
	}
}