using System;

namespace FaceRecognition.NeuralNetwork
{
    internal class Loglikelihood : IMetrics<double>
    {
        public double Calculate(double[] v1, double[] v2)
        {
            double d = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                d += v1[i] * Math.Log(v2[i]) + (1 - v1[i]) * Math.Log(1 - v2[i]);
            }
            return -d;
        }

        public double CalculatePartialDerivaitveByV2Index(double[] v1, double[] v2, int v2Index)
        {
            return -(v1[v2Index] / v2[v2Index] - (1 - v1[v2Index]) / (1 - v2[v2Index]));
        }
    }
}
