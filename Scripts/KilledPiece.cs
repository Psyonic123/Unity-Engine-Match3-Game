using UnityEngine;
using UnityEngine.UI;

public class KilledPiece : MonoBehaviour
{
    public bool falling;
    private float speed = 16f;
    private float gravity = 32f;
    private Vector2 moveDirection;
    private RectTransform rect;
    private Image img;

    public void Initialize(Sprite piece, Vector2 start)
    {
        falling = true;
        moveDirection = Vector2.up;
        moveDirection.x = Random.Range(-1.0f, 1.0f);
        moveDirection *= speed / 2;

        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        img.sprite = piece;
        rect.anchoredPosition = start;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!falling) return;
        moveDirection.y -= Time.deltaTime * gravity;
        moveDirection.x = Mathf.Lerp(moveDirection.x, 0, Time.deltaTime);
        rect.anchoredPosition += moveDirection * Time.deltaTime * speed;
        if (rect.position.x < -64f
            || rect.position.x > Screen.width + 64f
            || rect.position.y < -64f
            || rect.position.y > Screen.height + 64f)
        {
            falling = false;
        }
    }
}