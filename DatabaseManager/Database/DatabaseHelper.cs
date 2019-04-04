﻿using DatabaseManager.Model;
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

            var artists = EnumerateArtists(artistTO);
        }

        private static IList<Artist> EnumerateArtists(IList<ArtistTO> p_Artists)
        {
            IList<Artist> result = new List<Artist>();
            for (int index = 0; index < p_Artists.Count; index++)
            {
                var artist = new Artist();
                artist.Id = index + 1;
                artist.Name = p_Artists[index].Name;
                artist.Year = p_Artists[index].Year;
                artist.Country = p_Artists[index].Country;

                result.Add(artist);
            }
            return result;
        }
    }
}
