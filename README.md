# Interscape: 
An open-world topdown 2D game made in Unity! Uses procedural generation to generate very large maps and worlds that are fun to explore. Still under development; there are many more features that plan to be added. In its current state, the player can walk around, chop trees and pick up items.

Here are some different biomes so far. Biome blending has been implemented in a shader to enable smooth gradient transitions between biomes. 
<p align="center">
  <img src="Images/Desert.png" width="250" title="Desert">
  <img src="Images/Savanna.png" width="250" title="Savanna">
  <img src="Images/Grassland.png" width="250" title="Grassland">
  <img src="Images/Rainforest.png" width="250" title="Rainforest">
  <img src="Images/Forest.png" width="250" title="Forest">
  <img src="Images/Taiga.png" width="250" title="Taiga">
</p>

I am also working on adding buildings, floors and paths. The walls are made from individual tiles of which the player can place or remove as they please. The doors currently slide open/close automatically when the player walks near.
<p align="center">
  <img src="Images/Buildings.png" width="600" >
</p>

And just for fun, here is what the map of a random seed looks like. The warmer biomes generate at the equator and the cooler biomes toward the poles. There are also no large oceans so player travel doesn't get too boring. Layered perlin noise is used for the world generation by creating a heat, moisture and height map.
<p align="center">
  <img src="Images/Map.png" width="400" >
</p>

Here's some concept art for the player/npc sprites:
<p align="center">
  <img src="Images/CharacterConcepts.gif" width="300" >
</p>

## Demos
<figure class="video_container" align="center">
  <video controls="true" allowfullscreen="true" poster="Images/MenuDemoImage.png" width=600>
    <source src="Images/MenuDemo.mp4" type="video/mp4">
  </video>
</figure>

So far, this has been an entirely solo project. All animation/art is drawn from scratch by Jess in Procreate for iPad. 
Should you have any thoughts or enquires about this project, feel free to send an email to jrhammer50@gmail.com :D
