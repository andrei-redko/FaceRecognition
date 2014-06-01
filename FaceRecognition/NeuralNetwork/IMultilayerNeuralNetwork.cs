namespace FaceRecognition.NeuralNetwork
{
    public interface IMultilayerNeuralNetwork : INeuralNetwork
    {
        ILayer[] Layers { get; }
    }
}
