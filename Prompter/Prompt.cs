namespace Prompter
{
    internal class Prompt
    {
        public int Id { get; }
        public string Name { get; }
        public string Transcription { get; }
        public string Normalized { get; }
        public string WavPath { get; }

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