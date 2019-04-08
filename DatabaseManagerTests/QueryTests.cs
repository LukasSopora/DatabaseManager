using System;
using DatabaseManager.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseManagerTests
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void InitDB()
        {
            DatabaseHelper.InitDataBase();
        }

        [TestMethod]
        public void GetAllTests()
        {
            DatabaseHelper.InitDataBase();

            QueryHelper.GetAllArtists();
            QueryHelper.GetAllAlbums();
            QueryHelper.GetAllCollaborations();
        }
    }
}
