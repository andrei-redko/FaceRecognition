using System;
using System.Collections.Generic;

namespace FaceRecognition.NeuralNetwork
{
    internal class BackpropagationFCNLearningAlgorithm : ILearningStrategy<IMultilayerNeuralNetwork>
    {

        private LearningAlgorithmConfig _config = null;
        private Random _random = null;

        internal BackpropagationFCNLearningAlgorithm(LearningAlgorithmConfig config)
        {
            _config = config;
            //_random = new Random(Helper.GetSeed());
        }

        public void Train(IMultilayerNeuralNetwork network, IList<DataItem<double>> data)
        {
            if (_config.BatchSize < 1 || _config.BatchSize > data.Count)
            {
                _config.BatchSize = data.Count;
            }
            double currentError = Single.MaxValue;
            double lastError = 0;
            int epochNumber = 0;
            //Logger.Instance.Log("Start learning...");
            do
            {
                lastError = currentError;
                DateTime dtStart = DateTime.Now;

                #region one epoche

                int[] trainingIndices = new int[data.Count];
                for (int i = 0; i < data.Count; i++)
                {
                    trainingIndices[i] = i;
                }
                if (_config.BatchSize > 0)
                {
                    trainingIndices = Shuffle(trainingIndices);
                }

                int currentIndex = 0;
                do
                {
                    double[][][] nablaWeights = new double[network.Layers.Length][][];
                    double[][] nablaBiases = new double[network.Layers.Length][];

                    for (int inBatchIndex = currentIndex;
                        inBatchIndex < currentIndex + _config.BatchSize && inBatchIndex < data.Count;
                        inBatchIndex++)
                    {
                        double[] realOutput = network.ComputeOutput(data[trainingIndices[inBatchIndex]].Input);

                        nablaWeights[network.Layers.Length - 1] =
                            new double[network.Layers[network.Layers.Length - 1].Neurons.Length][];
                        nablaBiases[network.Layers.Length - 1] =
                            new double[network.Layers[network.Layers.Length - 1].Neurons.Length];
                        for (int j = 0; j < network.Layers[network.Layers.Length - 1].Neurons.Length; j++)
                        {
                            network.Layers[network.Layers.Length - 1].Neurons[j].dEdz =
                                _config.ErrorFunction.CalculatePartialDerivaitveByV2Index(data[inBatchIndex].Output,
                                    realOutput, j)*
                                network.Layers[network.Layers.Length - 1].Neurons[j].ActivationFunction.
                                    ComputeFirstDerivative(network.Layers[network.Layers.Length - 1].Neurons[j].LastNET);

                            nablaBiases[network.Layers.Length - 1][j] = _config.LearningRate*
                                                                        network.Layers[network.Layers.Length - 1]
                                                                            .Neurons[j].dEdz;

                            nablaWeights[network.Layers.Length - 1][j] =
                                new double[network.Layers[network.Layers.Length - 1].Neurons[j].Weights.Length];
                            for (int i = 0;
                                i < network.Layers[network.Layers.Length - 1].Neurons[j].Weights.Length;
                                i++)
                            {
                                nablaWeights[network.Layers.Length - 1][j][i] =
                                    _config.LearningRate*(network.Layers[network.Layers.Length - 1].Neurons[j].dEdz*
                                                          (network.Layers.Length > 1
                                                              ? network.Layers[network.Layers.Length - 1 - 1].Neurons[i]
                                                                  .LastState
                                                              : data[inBatchIndex].Input[i])
                                                          +
                                                          _config.RegularizationFactor*
                                                          network.Layers[network.Layers.Length - 1].Neurons[j].Weights[i
                                                              ]
                                                          /data.Count);
                            }
                        }

                        for (int hiddenLayerIndex = network.Layers.Length - 2;
                            hiddenLayerIndex >= 0;
                            hiddenLayerIndex--)
                        {
                            nablaWeights[hiddenLayerIndex] =
                                new double[network.Layers[hiddenLayerIndex].Neurons.Length][];
                            nablaBiases[hiddenLayerIndex] = new double[network.Layers[hiddenLayerIndex].Neurons.Length];
                            for (int j = 0; j < network.Layers[hiddenLayerIndex].Neurons.Length; j++)
                            {
                                network.Layers[hiddenLayerIndex].Neurons[j].dEdz = 0;
                                for (int k = 0; k < network.Layers[hiddenLayerIndex + 1].Neurons.Length; k++)
                                {
                                    network.Layers[hiddenLayerIndex].Neurons[j].dEdz +=
                                        network.Layers[hiddenLayerIndex + 1].Neurons[k].Weights[j]*
                                        network.Layers[hiddenLayerIndex + 1].Neurons[k].dEdz;
                                }
                                network.Layers[hiddenLayerIndex].Neurons[j].dEdz *=
                                    network.Layers[hiddenLayerIndex].Neurons[j].ActivationFunction.
                                        ComputeFirstDerivative(
                                            network.Layers[hiddenLayerIndex].Neurons[j].LastNET
                                        );

                                nablaBiases[hiddenLayerIndex][j] = _config.LearningRate*
                                                                   network.Layers[hiddenLayerIndex].Neurons[j].dEdz;

                                nablaWeights[hiddenLayerIndex][j] =
                                    new double[network.Layers[hiddenLayerIndex].Neurons[j].Weights.Length];
                                for (int i = 0; i < network.Layers[hiddenLayerIndex].Neurons[j].Weights.Length; i++)
                                {
                                    nablaWeights[hiddenLayerIndex][j][i] = _config.LearningRate*(
                                        network.Layers[hiddenLayerIndex].Neurons[j].dEdz*
                                        (hiddenLayerIndex > 0
                                            ? network.Layers[hiddenLayerIndex - 1].Neurons[i].LastState
                                            : data[inBatchIndex].Input[i])
                                        +
                                        _config.RegularizationFactor*
                                        network.Layers[hiddenLayerIndex].Neurons[j].Weights[i]/data.Count
                                        );
                                }
                            }
                        }
                    }

                    for (int layerIndex = 0; layerIndex < network.Layers.Length; layerIndex++)
                    {
                        for (int neuronIndex = 0;
                            neuronIndex < network.Layers[layerIndex].Neurons.Length;
                            neuronIndex++)
                        {
                            network.Layers[layerIndex].Neurons[neuronIndex].Bias -= nablaBiases[layerIndex][neuronIndex];
                            for (int weightIndex = 0;
                                weightIndex < network.Layers[layerIndex].Neurons[neuronIndex].Weights.Length;
                                weightIndex++)
                            {
                                network.Layers[layerIndex].Neurons[neuronIndex].Weights[weightIndex] -=
                                    nablaWeights[layerIndex][neuronIndex][weightIndex];
                            }
                        }
                    }

                    currentIndex += _config.BatchSize;
                } while (currentIndex < data.Count);

                currentError = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    double[] realOutput = network.ComputeOutput(data[i].Input);
                    currentError += _config.ErrorFunction.Calculate(data[i].Output, realOutput);
                }
                currentError *= 1d/data.Count;

                #endregion

                epochNumber++;
                //Logger.Instance.Log("Eposh #" + epochNumber.ToString() +
                //                    " finished; current error is " + currentError.ToString() +
                //                    "; it takes: " +
                //                    (DateTime.Now - dtStart).Duration().ToString());
            } while (epochNumber < _config.MaxEpoches &&
                     currentError > _config.MinError &&
                     Math.Abs(currentError - lastError) > _config.MinErrorChange);
        }

        private int[] Shuffle(int[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (_random.NextDouble() >= 0.3d)
                {
                    int newIndex = _random.Next(arr.Length);
                    int tmp = arr[i];
                    arr[i] = arr[newIndex];
                    arr[newIndex] = tmp;
                }
            }
            return arr;
        }
    }
}