namespace Utilities
{
    public class RecognizeAudioEventArgs
    {
        public RecognizeAudioEventArgs(string phrase, Error error)
        {
            Phrase = phrase;
            SaveTo = "";
            Error = error;
        }

        public RecognizeAudioEventArgs(string phrase, string saveTo)
        {
            Phrase = phrase;
            SaveTo = saveTo;
            Error = Error.None;
        }

        public string Phrase { get; }
        public Error Error { get; }
        public string SaveTo { get; }
    }
}
