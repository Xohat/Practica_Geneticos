using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public PlayerList PlayerList;
    public GameState GameState;


    public GameEvent EndGameEvent;
    public AttackResultEvent AttackResult;
    public PlayerEvent ChangeTurnEvent;

    public event Action<float, float, float, float> enemyHitted;

    private int _count = 0;
    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GameState.IsFinished = false;
        ChangeTurn();
    }

    
    public void ChangeTurn()
    {
        bool shouldFinish = _count == 1;

        var next = _count;
        _count = (_count + 1) % 2;
        GameState.CurrentPlayer = PlayerList.Players[next];
        if (!shouldFinish)
            ChangeTurnEvent.Raise(PlayerList.Players[next]);


    }

    private bool EndGameTest()
    {
        if (PlayerList.Players.Any(p => p.HP <= 0) && GeneticController1on1.finished)
        {
            GameState.IsFinished = true;
            EndGameEvent.Raise();
            return true;
        }
        return false;
    }

    public void OnAttackDone(Attack att)
    {

        Debug.Log($"Received Attack {att}");
        var hitRoll = Dice.PercentageChance();
        var result = ScriptableObject.CreateInstance<AttackResult>();
        result.IsHit = false;
        result.Attack = att;
        
        if (result.Attack != null)
        {
            result.Energy = att.AttackMade.Energy;
            if (att.Source.Energy >= att.AttackMade.Energy && hitRoll <= att.AttackMade.HitChance)
            {
                result.IsHit = true;

                result.Damage = Dice.RangeRoll(att.AttackMade.MinDam, att.AttackMade.MaxDam + 1);


                att.Target.HP -= result.Damage;

            }

            if (att.Source.Energy >= att.AttackMade.Energy)
            {
                att.Source.Energy -= result.Energy;
            }

            Debug.Log($"With Result \n    {result}");
            AttackResult.Raise(result);
        }

        enemyHitted?.Invoke(att.Target.HP, att.Target.Energy, att.Source.HP, att.Source.Energy);

        if (!EndGameTest())
            ChangeTurn();
    }
}
