namespace FaceRecognition.NeuralNetwork
{
    public interface IFunction
    {
        double Compute(double x);
        double ComputeFirstDerivative(double x);
    }
}
