using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Lib
{
    class GA
    {
        Random random = new Random();
        public string rep1 = "";

        public int[] genetic(Range[] chromosomeRange, int populationSize, double mutationRate, double crossoverRate, double retentionRate, int generationNo, Func<int[], double> fitness)
        {
            ////////Generate a random generation
            int chromLength = chromosomeRange.Length;
            int[][] generation = new int[populationSize][];
            double[] fitnessValues = new double[populationSize];
            double minFitness = 0;
            double maxFitness = 0;
            int[] return_value = new int[chromLength];
            int[] selected1 = new int[chromLength];
            int[] selected2 = new int[chromLength];
            int[][] tempGeneration = new int[populationSize][];
            int[] child1 = new int[chromLength];
            int[] child2 = new int[chromLength];
            int[] globalBestMatch = new int[chromLength];
            double globalBestFitness = double.MinValue;

            for (int i = 0; i < populationSize; i++)
                tempGeneration[i] = new int[chromLength];

            for (int i = 0; i < populationSize; i++)
                generation[i] = new int[chromLength];

            for (int i = 0; i < populationSize; i++)
                for (int j = 0; j < chromLength; j++)
                    generation[i][j] = random.Next(chromosomeRange[j].getMin(), chromosomeRange[j].getMax());

            for (int genCounter = 0; genCounter < generationNo; genCounter++)
            {
                ////////Calculate the fitness of each chromosome
                Console.WriteLine("chromoses' fitness:");
                for (int i = 0; i < populationSize; i++)
                {
                    fitnessValues[i] = fitness(generation[i]);
                    Console.Write(fitnessValues[i] + " [");
                    for (int j = 0; j < chromLength; j++)
                    {
                        Console.Write(generation[i][j] + ",");
                    }
                    Console.WriteLine("]");
                }

                //////Find min and max value of fitness
                minFitness = double.MaxValue;
                maxFitness = double.MinValue;
                int maxInedx = 0;
                for (int i = 0; i < populationSize; i++)
                {
                    if (fitnessValues[i] > maxFitness)
                    {
                        maxFitness = fitnessValues[i];
                        maxInedx = i;
                    }
                    if (fitnessValues[i] < minFitness)
                        minFitness = fitnessValues[i];
                    if (fitnessValues[i] > globalBestFitness)
                    {
                        globalBestFitness = fitnessValues[i];
                        for (int j = 0; j < chromLength; j++)
                        {
                            globalBestMatch[j] = generation[i][j];
                        }
                    }
                }

                Console.WriteLine("");
                var avg = average(fitnessValues);
                Console.WriteLine("Genaration No: " + (genCounter+1) + "\n  Max fitness: " + maxFitness + "\tAverage fitness: " + avg + "\n");
                rep1 += maxFitness + "," + avg + "\n";
                Console.WriteLine("--------------------");

                if (maxFitness != minFitness)
                {
                    /////normalize fitness
                    for (int i = 0; i < populationSize; i++)
                    {
                        fitnessValues[i] = (fitnessValues[i] - minFitness) / (maxFitness - minFitness);
                    }
                }
                else
                {
                    for (int i = 0; i < populationSize; i++)
                    {
                        fitnessValues[i] = 1;
                    }
                }

                ////calculate selection probability
                double sum = 0;
                for (int i = 0; i < populationSize; i++)
                {
                    sum += fitnessValues[i];
                }
                for (int i = 0; i < populationSize; i++)
                {
                    fitnessValues[i] = fitnessValues[i] / sum;
                }

                ////Roulette wheel selection, mating and mutation
                for (int i = 0; i < populationSize / 2; i++)
                {
                    int k = rouletteWheel(fitnessValues);

                    for (int j = 0; j < chromLength; j++)
                        selected1[j] = generation[k][j];

                    k = rouletteWheel(fitnessValues);

                    for (int j = 0; j < chromLength; j++)
                        selected2[j] = generation[k][j];

                    if (GetRandomNumber(0, 1) <= crossoverRate)
                    {
                        //Cross over
                        int randomCrossPoint = random.Next(0, chromLength);
                        for (int j = 0; j < randomCrossPoint; j++)
                        {
                            child1[j] = selected1[j];
                            child2[j] = selected2[j];
                        }
                        for (int j = randomCrossPoint; j < chromLength; j++)
                        {
                            child1[j] = selected2[j];
                            child2[j] = selected1[j];
                        }

                        for (int j = 0; j < chromLength; j++)
                        {
                            selected1[j] = child1[j];
                            selected2[j] = child2[j];
                        }
                    }

                    ////Mutation
                    for (int j = 0; j < chromLength; j++)
                    {
                        if (GetRandomNumber(0, 1) < mutationRate)
                        {
                            double addRandom = GetRandomNumber(-1, 1);
                            addRandom = Math.Pow(addRandom, 3);
                            if (addRandom >= 0)
                                addRandom *= (chromosomeRange[j].getMax() - selected1[j]);
                            else
                                addRandom *= (selected1[j] - chromosomeRange[j].getMin());
                            selected1[j] += (int)addRandom;
                        }
                        if (GetRandomNumber(0, 1) < mutationRate)
                        {
                            double addRandom = GetRandomNumber(-1, 1);
                            addRandom = Math.Pow(addRandom, 3);
                            if (addRandom >= 0)
                                addRandom *= (chromosomeRange[j].getMax() - selected2[j]);
                            else
                                addRandom *= (selected2[j] - chromosomeRange[j].getMin());
                            selected2[j] += (int)addRandom;
                        }
                    }

                    ////add to temp generation
                    for (int j = 0; j < chromLength; j++)
                        tempGeneration[i][j] = selected1[j];
                    for (int j = 0; j < chromLength; j++)
                        tempGeneration[i + populationSize / 2][j] = selected2[j];
                }

                ///////Retention
                int r;
                for (int i = 0; i < (int)(populationSize*retentionRate); i++)
                {
                    r=random.Next(0, populationSize);
                    for (int j = 0; j < chromLength; j++)
                    {
                        tempGeneration[r][j] = generation[r][j];
                    }
                }
                ////Replace generation with new generation
                for (int i = 0; i < populationSize; i++)
                    for (int j = 0; j < chromLength; j++)
                        generation[i][j] = tempGeneration[i][j];
            }
            Console.WriteLine("Global Maximum Fitness: " + globalBestFitness);
            string s = "[";
            for (int i = 0; i < chromLength; i++)
                s += globalBestMatch[i] + ", ";
            s = s.Substring(0, s.Length - 2);
            s += "]";
            Console.WriteLine("Global Best Chromosome: " + s);
            return globalBestMatch;
        }

        private int rouletteWheel(double[] fitnessValues)
        {
            List<double> l = new List<double>();
            double cumulative = 0;
            for (int i = 0; i < fitnessValues.Length; i++)
            {
                cumulative += fitnessValues[i];
                l.Add(cumulative);
            }
            double randomNo = GetRandomNumber(0, cumulative);
            int counter = 0;
            while (true)
            {
                if (randomNo < l[counter])
                    break;
                else
                    counter++;
            }
            return counter;
        }

        private double average(double[] vals)
        {
            double sum = 0;
            for (int i = 0; i < vals.Length; i++)
            {
                sum += vals[i];
            }
            return sum / (double)vals.Length;
        }

        private double GetRandomNumber(double minimum, double maximum)
        {

            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public class Range
        {
            private int min;
            private int max;
            public Range(int n, int x)
            {
                min = n;
                max = x;
            }
            public Range()
            {
                min = 0;
                max = 0;
            }
            public int getMin()
            {
                return min;
            }
            public int getMax()
            {
                return max;
            }
        }
    }
}
