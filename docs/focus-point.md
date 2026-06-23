# Focus Point

## Next Implementation Focus

The next major generator feature is biome generation.

The current map already behaves like a basic plains-style island. The next step is to split the generated land into broad biome zones based on vertical map position:

- Bottom 30%: desert biome
- Center 40%: plains biome, matching the current default look
- Top 30%: tundra biome

## Intended Direction

Biome generation should become its own layer of generation data, similar to the existing height, foliage, and mineral noise maps.

The biome layer should influence visuals and spawning rules without replacing the height map. Height still decides whether a tile is water, shore, land, or mountain. Biome data should decide what kind of land that tile belongs to.

## First Biome Set

- Desert: bottom 30% of the map
- Plains: middle 40% of the map, matching the current default look
- Tundra: top 30% of the map

## Notes

- Biome transitions should eventually be softened with noise instead of being hard horizontal lines.
- Resources should later react to biome type, for example fewer trees in desert and different resource weights in tundra.
- The current plains behavior should be preserved as the baseline while adding desert and tundra variation.
