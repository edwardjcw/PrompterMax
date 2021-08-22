using NAudio.Wave;
using System;

namespace Utilities
{
    public enum PlaybackStatus
    {
        Stopped,
        Playing
    }

    public enum RecordingStatus
    {
        Stopped,
        Recording
    }

    public class AudioHelper
    {
        private readonly WaveOutEvent player;
        private readonly WaveInEvent recorder;
        private AudioFileReader reader;
        private WaveFileWriter writer;

        public PlaybackStatus PlaybackStatus { get; private set; }
        public RecordingStatus RecordingStatus { get; private set; }

        public event AudioHelperEventHandler AudioChanged;

        public AudioHelper()
        {
            player = new WaveOutEvent();
            player.PlaybackStopped += Player_PlaybackStopped;
            PlaybackStatus = PlaybackStatus.Stopped;
            recorder = new WaveInEvent();
            recorder.RecordingStopped += Recorder_RecordingStopped;
            recorder.DataAvailable += Recorder_DataAvailable;
            recorder.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 2);
            RecordingStatus = RecordingStatus.Stopped;
        }

        private void Recorder_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            RecordingStatus = RecordingStatus.Stopped;
            writer?.Dispose();
            writer = null;
            OnRecordingStopped();
        }

        private void OnRecordingStopped()
        {
            AudioChanged?.Invoke(this, new AudioHelperEventArgs(AudioStatus.RecordStopped));
        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStatus = PlaybackStatus.Stopped;
            reader?.Dispose();
            reader = null;
            OnPlaybackStopped();
        }

        private void OnPlaybackStopped()
        {
            AudioChanged?.Invoke(this, new AudioHelperEventArgs(AudioStatus.PlayStopped));
        }

        public void Play(string wavPath)
        {
            reader = new AudioFileReader(wavPath);
            player.Init(reader);
            PlaybackStatus = PlaybackStatus.Playing;
            player.Play();
            OnPlaybackStarted();
        }

        private void OnPlaybackStarted()
        {
            AudioChanged?.Invoke(this, new AudioHelperEventArgs(AudioStatus.Playing));
        }

        public void StopPlaying()
        {
            player.Stop();
        }

        public void Record(string wavPath)
        {
            writer = new WaveFileWriter(wavPath, recorder.WaveFormat);
            RecordingStatus = RecordingStatus.Recording;
            recorder.StartRecording();
            OnRecordingStarted();
        }

        private void OnRecordingStarted()
        {
            AudioChanged?.Invoke(this, new AudioHelperEventArgs(AudioStatus.Recording));
        }

        public void StopRecording()
        {
            recorder.StopRecording();
        }
    }
}
