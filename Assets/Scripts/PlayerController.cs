using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int id;
    int x, y;
    int score;
    bool shield;
    bool functioning;

    private void Start()
    {
        x = y = 0;
        score = 0;
        shield = false;
        functioning = true;
    }

    // 0: N, 1: S, 2: E, 3: W
    int currentOrientation = 0;
    int[] nextCCWOrientation = { 3, 2, 0, 1 };
    int[] nextCWOrientation = { 2, 3, 1, 0 };
    int[] next180Orientation = { 1, 0, 3, 2 };
    Vector3[] direction =
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    Quaternion[] oriAngle =
    {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 0, 180),
        Quaternion.Euler(0, 0, -90),
        Quaternion.Euler(0, 0, 90)
    };

    // moveMap[ori, move]
    int[,] moveMap = { 
        { 0, 1, 2, 3 }, 
        { 1, 0, 3, 2 }, 
        { 3, 2, 0, 1 },
        { 2, 3, 1, 0 }
    };

    public void SetID(int _id)
    {
        id = _id;
    }

    public void SetPosition(Vector2Int pos)
    {
        x = pos.x;
        y = pos.y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public int GetPoint()
    {
        return score;
    }

    public int GetShield()
    {
        if (shield) return 1;
        return 0;
    }

    public void Die()
    {
        if (functioning)
        {
            Debug.Log($"[P{id}] dies.");
            functioning = false;
        }
    }

    int IntepretMove(int _x, int _y)
    {
        if (_x - x == -1 && _y - y == 0)
        {
            return 0;
        } 
        else if (_x - x == 1 && _y - y == 0)
        {
            return 1;
        }
        else if (_x - x == 0 && _y - y == 1)
        {
            return 2;
        }
        else if (_x - x == 0 && _y - y == -1)
        {
            return 3;
        }
        return -1;
    }

    public void Move(int _x, int _y)
    {
        int move = IntepretMove(_x, _y);
        if (move > -1 && functioning)
        {
            Debug.Log($"[P{id}] Move from ({x}, {y}) to ({_x}, {_y})");
            x = _x; y = _y;

            move = moveMap[currentOrientation, move];
            if (move == 0)
            {
                StartCoroutine(MoveForward(0.3f));
            }
            else if (move == 1)
            {
                currentOrientation = next180Orientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.forward * 180, 0.2f, 0.1f));
            }
            else if (move == 2)
            {
                currentOrientation = nextCWOrientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.forward * -90, 0.15f, 0.15f));
            }
            else if (move == 3)
            {
                currentOrientation = nextCCWOrientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.forward * 90, 0.15f, 0.15f));
            }
        }
    }

    IEnumerator Rotate(Vector3 byAngles, float rotateTime, float moveTime)
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t <= 1; t += Time.deltaTime / rotateTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        transform.rotation = oriAngle[currentOrientation];
        StartCoroutine(MoveForward(moveTime));
    }

    IEnumerator MoveForward(float inTime)
    {
        var fromPos = transform.position;
        var toPos = fromPos + direction[currentOrientation];
        for (var t = 0f; t <= 1; t += Time.deltaTime / inTime)
        {
            transform.position = Vector3.Lerp(fromPos, toPos, t);
            yield return null;
        }
        transform.position = new Vector3(y - 1, -x + 1);
    }

    public void EquipShield()
    {
        if (functioning)
        {
            Debug.Log($"[P{id}] Shield equipped!");
            shield = shield | true;
        }
    }

    public void EncounterTrap()
    {
        if (functioning)
        {
            Debug.Log($"[P{id}] Trap encountered!");
            if (!shield)
            {
                Die();
            }
        }
    }

    public void EarnPoint(int v)
    {
        Debug.Log($"[P{id}] Gain {v} coins, totaling {score+v}!");
        score += v;
    }
}
