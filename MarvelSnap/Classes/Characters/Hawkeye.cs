using System.Text.Json;

namespace MarvelSnap;

public class Hawkeye : CharacterCard
{
	private int _cardCount;
	private IPlayer? _player;
	private MarvelSnapGame? _controller;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public Hawkeye(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public Hawkeye()
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		_player = player;
		_controller = controller;
		if (!IsRevealed)
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			List<CharacterCard> arenaCards = controller.GetArenaCards(player, Arena);
			_cardCount = arenaCards.Count;
			controller.AddFutureTask(player.Id, new FutureTask(0, controller.Turn + 1) { Action = FutureTask });
			controller.NotifyCardRevealed(player, this);
		}
	}

	private void FutureTask()
	{
		if (_controller != null && _player != null)
		{
			List<CharacterCard> arenaCards = _controller.GetArenaCards(_player, Arena);

			if (arenaCards.Count > _cardCount)
			{
				int buffId = GetLatestBuffId(_player) + 1;
				AddBuff(_player.Id, new Buff(buffId, _BuffValue, _BuffType, _BuffOperation));
				_controller.NotifyPowerChanged(_player, this);
			}
		}
	}

	public override Hawkeye? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Hawkeye? card = JsonSerializer.Deserialize<Hawkeye>(json);
		return card;
	}

	public override void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}
}