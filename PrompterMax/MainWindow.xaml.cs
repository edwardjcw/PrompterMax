using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Utilities;

namespace PrompterMax
{
    public enum Recording
    {
        None,
        ActiveAuto,
        ActiveManual
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Recognize recognizer;
        private Prompter.Prompter prompter;
        private Recording recording;
        private AudioHelper audioHelper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreatePrompts_Click(object sender, RoutedEventArgs e)
        {
            string path = createPromptsInput.Text;
            ExtractPrompts extractor = new ExtractPrompts();

            void Output(string t)
            {
                createPromptsResults.Text += createPromptsResults.Text + Environment.NewLine + t;
            }

            extractor.Logger = Output;

            List<string> results = extractor.Extract(File.ReadAllText(path));

            // todo: make sure the version is correct and within 1 - 99999
            bool versionGood = uint.TryParse(versionInput.Text, out uint version);

            string toSave = Utilities.Utilities.TransformForOutput(results, versionGood ? version : 1);

            // save text
            File.WriteAllText(createPromptsOutput.Text, toSave);
            Output("saved");
        }

        private void Recognizer_AudioAvailable(object sender, RecognizeAudioEventArgs e)
        {
            Console.WriteLine("Got it");
            prompter.Next();
        }

        private void LoadMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            prompter = new Prompter.Prompter();
            prompter.PromptChanged += Prompter_PromptChanged;
            prompter.Load(loadMetaDataPath.Text, wavDirectory.Text);
            
            recordingButton.IsEnabled = true;
            nextButton.IsEnabled = true;
            previousButton.IsEnabled = true;
            gotoButton.IsEnabled = true;
        }

        private void Prompter_PromptChanged(object sender, Prompter.PrompterEventArgs e)
        {
            SetLocation(e.Index, prompter.Count);
            previous.Content = e.Previous;
            current.Content = e.Current;
            next.Content = e.Next;
            // === CASE 0: No recording occuring
            if (recording == Recording.None)
            {
                playButton.IsEnabled = Utilities.Utilities.WavExists(prompter.WavPath);
                var speechToNoiseRatio = playButton.IsEnabled ? Utilities.Utilities.SpeechToNoiseRatio(prompter.WavPath) : double.NaN;
                speechToNoise.Content = speechToNoiseRatio.CompareTo(double.NaN) != 0 ? $"Speech to Noise Ratio: {speechToNoiseRatio:P1}" : "";
                if (playButton.IsEnabled)
                {
                    RecognitionAccuracy.Start(prompter.WavPath, current.Content.ToString());
                }
                return;
            }
            // === CASE 1: Manual recording taking place
            if (recording == Recording.ActiveManual)
            {
                return;
            }
            // === CASE 2: Auto recording taking place
            recognizer.Stop();
            recognizer.Phrase = prompter.Normalized.Trim() == "" ? "This is a test." : prompter.Normalized;
            recognizer.SaveTo = prompter.WavPath;
            recognizer.Start();
        }

        private void SetLocation(int at, int count)
        {
            int atOneBase = WrapOneBase(at);
            location.Content = $"{atOneBase} of {count}";
        }

        private int WrapOneBase(int at)
        {
            return at + 1;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            prompter.Next();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            prompter.Previous();
        }

        private void GotoButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = int.TryParse(gotoInput.Text, out int resultInOneBase);
            int result = UnwrapOneBase(resultInOneBase);
            if (!success || result >= prompter.Count || result < 0) // TODO: user one-base for display
            {
                gotoInput.Text = "";
                _ = MessageBox.Show("Not a number or out of bounds");
                return;
            }

            prompter.Goto(result);
            gotoInput.Text = "";
        }

        private int UnwrapOneBase(int resultInOneBase)
        {
            return resultInOneBase - 1;
        }

        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (autoAdvance.IsChecked == true)
            {
                AutoRecording();
                return;
            }

            if (audioHelper == null)
            {
                audioHelper = new AudioHelper();
                audioHelper.AudioChanged += AudioHelper_AudioChanged;
            }

            if (audioHelper.RecordingStatus == RecordingStatus.Stopped)
            {
                audioHelper.Record(prompter.WavPath);
                return;
            }

            audioHelper.StopRecording();

        }

        private void AutoRecording()
        {
            if (recognizer == null)
            {
                recognizer = new Recognize();
                recognizer.AudioAvailable += Recognizer_AudioAvailable;
            }

            if (recognizer.Status == Status.On)
            {
                recognizer.Stop();
                recordingButton.Content = "Record";
                autoAdvance.IsEnabled = true;
                recording = Recording.None;
                playButton.IsEnabled = Utilities.Utilities.WavExists(prompter.WavPath);
                return;
            }

            recognizer.Phrase = prompter.Normalized.Trim() == "" ? "This is a test." : prompter.Normalized;
            recognizer.SaveTo = prompter.WavPath;

            recognizer.Start();
            recordingButton.Content = "Stop";
            autoAdvance.IsEnabled = false;
            recording = Recording.ActiveAuto;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (audioHelper == null)
            {
                audioHelper = new AudioHelper();
                audioHelper.AudioChanged += AudioHelper_AudioChanged;
            }

            if (audioHelper.PlaybackStatus == PlaybackStatus.Stopped)
            {
                audioHelper.Play(prompter.WavPath);
                return;
            }

            audioHelper.StopPlaying();
        }

        private void AudioHelper_AudioChanged(object sender, AudioHelperEventArgs e)
        {
            switch (e.Status)
            {
                case AudioStatus.PlayStopped:
                    playButton.Content = "Play";
                    recordingButton.IsEnabled = true;
                    break;
                case AudioStatus.Playing:
                    playButton.Content = "Stop";
                    recordingButton.IsEnabled = false;
                    break;
                case AudioStatus.RecordStopped:
                    playButton.IsEnabled = Utilities.Utilities.WavExists(prompter.WavPath);
                    var speechToNoiseRatio = playButton.IsEnabled ? Utilities.Utilities.SpeechToNoiseRatio(prompter.WavPath) : double.NaN;
                    speechToNoise.Content = speechToNoiseRatio.CompareTo(double.NaN) != 0 ? $"Speech to Noise Ratio: {speechToNoiseRatio:P1}" : "";
                    if (playButton.IsEnabled)
                    {
                        RecognitionAccuracy.Start(prompter.WavPath, current.Content.ToString());
                    }
                    recordingButton.Content = "Record";
                    autoAdvance.IsEnabled = true;
                    break;
                case AudioStatus.Recording:
                    playButton.IsEnabled = false;
                    recordingButton.Content = "Stop";
                    autoAdvance.IsEnabled = false;
                    break;
            }
        }
    }
}
