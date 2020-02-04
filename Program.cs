#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleCJKRedirect
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Filename must be specified with the command line.");
				Environment.Exit(255);
			}

			var filename = args[0];

			var dataLines = File.ReadAllLines(filename);

			var outputLines = new List<string>();
			var formattedData = new List<string>();
			var sourceData = new SortedDictionary<int, int>();
			var inlineComment = new Dictionary<int, string>();
			for (var i = 0; i < dataLines.Length; i++)
			{
				var line = dataLines[i].Trim();
				var dataString = line.Split(';')[0];
				if (string.IsNullOrEmpty(dataString))
				{
					formattedData.Add(line);
					continue;
				}

				var data = dataString.Split(',');
				if (data.Length != 2)
					throw new SyntaxException(i + 1);

				int src, dst;
				try
				{
					src = Convert.ToInt32(data[0].Trim(), 16);
					dst = Convert.ToInt32(data[1].Trim(), 16);
				}
				catch (Exception e)
				{
					throw new SyntaxException(i + 1, e);
				}

				try
				{
					sourceData.Add(src, dst);
				}
				catch (ArgumentException) {}

				if (line.IndexOf(';') < 0)
					continue;

				var thisInlineComment = line.Substring(line.IndexOf(';') + 1).Trim();
				if (string.IsNullOrEmpty(thisInlineComment))
					continue;
				try
				{
					inlineComment.Add(src, thisInlineComment);
				}
				catch (ArgumentException)
				{
					inlineComment[src] += ", " + thisInlineComment;
				}
			}

			formattedData.AddRange(sourceData.Select(i => $"{i.Key:X},{i.Value:X}{(inlineComment.ContainsKey(i.Key) ? $"\t; {inlineComment[i.Key]}" : "")}"));
			outputLines.AddRange(sourceData.Select(i => $"{char.ConvertFromUtf32(i.Key)} {char.ConvertFromUtf32(i.Value)}"));

			var outputFileName = Path.GetFullPath(filename + ".out");
			Console.WriteLine($"Characters are writing to {outputFileName} ...");
			File.WriteAllLines(filename, formattedData.Where(s => !string.IsNullOrEmpty(s)));
			File.WriteAllLines(outputFileName, outputLines.Where(s => !string.IsNullOrEmpty(s)));
		}

		private class SyntaxException : Exception
		{
			public SyntaxException(int line) : base($"Syntax error at line {line}.") {}

			public SyntaxException(int line, Exception? innerException) : base($"Syntax error at line {line}.", innerException) {}
		}
	}
}
