using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DNModuleBase : MonoBehaviour {

    protected DayNNight dayNightControl;

    private void OnEnable()
    {
        dayNightControl = GetComponent<DayNNight>();
        if (dayNightControl != null)
            dayNightControl.AddModule(this);
    }
    public abstract void DisableShadows();
    public abstract void EnableShadows();
    private void OnDisable()
    {
        if(dayNightControl != null)
            dayNightControl.RemoveModule(this);
    }

    public abstract void UpdateModule(float intensity);
}
