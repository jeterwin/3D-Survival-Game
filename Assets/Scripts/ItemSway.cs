using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSway : MonoBehaviour
{
    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot; 

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve);}
    float curveCos { get => Mathf.Cos(speedCurve);}

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;


    void Update()
    {
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }
    private bool isMoving
    {
        get { return ExampleCharacterController.Instance.RawInputs.MoveAxisForward != 0f; }
    }

    Vector2 lookInput;

    void GetInput()
    {
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
    }


    void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (ExampleCharacterController.Instance.TimeSinceLastAbleToJump > 0 ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical"))*bobExaggeration : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (ExampleCharacterController.Instance.TimeSinceLastAbleToJump > 0 ? 1:0)) - (ExampleCharacterController.Instance.RawInputs.MoveAxisRight * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(ExampleCharacterController.Instance.RawInputs.MoveAxisForward * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = isMoving ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRotation.y = isMoving ? multiplier.y * curveCos : 0;
        bobEulerRotation.z = (isMoving ? multiplier.z * curveCos * ExampleCharacterController.Instance.RawInputs.MoveAxisRight : 0);
    }

}