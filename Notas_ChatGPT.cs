PARTE 1. Creación de un sistema de disparo

Generar un proyecto que aprenda sobre el escenario propuesto
de la plataforma donde se deberán recoger las siguientes variaciones:
- El objetivo puede estar ubicado en cualquier posición de la pared frontal de disparo
- Podrán existir obstáculos fijos en la trayectoria de tiro

Se pide analizar los resultados con tres posibles operadores de cruce y/o mutación y con 3
tamaños diferentes de población. Un total de 9 posibles escenarios. 

El algoritmo genético consiste en varias partes:

	Población: Un conjunto de posibles soluciones al problema, en este caso, diferentes combinaciones de parámetros de disparo.

	Genes: Los parámetros individuales que conforman una solución, en este caso, podrían ser el ángulo y la fuerza de disparo.

	Fitness: Una función que evalúa qué tan buena es una solución, en este caso, qué tan cerca llega el proyectil al objetivo.

	Selección: Un proceso por el cual se eligen las mejores soluciones de la población actual para generar la siguiente generación.

	Crossover (Recombinación): Un proceso por el cual se combinan las soluciones seleccionadas para generar nuevas soluciones.

	Mutación: Un proceso por el cual se alteran aleatoriamente partes de las soluciones para mantener la diversidad en la población.

Aquí hay un ejemplo de cómo podrías implementar estas partes en C# para Unity:

public class GeneticAlgorithm
{
    public int populationSize;
    public float mutationRate;
    public CatapultController catapult;

    private List<Individual> population;
    private float totalFitness;

    public GeneticAlgorithm(int populationSize, float mutationRate, CatapultController catapult)
    {
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.catapult = catapult;

        // Inicializar población
        population = new List<Individual>();
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new Individual());
        }
    }

    public void NextGeneration()
    {
        // Calcular fitness total
        totalFitness = 0;
        foreach (Individual individual in population)
        {
            individual.fitness = CalculateFitness(individual);
            totalFitness += individual.fitness;
        }

        // Crear nueva población
        List<Individual> newPopulation = new List<Individual>();

        for (int i = 0; i < populationSize; i++)
        {
            // Selección
            Individual parent1 = SelectParent();
            Individual parent2 = SelectParent();

            // Crossover
            Individual offspring = parent1.Crossover(parent2);

            // Mutación
            offspring.Mutate(mutationRate);

            newPopulation.Add(offspring);
        }

        population = newPopulation;
    }

    private float CalculateFitness(Individual individual)
    {
        // Aquí necesitarías algún código para disparar la catapulta con los parámetros del individuo
        // y calcular qué tan cerca llegó al objetivo.
        // Esto puede ser complicado de implementar, ya que la física de Unity es determinista pero puede variar entre ejecuciones
        // y también puede variar dependiendo del framerate. Te recomendaría que investigues sobre cómo simular física en Unity
        // si necesitas implementar esto.
        // Por ahora, solo devolveremos una fitness aleatoria.
        return Random.Range(0f, 1f);
    }

    private Individual SelectParent()
    {
        // Ruleta de selección
        float random = Random.Range(0, totalFitness);
        foreach (Individual individual in population)
        {
            if (random < individual.fitness)
            {
                return individual;
            }

            random -= individual.fitness;
        }

        return null;
	}
}

public class Individual
{
	public float angle;
	public float force;
	public float fitness;
	
	public Individual()
	{
		// Inicializar genes aleatoriamente
		angle = Random.Range(0f, 180f);
		force = Random.Range(0f, 1f);
	}

	public Individual Crossover(Individual other)
	{
		// Crossover de un punto
		Individual offspring = new Individual();
		offspring.angle = Random.value < 0.5f ? angle : other.angle;
		offspring.force = Random.value < 0.5f ? force : other.force;
		return offspring;
	}

	public void Mutate(float rate)
	{
		// Mutación gaussiana
		if (Random.value < rate)
		{
			angle += Random.Range(-1f, 1f);
			angle = Mathf.Clamp(angle, 0f, 180f);
		}
		if (Random.value < rate)
		{
			force += Random.Range(-1f, 1f);
			force = Mathf.Clamp(force, 0f, 1f);
		}
	}
}

