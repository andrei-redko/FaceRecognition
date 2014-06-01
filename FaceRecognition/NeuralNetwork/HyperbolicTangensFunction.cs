using System;

namespace FaceRecognition.NeuralNetwork
{
    internal class HyperbolicTangensFunction : IFunction
    {

        private double _alpha = 1;

        internal HyperbolicTangensFunction(double alpha)
        {
            _alpha = alpha;
        }

        public double Compute(double x)
        {
            return (Math.Tanh(_alpha * x));
        }

        public double ComputeFirstDerivative(double x)
        {
            double t = Math.Tanh(_alpha * x);
            return _alpha * (1 - t * t);
        }
    }
}
