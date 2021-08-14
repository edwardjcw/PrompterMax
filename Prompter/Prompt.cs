using System;

namespace Prompter
{
    internal class Prompt
    {
        public string Id { get; private set; }
        public string Transcription { get; private set; }
        public string Normalized { get; private set; }
        public string WavPath { get; private set; }

        private static Prompt empty;
        public static Prompt Empty
        {
            get
            {
                if (empty == null)
                {
                    empty = new Prompt("", "", "", "");
                }
                return empty;

            }
        }

        public Prompt(string id, string transcription, string normalized, string wavPath)
        {
            Id = id;
            Transcription = transcription;
            Normalized = normalized;
            WavPath = wavPath;
        }


    }
}