Deberás adaptar el código a tu necesidad y juego, especialmente en la función `CalculateFitness` donde 
necesitarás implementar la lógica para simular el disparo de la catapulta y calcular qué tan cerca llega al objetivo. 
Esto puede ser un desafío, dependiendo de la complejidad de la física de tu juego.

----------

PARTE 2. Creación de una inteligencia de controlador de combate

Asociado el segundo escenario del mismo proyecto se desea construir un controlador que
aprenda a pelear en este juego

El controlador deberá poder funcionar en modo “aprendizaje”, es decir, jugar centenares de partidas
de modo autónomo, contra otro controlador (puede ser uno complejo o simple) y cuando se haya
alcanzado la cantidad suficiente de iteraciones y el fitness se considere oportuno, pasará a modo
“juego” donde deberá enfrentarse a un jugador “humano”.

Para implementar un algoritmo evolutivo en Unity C#, primero necesitamos establecer una estructura básica para nuestro problema. 
En este caso, como estamos tratando con un juego de lucha por turnos, podemos pensar en 
el estado del juego como un conjunto de variables, como la cantidad de energía de cada jugador, la vida restante, etc.

Primero definamos las acciones posibles y las reglas del juego:

public enum Action { Dummy, Light, Heavy }

public class Player
{
    public int Energy { get; set; }
    public int Health { get; set; }

    public Player(int energy, int health)
    {
        Energy = energy;
        Health = health;
    }
}

public class GameRules
{
    public static void PerformAction(Player attacker, Player defender, Action action)
    {
        switch (action)
        {
            case Action.Dummy:
                attacker.Energy -= 100;
                break;
            case Action.Light:
                if (Random.value <= 0.75f) // 75% chance to hit
                {
                    attacker.Energy -= 30;
                    defender.Health -= Random.Range(1, 3);
                }
                break;
            case Action.Heavy:
                if (Random.value <= 0.5f) // 50% chance to hit
                {
                    attacker.Energy -= 50;
                    defender.Health -= Random.Range(3, 6);
                }
                break;
        }
    }
}

Ahora, para el algoritmo evolutivo, necesitaremos una forma de representar una "solución" o "individuo" en nuestra población. 
En este caso, podríamos pensar en un individuo como una estrategia de juego, es decir, 
una secuencia de acciones a tomar en respuesta a diferentes estados de juego. Podríamos representar esto como una lista de acciones.

public class Strategy
{
    public List<Action> Actions { get; set; }

    public Strategy()
    {
        Actions = new List<Action>();
    }
}

Para la evolución, necesitaremos una forma de generar nuevas estrategias a partir de 
las existentes (crossover) y una forma de introducir variabilidad en nuestras estrategias (mutación). 
También necesitaremos una forma de evaluar qué tan buena es una estrategia (fitness).

public class GeneticAlgorithm
{
    public List<Strategy> Population { get; set; }

    public GeneticAlgorithm()
    {
        Population = new List<Strategy>();
    }

    // Generate a new strategy by combining parts of two existing strategies
    public Strategy Crossover(Strategy parent1, Strategy parent2)
    {
        // Implement crossover logic here
    }

    // Randomly change some part of a strategy to introduce variability
    public Strategy Mutate(Strategy strategy)
    {
        // Implement mutation logic here
    }

    // Evaluate how good a strategy is
    public float Fitness(Strategy strategy)
    {
        // Implement fitness evaluation logic here
    }

    // Run the genetic algorithm for one generation
    public void Evolve()
    {
        // Implement evolution logic here
    }
}

Estos son los componentes básicos para implementar un algoritmo genético en Unity C#. 
La lógica específica para el crossover, la mutación, la evaluación de fitness y 
la evolución dependerá de los detalles de tu juego y de cómo quieras que tu controlador 
aprenda y mejore sus estrategias de combate.

Por último, para el "modo de aprendizaje" y "modo de juego", puedes tener 
un bucle principal que ejecuta el algoritmo genético para una cierta 
cantidad de generaciones y luego cambia a un modo en el que el controlador 
juega contra un jugador humano utilizando la mejor estrategia que ha encontrado.

Es importante notar que esta es una forma muy simplificada de implementar un algoritmo 
evolutivo y puede que no sea la más eficiente o efectiva para tu problema específico. 
Los algoritmos evolutivos pueden ser muy complejos y pueden requerir mucha afinación 
y experimentación para obtener buenos resultados. También existen muchas variaciones 
y técnicas avanzadas que puedes explorar, como la selección de torneos, la elitismo, 
la codificación de genes, etc.

