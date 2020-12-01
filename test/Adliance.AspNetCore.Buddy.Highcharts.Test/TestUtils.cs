﻿using System;
using System.IO;

namespace Adliance.Highcharts.Buddy.Test
{
    public static class TestUtils
    {
        public static byte[] GetEmbeddedResource(params string[] parts)
        {
            var resourceName = "Adliance.Highcharts.Buddy.Test." + string.Join(".", parts);
            var stream = typeof(TestUtils).Assembly.GetManifestResourceStream("Adliance.Highcharts.Buddy.Test." + string.Join(".", parts));
            if (stream == null)
            {
                throw new Exception($"Resource \"{resourceName}\" does not exist.");
            }
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string GetEmbeddedString(params string[] parts)
        {
            var resourceName = "Adliance.Highcharts.Buddy.Test." + string.Join(".", parts);
            var stream = typeof(TestUtils).Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception($"Resource \"{resourceName}\" does not exist.");
            }
            using (var r = new StreamReader(stream))
            {
                return r.ReadToEnd();
            }
        }

        public static void StoreLocally(string filename, byte[] bytes)
        {
            var directory = @"C:\Users\hannes\Downloads";
            if (Directory.Exists(directory))
            {
                File.WriteAllBytes(Path.Combine(directory, filename), bytes);
            }
        }
    }
}