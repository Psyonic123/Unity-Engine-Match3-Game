using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public RectTransform gameBoard;

    public RectTransform killedBoard;

    public Sprite[] pieces;

    [Header("Prefabs")]
    public GameObject nodePiece;

    public GameObject killedPiece;

    private int width = 8;
    private int height = 8;
    private Node[,] board;

    private System.Random random;
    private int[] fills;

    private List<NodePieces> update; // list of values to be updated every frame
    private List<FlippedPieces> flipped;
    private List<NodePieces> deadPieces;
    private List<KilledPiece> killed;

    // Start is called before the first frame update
    private void Start()
    {
        StartGame();
    }

    // update is called once per frame
    private void Update()
    {
        List<NodePieces> finishedUpdating = new List<NodePieces>();
        for (int i = 0; i < update.Count; i++)
        {
            NodePieces piece = update[i];
            if (!piece.UpdatePiece())
            {
                finishedUpdating.Add(piece);
            }
        }

        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            NodePieces piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePieces flippedPiece = null;
            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);
            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped)
            {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }
            if (connected.Count == 0) //If we didn't make a match
            {
                if (wasFlipped) // If we flipped
                {
                    FlipPieces(piece.index, flippedPiece.index, false); //Flip back
                }
            }
            else //Being called if we made a match
            {
                foreach (Point pnt in connected) //Remove the node pieces when connected
                {
                    KillPiece(pnt);
                    Node node = getNodeAtPoint(pnt);
                    NodePieces nodePiece = node.getPiece();
                    if (nodePiece != null)
                    {
                        nodePiece.gameObject.SetActive(false);
                        deadPieces.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }
                BoardGravity();
            }
            flipped.Remove(flip); //remove the flip after update
            update.Remove(piece);
        }
    }

    private void BoardGravity()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = (height - 1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val != 0) continue; // if not a hole or 0, do nothing
                for (int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1) // if we did not hit an end, but its not 0 use this to fill current hole
                    {
                        Node got = getNodeAtPoint(next);
                        NodePieces piece = got.getPiece();

                        //Set hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        //replace hole
                        got.SetPiece(null);
                    }
                    else //hit an end
                    {
                        //fill hole
                        int newVal = fillPiece();
                        NodePieces piece;
                        Point fallPoint = new Point(x, (-1 - fills[x]));
                        if (deadPieces.Count > 0)
                        {
                            NodePieces revived = deadPieces[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;

                            deadPieces.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePieces n = obj.GetComponent<NodePieces>();
                            piece = n;
                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = GetPositionFromPoint(fallPoint);
                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    private void removeFlipped(NodePieces p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
    }

    private FlippedPieces getFlipped(NodePieces p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    private void StartGame() // Generates board, and random seed for gameboard
    {
        fills = new int[width];
        board = new Node[width, height];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePieces>();
        flipped = new List<FlippedPieces>();
        deadPieces = new List<NodePieces>();
        killed = new List<KilledPiece>();

        InitalizeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    private void InitalizeBoard() // Initalizes the game board
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    private void VerifyBoard() // Makes sure we have a working board
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

    private void InstantiateBoard() //Instantiates board
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));
                int val = board[x, y].value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (78 * x), -32 - (80 * y));
                NodePieces piece = p.GetComponent<NodePieces>();
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePieces piece) //resets a piece at old position if invalid move
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (getValueAtPoint(one) < 0) return;
        Node nodeOne = getNodeAtPoint(one);
        Node nodeTwo = getNodeAtPoint(two);
        NodePieces pieceOne = nodeOne.getPiece();
        NodePieces pieceTwo = nodeTwo.getPiece();
        if (getValueAtPoint(two) > 0)
        {
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);
            if (main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
            ResetPiece(pieceOne);
    }

    private Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    private void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
        {
            if (!killed[i].falling)
                available.Add(killed[i]);
        }
        KilledPiece set = null;
        if (available.Count > 0)
            set = available[0];
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = getValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < pieces.Length)
            set.Initialize(pieces[val], GetPositionFromPoint(p));
    }

    private List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);
        Point[] directions = { Point.up, Point.right, Point.down, Point.left };
        foreach (Point dir in directions) //Checking if there is 2 or more same shapes in the directions
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
        return connected;
    } // Checks if there are connections and matches near point

    private void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
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

    private int fillPiece() // Adds in a random piece
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    private int getValueAtPoint(Point p) // Utility function that returns values at given point
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    private void setValueAtPoint(Point p, int val)
    {
        board[p.x, p.y].value = val;
    }

    private int newValue(ref List<int> remove)
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

    private string getRandomSeed() //Creates random seed
    {
        string seed = "";
        string acceptedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptedChars[Random.Range(0, acceptedChars.Length)];
        }
        return seed;
    }

    public Vector2 GetPositionFromPoint(Point p)
    {
        return new Vector2(32 + (78 * p.x), -32 - (80 * p.y));
    }
}

[System.Serializable]
public class Node
{
    public int value; // 0 = blank, 1 = cube, 2 = sphere, 3 = cylinder, 4 = pyramid, 5 = diamond, -1 = hole
    public Point index;
    private NodePieces piece;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }

    public void SetPiece(NodePieces p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public NodePieces getPiece()
    {
        return piece;
    }
}

[System.Serializable]
public class FlippedPieces
{
    public NodePieces one;
    public NodePieces two;

    public FlippedPieces(NodePieces o, NodePieces t)
    {
        one = o;
        two = t;
    }

    public NodePieces getOtherPiece(NodePieces p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else return null;
    }
}