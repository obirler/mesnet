using System;
using System.Linq;

namespace Mesnet.Classes.Math
{
    public class Algebra
    {
        /// <summary>
        /// Solves the linear equation system like
        /// Ax + By = C
        /// Dx + Ey = F
        /// where A, B, D, E are the coefficients array members and C and F are the results array members.
        /// </summary>
        /// <param name="coefficients">The coefficients array.</param>
        /// <param name="results">The results array.</param>
        /// <returns>The solution array.</returns>
        /// <exception cref="System.ArgumentException">Throws Argument Exception when coefficients and results sizes are different.</exception>
        public static double[] LinearEquationSolver(double[,] coefficients, double[] results)
        {            
            if (coefficients.GetLength(0) != coefficients.GetLength(1) && coefficients.GetLength(0) != results.Length)
            {
                throw new ArgumentException("Different array sizes");
            }

            int count = coefficients.GetLength(0);

            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    double s = coefficients[j,i] / coefficients[i,i];
                    for (int k = i; k < count; k++)
                    {
                        coefficients[j,k] -= coefficients[i,k] * s;
                    }
                    results[j] -= results[i] * s;
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                results[i] /= coefficients[i,i];
                coefficients[i,i] /= coefficients[i,i];
                for (int j = i - 1; j >= 0; j--)
                {
                    double s = coefficients[j,i] / coefficients[i,i];
                    coefficients[j,i] -= s;
                    results[j] -= results[i] * s;
                }
            }
            
            return Enumerable.Range(0, count).Select(i => results[i] / coefficients[i,i]).ToArray();
        }

        /// <summary>
        /// Makes a number positive.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double Positive(double number)
        {
            if (number < 0)
            {
                return -1*number;
            }
            else
            {
                return number;
            }
        }

        /// <summary>
        /// Makes a number negative.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static double Negative(double number)
        {
            if (number > 0)
            {
                return -1*number;
            }
            else
            {
                return number;
            }
        }
    }
}
