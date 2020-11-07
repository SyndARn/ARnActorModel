using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
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
                wordExceptions["prudent"] = 2;
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
}
