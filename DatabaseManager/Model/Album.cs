using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.Model
{
    public class Album
    {
        private int m_Id;
        private int m_Year;
        private string m_Name;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public int Year
        {
            get { return m_Year; }
            set { m_Year = value; }
        }

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public override string ToString()
        {
            return m_Name;
        }
    }
}
