using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyFortressPersonalityParser
{
	class Character
	{
		private static Dictionary<String, String> Months = null;

		private String name = null;
		private String race = null;
		private String birth = null;
		private String gender = null;
		private String arrived = null;
		private String[] appearance = null;
		private String[] physical = null;
		private String[] physicalnegative = null;
		private String[] likemat = null;
		private String[] likefood = null;
		private String[] dislikes = null;
		private String[] mental = null;
		private String[] mentalnegative = null;
		private String[] disadvantages = null;
		private String[] values = null;
		private String[] disregards = null;
		private String dream = null;
		private String[] personality = null;

		private List<String> faultylines = null;

		public String Name
		{
			get { return name; }
			private set { name = value; }
		}



		private Character()
		{
			if (Character.Months == null)
			{
				Character.Months = new Dictionary<string, string>(12);
				Character.Months.Add("Granite",		"March");
				Character.Months.Add("Slate",		"April");
				Character.Months.Add("Felsite",		"May");

				Character.Months.Add("Hematite",	"June");
				Character.Months.Add("Malachite",	"July");
				Character.Months.Add("Galena",		"August");

				Character.Months.Add("Limestone",	"September");
				Character.Months.Add("Sandstone",	"October");
				Character.Months.Add("Timber",		"November");

				Character.Months.Add("Moonstone",	"December");
				Character.Months.Add("Opal",		"January");
				Character.Months.Add("Obsidian",	"February");
			}
		}

		public static Character Parse(String[] ProcessedLines) {
			String[] c = ProcessedLines; // Significantly quicker to write
			// Data section
			Character pon = new Character();
			//pon.faultylines = new String[c.Length];
			//for (int i = 0; i < c.Length; i++) pon.faultylines[i] = c[i];
			pon.faultylines = c.ToList();

			// Gender funk
			String pronoun1 = null;
			String pronoun2 = null;


			/* ------------------------------------- */
			/*		 ----Extraction time!----        */
			/* ------------------------------------- */
			// ---- Gender detection
			// Just checking if the first word is he or she
			if (c[0].Contains("He"))
			{
				pronoun1 = "He";
				pronoun2 = "His";
				pon.gender = "Stallion";
			}
			else
			{
				pronoun1 = "She";
				pronoun2 = "Her";
				pon.gender = "Mare";
			}
			Console.WriteLine("Gender extracted:\t" + pon.gender);


			// ---- Name extraction procedure
			// Find the egde of appearance. It stops either with his/her eyes or mane/tail stuff.
			// Physical is always only one line and the name is after that.
			// After fínding the line, we cut off at the second whitespace
			int nameline = 0;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Contains(pronoun2 + " eyes are") || c[i].Contains(pronoun2 + " mane and tail"))
					nameline = i + 2;
			}
			int nameend = c[nameline].IndexOf(' ');
			nameend = c[nameline].IndexOf(' ', nameend+1);
			pon.name = c[nameline].Substring(0,nameend);

			//pon.faultylines.Remove(c[nameline]); Don't do that! The line is for likes and such
			Console.WriteLine("Name extracted:\t\t" + pon.name);

			
			// ---- Race extraction procedure
			// Just check the last line
			String lastline = c.Last();
			int raceline = c.Length - 1;
			if (lastline.Contains("nature"))
				pon.race = "Earth Pony";
			else if (lastline.Contains("wings"))
				pon.race = "Pegasus";
			else if (lastline.Contains("horn"))
				pon.race = "Unicorn";
			else
				pon.race = "ERROR RACE";

			pon.faultylines.Remove(lastline);
			Console.WriteLine("Race extracted:\t\t" + pon.race);


			// ---- Birth date extraction procedure
			// Just iterate through and find a line with the unique wordings.
			// Then do some manipulation and get the day, month and year.
			int birthline = 0;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Contains("born on the"))
				{
					birthline = i;
					// Getting the string
					String b = c[i];
					// Day extraction
					int index = b.IndexOf("the ")+4;
					int oindex = b.IndexOf(" ", index);
					String day = b.Substring(index, oindex - index).Trim();
					// Month extraction
					index = b.IndexOf("of ") + 3;
					oindex = b.IndexOf(" ", index);
					String month = "ERRORMONTH";
					try
					{
						month = Character.Months[b.Substring(index, oindex - index).Trim()];
					}
					catch (KeyNotFoundException)
					{ }
					// Year extraction
					index = b.IndexOf("year ") + 5;
					oindex = b.Length-2;
					String year = b.Substring(index, oindex - index).Trim();


					// Adding the data
					pon.birth = day + " " + month + ", " + year;
					// Removing it from the fault line list
					pon.faultylines.Remove(b);

					// Done
					break;
				}
			}
			Console.WriteLine("Birth date extracted:\t" + pon.birth);


			// ---- Arrival date extraction procedure
			// Same as the Birth extraction
			// Just iterate through and find a line with the unique wordings.
			// Then do some manipulation and get the day, month and year.
			int arrivalline = 0;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Contains("arrived at"))
				{
					arrivalline = i;
					String b = c[i];

					int index = b.IndexOf("on the ") + 7;
					int oindex = b.IndexOf(" ", index);
					String day = b.Substring(index, oindex - index).Trim();

					index = b.IndexOf(" of ") + 4;
					oindex = b.IndexOf(" ", index);
					String month = "ERRORMONTH";
					try
					{
						month = Character.Months[b.Substring(index, oindex - index).Trim()];
					}
					catch (KeyNotFoundException)
					{ }

					index = b.IndexOf("year ") + 5;
					oindex = b.Length - 2;
					String year = b.Substring(index, oindex - index).Trim();

					// Adding the data
					pon.arrived = day + " " + month + ", " + year;
					// Removing it from the fault line list
					pon.faultylines.Remove(b);
					
					// Done
					break;
				}
			}
			Console.WriteLine("Arrived extracted:\t" + pon.arrived);


			// ---- Appearance extraction procedure
			// Find the start, for the end.
			// The start is right after the arrived date line. Easy to find.
			// The last line is the one before the first to not use the owning pronoun.
			int appearancestart = 0;
			int appearancestop = 0;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Contains("arrived at"))
				{
					appearancestart = i + 2;
					break;
				}
			}
			for (int i = appearancestart; i < c.Length; i++)
			{
				if (c[i].Trim().StartsWith(pronoun1))
				{
					appearancestop = i - 1;
					break;
				}
			}

			pon.appearance = new String[appearancestop - appearancestart + 1];
			int counter = 0;
			for (int i = appearancestart; i <= appearancestop; i++)
			{
				pon.appearance[counter++] = c[i];
			}
			Console.Write("Appearance extracted:\t");
			for (int i = 0; i < pon.appearance.Length; i++)
			{
				if (i == (pon.appearance.Length - 1))
					Console.WriteLine(pon.appearance[i]);
				else
				Console.Write(pon.appearance[i] + "\r\n\t\t\t");
			}
			// ---- Physical extraction procedure
			// Right after the last line of Appearance.
			int physicalline = appearancestop + 1;
			String p = c[physicalline];
			Console.WriteLine(p);
			String physicalpos = null;
			String physicalneg = null;
			Console.WriteLine(p.IndexOf("but"));
			if (p.IndexOf("but") != -1)
			{
				physicalpos = p.Substring(0, p.IndexOf("but")).Trim();
				physicalneg = p.Substring(p.IndexOf("but") + 3, p.Length - (p.IndexOf("but") + 3)).Trim();
				physicalneg = char.ToUpper(physicalneg[0]) + physicalneg.Substring(1);
			}
			else
			{
				physicalpos = p.Trim();
			}
			//------------------------------------
			//------------------------------------
			//------------------------------------
			// TO DO
			//------------------------------------
			//------------------------------------
			//------------------------------------

			pon.physical = new String[1];
			pon.physical[0] = physicalpos;
			pon.physicalnegative = new String[1];
			pon.physicalnegative[0] = physicalneg;

			// Remove it from the faulty lines list
			pon.faultylines.Remove(p);
			Console.WriteLine("positives:\t" + physicalpos);
			Console.WriteLine("negatives:\t" + physicalneg);


			return pon;
		}

		public override String ToString()
		{
			return Output(Format.Plain);
		}

		public String ToHTML()
		{
			return Output(Format.HTML);
		}


		public String ToRTF()
		{
			return Output(Format.RTF);
		}

		private String Output(Format format)
		{
			switch (format)
			{
				case Format.Plain:
					break;
				case Format.HTML:
					break;
				case Format.RTF:
					break;
				default:
					return null;
			}

			return null;
			StringBuilder builder = new StringBuilder();
			builder.Append("Name:\r\n" + name + "\r\n");
			builder.Append("\r\nRace:\r\n" + race + "\r\n");
			builder.Append("\r\nBirth date:\r\n" + birth + "\r\n");
			builder.Append("\r\nGender:\r\n" + gender + "\r\n");

			builder.Append("\r\nAppearance:\r\n");
			for (int i = 0; i < appearance.Length; i++)
			{
				builder.Append(appearance[i] + "\r\n");
			}

			builder.Append("\r\nPhysical:\r\n");
			for (int i = 0; i < physical.Length; i++)
			{
				builder.Append(physical[i] + "\r\n");
			}

			builder.Append("\r\nMaterial Likings:\r\n");
			for (int i = 0; i < likemat.Length; i++)
			{
				builder.Append(likemat[i] + "\r\n");
			}
			builder.Append("\r\nFood Likings:\r\n");
			for (int i = 0; i < likefood.Length; i++)
			{
				builder.Append(likefood[i] + "\r\n");
			}

			builder.Append("\r\nDislikes:\r\n");
			for (int i = 0; i < dislikes.Length; i++)
			{
				builder.Append(dislikes[i] + "\r\n");
			}

			builder.Append("\r\nMental:\r\n");
			for (int i = 0; i < mental.Length; i++)
			{
				builder.Append(mental[i] + "\r\n");
			}

			builder.Append("\r\nDisadvantages:\r\n");
			for (int i = 0; i < disadvantages.Length; i++)
			{
				builder.Append(disadvantages[i] + "\r\n");
			}

			builder.Append("\r\nValues:\r\n");
			for (int i = 0; i < values.Length; i++)
			{
				builder.Append(values[i] + "\r\n");
			}

			builder.Append("\r\nDisregards:\r\n");
			for (int i = 0; i < disregards.Length; i++)
			{
				builder.Append(disregards[i] + "\r\n");
			}

			builder.Append("\r\nDream:\r\n" + dream + "\r\n");

			builder.Append("\r\nPersonality:\r\n");
			for (int i = 0; i < personality.Length; i++)
			{
				builder.Append(personality[i] + "\r\n");
			}

			return builder.ToString();
		}
	}
	enum Format
	{
		Plain,
		HTML,
		RTF
	}
}
