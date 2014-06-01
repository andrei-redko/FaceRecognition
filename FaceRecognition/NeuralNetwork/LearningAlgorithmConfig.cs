namespace FaceRecognition.NeuralNetwork
{
    public class LearningAlgorithmConfig
    {

        public double LearningRate { get; set; }

        public int BatchSize { get; set; }

        public double RegularizationFactor { get; set; }

        public int MaxEpoches { get; set; }

        public double MinError { get; set; }

        public double MinErrorChange { get; set; }

        public IMetrics<double> ErrorFunction { get; set; }

    }
}
