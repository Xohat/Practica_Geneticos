using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[Serializable]
public class Individual: IComparable<Individual>
{
    public float degreeX;
    public float degreeY;
    public float strength;

    public float fitness;

    public Individual(float dX,float dY, float s)
    {
        fitness = +1000f;
        degreeY = dY;
        degreeX = dX;
        strength = s;
    }

    public int CompareTo(Individual other)
    {
        return fitness.CompareTo(other.fitness);
    }
}
