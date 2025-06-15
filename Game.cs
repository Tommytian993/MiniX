using Godot;
using System;

public partial class Game : Node
{
	 public override void _Ready(){
		  GD.Print("Game Ready");
	 }

	 public override void _Process(double delta){
		  GD.Print("Game Process");
	 }
}
