using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBar : MonoBehaviour
{
    [Header("Script bar")]
    public Bar HealthBar;
    public Bar ShieldBar;
    public Shield SuperShieldBar;

    // niveau actuelle de vie
    float _healt;
    //niveau maximun de vie
    float _maxHealth = 100f;
    // niveau actuelle du bouclier 
    float _shield;
    // niveau maximun du bouclier 
    float _maxShield = 100f;
    // niveau actuelle du superbouclier 
    float _superShield;
    // niveau maximun du superbouclier 
    float _maxSuperShield = 50f;
    //nombre de second avant lesquelles la barre de vie ce regener
    float wait=3.0f;
    //nombre de pv gagner par seconde quand la barre de vie se regénère
    float ShieldUp=5;
    //La derniere fois que le joueur a reçue un coup
    float lastTimeHeat;


    //pour prendre des dommage en mode debug
    public bool debugTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        //set la barre de vie pour qu'elle parte avec le bon max de vie
        HealthBar.SetBar(_healt, _maxHealth);
        ShieldBar.SetBar(_healt, _maxHealth);
        lastTimeHeat = Time.time; 
    }
    void Awake()
    {
        //set la vie avec de base le max pour chaque
        _healt = _maxHealth;
        _shield= _maxShield;
        _superShield= _maxSuperShield;
    }

    //fonction qui regénere la barre de vie
    void UpdateSuperShield()
    {
        _superShield += ShieldUp * Time.deltaTime;
        SuperShieldBar.SetSuperShield(_superShield, _maxSuperShield);
    }

    // Update is called once per frame
    void Update()
    {
        //regarde si on peut regenere le surbouclier
        if (_superShield< _maxSuperShield && Time.time > lastTimeHeat+wait)
            UpdateSuperShield();
        //Quand il recoit un coup
        if (debugTakeDamage)
        {
            lastTimeHeat = Time.time;
            //mode debug desactivé
            debugTakeDamage = false;
            //generation du coup donner pour l'enemie (en mode debug)
            float heat = Random.Range(5f, 10f);


            if (_superShield-heat > 0f)//regarde si il possede un super bouclier
            { 
                _superShield -= heat;//simulation du coup sur le surbouclier
            }
            else
            {
                heat -= _superShield;//enleve au coup de la puissance si il lui reste un peu de surbouclier
                _superShield = 0;//  mets le surbouclier a 0

                if (_shield-heat > 0f)//regarde si il a un bouclier
                {  
                    _shield -= heat; //simulation du coup sur le bouclier
                }
                else
                {
                    heat -= _shield;//enleve au coup de la puissance si il lui reste un peu de bouclier
                    _shield = 0;//  mets le bouclier a 0
                    if (_healt-heat > 0f)//regarde si il reste de la vie
                    { 
                        _healt -= heat; //simulation du coup sur la vie
                    }
                    else
                    {
                        //mort player (il n'a plus de vie /plus de bouclier/plus de surbouclier )
                        _healt = _maxHealth;
                        _shield = _maxShield;
                    }
                }
            }
            HealthBar.SetBar(_healt, _maxHealth);//set la barre de vie en fonction du max de pv et de la vie actuelle
            ShieldBar.SetBar(_shield, _maxShield);//set la barre de bouclier en fonction du max de bouclier et du bouclier
            SuperShieldBar.SetSuperShield(_superShield, _maxSuperShield);//set la barre du super bouclier en fonction du max du super bouclier et du bouclier
        }
    }
}
