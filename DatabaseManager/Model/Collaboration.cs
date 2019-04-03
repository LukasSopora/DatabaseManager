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
