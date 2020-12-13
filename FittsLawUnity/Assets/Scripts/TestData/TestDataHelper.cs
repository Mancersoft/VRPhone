using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class contains a number of mathematical functions used by other classes with 
/// the purpose of calculating various variables related to Fitts' Law.
/// </summary>
class TestDataHelper {

    private const double LOG_TWO = 0.693147181f;
    private const double SQRT_2_PI_E = 4.132731354f;


    /// <summary>
    /// Calculate the throughput of the sequence.
    /// </summary>
    /// <param name="amplitudes"> Amplitude of the sequence.</param>
    /// <param name="dx"></param>
    /// <param name="times"> Mean completion time of the sequence of trials.</param>
    /// <returns> Returns the throughput for the sequence of trials.</returns>
    public static double CalculateThroughput(List<double> ae, List<double> dx, List<double> times ) {
        double aeMean = Mean(ae);
        double we = CalculateEffectiveWidth(dx);
        //float ide = (float)Math.Log(aeMean / we + 1f) / LOG_TWO; // bits
        double ide = Math.Log(aeMean / we + 1f, 2); // bits
        double mtMean = Mean(times) / 1000f; // seconds
        return ide / mtMean; // bits per second
    }

    /// <summary>
    /// Calculate the effective width of the sequence.
    /// </summary>
    /// <param name="dx"></param>
    /// <returns> Returns the effective width.</returns>
    public static double CalculateEffectiveWidth(List<double> dx)
    {
        return SQRT_2_PI_E * StandardDeviation(dx);
    }

    /// <summary>
    /// Calculate the effective index of difficulty.
    /// </summary>
    /// <param name="amplitudes"></param>
    /// <param name="effectiveWidth"></param>
    /// <returns> Return the index of difficulty.</returns>
    public static double CalculateEffectiveDifficultyIndex(List<double> ae, double effectiveWidth)
    {
        return Math.Log(Mean(ae) / effectiveWidth + 1f) / LOG_TWO; // bits
    }

    /// <summary>
    /// Calculate the default index of difficulty.
    /// </summary>
    /// <param name="amplitude"></param>
    /// <param name="width"></param>
    /// <returns>Return the index of difficulty.</returns>
    public static double CalculateIndexOfDifficulty(double amplitude, double width) {
        return Math.Log(amplitude / width + 1) / LOG_TWO;
    }
    
    /// <summary>
    /// Calculate the mean of the values in a list of floats. 
    /// </summary>
    /// <param name="n"> List of floats n.</param>
    /// <returns> Return the mean.</returns>
    public static double Mean(List<double> n )
    {
        double mean = 0;
        foreach (double val in n)
            mean += val;
        return mean / n.Count;
    }


    /// <summary>
    /// Convert a list of ints to a list of floats.
    /// </summary>
    /// <param name="n"> List of ints n.</param>
    /// <returns> Return the list.</returns>
    public static double Mean(List<int> n)
    {
        return Mean(new List<double>(n.ConvertAll(x => (double)x)));
    }
    
    /// <summary>
    /// Calculates the hypotenuse of two numbers.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns> Return the hypotenuse.</returns>
    public static double Hypotenuse(double a, double b)
    {
        return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
    }
    
    /// <summary>
    /// Calculate the standard deviation of values in a float array.
    /// </summary>
    /// <param name="n"> List of floats n.</param>
    /// <returns> Return the standard deviation.</returns>
    public static double StandardDeviation(List<double> n) {
        double m = Mean(n);
        double t = 0;
        foreach (double val in n)
            t += (m - val) * (m - val);

        return Math.Sqrt(t / (n.Count - 1.0f));
    }
   
    /// <summary>
    /// Convert list of Vector2 offsets to list of magnitude values.
    /// </summary>
    public static double CalculateDeltaX(DxCalculationSet calculationSet)
    {
        double x = calculationSet.Selection.x;
        double y = calculationSet.Selection.y;
        double x1 = calculationSet.From.x;
        double y1 = calculationSet.From.y;
        double x2 = calculationSet.To.x;
        double y2 = calculationSet.To.y;

        double a = Hypotenuse(x1 - x2, y1 - y2);
        double b = Hypotenuse(x - x2, y - y2);
        double c = Hypotenuse(x1 - x, y1 - y);

        return (c * c - b * b - a * a) / (2.0f * a);
    }

    public static double CalculateA(DxCalculationSet calculationSet)
    {
        double x1 = calculationSet.From.x;
        double y1 = calculationSet.From.y;
        double x2 = calculationSet.To.x;
        double y2 = calculationSet.To.y;

        return Hypotenuse(x1 - x2, y1 - y2);
    }

    public struct DxCalculationSet
    {
        public Vector2 From;
        public Vector2 To;
        public Vector2 Selection;
    }
}
