# Folder structure

The folder **must** be arranged this specific way:

```
	<root>
	|- checker.exe
	|- Maps
		|- <.txt files>
		|- ...
	|- Players
		|- <Folder name starts with "Team">
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

To run on Python 3+:
```
	python checker.py <path_to_root> <map_id> <team_id> <team_id>
```

For example:
```
	python checker.py . Map TeamBaseline TeamRandom
```

To run ```.exe``` file, replace ```python checker.py``` with ```checker.exe```. The file was built using pyinstaller with Python 3.8.