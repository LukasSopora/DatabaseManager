using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class Artist
    {
        private string m_Name;
        private int m_Year;
        private string m_Country;
        private int m_Id;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public string Country
        {
            get { return m_Country; }
            set { m_Country = value; }
        }


        public int Year
        {
            get { return m_Year; }
            set { m_Year = value; }
        }


        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Artist()
        {

        }

        public Artist(string p_Name)
        {
            m_Name = p_Name;
        }

        public Artist(int p_Id, string p_Name, int p_Year, string p_Country)
        {
            m_Id = p_Id;
            m_Name = p_Name;
            m_Year = p_Year;
            m_Country = p_Country;
        }

        public override string ToString()
        {
            return m_Name;
        }
    }
}
