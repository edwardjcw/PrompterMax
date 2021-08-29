using System;

namespace Prompter
{
    internal class Prompt
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Transcription { get; private set; }
        public string Normalized { get; private set; }
        public string WavPath { get; private set; }

        private static Prompt s_Empty;
        public static Prompt Empty => s_Empty ?? (s_Empty = new Prompt(-1, "", "", "", ""));

        public Prompt(int id, string name, string transcription, string normalized, string wavPath)
        {
            Id = id;
            Name = name;
            Transcription = transcription;
            Normalized = normalized;
            WavPath = wavPath;
        }


    }
}