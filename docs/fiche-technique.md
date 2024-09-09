# Sommaire
- [Éléments de la carte](#éléments-de-la-carte)
  - [Île principale](#île-principale)
  - [Îles périphériques](#îles-périphériques)
  - [Plateforme](#plateforme)
  - [Coffre](#coffre)
- [Personnages](#personnages)
  - [Koua](#koua)
- [Raretés](#raretés)
- [Armes](#armes)
  - [AK-47](#ak-47)
  - [Glock 17](#glock-17)
  - [Bazooka](#bazooka)
  - [Desert Eagle](#desert-eagle)
  - [Sniper](#sniper)
  - [M16](#m16)
- [Objets](#objets)
  - [Kit de soin](#kit-de-soin)
  - [Potion bouclier](#potion-bouclier)
- [Grenades](#grenades)
  - [Grenade Explosive](#grenade-explosive)


# Elements de la carte

## Ile principale
C'est l'ile la plus grande (beaucoup beaucoup beaucoup plus grande que les autres) de la carte.
C'est ici ou se déroulera la plupart des combats, il y aura alors des cachette mais aussi des parcours qui mene à des coffre légendaire.

Cette ile se situe au centre de la carte.

Après 15 minute de jeu, les coté de l'ile commence à tomber petit à petit ...

## Iles périphériques
Ces ile sont présente à la périphérie de l'ile principale. Il existe plusieur type de ces iles :
- **Ile de spawn** : 8 au total
    - C'est ici ou les joueurs apparaissent au début de la partie
    - *Coffres* : 1 à 2 coffres maximum
    - *Taille* : petite
- **Ile intermédiaire** : 8 au total
    - C'est l'ile la plus proche à l'*ile de spawn*, elle sont aussi le premier point de contact avec d'autres joueurs
    - *Coffres* : 2 à 3 coffres maximum
    - *Taille* : moyenne
- **Ile secondaire** : 4 au total
    - C'est l'ile la plus proche à l'ile principale, elle est la porte d'entrée à l'ile principale. Elle est situé après l'ile intermédiaire.
    - *Coffres* : 3 à 5 coffres maximum
    - *Taille* : grande
- **Ile raccourci** : 4 au total
    - Ce sont des iles qui sont relier directement à l'ile principale mais elle nécessite de passer par un parcours long et difficile pour l'atteindre.
    - *Coffres* : 2 à 3 coffres maximum
    - *Taille* : très petite

*(Allez voir le shéma des ile dans le GDD, pour une explication visuelle)*

## Plateforme
Les plateforme sont des petite bout d'ile qui consitue des parcours que le joueur peut effectuer pour atteindre d'autres ile ou d'autres zone de la carte.

Les plateforme peuvent avoir des forme varié, mais reste quand meme assez petites.

## Coffre
Les coffres disséminés sur la carte qui contiennent des objets précieux. Les objets qui sont données sont aléatoire en fonction de la rareté du coffre.

### Attributs 
- Apparition
    - Certains coffres sont fixes et toujours présents, tandis que d'autres apparaissent aléatoirement à plusieur endroit prédéfinis
- Rareté des objets
    - La couleur des coffres (bronze, argent, or) indique la qualité des objets qu'ils contiennent (commun, rare, légendaire).

### Actions
- Ouvrir pour récupérer des objets

### Animations
- Ouverture du coffre

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
    - Couleur : *gris*
- Rare
    - Couleur : *bleu*
- Epic
    - Couleur : *Violet*
- Légendaire
    - Couleur : *dorée*

# Armes

## AK-47

Fusil automatique lourd, pas très précis mais inflige de lourd dégats

### Attributs
- **Dégats** : 30 - 45
- **Mode de tir** : automatique
- **Candence de tir** : 2 balle par secondes
- **Balles par chargeur** : 30
    - Nombre de balle à tirer avant de devoir recharger
- **Temps de rechargement** : 5 secondes
    - Temps nécessaire sans intéruption, pour pouvoir recharger les balles de l'armes complétement
- **Rareté** : Commune - Légendaire
    - Les dégats augmente en fonction de sa rareté

### Actions
- Tirer
- Recharger
- Viser
- Lacher au sol

### Animations

- Tirer
- Recharger
- Viser

## Glock 17
Pistolet semi-automatique léger et précis à courte distance, efficace en tant qu'arme secondaire.

### Attributs

- **Dégâts** : 15 - 25
- **Mode de tir** : semi-automatique
- **Cadence de tir** : 3 balles par seconde
- **Balles par chargeur** : 17
    - Nombre de balle à tirer avant de devoir recharger
- **Temps de rechargement** : 2 secondes
    - Temps nécessaire sans intéruption, pour pouvoir recharger les balles de l'armes complétement
- **Rareté** : Commune - Épique
    - Les dégâts et la précision augmentent en fonction de la rareté.

### Actions
- Tirer
- Recharger
- Viser
- Lâcher au sol

### Animations
- Tirer
- Recharger
- Viser

## Bazooka
Lance-roquettes, permet de tirer des missiles.

*IMPORTANT* : 
**LES MISSILES SONT EUX MEME DES OBJETS ILES DOIVENT AUSSI CREER POUR LE BON FONCTIONNEMENT DU LANCES-ROQUETTES !**

### Attributs

- **Dégâts** : 50 - 175
    - Les dégats augmente plus le joueur est proche de l'explosion de la roquette
- **Roquettes par chargeur** : 1
- **Temps de rechargement** : 6 secondes
    - Temps nécessaire sans intéruption, pour pouvoir recharger les balles de l'armes complétement
- **Rareté** : Epic

### Actions
- Tirer
- Recharger
- Viser
- Lâcher au sol

### Animations
- Tirer
- Recharger
- Viser
- explosion

## Desert Eagle
Pistolet semi-automatique lourd, très puissant mais avec une capacité limitée.

### Attributs

- **Dégâts** : 40 - 60
- **Mode de tir** : semi-automatique
- **Cadence de tir** : 1 balle par seconde
- **Balles par chargeur** : 7
    - Nombre de balle à tirer avant de devoir recharger
- **Temps de rechargement** : 3 secondes
    - Temps nécessaire sans intéruption, pour pouvoir recharger les balles de l'armes complétement
- **Rareté** : Épique - Légendaire
    - Les dégats augmente avec la rareté

### Actions
- Tirer
- Recharger
- Viser
- Lâcher au sol

### Animations
- Tirer
- Recharger
- Viser Desert eagle

## Sniper
Fusil à longue portée, extrêmement précis, conçu pour infliger de lourds dégâts à distance.

### Attributs

- **Dégâts** : 60 - 100
- **Mode de tir** : coup par coup
- **Cadence de tir** : 0.5 balle par seconde
- **Balles par chargeur** : 5
     - Nombre de balle à tirer avant de devoir recharger
- **Temps de rechargement** : 4 secondes
    - Temps nécessaire sans interruption pour pouvoir recharger les balles de l'arme complètement
- **Rareté** : Rare - Légendaire
    - Les dégâts et la précision augmentent avec la rareté

### Actions
- Tirer
- Recharger
- Viser (avec lunette)
- Lâcher au sol

### Animations
- Tirer
- Recharger
- Viser

## M16
Fusil d'assaut automatique et polyvalent, efficace à moyenne et longue distance avec une meilleure précision que l'AK-47.

### Attributs
- **Dégâts** : 25 - 35
- **Mode de tir** : automatique
- **Cadence de tir** : 3 balles par seconde
- **Balles par chargeur** : 30
    - Nombre de balle à tirer avant de devoir recharger
- **Temps de rechargement** : 3.5 secondes
    - Temps nécessaire sans interruption pour pouvoir recharger les balles de l'arme complètement
- **Rareté** : Commune - Légendaire
    - Les dégâts augmentent avec la rareté

### Actions
- Tirer automatique
- Recharger
- Viser
- Lâcher au sol

### Animations
- Tirer
- Recharger
- Viser


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
