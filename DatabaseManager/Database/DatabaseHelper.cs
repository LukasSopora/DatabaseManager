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

            var artists = EnumerateArtists(artistTOs.Values.ToList());
            FilterAlbums(albumTOs, artists);
            var albums = EnumerateAlbums(albumTOs);
            var collaborations = PrepareCollaborations(albumTOs, albums, artists);

            InitDatabaseStructure();

            ClearFile(DB_Constants.DB_Artist_Path);
            ClearFile(DB_Constants.DB_Album_Path);
            ClearFile(DB_Constants.DB_Collaboration_Path);

            CreateTable(artists.Values, DB_Constants.DB_Artist_Path);
            CreateTable(albums.Values, DB_Constants.DB_Album_Path);
            CreateTable(collaborations, DB_Constants.DB_Collaboration_Path);
        }

        private static void InitDatabaseStructure()
        {
            InitDBDirectories();
            InitDataBaseFiles();
            ClearDBDirectory();
        }

        private static void ClearDBDirectory()
        {
            foreach (var file in Directory.GetFiles(DB_Constants.DB_DataBase_Directory, "*", SearchOption.AllDirectories))
            {
                if (!file.Equals(GetWinFileName(DB_Constants.DB_Artist_Path)) &&
                    !file.Equals(GetWinFileName(DB_Constants.DB_Album_Path)) &&
                    !file.Equals(GetWinFileName(DB_Constants.DB_Collaboration_Path)))
                {
                    File.Delete(file);
                }
            }
        }

        private static void InitDataBaseFiles()
        {
            //Artist
            if (!File.Exists(DB_Constants.DB_Artist_Path))
            {
                File.Create(DB_Constants.DB_Artist_Path);
            }
            //Album
            if (!File.Exists(DB_Constants.DB_Album_Path))
            {
                File.Create(DB_Constants.DB_Album_Path);
            }
            //Collaboration
            if (!File.Exists(DB_Constants.DB_Collaboration_Path))
            {
                File.Create(DB_Constants.DB_Collaboration_Path);
            }
        }

        private static void InitDBDirectories()
        {
            //Database
            if (!Directory.Exists(DB_Constants.DB_DataBase_Directory))
            {
                Directory.CreateDirectory(DB_Constants.DB_DataBase_Directory);
            }
            //Artist
            if (!Directory.Exists(DB_Constants.DB_Artist_Directory))
            {
                Directory.CreateDirectory(DB_Constants.DB_Artist_Directory);
            }
            //Album
            if (!Directory.Exists(DB_Constants.DB_Album_Directory))
            {
                Directory.CreateDirectory(DB_Constants.DB_Album_Directory);
            }
            //Collaboration
            if (!Directory.Exists(DB_Constants.DB_Collaboration_Directory))
            {
                Directory.CreateDirectory(DB_Constants.DB_Collaboration_Directory);
            }
        }

        private static void ClearFile(string p_Path)
        {
            if(!File.Exists(p_Path))
            {
                return;
            }
            File.WriteAllText(p_Path, string.Empty);
        }

        private static void CreateTable(IEnumerable<object> p_Objects, string p_Path)
        {
            using (var writer = new StreamWriter(p_Path, append: true))
            {
                foreach (var obj in p_Objects)
                {
                    var json = JsonConvert.SerializeObject(obj);
                    writer.WriteLine(json);
                }
            }
        }

        private static IDictionary<string, Artist> EnumerateArtists(IList<ArtistTO> p_Artists)
        {
            IDictionary<string, Artist> result = new Dictionary<string, Artist>();
            for (int index = 0; index < p_Artists.Count; index++)
            {
                var artist = new Artist();
                artist.Id = index + 1;
                artist.Name = p_Artists[index].Name;
                artist.Year = p_Artists[index].Year;
                artist.Country = p_Artists[index].Country;

                result.Add(artist.Name, artist);
            }
            return result;
        }

        private static void FilterAlbums(IDictionary<int, AlbumTO> p_Albums, IDictionary<string, Artist> p_Artists)
        {
            foreach(var album in p_Albums)
            {
                if(!AllArtistsExist(album.Value, p_Artists))
                {
                    p_Albums.Remove(album);
                }
            }
        }

        private static IList<Collaboration> PrepareCollaborations(IDictionary<int, AlbumTO> p_AlbumTOs, IDictionary<int, Album> p_Albums, IDictionary<string, Artist> p_Artists)
        {
            IList<Collaboration> result = new List<Collaboration>();

            foreach (var pair in p_Albums)
            {
                string[] artistNames = p_AlbumTOs[pair.Key].Artists.ToArray();
                foreach (var artistName in artistNames)
                {
                    var collaboration = new Collaboration();
                    collaboration.AlbumId = pair.Value.Id;
                    collaboration.ArtistId = p_Artists[artistName].Id;

                    result.Add(collaboration);
                }
            }

            return result;
        }

        private static IDictionary<int, Album> EnumerateAlbums(IDictionary<int, AlbumTO> p_Albums)
        {
            IDictionary<int, Album> result = new Dictionary<int, Album>();
            foreach(var pair in p_Albums)
            {
                var album = new Album();
                album.Id = pair.Key;
                album.Name = pair.Value.Name;
                album.Year = pair.Value.Year;

                result.Add(pair.Key, album);
            }
            return result;
        }

        private static bool AllArtistsExist(AlbumTO p_Album, IDictionary<string, Artist> p_Artists)
        {
            foreach(var artistName in p_Album.Artists)
            {
                if (!p_Artists.ContainsKey(artistName))
                {
                    return false;
                }
            }
            return true;
        }

        private static string GetWinFileName(string p_Filename)
        {
            return p_Filename.Replace("//", @"\");
        }
    }
}
