using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum MutationOperator
{
    UNIFORM,
    TRADING
}
[Serializable]
public enum CrossoverOperator
{
    COMBINED,
    ARITHMETIC
}

[Serializable]
public class GeneticAlgorithm
{
    
    public List<Individual> population;
    private int _currentIndex;

    public int CurrentGeneration;
    public int MaxGenerations;

    public MutationOperator mutationOperator;
    public CrossoverOperator crossoverOperator;

    public string Summary;
    public GeneticAlgorithm(int numberOfGenerations, int populationSize, MutationOperator mutationOperator, CrossoverOperator crossoverOperator)
    {
        CurrentGeneration = 0;
        MaxGenerations = numberOfGenerations;

        this.mutationOperator = mutationOperator;
        this.crossoverOperator = crossoverOperator;

        GenerateRandomPopulation(populationSize);
        Summary = "";
    }
    public void GenerateRandomPopulation(int size)
    {
        population = new List<Individual>();
        for (int i = 0; i < size; i++)
        {
            population.Add(new Individual(Random.Range(0f, 90f), Random.Range(-60f, 60f), Random.Range(0f,12f)));
        }
        StartGeneration();
    }

    public Individual GetFittest()
    {
        population.Sort();
        return population[0];
    }


    public void StartGeneration()
    {
        _currentIndex = 0;
        CurrentGeneration ++;
    }
    public Individual GetNext()
    {
        if (_currentIndex == population.Count)
        {
            EndGeneration();
            if (CurrentGeneration >= MaxGenerations)
            {
                Debug.Log(Summary);

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
        Summary += $"{GetFittest().fitness};";
        if (CurrentGeneration < MaxGenerations)
        {
            massacre();
            switch (crossoverOperator)
            {
                case CrossoverOperator.ARITHMETIC:
                    CrossoverArithmetic();
                    break;
                case CrossoverOperator.COMBINED:
                    CrossoverCombined();
                    break;

            }
            switch (mutationOperator)
            {
                case MutationOperator.UNIFORM:
                    mutationUniform();
                    break;
                case MutationOperator.TRADING:
                    mutationTrading();
                    break;

            }
        }
    }

    public void massacre()
    {
        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);
    }

    public void CrossoverArithmetic()
    {
        //SELECCION
        var ind1 = population[0];
        var ind2 = population[1];

        
        float r = 0.6f;
        float rMinus = 1 - r;

        float x1 = r * ind1.degreeX + rMinus * ind2.degreeX;
        float x2 = r * ind1.degreeY + rMinus * ind2.degreeY;
        float x3 = r * ind1.strength + rMinus * ind2.strength;
        var new1 = new Individual(x1, x2, x3);

        float y1 = r * ind2.degreeX + rMinus * ind1.degreeX;
        float y2 = r * ind2.degreeY + rMinus * ind1.degreeY;
        float y3 = r * ind2.strength + rMinus * ind1.strength;
        var new2 = new Individual(y1, y2, y3);

        population.Add(new1);
        population.Add(new2);
    }
    public void CrossoverCombined()
    {
        //SELECCION
        var ind1 = population[0];
        var ind2 = population[1];
        float h;

        float alpha = 0.1f;
        float[] values = new float[2];

        for(int i = 0; i<2; ++i)
        {
            h = Mathf.Abs(ind1.degreeX - ind2.degreeX) * alpha;

            values[0] = ind1.degreeX - h; values[1] = ind2.degreeX + h;

            int index = Random.Range(0, values.Length);
            float x1 = values[index];

            h = Mathf.Abs(ind1.degreeY - ind2.degreeY) * alpha;
            values[0] = ind1.degreeY - h; values[1] = ind2.degreeY + h;
            index = Random.Range(0, values.Length);
            float x2 = values[index];

            h = Mathf.Abs(ind1.strength - ind2.strength) * alpha;
            values[0] = ind1.strength - h; values[1] = ind2.strength + h;
            index = Random.Range(0, values.Length);
            float x3 = values[index];
            var new1 = new Individual(x1, x2, x3);

            population.Add(new1);
        }


    }

    public void Crossover()
    {


        //SELECCION
        var ind1 = population[0];
        var ind2 = population[1];
        //

        //Cruce Plano Mono Punto//
        var new1 =new Individual(ind1.degreeX,ind1.degreeY, ind2.strength);
        var new2 = new Individual(ind2.degreeX, ind1.degreeY, ind1.strength);

        //REEMPLAZO
        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);
        population.Add(new1);
        population.Add(new2);
    }

    public void mutationUniform()
    {
        foreach(var individual in population)
        {
            int genToModify = Random.Range(0, 3);
            if (Random.Range(0f, 1f) < 0.02f)
            {
                if (genToModify == 0)
                {
                    individual.degreeX = Mathf.Clamp(Random.Range(individual.degreeX - 15, individual.degreeX + 15), 0, 90);
                }
                else if (genToModify == 1)
                {
                    individual.degreeY = Mathf.Clamp(Random.Range(individual.degreeY - 15, individual.degreeY + 15), -60, 60);
                }
                else if (genToModify == 2)
                {
                    individual.strength = Mathf.Clamp(Random.Range(individual.strength - 3, individual.strength + 3), 0, 12);
                }
                else
                {
                    Debug.Log("Not existing gen to modify");
                }
            }
        }
    }
    public void mutationTrading()
    {
        foreach (var individual in population)
        {

            if (Random.Range(0f, 1f) < 0.02f)
            {
                float cacheY = individual.degreeY;
                individual.degreeY = individual.degreeX;
                individual.degreeX = cacheY;
            }
        }
    }
    public void Mutation()
    {
        foreach (var individual in population)
        {
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.degreeX = Random.Range(0f, 90f);
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.degreeY = Random.Range(0f, 90f);
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.strength = Random.Range(0f, 12f);
            }
        }
    }
}
