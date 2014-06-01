namespace FaceRecognition.NeuralNetwork
{
    public interface ILayer
    {

        double[] Compute(double[] inputVector);

        double[] LastOutput { get; }

        INeuron[] Neurons { get; }

        int InputDimension { get; }
    }
}
