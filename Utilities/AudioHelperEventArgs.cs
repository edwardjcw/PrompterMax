namespace Utilities
{
    public enum AudioStatus
    {
        PlayStopped,
        Playing,
        Recording,
        RecordStopped
    }


    public class AudioHelperEventArgs
    {
        public AudioHelperEventArgs(AudioStatus status)
        {
            Status = status;
        }

        public AudioStatus Status { get; }
    }
}