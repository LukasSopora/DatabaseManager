using DatabaseManager.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseManager.Database
{
    public class QueryHelper
    {
        private IDictionary<int, Album> m_Albums;

        private IDictionary<int, Artist> m_ArtistsbyId;
        private IDictionary<string, Artist> m_ArtistsByName;

        private IDictionary<int, List<int>> m_ArtistCollaborations;
        private IDictionary<int, List<int>> m_AlbumCollaborations;

        private DateTime m_ArtistModified;
        private DateTime m_AlbumModified;
        private DateTime m_CollaboModified;

        public QueryHelper()
        {
            InitArtists();
            InitAlbums();
            InitCollaborations();
        }

        #region Initialization
        private void InitCollaborations()
        {
            if (!File.Exists(DB_Constants.DB_Collaboration_Path))
            {
                return;
            }

            m_ArtistCollaborations = new Dictionary<int, List<int>>();
            m_AlbumCollaborations = new Dictionary<int, List<int>>();

            using (var reader = new StreamReader(DB_Constants.DB_Collaboration_Path))
            {
                string line;
                Collaboration collaboration;
                while ((line = reader.ReadLine()) != null)
                {
                    collaboration = JsonConvert.DeserializeObject<Collaboration>(line);

                    //Add artist collaboration
                    if(!m_ArtistCollaborations.ContainsKey(collaboration.ArtistId))
                    {
                        m_ArtistCollaborations.Add(collaboration.ArtistId, new List<int> { collaboration.AlbumId });
                    }
                    else
                    {
                        m_ArtistCollaborations[collaboration.ArtistId].Add(collaboration.AlbumId);
                    }

                    //Add album collaboration
                    if(!m_AlbumCollaborations.ContainsKey(collaboration.AlbumId))
                    {
                        m_AlbumCollaborations.Add(collaboration.AlbumId, new List<int> { collaboration.ArtistId });
                    }
                    else
                    {
                        m_AlbumCollaborations[collaboration.AlbumId].Add(collaboration.ArtistId);
                    }
                }
            }
            m_CollaboModified = File.GetLastWriteTime(DB_Constants.DB_Collaboration_Path);
        }

        private void InitAlbums()
        {
            if(!File.Exists(DB_Constants.DB_Album_Path))
            {
                return;
            }

            m_Albums = new Dictionary<int, Album>();
            using (var reader = new StreamReader(DB_Constants.DB_Album_Path))
            {
                string line;
                Album album;
                while((line = reader.ReadLine()) != null)
                {
                    album = JsonConvert.DeserializeObject<Album>(line);
                    m_Albums.Add(album.Id, album);
                }
            }
            m_AlbumModified = File.GetLastWriteTime(DB_Constants.DB_Album_Path);
        }

        private void InitArtists()
        {
            if (!File.Exists(DB_Constants.DB_Artist_Path))
            {
                return;
            }

            m_ArtistsbyId = new Dictionary<int, Artist>();
            m_ArtistsByName = new Dictionary<string, Artist>();

            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                Artist artist;
                while ((line = reader.ReadLine()) != null)
                {
                    artist = JsonConvert.DeserializeObject<Artist>(line);
                    m_ArtistsbyId.Add(artist.Id, artist);
                    m_ArtistsByName.Add(artist.Name, artist);
                }
            }
            m_ArtistModified = File.GetLastWriteTime(DB_Constants.DB_Artist_Path);
        }
        #endregion

        #region Check and Update Values
        private void CheckUpdateArtistResources()
        {
            if(File.GetLastWriteTime(DB_Constants.DB_Artist_Path) == m_ArtistModified)
            {
                return;
            }
            InitArtists();
        }

        private void CkeckUpdateAlbumResources()
        {
            if (File.GetLastWriteTime(DB_Constants.DB_Album_Path) == m_AlbumModified)
            {
                return;
            }
            InitAlbums();
        }

        private void CheckUpdateCollaboResources()
        {
            if (File.GetLastWriteTime(DB_Constants.DB_Collaboration_Path) == m_CollaboModified)
            {
                return;
            }
            InitCollaborations();
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

        private void LockAlbums()
        {
            if(CheckAlbumsLocked())
            {
                throw new Exception("Albums already locked");
            }

            File.Copy(DB_Constants.DB_Album_Path, DB_Constants.DB_Album_Path_Locked);
        }
        private void LockArtists()
        {
            if (CheckArtistsLocked())
            {
                throw new Exception("Artists already locked");
            }

            File.Copy(DB_Constants.DB_Artist_Path, DB_Constants.DB_Artist_Path_Locked);
        }
        private void LockCollaborations()
        {
            if (CheckCollaborationsLocked())
            {
                throw new Exception("Collaborations already locked");
            }

            File.Copy(DB_Constants.DB_Collaboration_Path, DB_Constants.DB_Collaboration_Path_Locked);
        }

        private void UnlockAlbums()
        {
            if(!CheckAlbumsLocked())
            {
                throw new Exception("Albums already unlocked");
            }

            File.Copy(DB_Constants.DB_Album_Path_Locked, DB_Constants.DB_Album_Path, true);
            File.Delete(DB_Constants.DB_Album_Path_Locked);
        }
        private void UnlockArtists()
        {
            if (!CheckArtistsLocked())
            {
                throw new Exception("Artists already unlocked");
            }

            File.Copy(DB_Constants.DB_Artist_Path_Locked, DB_Constants.DB_Artist_Path, true);
            File.Delete(DB_Constants.DB_Artist_Path_Locked);
        }
        private void UnlockCollaborations()
        {
            if (!CheckCollaborationsLocked())
            {
                throw new Exception("Collaborations already unlocked");
            }

            File.Copy(DB_Constants.DB_Collaboration_Path_Locked, DB_Constants.DB_Collaboration_Path, true);
            File.Delete(DB_Constants.DB_Collaboration_Path_Locked);
        }
        #endregion

        #region CRUD Artist
        public bool CreateArtist(Artist p_Artist)
        {
            CheckUpdateArtistResources();

            if(CheckArtistsLocked())
            {
                int counter = 0;
                while(CheckArtistsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if(counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockArtists();
            if(p_Artist.Id == 0) //Id not initialized
            {
                p_Artist.Id = m_ArtistsbyId.Keys.Max();
            }
            else //Id initialized --> check if key already exists
            {
                if(m_ArtistsbyId.ContainsKey(p_Artist.Id))
                {
                    UnlockArtists();
                    return false;
                }
            }

            m_ArtistsbyId.Add(p_Artist.Id, p_Artist);
            m_ArtistsByName.Add(p_Artist.Name, p_Artist);

            UnlockArtists();

            return true;
        }

        public IList<Artist> ReadAllArtist()
        {
            CheckUpdateArtistResources();
            return m_ArtistsbyId.Values.ToList();
        }

        public Artist ReadArtistById(int p_ArtistId)
        {
            CheckUpdateArtistResources();
            Artist result;
            if (m_ArtistsbyId.TryGetValue(p_ArtistId, out result))
            {
                return result;
            }
            return null;
        }

        public Artist ReadArtistByName(string p_ArtistName)
        {
            CheckUpdateArtistResources();
            Artist result;
            if(m_ArtistsByName.TryGetValue(p_ArtistName, out result))
            {
                return result;
            }
            return null;
        }

        public IList<Artist> ReadArtistByCountry(string p_Country)
        {
            CheckUpdateArtistResources();
            return m_ArtistsbyId.Values.Where(x => x.Country.Equals(p_Country)).ToList();
        }

        public IList<Artist> ReadArtistByYear(int p_Year)
        {
            CheckUpdateArtistResources();
            return m_ArtistsbyId.Values.Where(x => x.Year.Equals(p_Year)).ToList();
        }
        #endregion

        #region Album
        public IList<Album> ReadAllAlbums()
        {
            CkeckUpdateAlbumResources();
            return m_Albums.Values.ToList();
        }

        public Album ReadAlbumById(int p_AlbumId)
        {
            CkeckUpdateAlbumResources();
            Album result;
            if (m_Albums.TryGetValue(p_AlbumId, out result))
            {
                return result;
            }
            return null;
        }

        public IList<Album> ReadAlbumByYear(int p_Year)
        {
            CkeckUpdateAlbumResources();
            return m_Albums.Values.Where(x => x.Year.Equals(p_Year)).ToList();
        }

        public IList<Album> ReadAlbumsByName(string p_Name)
        {
            CkeckUpdateAlbumResources();
            return m_Albums.Values.Where(x => x.Name.Equals(p_Name)).ToList();
        }
        #endregion

        #region Collaboration
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
        #endregion


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
                result.Add(ReadAlbumById(collab.AlbumId));
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
            var artists = ReadAllArtist();

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
