using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Starter.Shooter
{
	/// <summary>
	/// Main UI script for Shooter sample.
	/// </summary>
	public class UIShooter : MonoBehaviour
	{
		[Header("References")]
		public GameManager GameManager;
		public CanvasGroup CanvasGroup;
		public TextMeshProUGUI ChickenCount;
		public TextMeshProUGUI BestHunter;
		public GameObject AliveGroup;
		public GameObject DeathGroup;
		public Image[] HealthIndicators;
		public CanvasGroup HitIndicator;

		[Header("UI Sound Setup")]
		public AudioSource AudioSource;
		public AudioClip ChickenKillClip;
		public AudioClip HitReceivedClip;
		public AudioClip DeathClip;

		private int _lastChickens = -1;
		private int _lastHealth = -1;
		private PlayerRef _bestHunter;

		private void OnEnable()
		{
			BestHunter.gameObject.SetActive(false);
		}

		private void Update()
		{
			// Fadeout hit indicator
			HitIndicator.alpha = Mathf.Lerp(HitIndicator.alpha, 0f, Time.deltaTime * 2f);

			var player = GameManager.LocalPlayer;
			if (player == null)
			{
				CanvasGroup.alpha = 0f;
				return;
			}

			if (_bestHunter != GameManager.BestHunter)
			{
				_bestHunter = GameManager.BestHunter;

				var hunterObject = GameManager.Runner.GetPlayerObject(_bestHunter);
				BestHunter.text = hunterObject != null ? hunterObject.GetComponent<Player>().Nickname : string.Empty;
				BestHunter.gameObject.SetActive(hunterObject != null);
			}

			if (_lastHealth != player.Health.CurrentHealth)
			{
				bool isAlive = player.Health.IsAlive;

				if (_lastHealth > player.Health.CurrentHealth)
				{
					// Show hit received
					HitIndicator.alpha = 1f;

					var clip = isAlive ? HitReceivedClip : DeathClip;
					AudioSource.PlayOneShot(clip);
				}

				_lastHealth = player.Health.CurrentHealth;

				AliveGroup.SetActive(isAlive);
				DeathGroup.SetActive(isAlive == false);

				for (int i = 0; i < HealthIndicators.Length; i++)
				{
					HealthIndicators[i].enabled = _lastHealth > i;
				}
			}

			if (_lastChickens != player.ChickenKills)
			{
				if (player.ChickenKills > _lastChickens && player.ChickenKills > 0)
				{
					AudioSource.PlayOneShot(ChickenKillClip);
				}

				_lastChickens = player.ChickenKills;

				CanvasGroup.alpha = 1f;
				ChickenCount.text = $"\u00d7{_lastChickens}";
			}
		}
	}
}
