using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IndividualOneOne : IComparable<IndividualOneOne>
{

    public float fitness;

    public float energy;
    public float health;

    public float Eenergy;
    public float Ehealth;

    public float[] moveChances;

    public IndividualOneOne(float chance1, float chance2, float chance3, float chance4, float health, float energy)
    {
        this.health = health;
        this.energy = energy;
        Eenergy = energy;
        Ehealth = health;
        moveChances = new float[4] { chance1, chance2, chance3, chance4 };
        fitness = 10f;
    }
    public int CompareTo(IndividualOneOne other)
    {
        return fitness.CompareTo(other.fitness);
    }
}
