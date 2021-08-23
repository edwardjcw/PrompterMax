using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Speech.Recognition;
using NAudio;
using NAudio.Wave;

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
        private WaveInEvent recorder;
        private Stream audioSource;
        System.Speech.AudioFormat.SpeechAudioFormatInfo audioFormat;

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
            recorder = new WaveInEvent();
            recorder.DeviceNumber = 1;
            recorder.WaveFormat = recorder.WaveFormat.AsStandardWaveFormat();
            //WaveFormat wavFormat = recorder.WaveFormat; // WaveFormat.CreateIeeeFloatWaveFormat(16000, 2);
            //recorder.WaveFormat = WaveFormat.CreateCustomFormat(wavFormat.Encoding, wavFormat.SampleRate, wavFormat.Channels, wavFormat.AverageBytesPerSecond, wavFormat.BlockAlign, wavFormat.BitsPerSample);
            recorder.DataAvailable += Recorder_DataAvailable;
            recorder.RecordingStopped += Recorder_RecordingStopped;
            //recognizer.SetInputToDefaultAudioDevice();
            audioFormat = new System.Speech.AudioFormat.SpeechAudioFormatInfo(16000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono);

        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            using (FileStream outputStream = new FileStream(@"C:\Users\edwar\Downloads\wavOutput\000.wav", FileMode.Create))
            {
                audioSource.CopyTo(outputStream);
            }



            audioSource?.Dispose();
            audioSource = null;
            Console.WriteLine("recording stopped");
        }

        private void Recorder_DataAvailable(object sender, WaveInEventArgs e)
        {
            audioSource.Write(e.Buffer, 0, e.BytesRecorded);
            Console.WriteLine($"added to buffer {e.BytesRecorded}, {audioSource.Length}");
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
                e.Result.Audio.WriteToWaveStream(outputStream);
                outputStream.Close();
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
            audioSource = new MemoryStream();
            recognizer.SetInputToAudioStream(audioSource, audioFormat);
           
            //TODO: perhaps something needs to be done to make it a wav file first
            recorder.StartRecording();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            Status = Status.On;
        }

        public void Stop()
        {
            recognizer.RecognizeAsyncCancel();
            recorder.StopRecording();
            Status = Status.Off;
        }
    }
}
