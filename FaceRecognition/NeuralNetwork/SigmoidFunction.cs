using System;

namespace FaceRecognition.NeuralNetwork
{
    internal class SigmoidFunction : IFunction
    {
        private double _alpha = 1;

        internal SigmoidFunction(double alpha)
        {
            _alpha = alpha;
        }

        public double Compute(double x)
        {
            double r = (1 / (1 + Math.Exp(-1 * _alpha * x)));
            return r;
        }

        public double ComputeFirstDerivative(double x)
        {
            return _alpha * this.Compute(x) * (1 - this.Compute(x));
        }
    }
}
