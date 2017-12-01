/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/
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

        public static double Combination(int n, int k)
        {
            double sum = 0;
            for (long i = 0; i < k; i++)
            {
                sum += System.Math.Log10(n - i);
                sum -= System.Math.Log10(i + 1);
            }
            return System.Math.Pow(10, sum);
        }
    }
}
