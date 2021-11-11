using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            // TODO: Make this async
            var speechToNoiseRatio = General.SpeechToNoiseRatio(wavPath);
            SpeechToNoise = $"Speech to Noise Ratio: {speechToNoiseRatio:P1}";
            
            accuracy.Start(wavPath, text);
        }

        private void Accuracy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == "Correct")
            {
                Correct = accuracy.Correct;
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}
