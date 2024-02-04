using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonModule : DNModuleBase
{
    [SerializeField]
    private Light moon;
    [SerializeField]
    private Gradient moonColor;
    [SerializeField]
    private float baseIntensity;

    public override void DisableShadows()
    {
        moon.shadows = LightShadows.None;
    }

    public override void EnableShadows()
    {
        moon.shadows = LightShadows.Hard;
    }

    public override void UpdateModule(float intensity)
    {
        moon.color = moonColor.Evaluate(1 - intensity);
        moon.intensity = (1 - intensity) * baseIntensity;
    }
}