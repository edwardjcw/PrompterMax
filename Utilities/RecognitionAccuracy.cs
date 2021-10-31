using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class RecognitionAccuracy
    {
        private static readonly SpeechRecognitionEngine recognizer;
        private static string phrase;

        static RecognitionAccuracy()
        {
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.LoadGrammar(new DictationGrammar());
            
        }

        public static void Start(string wavPath, string text)
        {
            phrase = text;
            recognizer.SetInputToWaveFile(wavPath);
            recognizer.RecognizeAsync();
        }

        private static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
            Console.WriteLine(Utilities.Levenshtein(e.Result.Text, phrase));
        }


    }
}
