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

namespace PrompterMax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreatePrompts_Click(object sender, RoutedEventArgs e)
        {
            var path = createPromptsInput.Text;
            var extractor = new Utilities.ExtractPrompts();
            
            void output(string t) => createPromptsResults.Text += createPromptsResults.Text + Environment.NewLine + t;
            extractor.Logger = output;

            var results = extractor.Extract(File.ReadAllText(path));

            var toSave = string.Join(Environment.NewLine, results);

            // save text
            File.WriteAllText(createPromptsOutput.Text, toSave);
            output("saved");
        }
    }
}
