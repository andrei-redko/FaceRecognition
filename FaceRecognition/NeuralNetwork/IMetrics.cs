namespace FaceRecognition.NeuralNetwork
{
    public interface IMetrics<T>
    {
        double Calculate(T[] v1, T[] v2);

        T CalculatePartialDerivaitveByV2Index(T[] v1, T[] v2, int v2Index);
    }
}
