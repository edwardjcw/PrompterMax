using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class RecognitionAccuracy : INotifyPropertyChanged
    {
        private readonly SpeechRecognitionEngine recognizer;
        private string phrase;
        private int correct;

        public int Correct
        {
            get { return correct; }
            set { 
                correct = value;
                OnPropertyChanged(nameof(Correct)); 
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public RecognitionAccuracy()
        {
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.LoadGrammar(new DictationGrammar());
            correct = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start(string wavPath, string text)
        {
            recognizer.RecognizeAsyncCancel();
            phrase = General.RemovePunctuation(text);
            recognizer.SetInputToWaveFile(wavPath);
            recognizer.RecognizeAsync();
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var result = e.Result.Text.ToLower();
            int value = General.Levenshtein(result, phrase);
            Correct = value;
        }


    }
}
