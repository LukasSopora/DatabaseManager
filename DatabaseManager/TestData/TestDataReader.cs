using CsvHelper;
using DatabaseManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DatabaseManager.TestData
{
    public class TestDataReader
    {
        private const int m_CommasInLine = 3;
        private const string m_Album_Path = "TestData/album.csv";
        private const string m_Artist_Path = "TestData/artist.csv";

        public static IList<ArtistTO> GetArtists()
        {
            var result = new List<ArtistTO>();
            var lines = File.ReadAllLines(m_Artist_Path).Where(x => !x.First().Equals('#')).ToList();

            foreach (var line in lines)
            {
                var commas = GetCommasInLine(line, m_CommasInLine);
                if(commas.Count < 2)
                {
                    Console.WriteLine(string.Format("Incorrect line: Not enough commas in line {0}", lines.IndexOf(line)));
                    continue;
                }
                var indexPairs = commas.Count == 2 ? GetPropStartEndIndex(commas, line.Length) : GetPropStartEndIndex(commas);
                var subStrings = GetCSVSubStrings(indexPairs, line);

                var artist = new ArtistTO();
                artist.Name = subStrings[0];
                artist.Year = Convert.ToInt32(subStrings[1]);
                artist.Country = subStrings[2];

                result.Add(artist);
            }

            return result;
        }

        public static IList<AlbumTO> GetAlbums()
        {
            var result = new List<AlbumTO>();
            var lines = File.ReadAllLines(m_Album_Path).Where(x => !x.First().Equals('#')).ToList();

            foreach(string line in lines)
            {
                var commas = GetCommasInLine(line, m_CommasInLine);
                if (commas.Count < 2)
                {
                    Console.WriteLine(string.Format("Incorrect line: Not enough commas in line {0}", lines.IndexOf(line)));
                    continue;
                }
                var indexPairs = commas.Count == 2 ? GetPropStartEndIndex(commas, line.Length) : GetPropStartEndIndex(commas);
                var subStrings = GetCSVSubStrings(indexPairs, line);

                var album = new AlbumTO();
                album.Name = subStrings[0];
                album.Year = Convert.ToInt32(subStrings[2]);

                //Artists
                var artistLine = subStrings[1];
                commas = GetCommasInLine(artistLine);
                indexPairs = GetPropStartEndIndex(commas, artistLine.Length);
                subStrings = GetCSVSubStrings(indexPairs, artistLine);

                foreach(string artist in subStrings)
                {
                    album.Artists.Add(artist);
                }

                result.Add(album);
            }

            return result;
        }

        private static IList<int> GetCommasInLine(string p_Line, int p_ExpectedSize = 0)
        {
            IList<int> segmentIndex = new List<int>();
            bool foundQuote = false;


            for (int index = 0; index < p_Line.Length; index++)
            {
                if (foundQuote)
                {
                    if (p_Line[index] == '"')
                    {
                        foundQuote = false;
                    }
                }
                else
                {
                    if (p_Line[index] == ',')
                    {
                        segmentIndex.Add(index);
                        if(p_ExpectedSize != 0 && segmentIndex.Count == p_ExpectedSize)
                        {
                            return segmentIndex;
                        }
                    }
                    else if(p_Line[index] == '"')
                    {
                        foundQuote = true;
                    }
                }
            }
            return segmentIndex;
        }

        private static IList<Tuple<int, int>> GetPropStartEndIndex(IList<int> p_commaIndex, int p_LineLength = 0)
        {
            IList<Tuple<int, int>> indexPairs = new List<Tuple<int, int>>();
            int lastIndex = 0;

            for (int i = 0; i < p_commaIndex.Count; i++)
            {
                var indexPair = new Tuple<int, int>(lastIndex, p_commaIndex[i] - lastIndex);
                lastIndex = p_commaIndex[i] + 1;
                indexPairs.Add(indexPair);
            }
            if(p_LineLength != 0)
            {
                var indexPair = new Tuple<int, int>(lastIndex, p_LineLength - lastIndex);
                indexPairs.Add(indexPair);
            }

            return indexPairs;
        }

        private static IList<string> GetCSVSubStrings(IList<Tuple<int,int>> p_IndexPairs, string p_Line)
        {
            IList<string> result = new List<string>();

            foreach(Tuple<int,int> pair in p_IndexPairs)
            {
                string line = p_Line.Substring(pair.Item1, pair.Item2);
                line = line.Trim();
                line = line.Trim('"');
                result.Add(line);
            }

            return result;
        }
    }
}
