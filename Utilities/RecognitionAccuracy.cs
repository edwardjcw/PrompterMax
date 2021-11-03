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
            phrase = Utilities.RemovePunctuation(text);
            Console.WriteLine($"punctuation removed: {phrase}");
            recognizer.SetInputToWaveFile(wavPath);
            recognizer.RecognizeAsync();
        }

        private static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var result = e.Result.Text.ToLower();
            Console.WriteLine(result);
            Console.WriteLine(Utilities.Levenshtein(result, phrase));
        }


    }
}
