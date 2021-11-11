using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using FunctionalUtilities;

namespace Utilities
{
    public static class General
    {
        public static string TransformForOutput(List<string> texts, uint version)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < texts.Count; i++)
            {
                string text = texts[i];
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                string id = $"EJ{version:000}-{i:00000}";
                output.Add($"{id}|{text}|{text}");
            }
            return string.Join(Environment.NewLine, output);
        }

        public static bool WavExists(string wavPath)
        {
            return File.Exists(wavPath);
        }

        // calculate the speech to noise ratio of a wav file
        public static double SpeechToNoiseRatio(string filename)
        {
            // open the file
            using (var reader = new AudioFileReader(filename))
            {
                // get the length of the file
                var length = reader.Length;

                // get the number of samples
                var samples = length / 2;

                // create a new array of bytes
                var bytes = new byte[samples];

                // read the file
                reader.Read(bytes, 0, bytes.Length);

                // calculate the average power
                var power = 0.0;
                foreach (byte v in bytes)
                {
                    power += v * v;
                }
                power /= bytes.Length;

                // calculate the speech to noise ratio
                return power / (128 * 128);
            }
        }

        public static int Levenshtein(string first, string second)
        {
            return Calculate.levenshtein(first, second);
        }

        public static string RemovePunctuation(string text)
        {
            return Transform.removePunctuation(text);
        }
    }
}