V2 <-----------------

----

public class GeneticAlgorithm
{
    public List<Strategy> Population { get; private set; }
    private int populationSize;
    private float mutationRate;
    private int maxStrategyLength;

    public GeneticAlgorithm(int populationSize, float mutationRate, int maxStrategyLength)
    {
        this.populationSize = populationSize;
        this.mutationRate = mutationRate;
        this.maxStrategyLength = maxStrategyLength;

        // Inicializar la población
        Population = new List<Strategy>();
        for (int i = 0; i < populationSize; i++)
        {
            Strategy newStrategy = new Strategy();
            for (int j = 0; j < maxStrategyLength; j++)
            {
                newStrategy.Actions.Add((Action)Random.Range(0, 3)); // Añadir acciones aleatorias
            }
            Population.Add(newStrategy);
        }
    }

    // Cruce: Combina dos estrategias para crear una nueva
    public Strategy Crossover(Strategy parent1, Strategy parent2)
    {
        Strategy child = new Strategy();
        int crossoverPoint = Random.Range(0, maxStrategyLength); // Elegir un punto de cruce aleatorio

        for (int i = 0; i < maxStrategyLength; i++)
        {
            child.Actions.Add(i < crossoverPoint ? parent1.Actions[i] : parent2.Actions[i]);
        }

        return child;
    }

    // Mutación: Cambia aleatoriamente algunas acciones en una estrategia
    public void Mutate(Strategy strategy)
    {
        for (int i = 0; i < maxStrategyLength; i++)
        {
            if (Random.value < mutationRate)
            {
                strategy.Actions[i] = (Action)Random.Range(0, 3); // Cambiar a una acción aleatoria
            }
        }
    }

    // Fitness: Evalúa cuán buena es una estrategia. Aquí es donde necesitarías definir tu propio método de evaluación.
    public float Fitness(Strategy strategy)
    {
        // Implementar la lógica de evaluación de fitness aquí
        // Devolver un valor más alto para mejores estrategias
    }

    // Evolución: Genera una nueva población
    public void Evolve()
    {
        List<Strategy> newPopulation = new List<Strategy>();

        // Elitismo: mantener las mejores estrategias en la nueva población
        Population.Sort((s1, s2) => Fitness(s2).CompareTo(Fitness(s1))); // Ordenar por fitness
        for (int i = 0; i < populationSize / 10; i++)
        {
            newPopulation.Add(Population[i]);
        }

        // Cruce y mutación para el resto de la nueva población
        while (newPopulation.Count < populationSize)
        {
            Strategy parent1 = SelectParent();
            Strategy parent2 = SelectParent();
            Strategy child = Crossover(parent1, parent2);
            Mutate(child);
            newPopulation.Add(child);
        }

        Population = newPopulation;
    }

    // Selección: Elegir un padre para el cruce, con una mayor probabilidad para las mejores estrategias
    private Strategy SelectParent()
    {
        // Selección por torneo: elige varios candidatos al azar y selecciona el mejor
        Strategy bestCandidate = null;
        float bestFitness = float.MinValue;

        for (int i = 0; i < populationSize / 10; i++)
		{
			Strategy candidate = Population[Random.Range(0, Population.Count)];
			float candidateFitness = Fitness(candidate);
			
			if (candidateFitness > bestFitness)
			{
				bestCandidate = candidate;
				bestFitness = candidateFitness;
			}
		}	

		return bestCandidate;
	}
}


Por favor, ten en cuenta que este es solo un ejemplo y es probable que debas ajustarlo para tu problema específico. 
En particular, la función `Fitness` debe ser reemplazada por tu propio método para evaluar qué 
tan buena es una estrategia en tu juego de combate. Este es el corazón del algoritmo genético, 
y es donde se decide qué estrategias se consideran "buenas" y cuáles "malas".

Además, también podrías querer ajustar los parámetros del algoritmo genético 
(como el tamaño de la población, la tasa de mutación, etc.) y las técnicas de selección 
y cruce para ver qué funciona mejor en tu caso. La implementación de algoritmos 
genéticos puede implicar bastante experimentación y ajuste fino.

