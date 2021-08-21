﻿using System;
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
        private SortedDictionary<int, Prompt> prompts;
        private int at = 0;

        public event PromptChangedEventHandler PromptChanged;
        public int At { get => at; }
        public int Count { get => prompts.Count;}
        public string WavPath
        {
            get
            {
                bool success = prompts.TryGetValue(At, out Prompt result);
                return success ? result.WavPath : "";
            }
        }

        public void Load(string promptFile, string wavDirectory)
        {
            this.promptFile = promptFile;
            this.wavDirectory = wavDirectory;

            prompts = LoadPrompts(promptFile, wavDirectory);
            Prompt lastPromptWithWav = GetLastPromptWithWav(prompts);
            Next(lastPromptWithWav, prompts);
        }

        private void Next(Prompt prompt, SortedDictionary<int, Prompt> prompts)
        {
            at = prompt == Prompt.Empty || prompt.Id == prompts.Count - 1 ? 0 : prompt.Id + 1;
            string previous = at == 0 ? "" : prompts[at - 1].Normalized;
            string current = prompts[at].Normalized;
            string next = at == prompts.Count - 1 ? "" : prompts[at + 1].Normalized;
            OnNext(at, previous, current, next);
        }

        private void OnNext(int index, string previous, string current, string next)
        {
            PromptChanged?.Invoke(this, new PrompterEventArgs(index, previous, current, next));
        }

        private Prompt GetLastPromptWithWav(SortedDictionary<int, Prompt> prompts)
        {
            Prompt lastOne = prompts.LastOrDefault(p => File.Exists(p.Value.WavPath)).Value;
            return lastOne ?? Prompt.Empty;
        }

        private static SortedDictionary<int, Prompt> LoadPrompts(string promptFile, string wavDirectory)
        {

            Prompt ParsePromptWithWav(string prompt, int id) => ParsePrompt(id, wavDirectory, prompt);
            Dictionary<int, Prompt> dictionary = File.ReadAllLines(promptFile)
                .Select(ParsePromptWithWav)
                .Where(prompt => prompt != Prompt.Empty)
                .GroupBy(prompt => prompt.Id)
                .ToDictionary(group => group.Key, group => group.Single());
            return new SortedDictionary<int, Prompt>(dictionary);
        }

        private static Prompt ParsePrompt(int id, string wavDirectory, string rawPrompt)
        {
            string[] parts = rawPrompt.Split('|');
            if (parts.Length != 3)
            {
                return Prompt.Empty;
            }

            string wavPath = Path.Combine(wavDirectory, $"{parts[0]}.wav");
            Prompt prompt = new Prompt(id, parts[0], parts[1], parts[2], wavPath);

            return prompt;

        }
    }
}