﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    Match3 game;

    NodePieces moving;
    Point newIndex;
    Vector2 mouseStart;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<Match3>();

    }

    // Update is called once per frame
    void Update()
    {
        if (moving != null)
        {
            Vector2 direction = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 normalizedDirection = direction.normalized;
            Vector2 absoluteDirection = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if (direction.magnitude > 32) // if mouse is 32 pixels away from starting point of the mouse
            {
                //Make add either (1, 0) or (-1, 0) or (0, 1) or (0, -1) depending on direction of the mouse pointer

                if (absoluteDirection.x > absoluteDirection.y)
                    add = (new Point((normalizedDirection.x > 0) ? 1 : -1, 0));
                else if(absoluteDirection.y > absoluteDirection.x)
                    add = (new Point(0, (normalizedDirection.y > 0) ? -1 : 1));
            }
            newIndex.add(add);

            Vector2 pos = game.GetPositionFromPoint(moving.index); // get position of current piece we are moving
            if (!newIndex.Equals(moving.index))
                pos += Point.mult(new Point(add.x, -add.y), 16).ToVector();
            moving.MovePositionTo(pos);
        }
    }
    public void MovePiece(NodePieces piece)
    {
        if (moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;

    }
    public void DropPiece()
    {
        if (moving == null) return;
        Debug.Log("Dropped");

        if (!newIndex.Equals(moving.index))
            game.FlipPieces(moving.index, newIndex, true);
        else
            game.ResetPiece(moving);
        moving = null;
    }
}
