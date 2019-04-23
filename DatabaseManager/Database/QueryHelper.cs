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
        private IList<KeyValuePair<int, int>> m_Collaborations;
        private DateTime m_ArtistModified;
        private DateTime m_AlbumModified;
        private DateTime m_CollaboModified;

        public QueryHelper()
        {
            m_Artists = InitArtists();
            m_Albums = InitAlbums();
            m_Collaborations = InitCollaborations();
        }

        #region Initialization
        private IDictionary<int, Album> InitAlbums()
        {
            if(!File.Exists(DB_Constants.DB_Album_Path))
            {
                return null;
            }

            IDictionary<int, Album> result = new Dictionary<int, Album>();
            using (var reader = new StreamReader(DB_Constants.DB_Album_Path))
            {
                string line;
                Album album;
                while((line = reader.ReadLine()) != null)
                {
                    album = JsonConvert.DeserializeObject<Album>(line);
                    result.Add(album.Id, album);
                }
            }
            m_AlbumModified = File.GetLastWriteTime(DB_Constants.DB_Album_Path);
            return result;
        }

        private IDictionary<int, Artist> InitArtists()
        {
            if (!File.Exists(DB_Constants.DB_Artist_Path))
            {
                return null;
            }

            IDictionary<int, Artist> result = new Dictionary<int, Artist>();
            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                Artist artist;
                while ((line = reader.ReadLine()) != null)
                {
                    artist = JsonConvert.DeserializeObject<Artist>(line);
                    result.Add(artist.Id, artist);
                }
            }
            m_ArtistModified = File.GetLastWriteTime(DB_Constants.DB_Artist_Path);
            return result;
        }

        private IList<KeyValuePair<int, int>> InitCollaborations()
        {
            if (!File.Exists(DB_Constants.DB_Collaboration_Path))
            {
                return null;
            }

            IList<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>();
            using (var reader = new StreamReader(DB_Constants.DB_Collaboration_Path))
            {
                string line;
                Collaboration collaboration;
                while ((line = reader.ReadLine()) != null)
                {
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(line);
                    result.Add(new KeyValuePair<int, int>(collaboration.AlbumId, collaboration.ArtistId));
                }
            }
            m_CollaboModified = File.GetLastWriteTime(DB_Constants.DB_Collaboration_Path);
            return result;
        }

        private void CheckUpdateArtistResources()
        {
            if(File.GetLastWriteTime(DB_Constants.DB_Artist_Path) == m_ArtistModified)
            {
                return;
            }
            m_Artists = InitArtists();
        }

        private void CkeckUpdateAlbumResources()
        {
            if (File.GetLastWriteTime(DB_Constants.DB_Album_Path) == m_AlbumModified)
            {
                return;
            }
            m_Albums = InitAlbums();
        }

        private void CheckUpdateCollaboResources()
        {
            if (File.GetLastWriteTime(DB_Constants.DB_Collaboration_Path) == m_CollaboModified)
            {
                return;
            }
            m_Collaborations = InitCollaborations();
        }
        #endregion

        #region Locking
        private bool CheckAlbumsLocked()
        {
            return File.Exists(DB_Constants.DB_Album_Path_Locked);
        }
        private bool CheckArtistsLocked()
        {
            return File.Exists(DB_Constants.DB_Artist_Path_Locked);
        }
        private bool CheckCollaborationsLocked()
        {
            return File.Exists(DB_Constants.DB_Collaboration_Path_Locked);
        }

        public void LockAlbums()
        {
            if(CheckAlbumsLocked())
            {
                throw new Exception("Albums already locked");
            }

            File.Copy(DB_Constants.DB_Album_Path, DB_Constants.DB_Album_Path_Locked);
        }
        public void LockArtists()
        {
            if (CheckArtistsLocked())
            {
                throw new Exception("Artists already locked");
            }

            File.Copy(DB_Constants.DB_Artist_Path, DB_Constants.DB_Artist_Path_Locked);
        }
        public void LockCollaborations()
        {
            if (CheckCollaborationsLocked())
            {
                throw new Exception("Collaborations already locked");
            }

            File.Copy(DB_Constants.DB_Collaboration_Path, DB_Constants.DB_Collaboration_Path_Locked);
        }
        #endregion

        public IList<Artist> GetAllArtists()
        {
            CheckUpdateArtistResources();
            return m_Artists.Values.ToList();
        }

        public IList<Album> GetAllAlbums()
        {
            CkeckUpdateAlbumResources();
            return m_Albums.Values.ToList();
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
            Artist result;
            if (m_Artists.TryGetValue(p_ArtistId, out result))
            {
                return result;
            }
            return null;
        }

        public Album GetAlbumById(int p_AlbumId)
        {
            Album result;
            if (m_Albums.TryGetValue(p_AlbumId, out result))
            {
                return result;
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
