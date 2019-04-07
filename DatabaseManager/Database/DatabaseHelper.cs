using DatabaseManager.Model;
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
            var albumTO = TestDataReader.GetAlbums();

            var artists = EnumerateArtists(artistTO);
            FilterAlbums(albumTO, artists);
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

        private static void FilterAlbums(IList<AlbumTO> p_Albums, IList<Artist> p_Artists)
        {
            foreach(var album in p_Albums.ToList())
            {
                if(!AllArtistsExist(album, p_Artists))
                {
                    p_Albums.Remove(album);
                }
            }
        }

        private static bool AllArtistsExist(AlbumTO p_Album, IList<Artist> p_Artists)
        {
            foreach(var artistName in p_Album.Artists)
            {
                if(p_Artists.Where(x => x.Name.Equals(artistName)).Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
