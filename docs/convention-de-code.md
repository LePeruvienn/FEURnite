# Convention de code 

## 1. Les Scripts C#

Dans ce projet, nous utiliserons le **camelCase**.

Le **camelCase** consiste à écrire le nom des variables/fonctions en commençant toujours par une **minuscule** et à rajouter des majuscules seulement au début de chaque nouveau mot.

ex :

```
chienChaud
variableStyle
maxenceCanard
```

### Écriture de variables publics

- Lorsqu'on écrit des variables privées, il faut alors obligatoirement que la première lettre soient une **minuscule**

```cs
public int examplePlayerController;
public string maxHealthPoints;
public bool endOfFile;
```
### Écriture de variables privées
- Lorsqu'on écrit des variables privées, il faut alors obligatoirement que le nom de la variable commence par `_`

```cs
private int _examplePlayerController;
private string _maxHealthPoints;
private bool _endOfFile;
```

### Écriture des if et else

**Règle générales**

- Mettre un espace entre le if et les parenthèses.
- Utiliser des accolades si le bloc contient plusieurs instructions.
- Pas besoin d'accolades si le bloc contient une seule instruction, mais faire une tabulation si la ligne dépasse une longueur acceptable.

```cs
// Une seule instruction, pas besoin d'accolades
if (isPlayerDead) return;

// Bloc plus long
if (isPlayerDead)
  healthBar.fillAmount = 0.0f;

// Bloc avec plusieurs instructions
if (playerHealth <= 0)
{
  isPlayerDead = true;
  playDeathAnimation();
}
else
{
  respawnPlayer();
}
```

### Écriture de fonctions

- Voici un exemple de fonction. On n'oublie pas de mettre un espace entre le nom de la fonction et les parenthèses.

```cs
public void takeDamage (float damage, bool isCriticalHit)
{
  if (isCriticalHit) damage *= 2;
  _currentHealth -= damage;
}
```

### Écriture des enums

- Les `enum` sont toujours définis avec une majuscule au début, ainsi que ses attributs

```cs
public enum ItemType
{
  None = 0,
  Weapon = 1,
  Usable = 2,
  Grenade = 3
}
```

### Écriture des switchs

- L'écriture des `switch` se fait avec des accolades et des **tabulations** à chaque nouvelle `case`

```cs
switch (weaponType)
{
  case WeaponType.Sword:
    attackPower = 10;
    break;
  
  case WeaponType.Bow:
    attackPower = 7;
    break;

  case WeaponType.Staff:
    attackPower = 5;
    break;
}
```


### Écriture des clases

- Les `class` ont toujours une **majuscule** en première lettre.
- L'utilisation de `[Header("")]` est très apprécié sur les script attachable aux objets, elle permette plus de clarté dans l'éditeur pour les varaibles **publique**

```cs
public class PlayerStats : MonoBehaviour
{
  [Header("Player Info")]
  public string playerName;
  public int playerLevel;

  [Header("Player Stats")]
  public float maxHealth;
  public float stamina;

  private float _currentHealth;
  private float _currentStamina;

  public void healPlayer (float healingAmount)
  {
    _currentHealth += healingAmount;
    if (_currentHealth > maxHealth)
      _currentHealth = maxHealth;
  }
}
```
### Écriture des namespaces

- Les namespaces permettent de regrouper des classes dans des catégories logiques. Ils commencent par une majuscule et sont parfois subdivisés avec des points.

```cs
namespace Game.Characters
{
  public class PlayerController : MonoBehaviour
  {
    // ...
  }
}
```

### Exemple de Script
```cs
using UnityEngine;

namespace Game.Combat
{
  public class PlayerCombat : MonoBehaviour
  {
    [Header("Combat Settings")]
    public float attackPower = 10f;
    public float critMultiplier = 2f;
    public float maxHealth = 100f;

    private float _currentHealth;
    private bool _isDead;

    void Start()
    {
      _currentHealth = maxHealth;
      _isDead = false;
    }

    public void inflictDamage (float damage, bool isCritical)
    {
      float totalDamage = damage;
      
      if (isCritical) 
      {
        totalDamage *= critMultiplier;
      }

      _currentHealth -= totalDamage;

      if (_currentHealth <= 0) 
      {
        _isDead = true;
        die();
      }
    }

    private void die ()
    {
      Debug.Log("Player is dead");
      // Code pour gérer la mort du joueur
    }
  }
}
```

## 2. GameObjects et Structures du Projet

### Nom des Game Objects

Les objets dans Unity doivent suivre une convention de nommage claire et concise, en **PascalCase** (chaque mot commence par une **majuscule**) :

Exemples :

- PlayerCharacter
- EnemyOrc
- MainCamera
- HealthPotion

### Nom des fichiers et dossiers

- Les noms de fichiers de scripts doivent correspondre au nom de la classe qu'ils contiennent. Par exemple, si la classe s'appelle PlayerController, le fichier doit s'appeler PlayerController.cs.
- Les dossiers doivent également suivre une hiérarchie logique avec des noms en PascalCase pour faciliter la navigation dans le projet.

```
Assets/
  Scripts/
    Player/
      PlayerController.cs
      PlayerStats.cs
    Enemies/
      EnemyAI.cs
  Prefabs/
    Characters/
      PlayerCharacter.prefab
      EnemyOrc.prefab
  Materials/
    PlayerMaterials/
      PlayerArmor.mat
      PlayerSkin.mat
```

## 3. Branches Git, Nommage et Structure

Dans ce projet, nous organisons les branches Git en fonction des **groupes de fonctionnalités** (features) pour une meilleure gestion collaborative. Voici un résumé des conventions de nommage et d'utilisation des branches.

### Branche main

- Nom : main
- Rôle : Branche principale et **stable** du projet.
- Accès : Personne ne travaille directement sur cette branche.
- Fusion : **!! Uniquement lors des releases finales !!**

### Branche dev

- Nom : dev
- Rôle : Branche principale de développement où toutes les fonctionnalités validées sont fusionnées.
- Accès : Tous les développeurs y fusionnent leurs changements **après validation**.

### Branches de groupes de fonctionnalités

Chaque **groupe de fonctionnalités** (comme map, hud, etc.) a une branche dédiée dérivée de dev. Les développeurs **créent des sous-branches** spécifiques pour travailler sur des aspects particuliers de ces fonctionnalités.

- Format des branches de groupe : `dev-[groupe]`
  - Exemple : `dev-map` pour les cartes, dev-hud pour l'interface utilisateur.

- Format des sous-branches de développeurs : `dev-[groupe]-[nom-du-développeur]`
  - Exemple : `dev-map-arthur` pour Arthur travaillant sur une feature de la map.

### Processus de travail

- Création d'une **sous-branche** pour travailler sur une **fonctionnalité** spécifique dans un **groupe**.
- Pull Request vers la **branche du groupe** après **validation** et **tests**.
- Fusion dans **dev** : La branche du groupe est fusionnée dans dev après **validation** de toutes les **fonctionnalités**.

### Exemple de Structure

```
main
|
|-- dev
|   |
|   |-- dev-map
|   |   |-- dev-map-arthur
|   |   |-- dev-map-maxence
|   |
|   |-- dev-hud
|   |   |-- dev-hud-lechevaldemaxence
|   |   |-- dev-hud-dorysan
|
```
