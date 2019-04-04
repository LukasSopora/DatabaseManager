using DatabaseManager.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Database
{
    public static class DatabaseHelper
    {
        public static void InitDataBase()
        {
            var artistTO = TestDataReader.GetArtists();
            var albumIO = TestDataReader.GetAlbums();
        }

        private static void EnumarateArtist()
        {

        }
    }
}
