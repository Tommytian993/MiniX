using Godot;
using System;
using System.Collections.Generic;

public partial class City : Node2D
{
	 public HexTileMap map;
	 public Vector2I centerCoordinates;

	 public List<Hex> territory;
	 public List<Hex> borderTilePool;

	 public string name;

	 Label label;
	 Sprite2D sprite;

	 public override void _Ready()
	 {
		  label = GetNode<Label>("Label");
		  sprite = GetNode<Sprite2D>("Sprite2D");
		  label.Text = name;
		  territory = new List<Hex>();
		  borderTilePool = new List<Hex>();
	 }




}
