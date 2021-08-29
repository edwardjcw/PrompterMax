﻿using System;
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
        private IWaveIn recorder;
        private Stream continuousAudioSource;
        private Stream readyForUseAudioStream;
        private RawSourceWaveStream resourceWavStream;
        private WaveStream waveStream;
        private WaveFileWriter waveFileWriter;
        private WaveFileReader waveFileReader;
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
            
            //recorder.WaveFormat = recorder.WaveFormat.AsStandardWaveFormat();
            WaveFormat wavFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1);
            recorder.WaveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, wavFormat.SampleRate, wavFormat.Channels, wavFormat.AverageBytesPerSecond / 2, wavFormat.BlockAlign / 2, wavFormat.BitsPerSample / 2);
            recorder.DataAvailable += Recorder_DataAvailable;
            recorder.RecordingStopped += Recorder_RecordingStopped;
            //recognizer.SetInputToDefaultAudioDevice();
            audioFormat = new System.Speech.AudioFormat.SpeechAudioFormatInfo(48000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono);

        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveFileWriter == null)
            {
                return;
            }

            //audioSource.Flush();
            
            //waveFileWriter?.Flush();
            //var waveReader = new WaveFileReader(audioSource);
            

            //using (FileStream outputStream = new FileStream(@"C:\Users\edwar\Downloads\wavOutput\000.wav", FileMode.Create))
            //{
            //    continuousAudioSource.Position = 0;
            //    continuousAudioSource.CopyTo(outputStream);
            //    outputStream.Close();
            //}

            waveFileWriter?.Dispose();
            waveFileWriter = null;

            //waveStream.Dispose();
            //waveStream = null;

            resourceWavStream?.Dispose();
            resourceWavStream = null;

            continuousAudioSource?.Dispose();
            continuousAudioSource = null;
            
            readyForUseAudioStream?.Dispose();
            readyForUseAudioStream = null;

            Console.WriteLine("recording stopped");
        }

        private void Recorder_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFileWriter == null)
            {
                continuousAudioSource = new MemoryStream();
                waveFileWriter = new WaveFileWriter(continuousAudioSource, recorder.WaveFormat);
                readyForUseAudioStream = new MemoryStream();
                recognizer.SetInputToAudioStream(readyForUseAudioStream, audioFormat);

                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                Status = Status.On;
            }

            waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
            Console.WriteLine($"added to buffer {e.BytesRecorded}, {continuousAudioSource.Length}; {continuousAudioSource.Position};  {waveFileWriter.TotalTime}; {recognizer.RecognizerAudioPosition}");
            waveFileWriter.Flush();

            if (waveFileWriter.TotalTime.TotalSeconds % TimeSpan.FromSeconds(5).TotalSeconds == 0) {
                recognizer.RecognizeAsyncCancel();
                var currentPosition = continuousAudioSource.Position;
                continuousAudioSource.Position = 0;
                continuousAudioSource.CopyTo(readyForUseAudioStream);
                readyForUseAudioStream.Flush();
                readyForUseAudioStream.Position = 0;
                recognizer.SetInputToAudioStream(readyForUseAudioStream, audioFormat);
                continuousAudioSource.Position = currentPosition;
                recognizer.RecognizeAsync();
            }
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

            recorder.StartRecording();
        }

        public void Stop()
        {
            recorder.StopRecording();            
            recognizer.RecognizeAsyncCancel();
            Status = Status.Off;
        }
    }
}
