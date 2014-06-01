using System;
using System.Xml.Serialization;

namespace FaceRecognition.NeuralNetwork
{
    public class Neuron
    {

        [XmlAttribute("weight")] public string data;

        [XmlIgnore] public int[,] weight;

        [XmlIgnore] public int minimum = 50;

        [XmlIgnore] public int row = 64, column = 64;

        public Neuron()
        {
            weight = new int[row, column];
            randomizeWeights();
        }

        public int transferHard(int[,] input)
        {
            int Power = 0;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    Power += weight[r, c]*input[r, c];
                }
            }
            return Power >= minimum ? 1 : 0;
        }

        public int transfer(int[,] input)
        {
            int Power = 0;
            for (int r = 0; r < row; r++)
                for (int c = 0; c < column; c++)
                    Power += weight[r, c]*input[r, c];
            return Power;
        }

        private void randomizeWeights()
        {
            //for (int r = 0; r < row; r++)
            //    for (int c = 0; c < column; c++)
                    
            //        //weight[r, c] = Random.Range(0, 10);
        }

        public void changeWeights(int[,] input, int d)
        {
            for (int r = 0; r < row; r++)
                for (int c = 0; c < column; c++)
                    weight[r, c] += d*input[r, c];
        }

        public void prepareForSerialization()
        {
            data = "";
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    data += weight[r, c] + " ";
                }
                data += "\n";
            }
        }

        public void onDeserialize()
        {
            weight = new int[row, column];

            string[] rows = data.Split(new char[] {'\n'});
            for (int r = 0; r < row; r++)
            {
                string[] columns = rows[r].Split(new char[] {' '});
                for (int c = 0; c < column; c++)
                {
                    weight[r, c] = int.Parse(columns[c]);
                }
            }
        }
    }
}
