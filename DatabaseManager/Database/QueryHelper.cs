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
        private const string m_Album_Path = "TestData/albums.csv";
        private const string m_Artist_Path = "TestData/artists.csv";

        private IDictionary<int, Album> m_Albums;

        private IDictionary<int, Artist> m_ArtistsById;
        private IDictionary<string, Artist> m_ArtistsByName;

        private IDictionary<int, List<int>> m_ArtistCollaborations;
        private IDictionary<int, List<int>> m_AlbumCollaborations;

        private DateTime m_ArtistModified;
        private DateTime m_AlbumModified;
        private DateTime m_CollaboModified;

        private int[] m_AlbumCountIndex = new int[21];

        public QueryHelper()
        {
            InitArtists();
            InitAlbums();
            InitCollaborations();

            InitializeIndex();
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

            m_ArtistsById = new Dictionary<int, Artist>();
            m_ArtistsByName = new Dictionary<string, Artist>();

            using (var reader = new StreamReader(DB_Constants.DB_Artist_Path))
            {
                string line;
                Artist artist;
                while ((line = reader.ReadLine()) != null)
                {
                    artist = JsonConvert.DeserializeObject<Artist>(line);
                    m_ArtistsById.Add(artist.Id, artist);
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

        private void CheckUpdateAlbumResources()
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

        internal void ExportCSV()
        {
            File.WriteAllText(m_Artist_Path, string.Empty);
            File.WriteAllText(m_Album_Path, string.Empty);

            var writer = new StreamWriter(m_Artist_Path);
            string line;
            writer.WriteLine("# name, year, country");
            foreach(var artist in m_ArtistsById.Values)
            {
                line = "\"" + artist.Name + "\", " + artist.Year + ", " + artist.Country;
                writer.WriteLine(line);
            }

            writer = new StreamWriter(m_Album_Path);
            string artistName;
            writer.WriteLine("# name, artist, year,");
            foreach(var album in m_Albums.Values)
            {
                line = "\"" + album.Name + "\", \"";
                foreach(var artistId in m_AlbumCollaborations[album.Id])
                {
                    artistName = m_ArtistsById[artistId].Name;
                    line += artistName + ", ";
                }
                line.Remove(line.Length - 2);
                line.Remove(line.Length - 1);

                line += "\", " + album.Year + ",";
                writer.WriteLine();
            }
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
                p_Artist.Id = m_ArtistsById.Keys.Max() + 1;
            }
            else //Id initialized --> check if key already exists
            {
                if(m_ArtistsById.ContainsKey(p_Artist.Id))
                {
                    UnlockArtists();
                    return false;
                }
            }

            m_ArtistsById.Add(p_Artist.Id, p_Artist);
            m_ArtistsByName.Add(p_Artist.Name, p_Artist);

            UnlockArtists();

            return true;
        }

        public IList<Artist> ReadAllArtists()
        {
            CheckUpdateArtistResources();
            return m_ArtistsById.Values.ToList();
        }

        public Artist ReadArtistById(int p_ArtistId)
        {
            CheckUpdateArtistResources();
            Artist result;
            if (m_ArtistsById.TryGetValue(p_ArtistId, out result))
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
            return m_ArtistsById.Values.Where(x => x.Country.Equals(p_Country)).ToList();
        }

        public IList<Artist> ReadArtistByYear(int p_Year)
        {
            CheckUpdateArtistResources();
            return m_ArtistsById.Values.Where(x => x.Year.Equals(p_Year)).ToList();
        }

        public bool UpdateArtist(int p_ArtistId, Artist p_Artist)
        {
            CheckUpdateArtistResources();

            if (!m_ArtistsById.ContainsKey(p_ArtistId) || p_Artist.Id != p_ArtistId)
            {
                return false;
            }

            if (CheckArtistsLocked())
            {
                int counter = 0;
                while (CheckArtistsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockArtists();
            string name = m_ArtistsById[p_ArtistId].Name;
            m_ArtistsById[p_ArtistId] = p_Artist;
            m_ArtistsByName[name] = p_Artist;
            UnlockArtists();

            return true;
        }

        public bool UpdateArtist(string p_ArtistName, Artist p_Artist)
        {
            CheckUpdateArtistResources();

            if (!m_ArtistsByName.ContainsKey(p_ArtistName) || p_Artist.Name != p_ArtistName)
            {
                return false;
            }

            if (CheckArtistsLocked())
            {
                int counter = 0;
                while (CheckArtistsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockArtists();
            int id = m_ArtistsByName[p_ArtistName].Id;
            m_ArtistsByName[p_ArtistName] = p_Artist;
            m_ArtistsById[id] = p_Artist;
            UnlockArtists();

            return true;
        }

        public bool DeleteArtist(int p_ArtistId)
        {
            CheckUpdateArtistResources();
            CheckUpdateCollaboResources();

            if (CheckArtistsLocked())
            {
                int counter = 0;
                while (CheckArtistsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            if (CheckCollaborationsLocked())
            {
                int counter = 0;
                while (CheckCollaborationsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockArtists();
            LockCollaborations();
            string name = m_ArtistsById[p_ArtistId].Name;
            m_ArtistsById.Remove(p_ArtistId);
            m_ArtistsByName.Remove(name);
            IList<int> albumIds = m_ArtistCollaborations[p_ArtistId];
            foreach(var id in albumIds)
            {
                m_AlbumCollaborations[p_ArtistId].Remove(p_ArtistId);
            }
            m_ArtistCollaborations.Remove(p_ArtistId);
            UnlockCollaborations();
            UnlockArtists();
            return true;
        }

        public bool DeleteArtist(string p_ArtistName)
        {
            CheckUpdateArtistResources();
            CheckUpdateCollaboResources();

            if (CheckArtistsLocked())
            {
                int counter = 0;
                while (CheckArtistsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            if (CheckCollaborationsLocked())
            {
                int counter = 0;
                while (CheckCollaborationsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockArtists();
            LockCollaborations();
            int artistId = m_ArtistsByName[p_ArtistName].Id;
            m_ArtistsByName.Remove(p_ArtistName);
            m_ArtistsById.Remove(artistId);
            IList<int> albumIds = m_ArtistCollaborations[artistId];
            foreach (var id in albumIds)
            {
                m_AlbumCollaborations[id].Remove(artistId);
            }
            m_ArtistCollaborations.Remove(artistId);
            UnlockCollaborations();
            UnlockArtists();
            return true;
        }
        #endregion

        #region CRUD Album
        public bool CreateAlbum(Album p_Album)
        {
            CheckUpdateAlbumResources();

            if (CheckAlbumsLocked())
            {
                int counter = 0;
                while (CheckAlbumsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockAlbums();
            if (p_Album.Id == 0) //Id not initialized
            {
                p_Album.Id = m_Albums.Keys.Max() + 1;
            }
            else //Id initialized --> check if key already exists
            {
                if (m_Albums.ContainsKey(p_Album.Id))
                {
                    UnlockAlbums();
                    return false;
                }
            }

            m_Albums.Add(p_Album.Id, p_Album);

            if (p_Album.Year >= 1990 && p_Album.Year <= 2010)
            {
                m_AlbumCountIndex[p_Album.Year - 1990]++;
            }

            UnlockAlbums();

            return true;
        }

        public IList<Album> ReadAllAlbums()
        {
            CheckUpdateAlbumResources();
            return m_Albums.Values.ToList();
        }

        public Album ReadAlbumById(int p_AlbumId)
        {
            CheckUpdateAlbumResources();
            Album result;
            if (m_Albums.TryGetValue(p_AlbumId, out result))
            {
                return result;
            }
            return null;
        }

        public IList<Album> ReadAlbumByYear(int p_Year)
        {
            CheckUpdateAlbumResources();
            return m_Albums.Values.Where(x => x.Year.Equals(p_Year)).ToList();
        }

        public IList<Album> ReadAlbumsByName(string p_Name)
        {
            CheckUpdateAlbumResources();
            return m_Albums.Values.Where(x => x.Name.Equals(p_Name)).ToList();
        }

        public bool UpdateAlbum(int p_AlbumId, Album p_Album)
        {
            CheckUpdateAlbumResources();

            if (!m_Albums.ContainsKey(p_AlbumId) || p_Album.Id != p_AlbumId)
            {
                return false;
            }

            if (CheckAlbumsLocked())
            {
                int counter = 0;
                while (CheckAlbumsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockAlbums();

            //Decrease Index of old album
            var oldAlbum = m_Albums[p_AlbumId];
            if(oldAlbum.Year >= 1990 && oldAlbum.Year <= 2010)
            {
                m_AlbumCountIndex[oldAlbum.Year - 1990]--;
            }

            m_Albums[p_AlbumId] = p_Album;

            //Update Index
            if (p_Album.Year >= 1990 && p_Album.Year <= 2010)
            {
                m_AlbumCountIndex[p_Album.Year - 1990]++;
            }

            UnlockAlbums();

            return true;
        }

        public bool DeleteAlbum(int p_AlbumId)
        {
            CheckUpdateAlbumResources();
            CheckUpdateCollaboResources();

            if (CheckAlbumsLocked())
            {
                int counter = 0;
                while (CheckAlbumsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            if (CheckCollaborationsLocked())
            {
                int counter = 0;
                while (CheckCollaborationsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockAlbums();
            LockCollaborations();
            m_Albums.Remove(p_AlbumId);
            IList<int> artistIds = m_AlbumCollaborations[p_AlbumId];
            foreach (var id in artistIds)
            {
                m_ArtistCollaborations[id].Remove(p_AlbumId);
            }
            m_AlbumCollaborations.Remove(p_AlbumId);

            //Decrease Index of old album
            var oldAlbum = m_Albums[p_AlbumId];
            if(oldAlbum.Year >= 1990 && oldAlbum.Year <= 2010)
            {
                m_AlbumCountIndex[oldAlbum.Year - 1990]--;
            }

            UnlockCollaborations();
            UnlockAlbums();
            return true;
        }
        #endregion

        #region Collaboration
        public bool CreateCollaboration(int p_ArtistId, int p_AlbumId)
        {
            CheckUpdateArtistResources();
            CheckUpdateAlbumResources();
            CheckUpdateCollaboResources();

            if(!m_ArtistsById.ContainsKey(p_ArtistId) || !m_Albums.ContainsKey(p_AlbumId))
            {
                return false;
            }

            if (CheckCollaborationsLocked())
            {
                int counter = 0;
                while (CheckCollaborationsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }

            LockCollaborations();
            if(!m_ArtistCollaborations.ContainsKey(p_ArtistId))
            {
                m_ArtistCollaborations.Add(p_ArtistId, new List<int>() { p_AlbumId });
            }
            else
            {
                if(!m_ArtistCollaborations[p_ArtistId].Contains(p_AlbumId))
                {
                    m_ArtistCollaborations[p_ArtistId].Add(p_AlbumId);
                }
            }
            
            if(!m_AlbumCollaborations.ContainsKey(p_AlbumId))
            {
                m_AlbumCollaborations.Add(p_AlbumId, new List<int>() { p_ArtistId });
            }
            else
            {
                if(!m_AlbumCollaborations[p_AlbumId].Contains(p_ArtistId))
                {
                    m_AlbumCollaborations[p_AlbumId].Add(p_ArtistId);
                }
            }
            UnlockCollaborations();
            return true;
        }

        public IList<Collaboration> ReadAllCollaborations()
        {
            CheckUpdateCollaboResources();
            IList<Collaboration> result = new List<Collaboration>();

            foreach(var artist in m_ArtistCollaborations.Keys)
            {
                foreach(var album in m_ArtistCollaborations[artist])
                {
                    result.Add(new Collaboration(artist, album));
                }
            }

            return result;
        }

        public IList<Collaboration> ReadCollaborationsByArtist(int p_ArtistId)
        {
            CheckUpdateCollaboResources();
            IList<Collaboration> result = new List<Collaboration>();
            if(m_ArtistCollaborations.ContainsKey(p_ArtistId))
            {
                foreach(var albumId in m_ArtistCollaborations[p_ArtistId])
                {
                    result.Add(new Collaboration(p_ArtistId, albumId));
                }
            }
            return result;
        }

        public IList<Collaboration> ReadCollaborationsByAlbum(int p_AlbumId)
        {
            CheckUpdateCollaboResources();
            IList<Collaboration> result = new List<Collaboration>();
            if (m_AlbumCollaborations.ContainsKey(p_AlbumId))
            {
                foreach (var artistId in m_AlbumCollaborations[p_AlbumId])
                {
                    result.Add(new Collaboration(artistId, p_AlbumId));
                }
            }
            return result;
        }

        public bool UpdateCollaboration(int p_ArtistId, int p_AlbumId, int p_AlbumIdNew)
        {
            if(DeleteCollaboration(p_ArtistId, p_AlbumId))
            {
                CreateCollaboration(p_ArtistId, p_AlbumIdNew);
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool DeleteCollaboration(int p_ArtistId, int p_AlbumId)
        {
            CheckUpdateCollaboResources();
            
            if(!m_ArtistCollaborations.ContainsKey(p_ArtistId) || !m_AlbumCollaborations.ContainsKey(p_AlbumId))
            {
                return false;
            }

            if(!m_AlbumCollaborations[p_AlbumId].Contains(p_ArtistId) || !m_ArtistCollaborations[p_ArtistId].Contains(p_AlbumId))
            {
                return false;
            }

            if (CheckCollaborationsLocked())
            {
                int counter = 0;
                while (CheckCollaborationsLocked())
                {
                    counter++;
                    Thread.Sleep(5);
                    if (counter >= 10)
                    {
                        return false;
                    }
                }
            }
            
            LockCollaborations();
            m_ArtistCollaborations[p_ArtistId].Remove(p_AlbumId);
            m_AlbumCollaborations[p_AlbumId].Remove(p_ArtistId);
            UnlockCollaborations();
            return true;
        }
        #endregion

        #region Album Indexing
        private void InitializeIndex()
        {
            m_AlbumCountIndex = new int[21];
            foreach (var album in m_Albums.Values)
            {
                if(album.Year >= 1990 && album.Year <= 2010)
                {
                    m_AlbumCountIndex[album.Year - 1990]++;
                }
            }
        }
        #endregion

        #region Queries
        public IList<Album> GetAllAlbumsByArtistId(int p_ArtistId)
        {
            if(!m_ArtistsById.ContainsKey(p_ArtistId))
            {
                return null;
            }

            IList<Album> result = new List<Album>();
            foreach(var albumId in m_ArtistCollaborations[p_ArtistId])
            {
                result.Add(m_Albums[albumId]);
            }
            return result;
        }

        public int GetLatestAlbumRelease()
        {
            int latest = 0;
            foreach(var album in m_Albums.Values)
            {
                if(album.Year > latest)
                {
                    latest = album.Year;
                }
            }
            return latest;
        }

        public int GetArtistFoundingYear(int p_ArtistId)
        {
            if(m_ArtistsById.ContainsKey(p_ArtistId))
            {
                return m_ArtistsById[p_ArtistId].Year;
            }
            return -1; //Fail
        }

        public IList<Artist> GetArtistsNoAlbums()
        {
            IList<Artist> result = new List<Artist>();
            foreach(var artistId in m_ArtistsById.Keys)
            {
                if(!m_ArtistCollaborations.ContainsKey(artistId))
                {
                    result.Add(m_ArtistsById[artistId]);
                }
            }
            return result;
        }
        #endregion
    }
}