using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance;

	[Space(5)]
	[Header("Warmness Stats")]
	public bool isAroundHot = false;
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

	private float timeleft = 0.0f;

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
		}
		StartCoroutine(checkWarmness());
		yield return null;
    }
    public void Regen(int hungerAmount, int healthAmount, int hydrationAmount, int warmness)
	{
		//Start the coroutine in a different function because readable code
		timeleft = regenTime;
		StartCoroutine(regenCoroutine(hungerAmount, healthAmount, hydrationAmount, warmness));
	}
	private IEnumerator regenCoroutine(int hungerAmount, int healthAmount, int hydrationAmount, int warmness)
	{
		RestoreWarmness(warmness);
		while(timeleft > 0.0f)
		{
			RestoreHydration(hydrationAmount / regenTime);
			RestoreHunger(hungerAmount / regenTime);
			RestoreHealth(healthAmount / regenTime);

			UpdateGraphics();
			timeleft -= 1f;
			yield return new WaitForSeconds(1f);
		}
		yield return null;
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
