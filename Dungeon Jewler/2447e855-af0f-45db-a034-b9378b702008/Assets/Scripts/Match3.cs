using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityScript.Steps;

public class Match3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    public ArrayLayout boardLayout;
    [Header("UI Elements")]
    public RectTransform gameBoard;
    public Sprite[] pieces;
    [Header("Prefabs")]
    public GameObject nodePiece;

    int width = 8;
    int height = 8;
    Node[,] board;
    System.Random random;
    void StartGame() // Generates board, and random seed for gameboard
    {
        board = new Node[width, height];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        InitalizeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitalizeBoard() // Initalizes the game board
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? - 1 : fillPiece(), new Point(x, y));
            }
        }
    }
    void VerifyBoard() // Makes sure we have a working board
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setValueAtPoint(p, newValue(ref remove));
                }
                
            }
        }
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int val = board[x, y].value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece node = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (78 * x), -32 - (80 * y));
                node.Initialize(val, new Point(x, y), pieces[val - 1]);
            }
        }
    }
    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);
        Point[] directions = { Point.up, Point.right, Point.down, Point.left };
        foreach(Point dir in directions) //Checking if there is 2 or more same shapes in the directions
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }
            if (same > 1) //If there are more than 1 of the same shape in direction. AKA match
            {
                AddPoints(ref connected, line); //Add those points to connected list
            }
        }
        for (int i = 0; i < 2; i++) // Checking if we are in the middle of two of the same shapes
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };

            foreach (Point next in check) // check both sides of piece, if they are the same vakue add them to list
            { 
                if (getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }
            if (same > 1) //If there are more than 1 of the same shape in direction. AKA match
            {
                AddPoints(ref connected, line); //Add those points to connected list
            }
        }
        for (int i = 0; i < 4; i++) //check for 2x2
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };

            foreach (Point pnt in check) // check all sides of piece, if they are the same vakue add them to list
            {
                if (getValueAtPoint(pnt) == val)
                {
                    square.Add(pnt);
                    same++;
                }
            }
            if (same > 2)
                AddPoints(ref connected, square);
        }
        if (main) // Checks for other matches along current match
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }
        if (connected.Count > 0)
        {
            connected.Add(p);
        }
        return connected;
    } // Checks if there are connections and matches near point
    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach(Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd) points.Add(p);
        }
    }
    int fillPiece() // Adds in a random piece
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    int getValueAtPoint(Point p) // Utility function that returns values at given point
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }
    void setValueAtPoint(Point p, int val)
    {
        board[p.x, p.y].value = val;
    }
    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (int i in remove)
            available.Remove(i);
        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];

    }
    // Update is called once per frame
    void Update()
    {
        
    }
  string getRandomSeed() //Creates random seed
    {
        string seed = "";
        string acceptedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptedChars[Random.Range(0, acceptedChars.Length)];
        }
        return seed;
    }
}

[System.Serializable]
public class Node
{
    public int value; // 0 = blank, 1 = cube, 2 = sphere, 3 = cylinder, 4 = pyramid, 5 = diamond, -1 = hole
    public Point index;
    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }
}