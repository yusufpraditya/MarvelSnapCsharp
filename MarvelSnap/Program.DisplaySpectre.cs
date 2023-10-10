using System.Text;
using MarvelSnap;
using Spectre.Console;

namespace Program;

public partial class Program
{
	static void DisplaySpectre(MarvelSnapGame game, Player player1, Player player2) 
	{
		AnsiConsole.Write(MarvelSnapLayout());
	}
	
	static Table MarvelSnapLayout() 
	{
		var cards = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn("Medusa").Footer("Hawkeye").Centered())
			.AddColumn(new TableColumn("Mister Fantastic").Footer("Sentinel").Centered());
		
		StringBuilder sbLocation1 = new();
		sbLocation1.AppendLine();
		sbLocation1.AppendLine("Onslaught's Citadel");
		
		StringBuilder sbLocation2 = new();
		sbLocation2.AppendLine();
		sbLocation2.AppendLine("Dream Dimension");
		
		StringBuilder sbLocation3 = new();
		sbLocation3.AppendLine();
		sbLocation3.AppendLine("Kyln");
		
		var location1 = new Panel(sbLocation1.ToString());
		var location2 = new Panel(sbLocation2.ToString());
		var location3 = new Panel(sbLocation3.ToString());
			
		var table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Yellow)
			.Title("[yellow]Player 2[/]")
			.Caption("[yellow]Player 1[/]")
			.AddColumn(new TableColumn(cards).Footer(cards).Centered())
			.AddColumn(new TableColumn(cards).Footer(cards).Centered())
			.AddColumn(new TableColumn(cards).Footer(cards).Centered())
			.AddRow(Align.Center(location1), Align.Center(location2), Align.Center(location3));
		return table.Expand();
	}
}
