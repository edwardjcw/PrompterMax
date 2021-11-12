namespace Prompter
{
    public class PrompterEventArgs
    {
        public PrompterEventArgs(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}