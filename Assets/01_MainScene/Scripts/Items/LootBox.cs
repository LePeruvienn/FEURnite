using Starter.ThirdPersonCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Fusion;


namespace Starter.ThirdPersonCharacter
{

	public enum Status
	{
		IsOpen = 1,
		IsClose = 0
	}

	public class LootBox : NetworkBehaviour
	{
		public enum BoxRarity
		{
			Common = 1,
			Rare = 2,
			Epic = 3,
			Legendary = 4
		}

		public enum BoxType
		{
			WeaponBox = 1,
			ItemBox = 2
		}

		public BoxRarity boxRarity;
		public Status status;
		public BoxType type;
		public Animator Animator;

		public GameObject[] ListWeapon; // Liste des armes
		public GameObject[] ListItem;   // Liste des items

		private Transform _spawnItemPosition;
	   

		[Networked] private bool IsOpen { get; set; }

		public AudioSource audioSource;
		public static AudioClip lootBoxLoopSound;
		public static AudioClip openLootBoxSound;

		private NetworkObject _spawnedItem;
		
        void Start ()
		{
			// SEt spawned item
			_spawnedItem = null;

			// Charger les sons
			lootBoxLoopSound = Resources.Load<AudioClip>("SoundEffects/Chest Loop Sound");
			openLootBoxSound = Resources.Load<AudioClip>("SoundEffects/Open Chest");

			// Vérification des sons chargés
			if (lootBoxLoopSound == null)
			{
				Debug.LogError("Erreur : 'Chest Loop Sound' n'a pas pu être chargé. Vérifiez le chemin ou le fichier.");
			}
			else
			{
				Debug.Log("'Chest Loop Sound' chargé avec succès.");
			}

			if (openLootBoxSound == null)
			{
				Debug.LogError("Erreur : 'Open Chest' n'a pas pu être chargé. Vérifiez le chemin ou le fichier.");
			}
			else
			{
				Debug.Log("'Open Chest' chargé avec succès.");
			}

			// Vérification de l'AudioSource
			if (audioSource == null)
			{
				Debug.LogError("Erreur : L'AudioSource n'est pas assigné dans l'Inspector.");
				return; // Arrête l'exécution si l'AudioSource est null
			}

			// Associer le clip et jouer le son
			audioSource.clip = lootBoxLoopSound;
			audioSource.loop = true;

			if (lootBoxLoopSound != null)
			{
				Debug.Log("Lecture du son en boucle : 'Chest Loop Sound'.");
				audioSource.Play();
			}
			else
			{
				Debug.LogError("Erreur : Impossible de jouer 'Chest Loop Sound' car le clip est null.");
			}
		}

		public void Open()
		{
			Debug.Log("Attempting to open loot box...");

			if (status == Status.IsOpen)
			{
				Debug.Log("Loot box is already open.");
				return; // Si le coffre est déjà ouvert, on ne fait rien
			}

			if (Object.HasStateAuthority)
			{
				// Si ce client a l'autorité, ouvre le coffre
				Debug.Log("StateAuthority confirmed. Opening the box...");
				RPC_SetStatus(Status.IsOpen);
			}
			else
			{
				// Tous les clients peuvent exécuter ce RPC
				Debug.Log("Requesting to open loot box from any client.");
				RPC_SetStatus(Status.IsOpen);
			}
		}

		public void reset ()
		{
			// reset animation and remove item
			RPC_reset ();
			// reset status
			RPC_SetStatus(Status.IsClose);
		}

