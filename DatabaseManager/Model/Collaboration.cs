using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class Collaboration
    {
        private int m_ArtistId;
        private int m_AlbumId;

        public Collaboration()
        {
        }

        public Collaboration(int p_ArtistId, int p_AlbumId)
        {
            m_ArtistId = p_ArtistId;
            m_AlbumId = p_AlbumId;
        }

        public int AlbumId
        {
            get { return m_AlbumId; }
            set { m_AlbumId = value; }
        }

        public int ArtistId
        {
            get { return m_ArtistId; }
            set { m_ArtistId = value; }
        }
    }
}
