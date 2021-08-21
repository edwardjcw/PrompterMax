using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Utilities;

namespace PrompterMax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Recognize recognizer;
        private Prompter.Prompter prompter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreatePrompts_Click(object sender, RoutedEventArgs e)
        {
            string path = createPromptsInput.Text;
            ExtractPrompts extractor = new ExtractPrompts();

            void output(string t)
            {
                createPromptsResults.Text += createPromptsResults.Text + Environment.NewLine + t;
            }

            extractor.Logger = output;

            List<string> results = extractor.Extract(File.ReadAllText(path));

            // todo: make sure the version is correct and within 1 - 99999
            bool versionGood = uint.TryParse(versionInput.Text, out uint version);

            string toSave = Utilities.Utilities.TransformForOutput(results, versionGood ? version : 1);

            // save text
            File.WriteAllText(createPromptsOutput.Text, toSave);
            output("saved");
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (recognizer == null)
            {
                recognizer = new Recognize();
                recognizer.AudioAvailable += Recognizer_AudioAvailable;
            }

            if (recognizer.Status == Status.On)
            {
                recognizer.Stop();
                recordButton.Content = "Auto Next";
                return;
            }

            recognizer.Phrase = prompt.Text.Trim() == "" ? "This is a test." : prompt.Text;
            recognizer.SaveTo = @"C:\Users\edwar\Downloads\thisIsATest.wav";

            recognizer.Start();
            recordButton.Content = "Stop";
        }

        private void Recognizer_AudioAvailable(object sender, RecognizeAudioEventArgs e)
        {
            Console.WriteLine("Got it");
        }

        private void LoadMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            prompter = new Prompter.Prompter();
            prompter.PromptChanged += Prompter_PromptChanged;
            prompter.Load(loadMetaDataPath.Text, wavDirectory.Text);
        }

        private void Prompter_PromptChanged(object sender, Prompter.PrompterEventArgs e)
        {
            SetLocation(e.Index, prompter.Count);
            previous.Content = e.Previous;
            current.Content = e.Current;
            next.Content = e.Next;
            playButton.IsEnabled = Utilities.Utilities.WavExists(prompter.WavPath);
        }

        private void SetLocation(int at, int count)
        {
            location.Content = $"{at} of {count}";
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
            bool success = int.TryParse(gotoInput.Text, out int result);
            if (!success || result >= prompter.Count || result < 0) // TODO: user one-base for display
            {
                gotoInput.Text = "";
                _ = MessageBox.Show("Not a number or out of bounds");
                return;
            }

            prompter.Goto(result);
            gotoInput.Text = "";
        }
    }
}
