using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;

namespace PrompterMax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Recognize recognizer;

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

            string toSave = string.Join(Environment.NewLine, results);

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

    }
}
