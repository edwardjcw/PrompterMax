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

        public int Index { get; private set; }
        public string Previous { get; private set; }
        public string Current { get; private set; }
        public string Next { get; private set; }
    }
}