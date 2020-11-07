using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.IO;

namespace Actor.DbService.Core.Model
{

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
