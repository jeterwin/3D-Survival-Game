using FSR;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
 
public class FootstepsScript : MonoBehaviour
{
    [SerializeField] private FSR_Player fSR_Player;

    [SerializeField] private float stepfrequency = 0.5f;

    [SerializeField] private ExampleCharacterController rb;

    private float stepTimer;
    private void Update()
    {
        stepTimer += Time.deltaTime;
        if (stepTimer > stepfrequency && rb.MoveInputVector.magnitude > 0.1f)
        {
            stepTimer = 0;
            fSR_Player.step();
        }
    }
}