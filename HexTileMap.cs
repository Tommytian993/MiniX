using Godot;
using System;

public partial class HexTileMap : Node2D
{
     [Export]
     public int width = 100;
     [Export]
     public int height = 60;

     TileMapLayer baseLayer, borderLayer, overlayLayer;

     public override void _Ready()
     {
        baseLayer = GetNode<TileMapLayer>("BaseLayer");
        borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
        overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
     }
     public override void _Process(double delta){

     }
}
