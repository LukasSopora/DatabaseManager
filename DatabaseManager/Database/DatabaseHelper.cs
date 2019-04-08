using DatabaseManager.Model;
using DatabaseManager.TestData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Database
{
    public static class DatabaseHelper
    {
        public static void InitDataBase()
        {
            var artistTOs = TestDataReader.GetArtists();
            var albumTOs = TestDataReader.GetAlbums();

            var artists = EnumerateArtists(artistTOs);
            FilterAlbums(albumTOs, artists);
            var albums = EnumerateAlbums(albumTOs);
            var collaborations = PrepareCollaborations(albumTOs, albums, artists);

            CreateTable(artists, DB_Constants.DB_Artist_Path);
            CreateTable(albums, DB_Constants.DB_Album_Path);
            CreateTable(collaborations, DB_Constants.DB_Collaboration_Path);
        }

        private static void CreateTable(object p_Objects, string p_Path)
        {
            if(!File.Exists(p_Path))
            {
                File.Create(p_Path);
            }
            var json = JsonConvert.SerializeObject(p_Objects);
            File.WriteAllText(p_Path, json);
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

        private static IList<Collaboration> PrepareCollaborations(IList<AlbumTO> p_AlbumTOs, IList<Album> p_Albums, IList<Artist> p_Artists)
        {
            IList<Collaboration> result = new List<Collaboration>();

            foreach(var album in p_Albums)
            {
                foreach(var artistName in p_AlbumTOs.Where(x => x.Name.Equals(album.Name)).First().Artists)
                {
                    var collaboration = new Collaboration();
                    collaboration.AlbumId = album.Id;
                    collaboration.ArtistId = p_Artists.Where(x => x.Name.Equals(artistName)).First().Id;

                    result.Add(collaboration);
                }
            }

            return result;
        }

        private static IList<Album> EnumerateAlbums(IList<AlbumTO> p_Albums)
        {
            IList<Album> result = new List<Album>();
            for (int index = 0; index < p_Albums.Count; index++)
            {
                var album = new Album();
                album.Id = index + 1;
                album.Name = p_Albums[index].Name;
                album.Year = p_Albums[index].Year;

                result.Add(album);
            }
            return result;
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
