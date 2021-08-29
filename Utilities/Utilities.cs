using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Utilities
{
    public static class Utilities
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
    }
}
