In order to add a new minigame to the game:
1 - Add its ID to enum Minigames
2 - Open Tools - Minigames - Create
3 - Enter data and click Create
4 - a new scene will be created in the folder Content/Remote/Minigames/{ID}. 
5 - Please add minigame model to the scene manually (it should be in the Scripts folder) - {ID}MinigameModel

Working with Minigames
6 - You can run each minigame scene from its own scene (TestRunner objbect will do it) - you just need to run the scene and press S - it will load the scene and start
7 - Name and Description can be changed in Content/Local/Configs/
8 - For a newly created minigame you can use services (IMinigameServices)
9 - IGameObjectPool - Pools objects from Local content folder. Uses enums for file identification. Enum structure mimics content path
10 - IMinigameGameObjectPool - Pool for minigame folder content only. 
11 - IMinigamesConfigLoader - loads configs only from minigame's folder

Known issues
12 - Cannot automatically add MinigameModel component through editor Scripts
13 - Some objects might be created after MetaUI Dispose 
14 - Second game (memory) is very poor in graphics. 1st one is way better.
