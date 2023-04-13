using System;
namespace Jyben.Racing.Telemetry.Helpers
{
    public class KalmanFilter
    {
        private double _measurementVariance;
        private double _processVariance;
        private double _estimatedValue;
        private double _estimatedError;

        public KalmanFilter(double measurementVariance, double processVariance, double initialValue)
        {
            _measurementVariance = measurementVariance;
            _processVariance = processVariance;
            _estimatedValue = initialValue;
            _estimatedError = processVariance;
        }

        public double Filter(double measurement)
        {
            // Time update (prediction)
            double predictedValue = _estimatedValue;
            double predictedError = _estimatedError + _processVariance;

            // Measurement update (correction)
            double kalmanGain = predictedError / (predictedError + _measurementVariance);
            _estimatedValue = predictedValue + kalmanGain * (measurement - predictedValue);
            _estimatedError = (1 - kalmanGain) * predictedError;

            return _estimatedValue;
        }
    }
}

