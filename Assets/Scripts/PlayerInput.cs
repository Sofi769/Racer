using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private IPlayerController playerController;

    void Start()
    {
        playerController = GetComponent<IPlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Missing IPlayerController component on " + gameObject.name);
            enabled = false;
        }
    }

    void Update()
    {
        if (playerController == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);

        playerController.Move(movement);
    }
}
