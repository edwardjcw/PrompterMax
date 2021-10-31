using System;
using System.IO;
using System.Speech.Recognition;

namespace Utilities
{
    public enum Status
    {
        Off,
        On
    }

    public enum Error
    {
        NoSaveToPath,
        None
    }

    public class Recognize
    {
        private readonly SpeechRecognitionEngine recognizer;
        private string phrase;

        public string Phrase
        {
            get => phrase;
            set
            {
                SubsetMatchingMode mode = SubsetMatchingMode.OrderedSubsetContentRequired;
                GrammarBuilder gb = new GrammarBuilder(value, mode);
                Grammar grammar = new Grammar(gb)
                {
                    Name = mode.ToString(),
                    Enabled = true
                };
                recognizer.UnloadAllGrammars();
                recognizer.LoadGrammar(grammar);
                phrase = value;
            }
        }

        public string SaveTo { get; set; }
        public Status Status { get; private set; }
        public event RecognizeAudioEventHandler AudioAvailable;

        public Recognize()
        {
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine($"Recognized: {e.Result.Text}");
            if (SaveTo.Trim() == "")
            {
                OnRecognition(phrase, Error.NoSaveToPath);
                return;
            }

            using (Stream outputStream = new FileStream(SaveTo, FileMode.Create))
            {
                e.Result.Audio?.WriteToWaveStream(outputStream);
                outputStream.Close();
                Console.WriteLine(e.Result.Text);
                OnRecognition(phrase, SaveTo);
            }
        }

        private void OnRecognition(string phrase, string saveTo)
        {
            AudioAvailable?.Invoke(this, new RecognizeAudioEventArgs(phrase, saveTo));
        }

        private void OnRecognition(string phrase, Error error)
        {
            AudioAvailable?.Invoke(this, new RecognizeAudioEventArgs(phrase, error));
        }

        public void Start()
        {
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            Status = Status.On;
        }

        public void Stop()
        {
            recognizer.RecognizeAsyncCancel();
            Status = Status.Off;
        }
    }
}
