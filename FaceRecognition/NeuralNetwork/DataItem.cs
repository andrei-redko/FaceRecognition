namespace FaceRecognition.NeuralNetwork
{
    public class DataItem<T>
    {
        private T[] _input = null;
        private T[] _output = null;

        public DataItem()
        {
        }

        public DataItem(T[] input, T[] output)
        {
            _input = input;
            _output = output;
        }

        public T[] Input
        {
            get { return _input; }
            set { _input = value; }
        }

        public T[] Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }
}
