namespace Prompter
{
    public class PrompterEventArgs
    {
        public PrompterEventArgs(int index, string previous, string current, string next)
        {
            Index = index;
            Previous = previous;
            Current = current;
            Next = next;
        }

        public int Index { get; }
        public string Previous { get; }
        public string Current { get; }
        public string Next { get; }
    }
}