		[Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_reset () {

			// Despawn the item
			if (_spawnedItem != null) {

				Runner.Despawn (_spawnedItem);
				_spawnedItem = null;
			}
			else 
				Debug.Log ("ITEM NULL");

			// Close chest
			Animator.SetBool ("isOpen", false);
		}


		[Rpc(RpcSources.All, RpcTargets.All)]
		private void RPC_SetStatus(Status newStatus)
		{
			Debug.Log($"Setting loot box status to {newStatus}...");
			status = newStatus;

			if (newStatus == Status.IsOpen)
			{
				if (audioSource != null)
				{
					audioSource.Stop(); 
					audioSource.PlayOneShot(openLootBoxSound);
				}
				openChestAnimation();
				ChoiceBoxType(); // Ouvre le bon type de coffre
			}
		}

		private ItemRarity GetItemDrop()
		{
			int randomValue = Random.Range(0, 100);

			switch (boxRarity)
			{
				case BoxRarity.Common:
					return GetItemFromProbabilities(randomValue, 85, 12, 3, 0);
				case BoxRarity.Rare:
					return GetItemFromProbabilities(randomValue, 60, 25, 10, 5);
				case BoxRarity.Epic:
					return GetItemFromProbabilities(randomValue, 40, 30, 20, 10);
				case BoxRarity.Legendary:
					return GetItemFromProbabilities(randomValue, 30, 20, 20, 30);
				default:
					return ItemRarity.Common;
			}
		}

		private ItemRarity GetItemFromProbabilities(int randomValue, int commonProb, int rareProb, int epicProb, int legendaryProb)
		{
			if (randomValue < commonProb)
				return ItemRarity.Common;
			else if (randomValue < commonProb + rareProb)
				return ItemRarity.Rare;
			else if (randomValue < commonProb + rareProb + epicProb)
				return ItemRarity.Epic;
			else
				return ItemRarity.Legendary;
		}

		private int GetWeaponFromList()
		{
			ItemRarity itemRarity = GetItemDrop();
			List<GameObject> listObjectRecuperer = new List<GameObject>();

			foreach (GameObject wp in ListWeapon)
			{
				Item weapon = wp.GetComponent<Item>();
				if (weapon.GetRarity() == itemRarity)
				{
					listObjectRecuperer.Add(wp);
				}
			}

			if (listObjectRecuperer.Count == 0)
			{
				
				return -1;
			}

			int randomIndex = Random.Range(0, listObjectRecuperer.Count);
			return randomIndex;
		}

		private int GetItemFromList()
		{
			ItemRarity itemRarity = GetItemDrop();
			List<GameObject> listObjectRecuperer = new List<GameObject>();

			foreach (GameObject wp in ListItem)
			{
				Item item = wp.GetComponent<Item>();
				if (item.GetRarity() == itemRarity)
				{
					listObjectRecuperer.Add(wp);
				}
			}

			if (listObjectRecuperer.Count == 0)
			{
				
				return -1;
			}

			int randomIndex = Random.Range(0, listObjectRecuperer.Count);
			return randomIndex;
		}

		private void ChoiceBoxType()
		{
			Debug.Log($"Box type: {type}");
			if (type == BoxType.WeaponBox)
				OpenWeaponBox();
			else
				OpenItemBox();
		}

		private void OpenItemBox()
		{
			if (HasStateAuthority)
			{
				int index = GetItemFromList();
				// Appel du RPC pour ouvrir le coffre des items pour tous les clients
				if (index >= 0)
					RPC_OpenItemBox(index);
			}
			
		}
		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		private void RPC_OpenItemBox(int index)
		{
			Debug.Log("Opening item box...");
			GameObject item = ListItem[index];

			if (item != null)
			{
				if (_spawnItemPosition == null)
				{
					_spawnItemPosition = transform.Find("spawnObjectPos").transform;
				}

				// Spawn de l'item sur tous les clients sans autorité spécifique
				NetworkObject netObj = item.GetComponent<NetworkObject>();
				if (netObj != null)
				{
					
					_spawnedItem = Runner.Spawn(item, _spawnItemPosition.position, Quaternion.identity, null);

					Debug.Log(_spawnedItem);
					Debug.Log("Item spawned successfully on all clients with no specific authority.");
				}
			}
		}

		// Correction du RPC pour les armes
		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		private void RPC_OpenWeaponBox(int index)
		{
		   

			Debug.Log("Opening weapon box");
			GameObject weapon = ListWeapon[index];

			if (weapon != null)
			{
				Item item = weapon.GetComponent<Item>();

				if (item != null)
				{
					if (_spawnItemPosition == null)
						_spawnItemPosition = transform.Find("spawnObjectPos").transform;

					// Spawn l'objet sur tous les clients sans donner d'autorité spécifique
					NetworkObject netObj = weapon.GetComponent<NetworkObject>();
					if (netObj != null)
					{
						_spawnedItem = Runner.Spawn(weapon, _spawnItemPosition.position, Quaternion.identity, null);
					   
						Debug.Log("Weapon spawned successfully on all clients with no specific authority.");
					}
				}
			}
		}



		


		private void OpenWeaponBox()
		{
			if (HasStateAuthority)
			{
				int index = GetWeaponFromList();
				// Appel du RPC pour ouvrir le coffre pour tous les clients
				if (index >= 0)
					RPC_OpenWeaponBox(index);
			}
			else
			{
				Debug.LogWarning("Only the player with state authority can open the weapon box.");
			}
		}

		private void openChestAnimation()
		{
			Debug.Log("Playing open chest animation...");
			if (Animator.GetBool("isOpen") == false)
			{
				Animator.SetBool("isOpen", true);
			}
		}
	}
}
