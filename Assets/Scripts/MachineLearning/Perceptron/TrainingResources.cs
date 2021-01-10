// ReSharper disable UnusedMember.Global
namespace MachineLearning.Perceptron
{
    public static class TrainingResources
    {
        public static TrainingSet[] or = {
            new TrainingSet()
            {
                inputs = new double[] {0, 0}, output = 0
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 0}, output = 1
            },
            new TrainingSet()
            {
                inputs = new double[] {0, 1}, output = 1
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 1}, output = 1
            }
        };
        
        public static TrainingSet[] and = {
            new TrainingSet()
            {
                inputs = new double[] {0, 0}, output = 0
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 0}, output = 0
            },
            new TrainingSet()
            {
                inputs = new double[] {0, 1}, output = 0
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 1}, output = 1
            }
        };
        
        public static readonly TrainingSet[] xor = {
            new TrainingSet()
            {
                inputs = new double[] {0, 0}, output = 0
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 0}, output = 1
            },
            new TrainingSet()
            {
                inputs = new double[] {0, 1}, output = 1
            },
            new TrainingSet()
            {
                inputs = new double[] {1, 1}, output = 0
            }
        };
    }
}