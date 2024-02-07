using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Headbop : MonoBehaviour
{
    [SerializeField] private ExampleCharacterController characterController;

    [SerializeField] private float walkingBobbingSpeed = 14f;
    [SerializeField] private float bobbingAmount = 0.05f;

    private float defaultPosY = 0;
    private float timer = 0;

    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        if(characterController.MoveInputVector.magnitude > 0.1f)
        {
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
        }
    }
}
