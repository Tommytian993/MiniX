This is a collection of ideas and experiment about implementing the turn and city expansion system

# 1. Creating GeneralUI Scene
- To create a turn system, we create a new scene Panel, and call it GeneralUI. We add a Label to denote the turn number and a Button that we can press and end the turn.

- We attach a script, include initial variables, references and the basic turn-counter function

using Godot;
using System;
public partial class GeneralUI : Panel
{
    int turns = 0;
    Label turnLabel;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        turnLabel = GetNode<label>("TurnLabel");
        turnLabel.Text = "Turn: " + turns;
    }
    public void IncrementTurnCounter()
    {
        turns += 1;
        turnLabel.Text = "Turn: " + turns;
    }
}

# 2. Integrating GeneralUI to UIManager

- CityUI, TerrainUI are ephemeral, GeneralUi should be persistent. Thus we instantiate it as a child scene under UIManger. And it will display throughout the game.

- In UIManager script, we do not need to import a packed scene since we already made an instance. We still need to get the reference like this:

GeneralUI generalUI = GetNode<GeneralUI>("GeneralUI");

- Let's create the end turn signal in UIManager:

[Signal]
public delegate void EndTurnEventHandler();

- We get the button reference from GeneralUI:

Button endTurnButton = generalUI.GetNode<Button>("EndTurnButton");