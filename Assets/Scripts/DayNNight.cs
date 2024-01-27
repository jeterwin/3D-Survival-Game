using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNNight : MonoBehaviour
{
    [SerializeField] private float dayLength = 360f; //In seconds
    [SerializeField] private float timeOfDay;
    [SerializeField] private int dayCount = 0;
    [SerializeField] private bool isNight = false;
    private void Update()
    {
        timeOfDay += Time.deltaTime / dayLength;
        timeOfDay = Mathf.Clamp01(timeOfDay);
        RenderSettings.skybox.SetFloat("_CubemapTransition", timeOfDay);
    }
}
