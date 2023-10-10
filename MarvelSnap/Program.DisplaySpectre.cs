using System.Text;
using MarvelSnap;
using Spectre.Console;

namespace Program;

public partial class Program
{
	static void DisplaySpectre(MarvelSnapGame game, Player player1, Player player2) 
	{
		//AnsiConsole.Write(MarvelSnapLayout());
		// while (true) 
		// {
		// 	var key = Console.ReadKey(true).Key;
		// 	//Console.WriteLine(key);
		// 	Console.Clear();
		// 	AnsiConsole.Write(MarvelSnapLayout(key.ToString()));
		// }
		Test();
	}
	
	static void Test()
	{
		var (row, column) = (0, 0);
		while (true) {
			var key = Console.ReadKey(true).Key;
			Console.Clear();
			switch (key) {
				case ConsoleKey.UpArrow:
					row = row <= 0 ? 2 : row - 1;
					break;
				case ConsoleKey.DownArrow:
					row = row >= 2 ? 0 : row + 1;
					break;
				case ConsoleKey.LeftArrow:
					column = column <= 0 ? 2 : column - 1;
					break;
				case ConsoleKey.RightArrow:
					column = column >= 2 ? 0 : column + 1;
					break;
			}
			Console.WriteLine((row, column));
		}
	}
	
	static Table MarvelSnapLayout(string key) 
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
		sbLocation1.Append("[[■]]");
		
		StringBuilder sbLocation2 = new();
		sbLocation2.AppendLine();
		sbLocation2.AppendLine("Dream Dimension");
		sbLocation2.Append($"[[{key}]]");
		
		StringBuilder sbLocation3 = new();
		sbLocation3.AppendLine();
		sbLocation3.AppendLine("Kyln");
		sbLocation3.Append("[[ ]]");
		
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
