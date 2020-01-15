using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    string id;
    int x, y;
    int score;
    bool isShield;
    bool functioning;

    public GameObject shieldPrefab, smokePrefab, startAuraPrefab;
    public GameObject shield;

    private void Start()
    {
        x = y = 0;
        score = 0;
        isShield = false;
        functioning = true;
    }

    // 0: N, 1: S, 2: E, 3: W
    int currentOrientation = 0;
    int[] nextCCWOrientation = { 3, 2, 0, 1 };
    int[] nextCWOrientation = { 2, 3, 1, 0 };
    int[] next180Orientation = { 1, 0, 3, 2 };
    Vector3[] direction =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left
    };
    Quaternion[] oriAngle =
    {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 180, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, -90, 0)
    };

    // moveMap[ori, move]
    int[,] moveMap = { 
        { 0, 1, 2, 3 }, 
        { 1, 0, 3, 2 }, 
        { 3, 2, 0, 1 },
        { 2, 3, 1, 0 }
    };

    public void SetID(string _id)
    {
        id = _id;
    }

    public string GetID()
    {
        return id;
    }

    public void SetPosition(Vector2Int pos)
    {
        x = pos.x;
        y = pos.y;
        Instantiate(startAuraPrefab, new Vector3(y - 1, 0, -x + 1), Quaternion.identity);
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
        if (isShield) return 1;
        return 0;
    }

    public void Die()
    {
        if (functioning)
        {
            Debug.Log($"[{id}] dies.");

            // Explode yourself here!
            transform.Rotate(Vector3.right * 180);
            Instantiate(smokePrefab, new Vector3(y - 1, 0, -x + 1), Quaternion.identity);

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
            Debug.Log($"[{id}] Move from ({x}, {y}) to ({_x}, {_y})");
            x = _x; y = _y;

            move = moveMap[currentOrientation, move];
            if (move == 0)
            {
                StartCoroutine(MoveForward(0.3f));
            }
            else if (move == 1)
            {
                currentOrientation = next180Orientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.down * 180, 0.2f, 0.1f));
            }
            else if (move == 2)
            {
                currentOrientation = nextCWOrientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.down * -90, 0.15f, 0.15f));
            }
            else if (move == 3)
            {
                currentOrientation = nextCCWOrientation[currentOrientation];
                StartCoroutine(Rotate(Vector3.down * 90, 0.15f, 0.15f));
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
        transform.position = new Vector3(y - 1, 0, -x + 1);
    }

    public void EquipShield()
    {
        if (functioning)
        {
            Debug.Log($"[{id}] Shield equipped!");
            isShield = isShield | true;
            shield = Instantiate(shieldPrefab, transform);
            shield.GetComponent<ShieldController>().Follow(gameObject);
        }
    }

    public void EncounterTrap(GameObject gameObject)
    {
        if (functioning)
        {
            Debug.Log($"[{id}] Trap encountered!");
            if (!isShield)
            {
                Die();
                Destroy(gameObject);
            }
            else
            {
                gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public void EarnPoint(int v)
    {
        Debug.Log($"[{id}] Gain {v} coins, totaling {score+v}!");
        if (functioning)
        {
            score += v;
        }
    }
}
