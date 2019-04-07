using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class AlbumTO
    {
        private string m_Name;
        private IList<string> m_Artists =
            new List<string>();
        private int m_Year;

        public int Year
        {
            get { return m_Year; }
            set { m_Year = value; }
        }


        public IList<string> Artists
        {
            get { return m_Artists; }
            set { m_Artists = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public AlbumTO()
        {

        }

        public AlbumTO(string p_Name, IList<string> p_Artists, int p_Year)
        {
            m_Name = p_Name;
            m_Artists = p_Artists;
            m_Year = p_Year;
        }

        public override string ToString()
        {
            return m_Name;
        }
    }
}
