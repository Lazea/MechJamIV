using System;
using System.Collections.Generic;
using System.IO;

public class IniParser
{
    private Dictionary<string, Dictionary<string, string>> data;

    public IniParser()
    {
        data = new Dictionary<string, Dictionary<string, string>>();
    }

    public void Parse(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("INI file not found.", filePath);
        }

        string[] lines = File.ReadAllLines(filePath);
        string currentSection = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                if (!data.ContainsKey(currentSection))
                {
                    data[currentSection] = new Dictionary<string, string>();
                }
            }
            else if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith(";"))
            {
                int equalSignIndex = trimmedLine.IndexOf('=');
                if (equalSignIndex > 0)
                {
                    string key = trimmedLine.Substring(0, equalSignIndex).Trim();
                    string value = trimmedLine.Substring(equalSignIndex + 1).Trim();
                    if (currentSection != null && !data[currentSection].ContainsKey(key))
                    {
                        data[currentSection][key] = value;
                    }
                }
            }
        }
    }

    public void WriteToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> section in data)
            {
                writer.WriteLine($"[{section.Key}]");

                foreach (KeyValuePair<string, string> pair in section.Value)
                {
                    writer.WriteLine($"{pair.Key}={pair.Value}");
                }

                writer.WriteLine();
            }
        }
    }

    public T GetValue<T>(string section, string key)
    {
        if (data.TryGetValue(section, out var sectionData) && sectionData is Dictionary<string, string> keyValuePairs)
        {
            if (keyValuePairs.TryGetValue(key, out string valueString))
            {
                try
                {
                    return (T)Convert.ChangeType(valueString, typeof(T));
                }
                catch (FormatException)
                {
                    throw new FormatException("Invalid format for valueString");
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException("Invalid cast for valueString");
                }
                catch (NotSupportedException)
                {
                    throw new NotSupportedException("Conversion not supported for type T");
                }
                catch (Exception)
                {
                    // Handle any other unexpected exceptions
                    throw;
                }
            }
        }

        return default(T);
    }

    public void SetValue(string section, string key, string value)
    {
        if (!data.ContainsKey(section))
        {
            data[section] = new Dictionary<string, string>();
        }

        data[section][key] = value;
    }
}

