using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;

namespace Actor.DbService.Core.Model
{

    public static class SyllableParser
    {
        public static List<string> voyels = new List<string> { "a", "e", "i", "o", "u", "y", "ï", "î", "â", "é", "è", "ê", "ù", "ô" };

        public static bool IsVoyel(char c)
        {
            return voyels.Contains(c.ToString());
        }

        public static Dictionary<string,int> wordExceptions = new Dictionary<string, int>();

        public static CultureInfo FrenchCulture = new CultureInfo("fr-FR");

        public static int Parse(string word)
        {
            if (!wordExceptions.Any())
            {
                wordExceptions["prudent"] = 1;
                wordExceptions["imprudent"] = 3;
            }
            var lowerword = word.ToLower(FrenchCulture);
            if (wordExceptions.TryGetValue(lowerword, out int sylexcep))
            {
                return sylexcep;
            }
            var schemeToArray = new StringBuilder();
            bool isFirstVoyel = true;
            bool isLastVoyel = false;
            int i = 0;
            while (i < lowerword.Length)
            {
                char currentChar = lowerword[i];
                if (char.IsDigit(currentChar))
                {
                    break;
                }
                switch (currentChar)
                {
                    case 'q':
                        if ((i + 1) < lowerword.Length && (lowerword[i + 1] == 'u'))
                        {
                            schemeToArray.Append("-");
                            i = i + 2;
                        }
                        break;
                    case '\'':
                        i = i + 1;
                        break;
                    default:
                        if (IsVoyel(currentChar))
                        {
                            if (currentChar == 'e')
                            {
                                if (isFirstVoyel)
                                {
                                    schemeToArray.Append('*');
                                    i++;
                                }
                                else
                                {
                                    isLastVoyel = true;
                                    for (int j = i + 1; j < lowerword.Length; j++)
                                    {
                                        if (IsVoyel(lowerword[j]))
                                        {
                                            isLastVoyel = false;
                                            break;
                                        }
                                    }
                                    if (isLastVoyel)
                                    {
                                        schemeToArray.Append('e');
                                        i++;
                                    }
                                    else
                                    {
                                        schemeToArray.Append('*');
                                        i++;
                                    }
                                }
                            }
                            else if (currentChar == 'ï')
                            {
                                schemeToArray.Append("*-*");
                                i++;
                            }
                            else if (currentChar == 'i')
                            {
                                if ((i + 1) < lowerword.Length && (lowerword[i + 1] == 'o'))
                                {
                                    schemeToArray.Append("*-*");
                                    i = i + 2;
                                }
                                else if ((i + 2) < lowerword.Length && (lowerword[i + 1] == 'e') && (lowerword[i + 2] == 'u'))
                                {
                                    schemeToArray.Append("*-*");
                                    i = i + 3;
                                }
                                else
                                {
                                    schemeToArray.Append("*");
                                    i++;
                                }
                            }
                            else
                            {
                                schemeToArray.Append("*");
                                i++;
                            }
                            isFirstVoyel = false;
                        }
                        else
                        {
                            if (isLastVoyel)
                            {
                                schemeToArray.Append(currentChar);
                                i++;

                            }
                            else
                            {
                                schemeToArray.Append("-");
                                i++;

                            }
                        }
                        break;
                }
            }
            string scheme = schemeToArray.ToString();
            List<string> patterns = new List<string> { "*", "*-", "**", "**-", "-*", "-**", "--**", "-*-", "-**-", "--*", "--er", "-er", "er", "-***" };
            int syllabeCount = 0;
            while (!(scheme == ""))
            {
                bool patternFound = false;
                foreach (var pattern in patterns.OrderByDescending(p => p.Length))
                {
                    if (scheme.StartsWith(pattern))
                    {
                        scheme = scheme.Substring(pattern.Length);
                        syllabeCount++;
                        patternFound = true;
                        break;
                    }
                }
                if (!patternFound)
                    break;
            }
            return syllabeCount;
        }
    }

    public static class Functer
    {
        public static Dictionary<string, int> WordSyllabe = new Dictionary<string, int>();
        public static Dictionary<string, string> WordRime = new Dictionary<string, string>();
        public static CultureInfo frenchCulture = new CultureInfo("FR-fr");


        public static void InitDico(string wordSyllabeFilename, string wordRimeFilename)
        {
            // word syllabe
            using (var reader = new StreamReader(wordSyllabeFilename))
            {
                while (!reader.EndOfStream)
                {
                    string s = reader.ReadLine();
                    var ws = s.Split(',');
                    WordSyllabe[ws[0]] = int.Parse(ws[1]);
                }
            }
            // word rime
            using (var reader = new StreamReader(wordRimeFilename))
            {
                while (!reader.EndOfStream)
                {
                    string s = reader.ReadLine();
                    var ws = s.Split(',');
                    WordRime[ws[0]] = ws[1];
                }
            }
        }

        public static readonly Func<string, DataFolder, IEnumerable<Field>> RimeFuncter = (s, folder) =>
        {
            var word = s.Trim().Trim(new char[] { ',', ';', ':' });
            var lowerWord = word.ToLower(frenchCulture);

            List<Field> fields = new List<Field>();

            if (WordSyllabe.TryGetValue(lowerWord, out int syllabeCount))
            {
                fields.Add(new Field { Uuid = folder.Uuid, FieldName = "Syllabe", Value = $"{syllabeCount}", Keyword = word });
            }

            // or parse
            if (!fields.Any())
            {
                syllabeCount = SyllableParser.Parse(lowerWord);
                if (syllabeCount != 0)
                {
                    fields.Add(new Field { Uuid = folder.Uuid, FieldName = "Syllabe", Value = $"{syllabeCount}", Keyword = word });
                }
            }

            if (WordRime.TryGetValue(lowerWord, out string rime))
            {
                fields.Add(new Field { Uuid = folder.Uuid, FieldName = "Rime", Value = rime, Keyword = word });
                if (WordSyllabe.TryGetValue(rime, out int rimeCount))
                {
                    fields.Add(new Field { Uuid = folder.Uuid, FieldName = "Rich", Value = $"{rimeCount}", Keyword = word });
                }
            }


            fields.Add(new Field { Uuid = folder.Uuid, FieldName = "Word", Value = word, Keyword = word });
            fields.Add(new Field { Uuid = folder.Uuid, FieldName = "TimeStamp", Value = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture), Keyword = word });
            return fields;
        };

        public static readonly Func<string, DataFolder, IEnumerable<Field>> WordFuncter = (s, folder) =>
        {
            List<Field> fields = new List<Field>
            {
            new Field { Uuid = folder.Uuid, FieldName = "Word", Value = s },
            new Field { Uuid = folder.Uuid, FieldName = "Syllabe", Value = "1" },
            new Field { Uuid = folder.Uuid, FieldName = "TimeStamp", Value = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff",CultureInfo.InvariantCulture) },
            };
            return fields;
        };
    }
}
