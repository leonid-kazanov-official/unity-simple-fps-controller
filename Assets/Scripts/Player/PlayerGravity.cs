using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public CharacterController controller;

    public float gravity = -9.81f;
    public float groundStickForce = -2f;

    private float velocityY;

    void Update()
    {
        if (controller.isGrounded && velocityY < 0)
        {
            velocityY = groundStickForce;
        }

        velocityY += gravity * Time.deltaTime;

        Vector3 move = new Vector3(0, velocityY, 0);

        controller.Move(move * Time.deltaTime);
    }
}