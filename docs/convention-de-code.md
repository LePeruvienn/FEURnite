# Convention de code 

Dans ce projet, nous utiliserons le **camelCase**.

Le **camelCase** consiste à écrire le nom des variables/fonctions en commençant toujours par une **minuscule** et à rajouter des majuscules seulement au début de chaque nouveau mot.

ex:
```cs
public int examplePlayerController;
public string maxHealthPoints;
public bool endOfFile;
```
**Écriture de variables privées**
- Lorsqu'on écrit des variables privées, il faut alors obligatoirement que le nom de la variable commence par `_`
ex: 
```cs
private int _examplePlayerController;
private string _maxHealthPoints;
private bool _endOfFile;
```

**Écriture des if et else**
- Voici un exemple de `if`et `else`. On n'oublie pas de mettre un espace entre le `if`/`else`et les parenthèses.
```cs
if (isSpecialDamage)
{
  totalDamage *= DamageMultiplier;
}
```

**Écriture de fonctions**
- Voici un exemple de fonction. On n'oublie pas de mettre un espace entre le nom de la fonction et les parenthèses.
```cs
public void inflictDamage (float damage, bool isSpecialDamage)
{
  // ...
}
```

## Exemple global
```cs
public float damageMultiplier = 1.5f;
public float maxHealth;
public bool isInvincible;

private bool _isDead;
private float _currentHealth;

// parameters
public void inflictDamage (float damage, bool isSpecialDamage)
{
    // local variable
    int totalDamage = damage;

    // local variable versus public member variable
    if (isSpecialDamage)
    {
        totalDamage *= damageMultiplier;
    }

    // local variable versus private member variable
    if (totalDamage > _currentHealth)
    {
        /// ...
    }
}
```
