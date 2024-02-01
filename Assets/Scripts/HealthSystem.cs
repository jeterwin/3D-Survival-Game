using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance;

	[Header("Decreasing Stats")]
	[SerializeField] private float reduceHealthPeriod = 1f;
	[SerializeField] private float healthReduction = 1f;
	//The frequency at which we will deduce hydration / hunger.
	[SerializeField] private float timeToReduceHealth;
	[SerializeField] private float hydrationDecreaseTime = 1.5f;
	[SerializeField] private float hungerDecreaseTime = 3f;
	//The amounts that we will deduce from our hydration / hunger bars.
	[SerializeField] private float hydrationDecreaseAmount = 1f;
	[SerializeField] private float hungerDecreaseAmount = 1f;
	private float hydrationTimer = 0;
	private float hungerTimer = 0;

	[Space(5)]
	[Header("Food Stats")]
	[SerializeField] private Image currentFoodBar;
	[SerializeField] private TextMeshProUGUI FoodText;
	[SerializeField] private float hungerPoint = 100f;
	[SerializeField] private float maxFoodPoint = 100f;

	[Space(5)]
	[Header("Health Stats")]
	[SerializeField] private Image currentHealthBar;
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private float healthPoint = 100f;
	[SerializeField] private float maxHitPoint = 100f;

	[Space(5)]
	[Header("Hydration Stats")]
	[SerializeField] private Image currentWaterBar;
	[SerializeField] private TextMeshProUGUI hydrationText;
	[SerializeField] private float waterPoint = 100f;
	[SerializeField] private float maxWaterPoint = 100f;

	[Space(5)]
	[SerializeField] private float regenTime = 3f;
	private float timeleft = 0.0f;

	void Awake()
	{
		Instance = this;
	}
	
  	void Start()
	{
		StartCoroutine(checkHydration());
		StartCoroutine(checkHunger());

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
		hydrationTimer -= Time.deltaTime;
		hungerTimer -= Time.deltaTime;
    }

    void Update()
	{
		UpdateTimers();
		if(healthPoint <= 0)
		{
			Die();
		}
		//Make them co-routines
		//checkHydration();
		//checkHunger();
	}

    private void Die()
    {
        healthPoint = 0;
    }

    private IEnumerator checkHydration()
    {
		yield return new WaitForSeconds(reduceHealthPeriod);
		if(waterPoint <= 0)
		{
			healthPoint -= healthReduction;
		}
		StartCoroutine(checkHydration());
		yield return null;
    }
    private IEnumerator checkHunger()
    {
		yield return new WaitForSeconds(reduceHealthPeriod);
		if(hungerPoint <= 0)
		{
			healthPoint -= healthReduction;
		}
		StartCoroutine(checkHunger());
		yield return null;
    }
    public void Regen(int hungerAmount, int healthAmount, int hydrationAmount)
	{
		//Start the coroutine in a different function because readable code
		timeleft = regenTime;
		StartCoroutine(regenCoroutine(hungerAmount, healthAmount, hydrationAmount));
	}
	private IEnumerator regenCoroutine(int hungerAmount, int healthAmount, int hydrationAmount)
	{
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
	private void UpdateHealthBar()
	{
		float ratio = healthPoint / maxHitPoint;
		currentHealthBar.fillAmount = ratio;
		//healthText.text = healthPoint.ToString("0") + "/" + maxHitPoint.ToString("0");
	}

	public void TakeDamage(float Damage)
	{
		healthPoint -= Damage;
		if (healthPoint < 1)
			healthPoint = 0;

		UpdateGraphics();
	}

	public void RestoreHealth(float Heal)
	{
		healthPoint += Heal;
		if (healthPoint > maxHitPoint) 
			healthPoint = maxHitPoint;

		UpdateGraphics();
	}

	private void UpdateHydrationBar()
	{
		float ratio = waterPoint / maxWaterPoint;
		currentWaterBar.fillAmount = ratio;
		//hydrationText.text = hydrationPoint.ToString ("0") + "/" + maxhydrationPoint.ToString ("0");
	}
	private void UpdateHungerBar()
	{
		float ratio = hungerPoint / maxFoodPoint;
		currentFoodBar.fillAmount = ratio;
		//FoodText.text = hungerPoint.ToString ("0") + "/" + maxFoodPoint.ToString ("0");
	}

	public void ReduceHydration(float waterAmount)
	{
		waterPoint -= waterAmount;
		if (waterPoint < 1)
			waterPoint = 0;

		UpdateGraphics();
	}

	public void RestoreHydration(float Mana)
	{
		waterPoint += Mana;
		if (waterPoint > maxWaterPoint) 
			waterPoint = maxWaterPoint;

		UpdateGraphics();
	}
	public void ReduceHunger(float hungerAmount)
	{
		hungerPoint -= hungerAmount;
		if (hungerPoint < 1)
			hungerPoint = 0;

		UpdateGraphics();
	}

	public void RestoreHunger(float hungerAmount)
	{
		hungerPoint += hungerAmount;
		if (hungerPoint > maxWaterPoint) 
			hungerPoint = maxWaterPoint;

		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		UpdateHealthBar();
		UpdateHydrationBar();
		UpdateHungerBar();
	}
}
