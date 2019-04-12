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
    public class QueryHelper
    {
        private IDictionary<int, Album> m_Albums;
        private IDictionary<int, Artist> m_Artists;
        private IDictionary<int, int> m_Collaborations;

        public QueryHelper()
        {
            m_Artists = InitArtists();
            m_Albums = InitAlbums();
            m_Collaborations = InitCollaborations();
        }

        private IDictionary<int, Album> InitAlbums()
        {
            if(!File.Exists(DB_Constants.DB_Album_Directory))
            {
                return null;
            }

            IDictionary<int, Album> result = new Dictionary<int, Album>();
            var reader = new StreamReader(DB_Constants.DB_Album_Directory);
            string line;
            while((line = reader.ReadLine()) != null)
            {
                var album = JsonConvert.DeserializeObject<Album>(line);
                result.Add(album.Id, album);
            }
            return result;
        }

        private IDictionary<int, Artist> InitArtists()
        {
            if (!File.Exists(DB_Constants.DB_Artist_Directory))
            {
                return null;
            }

            IDictionary<int, Artist> result = new Dictionary<int, Artist>();
            var reader = new StreamReader(DB_Constants.DB_Artist_Directory);
            string line;
            while((line = reader.ReadLine()) != null)
            {
                var artist = JsonConvert.DeserializeObject<Artist>(line);
                result.Add(artist.Id, artist);
            }
            return result;
        }

        private IDictionary<int, int> InitCollaborations()
        {
            if (!File.Exists(DB_Constants.DB_Collaboration_Directory))
            {
                return null;
            }

            IDictionary<int, int> result = new Dictionary<int, int>();
            var reader = new StreamReader(DB_Constants.DB_Collaboration_Directory);
            string line;
            while((line = reader.ReadLine()) != null)
            {
                var collabo = JsonConvert.DeserializeObject<Collaboration>(line);
                result.Add(collabo.ArtistId, collabo.AlbumId);
            }
            return result;
        }

        public IList<Artist> GetAllArtists()
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

        public IList<Album> GetAllAlbums()
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


        public IList<Collaboration> GetAllCollaborations()
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

        public Artist GetArtistById(int p_ArtistId)
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

        public Album GetAlbumById(int p_AlbumId)
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

        public IList<Collaboration> GetCollaborationByArtistId(int p_ArtistId)
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

        public IList<Collaboration> GetCollaborationByAlbumId(int p_AlbumId)
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
        public IList<Album> GetAllAlbumsByArtistId(int p_ArtistId)
        {
            IList<Album> result = new List<Album>();
            var collaborations = GetCollaborationByArtistId(p_ArtistId);
            foreach(var collab in collaborations)
            {
                result.Add(GetAlbumById(collab.AlbumId));
            }
            return result;
        }

        public int GetLatestAlbumRelease()
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

        public int GetArtistFoundingYear(int p_ArtistId)
        {
            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    var artist = JsonConvert.DeserializeObject<Artist>(line);
                    if(artist.Id == p_ArtistId)
                    {
                        return artist.Year;
                    }
                }
            }
            return -1;
        }

        public IList<Artist> GetArtistsNoAlbums()
        {
            IList<Artist> result = new List<Artist>();
            var collaborations = GetAllCollaborations();
            var artists = GetAllArtists();

            foreach(var artist in artists)
            {
                if(GetAllAlbumsByArtistId(artist.Id).Count == 0)
                {
                    result.Add(artist);
                }
            }
            return result;
        }
        #endregion
    }
}
