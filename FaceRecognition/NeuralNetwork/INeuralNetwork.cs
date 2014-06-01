using System.Collections.Generic;
using System.IO;

namespace FaceRecognition.NeuralNetwork
{
    public interface INeuralNetwork
    {

        double[] ComputeOutput(double[] inputVector);

        Stream Save();

        void Train(IList<DataItem<double>> data);
    }
}
