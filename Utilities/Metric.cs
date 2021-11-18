using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Metric : INotifyPropertyChanged
    {
        private readonly RecognitionAccuracy accuracy;
        private string correct;
        private string speechToNoise;
        private Dictionary<string, MetricData> metricData;
        private MetricData metricDataForWav;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Correct
        {
            get { return correct; }
            set
            {
                if (correct != value)
                {
                    correct = value;
                    OnPropertyChanged(nameof(Correct));
                }
            }
        }

        public string SpeechToNoise
        {
            get { return speechToNoise; }
            set
            {
                if (speechToNoise != value)
                {
                    speechToNoise = value;
                    OnPropertyChanged(nameof(SpeechToNoise));
                }
            }
        }

        public Metric()
        {
            metricData = new Dictionary<string, MetricData>();
            accuracy = new RecognitionAccuracy();
            accuracy.PropertyChanged += Accuracy_PropertyChanged;
        }

        public void Calculate(string wavPath, string text)
        {
            if (!General.WavExists(wavPath))
            {
                Correct = "";
                SpeechToNoise = "";
                return;
            }

            FileInfo wavFile = new FileInfo(wavPath);
            var modifiedOnDate = wavFile.LastWriteTime;

            if (metricData.ContainsKey(wavPath) && modifiedOnDate.Equals(metricData[wavPath].Date))
            {
                SpeechToNoise = $"Speech to Noise Ratio: {metricData[wavPath].SpeechToNoise:P1}";
                Correct = $"Mistake Distance: {metricData[wavPath].Correct}";
                metricDataForWav = null;
                return;
            }

            // TODO: Make this async

            metricDataForWav = metricData[wavPath] = new MetricData(modifiedOnDate);

            var speechToNoiseRatio = General.SpeechToNoiseRatio(wavPath);
            SpeechToNoise = $"Speech to Noise Ratio: {speechToNoiseRatio:P1}";
            metricDataForWav.SpeechToNoise = speechToNoiseRatio;

            accuracy.Start(wavPath, text);
        }

        private void Accuracy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == "Correct")
            {
                Correct = $"Mistake Distance: {accuracy.Correct}";
                metricDataForWav.Correct = accuracy.Correct;
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}
