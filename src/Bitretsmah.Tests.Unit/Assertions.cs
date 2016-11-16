using Newtonsoft.Json;
using System;
using System.IO;

namespace Bitretsmah.Tests.Unit
{
    public static class Assertions
    {
        public static void ShouldSerializeSameAs(this object actual, object expected)
        {
            if (expected == actual) return;

            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);

            if (expectedJson == actualJson) return;

            string path = string.Empty;

            try
            {
                path = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, "expected.json"), expectedJson);
                File.WriteAllText(Path.Combine(path, "actual.json"), actualJson);
            }
            finally
            {
                throw new JsonException($"The actual JSON does not match the expected. Compare files in directory: '{path}'.");
            }
        }
    }
}