﻿namespace MarvelSnap;

public class Player
{
	public int Id { get; set; }
	public string Name { get; set; }
	
	public Player(int id, string name) 
	{
		Id = id;
		Name = name;
	}
}
