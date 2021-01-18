using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Point {
    public int x;
    public int y;
    public Point(int nx, int ny)
    {
        x = nx;
        y = ny;

    }

    //Turn point into vector, from vector to point
    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }
    public bool Equals(Point p)
    {
        return (x == p.x && y == p.y);
    }
    public static Point fromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);

    }
    public static Point fromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);

    }
    //Add or multiply by a point
    public void mult (int m)
    {
        x *= m;
        y *= m;
    }
    public  void add(Point p )
    {
        x += p.x;
        y += p.y;
    }
    public static Point mult(Point p, int m)
    {
        return new Point(p.x * m, p.y * m);
        
    }
    public static Point add(Point p, Point o)
    {
        return new Point(p.x + o.x, p.y + o.y);

    }
    public static Point clone(Point p)
    {
        return new Point(p.x, p.y);
    }

//Zero point or directional point
    public static Point zero
    {
        get { return new Point(0, 0); }
    }
    public static Point up
    {
        get { return new Point(0, 1); }
    }
    public static Point down
    {
        get { return new Point(0, -1); }
    }
    public static Point left
    {
        get { return new Point(-1, 0); }
    }
    public static Point right
    {
        get { return new Point(1, 0); }
    }
}

