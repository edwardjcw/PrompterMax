using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System;

namespace Prompter
{
    public class Prompter : INotifyPropertyChanged
    {
        private string promptFile;
        private string wavDirectory;
        private SortedDictionary<int, Prompt> prompts;
        private int at = 0;

        private string previous;
        private string current;
        private string next;

        public event PromptChangedEventHandler PromptChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public int At => at;
        public int Count => prompts.Count;

        public string Previous
        {
            get => previous;
            set
            {
                if (previous != value)
                {
                    previous = value;
                    OnPropertyChanged(nameof(Previous));
                }
            }
        }

        public string Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        public string Next
        {
            get => next;
            set
            {
                if (next != value)
                {
                    next = value;
                    OnPropertyChanged(nameof(Next));
                }
            }
        }

        public string WavPath
        {
            get
            {
                bool success = prompts.TryGetValue(At, out Prompt result);
                return success ? result.WavPath : "";
            }
        }

        public string Normalized
        {
            get
            {
                bool success = prompts.TryGetValue(At, out Prompt result);
                return success ? result.Normalized : "";
            }
        }

        public void Load(string promptFile, string wavDirectory)
        {
            this.promptFile = promptFile;
            this.wavDirectory = wavDirectory;

            prompts = LoadPrompts(promptFile, wavDirectory);
            Prompt lastPromptWithWav = GetLastPromptWithWav(prompts);
            GoNext(lastPromptWithWav);
        }
        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private void GoNext(Prompt prompt)
        {
            at = prompt == Prompt.Empty || prompt.Id == prompts.Count - 1 ? 0 : prompt.Id + 1;
            Previous = at == 0 ? "" : prompts[at - 1].Normalized;
            Current = prompts[at].Normalized;
            Next = at == prompts.Count - 1 ? "" : prompts[at + 1].Normalized;
            OnMove(at);
        }
        private void GoPrevious(Prompt prompt)
        {
            at = prompt == Prompt.Empty || prompt.Id == 0 ? 0 : prompt.Id - 1;
            Previous = at == 0 ? "" : prompts[at - 1].Normalized;
            Current = prompts[at].Normalized;
            Next = at == prompts.Count - 1 ? "" : prompts[at + 1].Normalized;
            OnMove(at);
        }

        private void Goto(Prompt prompt)
        {
            at = prompt.Id;
            Previous = at == 0 ? "" : prompts[at - 1].Normalized;
            Current = prompts[at].Normalized;
            Next = at == prompts.Count - 1 ? "" : prompts[at + 1].Normalized;
            OnMove(at);
        }

        private void OnMove(int index)
        {
            PromptChanged?.Invoke(this, new PrompterEventArgs(index));
        }



        private static Prompt GetLastPromptWithWav(SortedDictionary<int, Prompt> prompts)
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

        public void NextPrompt()
        {
            bool success = prompts.TryGetValue(At, out Prompt result);
            if (!success)
            {
                return;
            }

            GoNext(result);
        }

        public void PreviousPrompt()
        {
            bool success = prompts.TryGetValue(At, out Prompt result);
            if (!success)
            {
                return;
            }

            GoPrevious(result);
        }

        public void Goto(int index)
        {
            bool success = prompts.TryGetValue(index, out Prompt result);
            if (!success)
            {
                return;
            }

            Goto(result);
        }
    }
}
