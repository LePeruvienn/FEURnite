using Fusion;
using TMPro;
using UnityEngine;

namespace Starter.Platformer
{
	/// <summary>
	/// Main UI script for Platformer sample.
	/// </summary>
	public class UIPlatformer : MonoBehaviour
	{
		public GameManager GameManager;
		public CanvasGroup CanvasGroup;
		public TextMeshProUGUI Instructions;
		public TextMeshProUGUI CoinsCount;
		public TextMeshProUGUI WinnerText;

		private int _lastCoins = -1;
		private PlayerRef _winner;

		private void Update()
		{
			var player = GameManager.LocalPlayer;
			if (player == null)
			{
				CanvasGroup.alpha = 0f;
				return;
			}

			if (_winner != GameManager.Winner)
			{
				_winner = GameManager.Winner;

				var winnerObject = GameManager.Runner.GetPlayerObject(_winner);
				WinnerText.text = winnerObject != null ? $"We have a winner!\n{winnerObject.GetComponent<Player>().Nickname}" : string.Empty;
			}

			if (_lastCoins == player.CollectedCoins)
				return;

			CanvasGroup.alpha = 1f;
			_lastCoins = player.CollectedCoins;

			CoinsCount.text = $"\u00d7{_lastCoins}";
			Instructions.text = _lastCoins >= GameManager.MinCoinsToWin ? "Run to the TOP!" : $"Collect {GameManager.MinCoinsToWin} COINS";
		}
	}
}
