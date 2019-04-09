using DatabaseManager.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Database
{
    public static class QueryHelper
    {
        public static IList<Artist> GetAllArtists()
        {
            if(!File.Exists(DB_Constants.DB_Artist_Path))
            {
                return null;
            }

            IList<Artist> result = new List<Artist>();
            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(JsonConvert.DeserializeObject<Artist>(line));
                }
            }
            return result;
        }

        public static IList<Album> GetAllAlbums()
        {
            if (!File.Exists(DB_Constants.DB_Album_Path))
            {
                return null;
            }

            IList<Album> result = new List<Album>();
            using (var reader = new StreamReader(DB_Constants.DB_Album_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(JsonConvert.DeserializeObject<Album>(line));
                }
            }
            return result;
        }


        public static IList<Collaboration> GetAllCollaborations()
        {
            if (!File.Exists(DB_Constants.DB_Collaboration_Path))
            {
                return null;
            }

            IList<Collaboration> result = new List<Collaboration>();
            using (var reader = new StreamReader(DB_Constants.DB_Collaboration_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(JsonConvert.DeserializeObject<Collaboration>(line));
                }
            }
            return result;
        }

        public static Artist GetArtistById(int p_ArtistId)
        {
            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var artist = JsonConvert.DeserializeObject<Artist>(line);
                    if(artist.Id == p_ArtistId)
                    {
                        return artist;
                    }
                }
            }
            return null;
        }

        public static Album GetAlbumById(int p_AlbumId)
        {
            using (var reader = new StreamReader(DB_Constants.DB_Album_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var album = JsonConvert.DeserializeObject<Album>(line);
                    if (album.Id == p_AlbumId)
                    {
                        return album;
                    }
                }
            }
            return null;
        }

        public static IList<Collaboration> GetCollaborationByArtistId(int p_ArtistId)
        {
            IList<Collaboration> result = new List<Collaboration>();
            using (var reader = new StreamReader(DB_Constants.DB_Collaboration_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var collaboration = JsonConvert.DeserializeObject<Collaboration>(line);
                    if (collaboration.ArtistId == p_ArtistId)
                    {
                        result.Add(collaboration);
                    }
                }
            }
            return result;
        }

        public static IList<Collaboration> GetCollaborationByAlbumId(int p_AlbumId)
        {
            IList<Collaboration> result = new List<Collaboration>();
            using (var reader = new StreamReader(DB_Constants.DB_Collaboration_Path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var collaboration = JsonConvert.DeserializeObject<Collaboration>(line);
                    if (collaboration.AlbumId == p_AlbumId)
                    {
                        result.Add(collaboration);
                    }
                }
            }
            return result;
        }

        #region Queries
        public static IList<Album> GetAllAlbumsByArtistId(int p_ArtistId)
        {
            IList<Album> result = new List<Album>();
            var collaborations = GetCollaborationByArtistId(p_ArtistId);
            foreach(var collab in collaborations)
            {
                result.Add(GetAlbumById(collab.AlbumId));
            }
            return result;
        }

        public static int GetLatestAlbumRelease()
        {
            int latest = 0;
            using (var reader = new StreamReader(DB_Constants.DB_Album_Path))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var album = JsonConvert.DeserializeObject<Album>(line);
                    if(album.Year > latest)
                    {
                        latest = album.Year;
                    }
                }
            }
            return latest;
        }
        #endregion
    }
}
