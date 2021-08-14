using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Optional;

namespace Prompter
{
    public class Prompter
    {
        private string promptFile;
        private string wavDirectory;
        private SortedDictionary<string, Prompt> prompts;
        private Prompt lastPromptWithWav;

        public Prompter(string promptFile, string wavDirectory)
        {
            this.promptFile = promptFile;
            this.wavDirectory = wavDirectory;

            prompts = LoadPrompts(promptFile, wavDirectory);
            lastPromptWithWav = GetLastPromptWithWav(prompts);
        }

        private Prompt GetLastPromptWithWav(SortedDictionary<string, Prompt> prompts)
        {
            return prompts.LastOrDefault(p => File.Exists(p.Value.WavPath)).Value;
        }

        private static SortedDictionary<string, Prompt> LoadPrompts(string promptFile, string wavDirectory)
        {

            Prompt ParsePromptWithWav(string prompt) => ParsePrompt(wavDirectory, prompt);
            Dictionary<string, Prompt> dictionary = File.ReadAllLines(promptFile)
                .Select(ParsePromptWithWav)
                .Where(prompt => prompt != Prompt.Empty)
                .GroupBy(prompt => prompt.Id)
                .ToDictionary(group => group.Key, group => group.Single());
            return new SortedDictionary<string, Prompt>(dictionary);
        }

        private static Prompt ParsePrompt(string wavDirectory, string rawPrompt)
        {
            string[] parts = rawPrompt.Split('|');
            if (parts.Length != 3)
            {
                return Prompt.Empty;
            }

            string wavPath = Path.Combine(wavDirectory, $"{parts[0]}.wav");
            Prompt prompt = new Prompt(parts[0], parts[1], parts[2], wavPath);

            return prompt;

        }
    }
}
