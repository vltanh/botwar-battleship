using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public GameObject lightPrefab;
    public GameObject goldPrefab;
    public GameObject cloudPrefab;
    public GameObject[] playerPrefabs;

    private int numPlayers = 2;
    private int numRows;
    private int numCols;
    private int numMoves;

    private string[,] boardData;

    public string mapConfigPath;

    private GameObject[,] boardTiles;
    private PlayerController[] players;

    private int currentMove = 0;

    public Vector2Int[] playerInitialPosition =
    {
        new Vector2Int(3, 5),
        new Vector2Int(6, 4)
    };

    public Vector2Int[][] playerMoves =
    {
        new Vector2Int[] { 
            new Vector2Int(3, 6),
            new Vector2Int(7, 4),
        },
        new Vector2Int[] {
            new Vector2Int(3, 5),
            new Vector2Int(6, 4),
        },
        new Vector2Int[] {
            new Vector2Int(4, 5),
            new Vector2Int(5, 4),
        },
        new Vector2Int[] {
            new Vector2Int(4, 5),
            new Vector2Int(5, 5)
        },
        new Vector2Int[] {
            new Vector2Int(5, 5),
            new Vector2Int(4, 5)
        }
    };

    // Start is called before the first frame update
    void Start()
    {
        SetupBoardView(mapConfigPath);
        SetupPlayerView();
        SetupSceneView();
    }

    private void SetupSceneView()
    {
        Camera.main.transform.position = new Vector3((numCols - 1.0f) / 2.0f, -(numRows - 1.0f) / 2.0f, -Mathf.Max(numCols, numRows));
        Instantiate(lightPrefab, new Vector3(), Quaternion.Euler(11.0f, 11.0f, 0.0f));
    }

    private void SetupPlayerView()
    {
        Vector2Int[] initialPositions = GetPlayerInitialPositions();
        players = new PlayerController[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(playerPrefabs[i], new Vector3(initialPositions[i].y - 1, -initialPositions[i].x + 1), Quaternion.identity).GetComponent<PlayerController>();
            players[i].SetPosition(playerInitialPosition[i]);
            players[i].SetID(i);
        }
    }

    Vector2Int[] GetPlayerInitialPositions()
    {
        return playerInitialPosition;
    }

    private void SetupBoardView(string config_path)
    {
        GetBoardDataFromFile(config_path);

        boardTiles = new GameObject[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Instantiate(groundTilePrefab, new Vector3(i, -j), Quaternion.identity);
                if (boardData[i, j] == "W")
                {
                    boardTiles[i, j] = Instantiate(cloudPrefab, new Vector3(j, -i), Quaternion.identity);
                }
                else if (boardData[i, j] == "M")
                {

                }
                else if (boardData[i, j] != "0")
                {
                    boardTiles[i, j] = Instantiate(goldPrefab, new Vector3(j, -i), Quaternion.identity);
                }
            }
        }
    }

    void GetBoardDataFromFile(string config_path)
    {
        var sr = new StreamReader(config_path);
        var fileContents = sr.ReadToEnd();
        sr.Close();

        var lines = fileContents.Split('\n');

        string[] line = lines[0].Split(' ');
        numRows = int.Parse(line[0]);
        numCols = int.Parse(line[1]);
        numMoves = int.Parse(line[2]);

        Debug.Log($"{numRows} x {numCols}, {numMoves} move(s)");

        boardData = new string[numRows, numCols];
        for (int i = 1; i < lines.Length; i++)
        {
            line = lines[i].Split(' ');
            for (int j = 0; j < line.Length; j++)
            {
                boardData[i - 1, j] = Regex.Replace(line[j], @"\t|\n|\r", "");
            }
        }
    }

    private bool CheckInBound(int x, int a, int b)
    {
        return x <= b && x >= a;
    }

    private bool CheckLegitMove(int _x, int _y)
    {
        return CheckInBound(_x, 1, numRows) && CheckInBound(_y, 1, numCols);
    }

    private void SendMove(int pid, int _x, int _y)
    {
        if (CheckLegitMove(_x, _y))
        {
            players[pid].Move(_x, _y);
        }
    }

    KeyCode[,] moveCmdMap =
    {
        { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow},
        { KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A }
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentMove < numMoves)
        {
            PlayNextMove();
        }

        for (int i = 0; i < numPlayers; i++)
        {
            Vector2Int curPos = players[i].GetPosition();
            int x = curPos.x, y = curPos.y;
            if (Input.GetKeyDown(moveCmdMap[i, 3]))
            {
                Vector2Int[] previousPositions = {
                    players[0].GetPosition() - Vector2Int.one ,
                    players[1].GetPosition() - Vector2Int.one
                };
                SendMove(i, x, y - 1);
                ProcessAfterMove(previousPositions);
            }
            else if (Input.GetKeyDown(moveCmdMap[i, 2]))
            {
                Vector2Int[] previousPositions = {
                    players[0].GetPosition() - Vector2Int.one ,
                    players[1].GetPosition() - Vector2Int.one
                };
                SendMove(i, x, y + 1);
                ProcessAfterMove(previousPositions);
            }
            else if (Input.GetKeyDown(moveCmdMap[i, 0]))
            {
                Vector2Int[] previousPositions = {
                    players[0].GetPosition() - Vector2Int.one ,
                    players[1].GetPosition() - Vector2Int.one
                };
                SendMove(i, x - 1, y);
                ProcessAfterMove(previousPositions);
            }
            else if (Input.GetKeyDown(moveCmdMap[i, 1]))
            {
                Vector2Int[] previousPositions = {
                    players[0].GetPosition() - Vector2Int.one ,
                    players[1].GetPosition() - Vector2Int.one
                };
                SendMove(i, x + 1, y);
                ProcessAfterMove(previousPositions);
            }
        }
    }

    private void PlayNextMove()
    {
        Vector2Int[] previousPositions = {
            players[0].GetPosition() - Vector2Int.one ,
            players[1].GetPosition() - Vector2Int.one
        };

        Vector2Int[] cmds = GetPlayerMoves();
        for (int i = 0; i < numPlayers; ++i)
        {
            SendMove(i, cmds[i].x, cmds[i].y);
        }
        ProcessAfterMove(previousPositions);

        ++currentMove;
    }

    private void ProcessAfterMove(Vector2Int[] previousPositions)
    {
        Vector2Int[] currentPositions = {
            players[0].GetPosition() - Vector2Int.one ,
            players[1].GetPosition() - Vector2Int.one
        };

        for (int i = 0; i < numPlayers - 1; i++)
        {
            for (int j = i + 1; j < numPlayers; j++)
            {
                if (currentPositions[i] == currentPositions[j] ||
                    currentPositions[i] == previousPositions[j] && currentPositions[j] == previousPositions[i])
                {
                    Debug.Log($"[P{i}] hits [P{j}].");
                    players[i].Die();
                    players[j].Die();
                }
            }
        }

        for (int i = 0; i < numPlayers; i++)
        {
            int x = currentPositions[i].x, y = currentPositions[i].y;
            string cell = boardData[x, y];
            if (cell == "M")
            {
                ClearCell(x, y);
                players[i].EquipShield();
            }
            else if (cell == "W")
            {
                players[i].EncounterTrap();
            }
            else if (cell != "0")
            {
                ClearCell(x, y);
                boardData[currentPositions[i].x, currentPositions[i].y] = "0";
                players[i].GetPoint(int.Parse(cell));
            }
        }
    }

    private void ClearCell(int x, int y)
    {
        boardData[x, y] = "0";
        Destroy(boardTiles[x, y], 0.3f);
    }

    Vector2Int[] GetPlayerMoves()
    {
        return playerMoves[currentMove];
    }
}
