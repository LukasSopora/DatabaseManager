using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class Album
    {
        private string m_Name;
        private ObservableCollection<Artist> m_Artists;
        private int m_Year;
        private int m_Id;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }


        public int Year
        {
            get { return m_Year; }
            set { m_Year = value; }
        }


        public ObservableCollection<Artist> Artists
        {
            get { return m_Artists; }
            set { m_Artists = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Album()
        {

        }

        public Album(int p_Id, string p_Name, ObservableCollection<Artist> p_Artists, int p_Year)
        {
            m_Id = p_Id;
            m_Name = p_Name;
            m_Artists = p_Artists;
            m_Year = p_Year;
        }
    }
}
