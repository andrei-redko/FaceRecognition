using System.Collections.Generic;

namespace FaceRecognition.NeuralNetwork
{
    public interface ILearningStrategy<T>
    {
        void Train(T network, IList<DataItem<double>> data);
    }
}
