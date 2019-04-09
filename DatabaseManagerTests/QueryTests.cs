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
        public void GetAllArtists()
        {
            QueryHelper.GetAllArtists();
        }

        [TestMethod]
        public void GetAllAlbums()
        {
            QueryHelper.GetAllAlbums();
        }

        [TestMethod]
        public void GetAllCollaborations()
        {
            QueryHelper.GetAllCollaborations();
        }

        [TestMethod]
        public void GetArtistById()
        {
            QueryHelper.GetArtistById(1);
            QueryHelper.GetArtistById(2);
            QueryHelper.GetArtistById(3);
            QueryHelper.GetArtistById(4);
            QueryHelper.GetArtistById(5);
        }

        [TestMethod]
        public void GetAlbumtById()
        {
            QueryHelper.GetAlbumById(1);
            QueryHelper.GetAlbumById(2);
            QueryHelper.GetAlbumById(3);
            QueryHelper.GetAlbumById(4);
            QueryHelper.GetAlbumById(5);
        }

        [TestMethod]
        public void GetCollaborationByArtistId()
        {
            QueryHelper.GetCollaborationByArtistId(1);
            QueryHelper.GetCollaborationByArtistId(2);
            QueryHelper.GetCollaborationByArtistId(3);
            QueryHelper.GetCollaborationByArtistId(4);
            QueryHelper.GetCollaborationByArtistId(5);
        }

        [TestMethod]
        public void GetCollaborationByAlbumId()
        {
            QueryHelper.GetCollaborationByAlbumId(1);
            QueryHelper.GetCollaborationByAlbumId(2);
            QueryHelper.GetCollaborationByAlbumId(3);
            QueryHelper.GetCollaborationByAlbumId(4);
            QueryHelper.GetCollaborationByAlbumId(5);
        }

        [TestMethod]
        public void GetAllAlbumsByArtistId()
        {
            QueryHelper.GetAllAlbumsByArtistId(1);
            QueryHelper.GetAllAlbumsByArtistId(2);
            QueryHelper.GetAllAlbumsByArtistId(3);
            QueryHelper.GetAllAlbumsByArtistId(4);
            QueryHelper.GetAllAlbumsByArtistId(5);
        }
    }
}
