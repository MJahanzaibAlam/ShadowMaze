# ShadowMaze
C# with XNA Game Project

<p>To try playing the game, download ShadowMaze.zip from the root directory, extract it, and run ShadowMaze.exe (note: this creates a folder in your documents folder for storing maps).</p>

Game classes are located in the <a href="https://github.com/MJahanzaibAlam/ShadowMaze/tree/master/ShadowMaze/ShadowMaze">main folder</a>.
<p>They are split into multiple folders: GamePlay, MapRelated, Screens, Other.</p>

Gameplay folder:
Contains classes of entities such as players and enemies as well as classes to help control their behaviour (pathfinding).

MapRelated folder:
Contains classes which make up the map grid of the game, used for both being able to load and save maps into the grid and also for holding nodes for the pathfinding algorithm.

Screens folder:
Stores all the different screen states of the game. Each screen contains objects and variables which are used for that specific screen. E.g. GameScreen holds player and enemy objects as well as other things such as the grid for the game map.

Other folder:
Contains the class for animation of entities as well as user interface related classes such as keyboard input, and sound management.
