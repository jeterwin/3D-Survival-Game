using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Linq.Expressions;
using Unity.VisualScripting;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance;

	[Space(5)]
	[Header("Warmness Stats")]
	public bool isAroundHot = false;

	[SerializeField] private Material freezeMaterial;
    [SerializeField] private float freezeNormIntensity = 0.5f;
	[SerializeField] private float freezeFullIntensity = 1.5f;

	private float currentFreezeIntensity = 0f;

	[SerializeField] private Image currentWarmnessBar;
	[SerializeField] private float warmnessPoints = 100f;
	[SerializeField] private float maxWarmnessPoints = 100f;
	[SerializeField] private float warmnessDecreaseTime = 2f;
	[SerializeField] private float warmnessDecreaseAmount = 1f;

	[Space(5)]
	[Header("Food Stats")]
	[SerializeField] private Image currentFoodBar;
	[SerializeField] private float hungerPoints = 100f;
	[SerializeField] private float maxFoodPoints = 100f;
	[SerializeField] private float hungerDecreaseTime = 3f;
	[SerializeField] private float hungerDecreaseAmount = 1f;

	[Space(5)]
	[Header("Health Stats")]
	[SerializeField] private Material damageMaterial;
    [SerializeField] private float vignetteIntensity = 1f;

	private float currentVignetteIntensity = 0f;
	private bool tookDamage = false;

	[SerializeField] private Image currentHealthBar;
	[SerializeField] private float healthPoints = 100f;
	[SerializeField] private float maxHitPoints = 100f;
	[SerializeField] private float healthReduction = 1f;
	[SerializeField] private float healthDecreaseTime = 1f;

	[Space(5)]
	[Header("Hydration Stats")]
	[SerializeField] private Image currentWaterBar;
	[SerializeField] private float waterPoints = 100f;
	[SerializeField] private float maxWaterPoints = 100f;
	[SerializeField] private float hydrationDecreaseTime = 1.5f;
	[SerializeField] private float hydrationDecreaseAmount = 1f;

	[Space(5)]
	[SerializeField] private float regenTime = 3f;

	private float hydrationTimer = 0;
	private float hungerTimer = 0;
	private float warmnessTimer = 0;
	private float half = 0.5f;

	void Awake()
	{
		Instance = this;
	}
	
  	void Start()
	{
		StartCoroutine(checkHydration());
		StartCoroutine(checkHunger());
		StartCoroutine(checkWarmness());

		UpdateGraphics();
	}

    private void UpdateTimers()
    {
        //When the timers hit 0 we should decrease hydration / hunger bars amnd reset our timers.

		if(hydrationTimer <= 0)
		{
			ReduceHydration(hydrationDecreaseAmount);
			hydrationTimer = hydrationDecreaseTime;
		}

		if(hungerTimer <= 0)
		{
			ReduceHunger(hungerDecreaseAmount);
			hungerTimer = hungerDecreaseTime;
		}

		if(warmnessTimer <= 0)
		{
			if(!isAroundHot)
			{
				ReduceWarmness(warmnessDecreaseAmount);
			}
			else
			{
				RestoreWarmness(warmnessDecreaseAmount);
			}
			warmnessTimer = warmnessDecreaseTime;
		}

		hydrationTimer -= Time.deltaTime;
		hungerTimer -= Time.deltaTime;
		warmnessTimer -= Time.deltaTime;
    }

    void Update()
	{
		UpdateTimers();
		if(healthPoints <= 0)
		{
			Die();
		}

		updateFreezeScreen();
	}

    private void updateFreezeScreen()
    {
		currentFreezeIntensity = Mathf.Lerp(freezeFullIntensity, freezeNormIntensity, warmnessPoints / maxWarmnessPoints);
        freezeMaterial.SetFloat("_VignetteIntensity", currentFreezeIntensity);
    }

    private void Die()
    {
        healthPoints = 0;
    }

    private IEnumerator checkHydration()
    {
		yield return new WaitForSeconds(hydrationDecreaseTime);
		if(waterPoints <= 0)
		{
			healthPoints -= healthReduction;
			if(!tookDamage)
			{
				tookDamage = true;
				float time = 0;
				while(time <= hungerDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(0, vignetteIntensity, time / (hungerDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				time = 0;
				while(time <= hungerDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(vignetteIntensity, 0, time / (hungerDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				tookDamage = false;
			}
		}

		StartCoroutine(checkHydration());
		yield return null;
    }
    private IEnumerator checkHunger()
    {
		yield return new WaitForSeconds(hungerDecreaseTime);
		if(hungerPoints <= 0)
		{
			healthPoints -= healthReduction;
			if(!tookDamage)
			{
				tookDamage = true;
				float time = 0;
				while(time <= hungerDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(0, vignetteIntensity, time / (hungerDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				time = 0;
				while(time <= hungerDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(vignetteIntensity, 0, time / (hungerDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				tookDamage = false;
			}
		}
		StartCoroutine(checkHunger());
		yield return null;
    }
	private IEnumerator checkWarmness()
    {
		yield return new WaitForSeconds(warmnessDecreaseTime);
		if(warmnessPoints <= 0)
        {
			healthPoints -= healthReduction;
			if(!tookDamage)
			{
				tookDamage = true;
				float time = 0;
				while(time <= warmnessDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(0, vignetteIntensity, time / (warmnessDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				time = 0;
				while(time <= warmnessDecreaseTime * half)
				{
					currentVignetteIntensity = Mathf.Lerp(vignetteIntensity, 0, time / (warmnessDecreaseTime * half));
					damageMaterial.SetFloat("_VignetteIntensity", currentVignetteIntensity);
					time += Time.deltaTime;
					yield return null;
				}
				tookDamage = false;
			}
        }
        StartCoroutine(checkWarmness());
		yield return null;
    }

	private IEnumerator regenHunger(int hungerAmount)
	{
		if(hungerAmount == 0) { yield break; }

		float timeleft = regenTime;
		while(timeleft > 0.0f)
		{
			RestoreHunger(hungerAmount / regenTime);
			timeleft -= 1f;
			UpdateGraphics();
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
	private IEnumerator regenHealth(int healthAmount)
	{
		if(healthAmount == 0) { yield break; }

		float timeleft = regenTime;
		while(timeleft > 0.0f)
		{
			RestoreHealth(healthAmount / regenTime);
			timeleft -= 1f;
			UpdateGraphics();
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
	private IEnumerator regenHydration(int hydrationAmount)
	{
		if(hydrationAmount == 0) { yield break; }

		float timeleft = regenTime;
		while(timeleft > 0.0f)
		{
			RestoreHydration(hydrationAmount / regenTime);
			timeleft -= 1f;
			UpdateGraphics();
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}
	private IEnumerator regenWarmness(int warmness)
	{
		if(warmness == 0) { yield break; }

		float timeleft = regenTime;
		while(timeleft > 0.0f)
		{
			RestoreWarmness(warmness / regenTime);
			timeleft -= 1f;
			UpdateGraphics();
			yield return new WaitForSeconds(1f);
		}
		yield return null;
	}

	//Better to have 1 function that can call 4 different functions that can each stop immediately
	//than have 4 functions run at the same time and get called n times when the respective resource
	//(hunger) is 0 lmao
	public void Regen(int hunger, int health, int hydration, int warmness)
	{
		StartCoroutine(regenHunger(hunger));
		StartCoroutine(regenHealth(health));
		StartCoroutine(regenHydration(hydration));
		StartCoroutine(regenWarmness(warmness));
	}

	public void TakeDamage(float Damage)
	{
		healthPoints -= Damage;
		if (healthPoints < 1)
			healthPoints = 0;

		UpdateGraphics();
	}

	private void UpdateHealthBar()
	{
		float ratio = healthPoints / maxHitPoints;
		currentHealthBar.fillAmount = ratio;
	}

	private void UpdateHydrationBar()
	{
		float ratio = waterPoints / maxWaterPoints;
		currentWaterBar.fillAmount = ratio;
	}

	private void UpdateHungerBar()
	{
		float ratio = hungerPoints / maxFoodPoints;
		currentFoodBar.fillAmount = ratio;
	}

	private void UpdateWarmnessBar()
	{
		float ratio = warmnessPoints / maxWarmnessPoints;
		currentWarmnessBar.fillAmount = ratio;
	}

	public void RestoreHealth(float Heal)
	{
		healthPoints += Heal;
		if (healthPoints > maxHitPoints) 
			healthPoints = maxHitPoints;
	}

	public void RestoreHydration(float Mana)
	{
		waterPoints += Mana;
		if (waterPoints > maxWaterPoints) 
			waterPoints = maxWaterPoints;
	}

	public void RestoreHunger(float hungerAmount)
	{
		hungerPoints += hungerAmount;
		if (hungerPoints > maxWaterPoints) 
			hungerPoints = maxWaterPoints;
	}

	public void RestoreWarmness(float hot)
	{
		warmnessPoints += hot;
		if (warmnessPoints > maxWarmnessPoints) 
			warmnessPoints = maxWarmnessPoints;
	}

	public void ReduceWarmness(float hot)
	{
		warmnessPoints -= hot;
		if (warmnessPoints < 0) 
			warmnessPoints = 0;

		UpdateGraphics();
	}

	public void ReduceHydration(float waterAmount)
	{
		waterPoints -= waterAmount;
		if (waterPoints < 1)
			waterPoints = 0;

		UpdateGraphics();
	}

	public void ReduceHunger(float hungerAmount)
	{
		hungerPoints -= hungerAmount;
		if (hungerPoints < 1)
			hungerPoints = 0;

		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		UpdateHealthBar();
		UpdateHydrationBar();
		UpdateHungerBar();
		UpdateWarmnessBar();
	}
}
