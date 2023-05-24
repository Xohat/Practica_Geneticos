using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class geneticAlgorithmOneOne
{

    public List<IndividualOneOne> population;
    private int _currentIndex;

    public int CurrentGeneration;
    public int MaxGenerations;

    float initialHealth;
    float initialEnergy;
    public geneticAlgorithmOneOne(int numberOfGenerations, int populationSize,float initialHealth, float initialEnergy)
    {
        CurrentGeneration = 0;
        MaxGenerations = numberOfGenerations;
        this.initialHealth = initialHealth;
        this.initialEnergy = initialEnergy;
        GenerateRandomPopulation(populationSize, initialHealth, initialEnergy);
    }

    public void GenerateRandomPopulation(int size, float initialHealth, float initialEnergy)
    {
        population = new List<IndividualOneOne>();
        for (int i = 0; i < size; i++)
        {
            
            population.Add(new IndividualOneOne(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),initialHealth,initialEnergy)        );
        }
        StartGeneration();
    }

    public IndividualOneOne GetFittest()
    {
        population.Sort();
        return population[0];
    }

    public void StartGeneration()
    {
        _currentIndex = 0;
        CurrentGeneration++;
    }

    public IndividualOneOne GetNext()
    {
        if (_currentIndex == population.Count)
        {
            EndGeneration();
            if (CurrentGeneration >= MaxGenerations)
            {

                TIMER.stopTimer("TIMER");
                Debug.Log(TIMER.getTimer("TIMER") / 1000.0f + " SEGUNDOS");

                return null;
            }
            StartGeneration();
        }

        return population[_currentIndex++];
    }

    public void EndGeneration()
    {
        population.Sort();
        if (CurrentGeneration < MaxGenerations)
        {
            massacre();
            CrossoverArithmetic();
            Mutation();
        }
    }


    public void massacre()
    {
        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);
    }


    public void Mutation()
    {
        int valueToModify = Random.Range(0, 4);
        foreach (var individual in population)
        {
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.moveChances[valueToModify] = Random.Range(0f, 1f);
            }
        }
    }

    public void CrossoverArithmetic()
    {


        //SELECCION
        var ind1 = population[0];
        var ind2 = population[1];


        float r = 0.6f;
        float rMinus = 1 - r;

        float x1 = r * ind1.moveChances[0] + rMinus * ind2.moveChances[0];
        float x2 = r * ind1.moveChances[1] + rMinus * ind2.moveChances[1];
        float x3 = r * ind1.moveChances[2] + rMinus * ind2.moveChances[2];
        float x4 = r * ind1.moveChances[3] + rMinus * ind2.moveChances[3];
        var new1 = new IndividualOneOne(x1, x2, x3, x4, initialHealth,initialEnergy);

        float y1 = r * ind2.moveChances[0] + rMinus * ind1.moveChances[0];
        float y2 = r * ind2.moveChances[1] + rMinus * ind1.moveChances[1];
        float y3 = r * ind2.moveChances[2] + rMinus * ind1.moveChances[2];
        float y4 = r * ind1.moveChances[3] + rMinus * ind2.moveChances[3];
        var new2 = new IndividualOneOne(y1, y2, y3, y4, initialHealth, initialEnergy);

        population.Add(new1);
        population.Add(new2);
    }
}
