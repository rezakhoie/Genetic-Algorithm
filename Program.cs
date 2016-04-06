using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optimization.Lib;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            GA.Range[] r=new GA.Range[20];
            for (int i = 0; i < r.Length;i++)
                r[i] = new GA.Range(0,2);
            GA ga = new GA();
            ga.genetic(r, 200, 0.001, 0.7, 0.1, 100, fitness);
            Console.ReadLine();
        }
        public static double fitness(int[] a)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += (Math.Pow(2, a.Length - i - 1)*a[i]);            
            }

            return (-Math.Pow((sum - 3),2) + 5);
        }
    }
}
