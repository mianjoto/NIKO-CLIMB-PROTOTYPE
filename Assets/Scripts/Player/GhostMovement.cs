using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    Vector2 oldPos, newPos;
    float heightDiff = 10;

    void Update()
    {
        oldPos = transform.position;
        newPos = new Vector2(oldPos.x, oldPos.y + heightDiff);
        transform.position = Vector2.MoveTowards(oldPos, newPos, Time.deltaTime);
    }
}
