using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PonyFortressPersonalityParser
{
	class Program
	{
		private int done = 0;
		static void Main(string[] args)
		{
			Program prog = new Program();
			StreamReader input = null;
			StreamWriter output = null;
			String genderform1 = null;
			String genderform2 = null;

			Console.WriteLine("Received " + args.Length + " raw pony personality files.");
			Console.WriteLine("Parsing...\r\n");
			for (int l = 0; l < args.Length; l++)
			{
				// Static data
				List<string> deleted = new List<string>();
				char[] seps = { '\\', '/' };
				String[] s = args[l].Split(seps);
				char[] caps = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
								'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
				// Welcoming message
				Console.WriteLine("Parsing \"" + s[s.Length - 1] + "\"...");

				// Initial security
				if (!File.Exists(args[l]))
				{
					Console.WriteLine("\"" + args[l] + "\" does not exist");
					continue;
				}

				
				// Character info
				Character pony = null;
				String[] processedlines = null;

				// File Reading
				// --------------------------------------
				// Reading raw file
				input = new StreamReader(args[l]);
				String raw = input.ReadToEnd();
				input.Close();





				// Processing
				// ----------------------------------

				// Determining gender
				if (raw.IndexOf("She") == -1)
				{
					genderform1 = "He";
					genderform2 = "His";
				}
				else
				{
					genderform1 = "She";
					genderform2 = "Her";
				}

				// Whitespace removal
				String processed = raw.Replace("\t", " ").Replace("\r\n", " ");
				for (int i = 0; i < caps.Length; i++)
				{
					processed = processed.Replace(caps[i].ToString(), "\r\n" + caps[i]);
				}

				// Motto removal
				int index = processed.LastIndexOf("&");
				if (index != -1)
				{
					index++;
					String del = processed.Substring(0, index);
					deleted.Add(del);
					processed = processed.Substring(index, processed.Length - index);
				}

				// race removal
				/*index = processed.IndexOf("\r\nA pony");
				if (index != -1)
				{
					String del = processed.Substring(index, processed.Length - index);
					deleted.Add(del);
					processed = processed.Substring(0, index);
				}*/

				// Sort out lineendings
				index = 0;
				index = processed.IndexOf("\r\n", index + 1);
				while (index != -1)
				{
					//Console.WriteLine(index);
					String test = processed.Substring(index, 6);
					//Console.WriteLine(test);
					if (!test.Contains(genderform1 + " ") && !test.Contains(genderform2 + " ") && !test.Contains("Like"))
					{
						int tindex = processed.IndexOf("\r\n", index + 1);
						tindex = processed.IndexOf("\r\n", tindex + 1);

						//processed = processed.Replace(test, test.Replace("\r\n", " "));
						//Console.WriteLine(index + "\t" + tindex);
						//Console.WriteLine("=\"" + processed.Substring(index + 1, 10) + "\"");
						//Console.WriteLine("=\"" + processed.Substring(index + 1, tindex - index) + "\"");
						if (tindex >= index)
						{
							String p = processed.Substring(index+1, tindex - index);

							if (!p.Contains("likes"))
								processed = processed.Replace(test, test.Replace("\r\n", " "));
							else
								processed = processed.Replace(p, p.Replace("\r\n", " "));

						}


					}

					index = processed.IndexOf("\r\n", index + 1);
				}

				// Filtering out whitespaces again
				while (processed.Contains("  ")) processed = processed.Replace("  ", " ");
				processed = processed.Trim();
				processedlines = processed.Split('\n');
				foreach (string item in processedlines) item.Trim();


				// Parsing
				// -----------------------------------
				pony = Character.Parse(processedlines);
				//Console.WriteLine(processed);




				// Data Extraction
				// ------------------------------------
				Console.WriteLine(pony.ToString());

				output = new StreamWriter(args[l].Replace(s[s.Length - 1], s[s.Length - 1].Replace(".txt", "_processed.txt")), false);
				output.Write(processed);
				output.Flush();
				output.Close();
				output = new StreamWriter(args[l].Replace(s[s.Length - 1], s[s.Length - 1].Replace(".txt", "_sheet.txt")), false);
				output.Write(pony.ToString());
				output.Flush();
				output.Close();
				output = new StreamWriter(args[l].Replace(s[s.Length - 1], s[s.Length - 1].Replace(".txt", "_sheet2.txt")), false);
				output.Write(pony);
				output.Flush();
				output.Close();
				output = new StreamWriter(args[l].Replace(s[s.Length - 1], s[s.Length - 1].Replace(".txt", "_html.txt")), false);
				output.Write(pony.ToHTML());
				output.Flush();
				output.Close();

				//Console.WriteLine();
				//Console.WriteLine();
				//Console.WriteLine(args[l].Replace(s[s.Length - 1], s[s.Length - 1].Replace(".txt","_sheet.txt")));
				Console.WriteLine("The pony, " + pony.Name + ", has been parsed");
				prog.done++;
			}
			

			Console.WriteLine();
			string pon = "ponies";
			if (prog.done == 1)
				pon = "pony";
			Console.WriteLine(prog.done + " "+pon+" parsed, out of " + args.Length);
			Console.WriteLine("Personality parsing, done.");
			Console.ReadKey();
		}
	}
}