# Team

**Team Name**: Yuta & Bella

**Members**:

|ID|Name|Student ID|
|---|---|---|
|1|Vũ Lê Thế Anh|1612838|
|2|Nguyễn Lê Hồng Hạnh|1612849|

# Game

![](imgs/SS.gif)

Menu |  Game 1 | Game 2
:-------------------------:|:-------------------------:|:-------------------------:
![](imgs/SS0.png)  |  ![](imgs/SS1.jpg)  | ![](imgs/SS2.jpg)

# Folder structure

The folder **must** be arranged this specific way:

```
	<root>
	|- checker.exe
	|- Maps
		|- <map_id>.txt
		|- ...
	|- Players
		|- <team_id> (always start with "Team", i.e "Team*")
			|- <team_id>.exe
		|- ...
	|- Match
```

## Maps

Input: ```<map_id>.txt```

```
	m n k
	<map data of m lines, each containing n strings seperated by space>
```

with m/n = number of rows/cols, k = number of steps.

## Players

```
	<team_id>
	|- <team_id>.exe
```

where ```team_id = Team*```

## Match

This is where the program outputs the log files of the form ```<team_id>_<team_id>_<map_id>.txt```.

# Usage

## Checker

To run on Python 3+:
```
	python checker.py <path_to_root> <map_id> <team_id> <team_id>
```

For example:
```
	python checker.py . Map TeamBaseline TeamRandom
```

## Visualizer

To run ```.exe``` file, replace ```python checker.py``` with ```checker.exe```. The file was built using pyinstaller with Python 3.8.

To run visualize, run ```TrashCollecter.exe```, press **Load** at the menu and point to the ```Tournament``` folder (or any folder that has the same structure as described above).

Choose the map, and the two teams and press **Play**. It will take time since it will call ```checker.exe``` to run the two players` programs.

In the game:

- **Space** to run the next move
- **Arrow Keys** to (Up/Down) change camera height (Left/Right) change camera`s angle.

# Special Thanks

I would like to offer my gratitude to

- Nguyen Ho Thang Long (HCMUS, CNTN2018) for providing the wonder assets; and
- Vuong Hy (HCMUS, CNTN2016) for providing suggestions on tournament rules and matches arrangement, for writing the random map generator, and for the baseline + random bot.

# Resources

- Song: 母港 (Homeport), Kantai Collection
- Assets:
	- [Low-poly Water](https://assetstore.unity.com/packages/tools/particles-effects/lowpoly-water-107563)
	- [Polygon Pirates Package](https://assetstore.unity.com/packages/3d/environments/historic/polygon-pirates-pack-92579)
	- [Polygon Arsenal](https://assetstore.unity.com/packages/vfx/particles/polygon-arsenal-109286)
- Menu Background: Kongou, Kantai Collection
