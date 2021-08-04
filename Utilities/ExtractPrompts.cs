using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.IO;

namespace Utilities
{
    public class ExtractPrompts
    {
        private const int maxPauseLength = 900;
        private string text;
        private int lastStopIndex = 0;
        private readonly List<string> phrases = new List<string>();
        private TimeSpan lastAudioOffset = TimeSpan.Zero;
        private TimeSpan lastPhraseAudioOffset = TimeSpan.Zero;
        public Action<string> Logger { get; set; } = t => Console.WriteLine(t);

        public List<string> Extract(string text)
        {
            this.text = text;

            Logger($"Number of characters: {text.Length}");
            Logger($"Start time: {DateTime.Now}");

            // speech synthesizer
            SynthesizeToSpeech();

            Logger($"End time: {DateTime.Now}");
            return phrases;
        }

        private void SynthesizeToSpeech()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToNull();
            synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;

            synthesizer.Speak(text);

        }

        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            // finish last sentence
            string phrase = text.Substring(lastStopIndex);
            phrases.Add(phrase);
            Logger($"Phrase: {phrase}");
        }

        private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            TimeSpan durationSinceLastAudioOffset = e.AudioPosition - lastAudioOffset;
            TimeSpan durationSinceLastPhraseAudioOffset = e.AudioPosition - lastPhraseAudioOffset;
            lastAudioOffset = e.AudioPosition;

            //Logger($"PROMPT: {e.Prompt}");

            string word = e.Text;

            // the longer the phrase is running, the less duration difference to trigger the end of a phrase
            TimeSpan maxLength(TimeSpan difference)
            {
                return TimeSpan.FromMilliseconds((-0.053 * difference.TotalMilliseconds) + maxPauseLength);
            }

            // CASE 1: pause isn't long enough to create phrase
            if (durationSinceLastAudioOffset < maxLength(durationSinceLastPhraseAudioOffset))
            {
                //Logger($"Word: {word} \t Audio Offset: {e.AudioPosition}");
                return;
            }

            // CASE 2: pause is long enough to create phrase

            int length = e.CharacterPosition - lastStopIndex - 1;
            string phrase = text.Substring(lastStopIndex, length < 0 ? 0 : length).Trim();
            phrases.Add(RemoveNewLines(phrase));

            lastStopIndex = e.CharacterPosition;
            //Logger($"Phrase: {phrase}");
            lastPhraseAudioOffset = e.AudioPosition;

            //Logger($"Word: {word} \t Audio Offset: {e.AudioPosition} \t Character Position: {e.CharacterPosition}");
        }
        private static string RemoveNewLines(string phrase)
        {
            return phrase.Replace(Environment.NewLine, " ");
        }
    }
}
