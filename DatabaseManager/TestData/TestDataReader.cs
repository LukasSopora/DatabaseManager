using DatabaseManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.TestData
{
    public class TestDataReader
    {
        private const string m_ALBUM_PATH = "TestData/album.csv";
        private const string m_ARTIST_PATH = "TestData/artist.csv";

        public static ObservableCollection<Artist> GetArtists()
        {
            var result = new ObservableCollection<Artist>();
            
            return result;
        }
    }
}
