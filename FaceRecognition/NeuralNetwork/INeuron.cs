using System.Collections.Generic;

namespace FaceRecognition.NeuralNetwork
{
    public interface INeuron
    {
        double[] Weights { get; }

        double Bias { get; set; }

        double NET(double[] inputVector);

        double Activate(double[] inputVector);

        double LastState { get; set; }

        double LastNET { get; set; }

        IList<INeuron> Childs { get; }

        IList<INeuron> Parents { get; }

        IFunction ActivationFunction { get; set; }

        double dEdz { get; set; }
    }
}
