This is a collection of ideas and experiment about creating the procedural terrain generation in this game(not a formal design document, just a notebook about updates during the development)

# 1. Terrain type Representations
- First we can use an enum structure to represent all the available types

public enum TerrainType { PLAINS, WATER, DESERT, MOUNTAIN, ICE, SHALLOW_WATER, FOREST, BEACH }