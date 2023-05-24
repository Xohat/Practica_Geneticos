using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


public class GeneticController1on1 : AIController
{
    public GameLogic gl;

    public geneticAlgorithmOneOne Genetic;

    public IndividualOneOne CurrentIndividual;
    float chance0;
    float chance1;
    float chance2;
    float chance3;

    float currentEnemyHealth;
    float currentEnemyEnergy;

    float currentIndHealth;
    float currentIndEnergy;

    bool attacking = false;

    public static bool finished;


    public void Start()
    {
        Debug.Log("Start Genetic");
        gl.enemyHitted += GetResult;
        Time.timeScale = 50f;

        Genetic = new geneticAlgorithmOneOne(10, 12, gl.GameState.CurrentPlayer.HP, gl.GameState.CurrentPlayer.Energy);

        TIMER.startTimer("TIMER");

        finished = false;
    }

    
    protected override void Think()
    {

        CurrentIndividual = Genetic.GetNext();

        _player.HP = CurrentIndividual.health;
        _player.Energy = CurrentIndividual.energy;

        GameState.ListOfPlayers.Players[_player.EnemyId].HP = CurrentIndividual.Ehealth;
        GameState.ListOfPlayers.Players[_player.EnemyId].Energy = CurrentIndividual.Eenergy;

        geneticConfigure(
    CurrentIndividual.moveChances[0],
    CurrentIndividual.moveChances[1],
    CurrentIndividual.moveChances[2],
    CurrentIndividual.moveChances[3],
    _player.HP,
    _player.Energy,
    GameState.ListOfPlayers.Players[_player.EnemyId].HP,
    GameState.ListOfPlayers.Players[_player.EnemyId].Energy
    );

        if (CurrentIndividual != null)
        {
            _attackToDo = ScriptableObject.CreateInstance<Attack>();


            _attackToDo.AttackMade = _player.Attacks[action()];
            _attackToDo.Source = _player;
            _attackToDo.Target = GameState.ListOfPlayers.Players[_player.EnemyId];

            attacking = true;
        }
        else
        {
            CurrentIndividual = Genetic.GetFittest();
            finished = true;
        }
       
    }

    public void GetResult(float Enemylife, float Enemyenergy, float playerLife, float playerEnergy)
    {
        if (CurrentIndividual == null) return;
        if(attacking)
        {
            CurrentIndividual.fitness = Enemylife - playerLife;

            CurrentIndividual.health = Enemylife;
            CurrentIndividual.energy = Enemyenergy;

            CurrentIndividual.Ehealth = playerLife;
            CurrentIndividual.Eenergy = playerEnergy;
        }
        else
        {
            CurrentIndividual.health = Enemylife;
            CurrentIndividual.energy = Enemyenergy;

            CurrentIndividual.Ehealth = playerLife;
            CurrentIndividual.Eenergy = playerEnergy;

            CurrentIndividual.fitness = Enemylife - playerLife;
        }

        attacking = false;
    }

    public void geneticConfigure(float ch0, float ch1, float ch2, float ch3, float pH,float pE, float eH, float eE)
    {
        chance0 = ch0;
        chance1 = ch1;
        chance2 = ch2;
        chance3 = ch3;

        currentIndHealth = pH;
        currentIndEnergy = pE;

        currentEnemyHealth = eH;
        currentEnemyEnergy = eE;
    }

    public int action()
    {
        List<indexChance> chances = new List<indexChance>();
        chances.Add(new indexChance(0, chance0));
        chances.Add(new indexChance(1, chance1));
        chances.Add(new indexChance(2, chance2));
        chances.Add(new indexChance(3, chance3));

        chances.Sort();
        for (int i = chances.Count-1; i>= 0; --i)
        {
            if(Random.Range(0,1.0f) < chances[i].chance)
            {
                if (_player.Attacks[i].Energy > currentIndEnergy)
                    return 3;
                return chances[i].index;
            }
        }
        return chances[0].index;

    }

    public class indexChance: IComparable<indexChance>
    {
        public int index;
        public float chance;

        public indexChance(int indexx, float chancee)
        {
            index = indexx;
            chance = chancee;
        }
        public int CompareTo(indexChance other)
        {
            return chance.CompareTo(other.chance);
        }
    }
}

