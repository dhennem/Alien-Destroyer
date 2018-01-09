# Alien-Destroyer

I completed this project on my own during winter break 2017-2018.

Alien Destroyer is a 2D shooter game designed with Unity in C#. The player's objective is to shoot down every enemy ship, while also
avoiding enemy shots and making sure to not run out of ammunition. The game is separated into five stages, with each stage containing three
waves of "smaller" alien ships and one scene against a "larger" commander ship. Each stage has its own unique difficulty settings defined in
the LevelManager script. These settings, which ensure that the game's difficulty increases with each consecutive stage, specify values for each stage, such
as enemy shot velocity, enemy shot frequency, minimum and maximum enemy ship speeds, and the number of lives and shots allotted to the player. 
At the top of each scene, a text header displays the number of player lives and shots left, the number of enemy ships still alive, and the 
current stage number. 

Each enemy ship requires 1, 2, or 3 hits from the player in order to be destroyed. Every stage has a preset quota of 1-hit, 2-hit, and 3-hit
ships that must be spawned in every wave. However, an algorithm in the ShipGenerator script ensures that the ships are always spawned in a
random arrangement. Moreover, the direction and path of each ship are constantly randomized during runtime in order to prevent any 
predictable patterns from forming throughout the arrangement of enemy ships. 

Another interesting feature keeps track of how far the player progressed through the game before losing in the current play session. 
Upon losing, the player has the option to either begin from the highest stage that they reached before losing, or simply begin again 
from the start. 

See comments throughout each of the scripts for more detail on the game's features.
  
