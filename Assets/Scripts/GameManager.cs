using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Grid { public int x, y; };

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject piece;

    List<GameObject> pieces;
    List<Grid> positions;

    Grid direction;

    Grid food;
    List<Grid> foods;
    List<GameObject> foodObjects;

    int size;

    bool canGrow, hasGameFinsished;

    private void Awake()
    {
        pieces = new List<GameObject>();
        positions = new List<Grid>();
        direction = new Grid() { x = 0, y = -1 };
        food = new Grid() { x = 7, y = -7 };
        foods = new List<Grid>();
        foodObjects = new List<GameObject>();

        size = 4;
        canGrow = false;
        hasGameFinsished = false;

        for (int i = 0; i < size; i++)
        {
            GameObject temp = Instantiate(piece);
            Grid tempPos = new Grid() { x = 0, y = -i };
            temp.transform.position = new Vector3(tempPos.x, tempPos.y, -1);
            positions.Insert(0, tempPos);
            pieces.Insert(0, temp);
        }

        GameObject currentFood = Instantiate(piece);
        currentFood.transform.position = new Vector3(food.x, food.y, -1);
        currentFood.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        foodObjects.Add(currentFood);

        StartCoroutine(UpdateTurn());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            direction = direction.x != 1 ? new Grid() {x= -1,y= 0 } : direction;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = direction.x != -1 ? new Grid() { x = 1, y = 0 } : direction;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = direction.y != -1 ? new Grid() { x = 0, y = 1 } : direction;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = direction.y != 1 ? new Grid() { x = 0, y = -1 } : direction;
        }
    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    IEnumerator UpdateTurn()
    {
        yield return new WaitForSeconds(0.25f);

        if (positions[0].x == food.x && positions[0].y == food.y)
        {
            foods.Add(food);
            food = new Grid() { x = Random.Range(0, 15), y = -Random.Range(0, 15) };
            while(!IsValidPosition(food))
            {
                food = new Grid() { x = Random.Range(0, 15), y = -Random.Range(0, 15) };
            }
            GameObject newFood = Instantiate(piece);
            newFood.transform.position = new Vector3(food.x, food.y, -1);
            newFood.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            foodObjects.Add(newFood);
        }

        if(foods.Count != 0 && positions[size - 1].x == foods[0].x && positions[size - 1].y == foods[0].y)
        {
            canGrow = true;
        }

            for (int i = size - 1; i > 0; --i)
        {
            positions[i] = positions[i - 1];
        }

        positions[0] = new Grid() { x = positions[0].x + direction.x, y = positions[0].y + direction.y};
        if(positions[0].x < 0)
        {
            positions[0] = new Grid() { x = 14, y = positions[0].y };
        }
        if (positions[0].x > 14)
        {
            positions[0] = new Grid() { x = 0, y = positions[0].y };
        }
        if (positions[0].y > 0)
        {
            positions[0] = new Grid() { x = positions[0].x, y = -14};
        }
        if (positions[0].y < -14)
        {
            positions[0] = new Grid() { x = positions[0].x, y = 0 };
        }

        if(canGrow)
        {
            positions.Add(foods[0]);
            foods.RemoveAt(0);
            foodObjects[0].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
            pieces.Add(foodObjects[0]);
            foodObjects.RemoveAt(0);
            size++;
            canGrow = false;
        }

        if(!IsValidPosition(positions[0],1))
        {
            hasGameFinsished = true;
        }

        if(hasGameFinsished)
        {
            yield break;
        }

        for (int i = 0; i < size; i++)
        {
            pieces[i].transform.position = new Vector3(positions[i].x, positions[i].y, -1);
        }
        StartCoroutine(UpdateTurn());
    }

    bool IsValidPosition(Grid match)
    {
        foreach(Grid temp in positions)
        {
            if (temp.x == match.x && temp.y == match.y) return false;
        }
        return true;
    }

    bool IsValidPosition(Grid match,int x)
    {
        for (int i = 1; i < positions.Count; i++)
        {
            if (match.x == positions[i].x && match.y == positions[i].y) return false;
        }
        return true;
    }
}
