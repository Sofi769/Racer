using UnityEngine;

public interface IPlayerController
{
    void Move(Vector2 direction);
    void HandleCollision(Collision2D collision);
}
