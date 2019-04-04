using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class Artist
    {
        private int m_Id;
        private string m_Name;
        private int m_Year;
        private string m_Country;

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

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
    }
}
