using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
	public static HealthSystem Instance;

	[Header("Decreasing Stats")]
	//The frequency at which we will deduce hydration / hunger.
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
	[SerializeField] private Image currentHealthGlobe;
	[SerializeField] private TextMeshProUGUI healthText;
	[SerializeField] private float hitPoint = 100f;
	[SerializeField] private float maxHitPoint = 100f;

	[Space(5)]
	[Header("Hydration Stats")]
	[SerializeField] private Image currentHydrationBar;
	[SerializeField] private TextMeshProUGUI hydrationText;
	[SerializeField] private float hydrationPoint = 100f;
	[SerializeField] private float maxhydrationPoint = 100f;

	[Space(5)]
	[SerializeField] private float regenTime = 3f;
	private float timeleft = 0.0f;

	void Awake()
	{
		Instance = this;
	}
	
  	void Start()
	{
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
	private void UpdateHealthGlobe()
	{
		float ratio = hitPoint / maxHitPoint;
		currentHealthGlobe.rectTransform.localPosition = new Vector3(0, currentHealthGlobe.rectTransform.rect.height * ratio - currentHealthGlobe.rectTransform.rect.height, 0);
		healthText.text = hitPoint.ToString("0") + "/" + maxHitPoint.ToString("0");
	}

	public void TakeDamage(float Damage)
	{
		hitPoint -= Damage;
		if (hitPoint < 1)
			hitPoint = 0;

		UpdateGraphics();
	}

	public void RestoreHealth(float Heal)
	{
		hitPoint += Heal;
		if (hitPoint > maxHitPoint) 
			hitPoint = maxHitPoint;

		UpdateGraphics();
	}

	private void UpdateHydrationBar()
	{
		float ratio = hydrationPoint / maxhydrationPoint;
		currentHydrationBar.rectTransform.localPosition = new Vector3(currentHydrationBar.rectTransform.rect.width * ratio - currentHydrationBar.rectTransform.rect.width, 0, 0);
		hydrationText.text = hydrationPoint.ToString ("0") + "/" + maxhydrationPoint.ToString ("0");
	}
	private void UpdateHungerBar()
	{
		float ratio = hungerPoint / maxFoodPoint;
		currentFoodBar.rectTransform.localPosition = new Vector3(currentFoodBar.rectTransform.rect.width * ratio - currentFoodBar.rectTransform.rect.width, 0, 0);
		FoodText.text = hungerPoint.ToString ("0") + "/" + maxFoodPoint.ToString ("0");
	}

	public void ReduceHydration(float waterAmount)
	{
		hydrationPoint -= waterAmount;
		if (hydrationPoint < 1)
			hydrationPoint = 0;

		UpdateGraphics();
	}

	public void RestoreHydration(float Mana)
	{
		hydrationPoint += Mana;
		if (hydrationPoint > maxhydrationPoint) 
			hydrationPoint = maxhydrationPoint;

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
		if (hungerPoint > maxhydrationPoint) 
			hungerPoint = maxhydrationPoint;

		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		UpdateHealthGlobe();
		UpdateHydrationBar();
		UpdateHungerBar();
	}
}
