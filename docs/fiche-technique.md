# Personnages

## Koua

Personnage prinipale du jeu, c'est celui que le joueur incarne.

### Attributs :

- **Point de vie** :
    - Va de 0 à 100, si le joueur est à 0 ou moins il meurt.
- **Point de bouclier** :
    - Va de 0 à 100, si le joueur possède un boulcier alors les dégats qu'il reçoit seront absorbé par celuici.
- **Inventaire**:
    - Inventaire permettant de sotcker 4 objets différents, le koua peut sélectionner que 1 objets à la fois et peut changer la sélection à sa guise.
- **Vitesse**:
    - Vitesse de déplacement du Koua

### Actions :

- Saut *(capacité de base)*
- Courrir *(Capacité de base)*
- Tirer *(nécéssite Arme)*
- Utiliser Objet *(nécéssite Objet)*
- Lancer grenade *(nécéssite Grenade)*
- Double Saut *(à débloquer)*
    - permet de faire un saut supplémentaire dans les aires
- Dash *(à débloquer)*
    - permet de se rué vers une direction très rapidement

### Animation :
- Walking
- Running
- Crouching
- Resting
- Walking (with a weapon)
- Running (with a weapon)
- Crouching (with a weapon)
- Resting (with a weapon)
- visée
- Pick up of the ground
- Changing weapon
- Emote 
- Use object (eat)
- Throw
- Jump



# Raretés

Listes des différentes rareté possibles :
- Commun
    - Couleur : *bleu*
- Rare
    - Couleur : *Vert*
- Epic
    - Couleur : *Violet*
- Légendaire
    - Couleur : *dorée*

# Armes

## AK-47
## Glock 17
## Bazooka
## Desert eagle
## Sniper
## M16




```
Partie à compléter ...
```

# Objets

## Kit de soin

Objets que le joueur peut utiliser pour gagner de la vie

### Attributs

- **Montant de vie** : 100
    - C'est le montant de ve qui sera données à son utilisateur après l'utilisation de l'objet
- **Durée d'utilisation** : 10 seconde
    - La durée qui'il faut attendre sans intérruption pour pouvoir utilisé l'objet
- **Rareté** : Commune
    - Ses attributs ne change pas en fonction de la rareté
- **Empilable** : Non
    - Est ce que l'objet peut être emplier dans l'inventaire

### Action

- Peut être laché au sol
- Peut être utiliser


## Potion bouclier

Objets que le joueur peut utiliser pour gagner des points de bouclier

### Attributs

- **Montant de bouclier** : 50
    - C'est le montant de ve qui sera données à son utilisateur après l'utilisation de l'objet
- **Durée d'utilisation** : 5 seconde
    - La durée qui'il faut attendre sans intérruption pour pouvoir utilisé l'objet
- **Rareté** : Rare
    - Ses attributs ne change pas en fonction de la rareté
- **Empilable** : Non
    - Est ce que l'objet peut être emplier dans l'inventaire

### Action

- Peut être laché au sol
- Peut être utiliser

# Grenades

## Grenade Explosive

Ce sont des grenades qui sont faites pour être lancé sur d'autres joueur et leurs infligé des dégats

### Attributs

- Dégats : 10 - 150
    - Ce sont les dégats qui seront infligé aux joueur proches. Plus le joueur est proche de la grenade au moment de l'explosion, plus le joueur prend des dégats.
- **Distance de lancée**: Moyenne
    - C'est la distance de lancée maximale à laquelle on peut lancé la grenade
- **Rareté**: Commune - Epic
    - Ses dégats peuvent augmenter en fonction de la rareté, et peuvent allez jusqu'a 200 de dégats max
- **Empilable** : Oui
    - Jusqu'a 3 grenade max de la meme rareté

### Action

- Etre lancée
- Exploser
- Etre laché au sol
