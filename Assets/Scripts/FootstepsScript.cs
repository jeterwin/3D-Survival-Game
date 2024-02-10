using FSR;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
 
public class FootstepsScript : MonoBehaviour
{
    public float Stepfrequency = 0.5f;

    [SerializeField] private FSR_Player fSR_Player;

    [SerializeField] private ExampleCharacterController rb;

    private float stepTimer;
    private void Update()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer > Stepfrequency && rb.MoveInputVector.magnitude > 0.1f)
        {
            stepTimer = 0;
            fSR_Player.step();
        }
    }
}