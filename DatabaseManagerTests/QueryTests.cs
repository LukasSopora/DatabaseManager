﻿using System;
using System.Threading;
using DatabaseManager.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseManagerTests
{
    [TestClass]
    public class QueryTests
    {
        private QueryHelper m_QueryHelper;

        private void CheckQueryHelper()
        {
            if(m_QueryHelper == null)
            {
                m_QueryHelper = new QueryHelper();
            }
        }

        [TestMethod]
        public void InitDB()
        {
            DatabaseHelper.InitDataBase();
        }

        [TestMethod]
        public void InitQueryHelper()
        {
            m_QueryHelper = new QueryHelper();
        }

        [TestMethod]
        public void GetAllArtists()
        {
            CheckQueryHelper();

            m_QueryHelper.ReadArtist();
        }

        [TestMethod]
        public void CheckUpdateResources()
        {
            DatabaseHelper.InitDataBase();
            CheckQueryHelper();

            m_QueryHelper.ReadArtist();

            DatabaseHelper.InitDataBase();
            m_QueryHelper.ReadArtist();
        }

        [TestMethod]
        public void GetAllAlbums()
        {
            CheckQueryHelper();

            m_QueryHelper.ReadAllAlbums();
        }

        [TestMethod]
        public void GetAllCollaborations()
        {
            CheckQueryHelper();

            m_QueryHelper.GetAllCollaborations();
        }

        [TestMethod]
        public void GetArtistById()
        {
            CheckQueryHelper();

            m_QueryHelper.GetArtistById(1);
            m_QueryHelper.GetArtistById(2);
            m_QueryHelper.GetArtistById(3);
            m_QueryHelper.GetArtistById(4);
            m_QueryHelper.GetArtistById(5);
        }

        [TestMethod]
        public void GetAlbumtById()
        {
            CheckQueryHelper();

            m_QueryHelper.ReadAlbumById(1);
            m_QueryHelper.ReadAlbumById(2);
            m_QueryHelper.ReadAlbumById(3);
            m_QueryHelper.ReadAlbumById(4);
            m_QueryHelper.ReadAlbumById(5);
        }

        

        //[TestMethod]
        //public void GetCollaborationByArtistId()
        //{
        //    QueryHelper.GetCollaborationByArtistId(1);
        //    QueryHelper.GetCollaborationByArtistId(2);
        //    QueryHelper.GetCollaborationByArtistId(3);
        //    QueryHelper.GetCollaborationByArtistId(4);
        //    QueryHelper.GetCollaborationByArtistId(5);
        //}

        //[TestMethod]
        //public void GetCollaborationByAlbumId()
        //{
        //    QueryHelper.GetCollaborationByAlbumId(1);
        //    QueryHelper.GetCollaborationByAlbumId(2);
        //    QueryHelper.GetCollaborationByAlbumId(3);
        //    QueryHelper.GetCollaborationByAlbumId(4);
        //    QueryHelper.GetCollaborationByAlbumId(5);
        //}

        //[TestMethod, TestCategory("Queries")]
        //public void GetAllAlbumsByArtistId()
        //{
        //    QueryHelper.GetAllAlbumsByArtistId(1);
        //    QueryHelper.GetAllAlbumsByArtistId(2);
        //    QueryHelper.GetAllAlbumsByArtistId(3);
        //    QueryHelper.GetAllAlbumsByArtistId(4);
        //    QueryHelper.GetAllAlbumsByArtistId(5);
        //}

        //[TestMethod, TestCategory("Queries")]
        //public void GetLatestAlbumRelease()
        //{
        //    QueryHelper.GetLatestAlbumRelease();
        //}

        //[TestMethod, TestCategory("Queries")]
        //public void GetArtistFoundingYear()
        //{
        //    QueryHelper.GetArtistFoundingYear(1);
        //    QueryHelper.GetArtistFoundingYear(2);
        //    QueryHelper.GetArtistFoundingYear(3);
        //    QueryHelper.GetArtistFoundingYear(4);
        //    QueryHelper.GetArtistFoundingYear(5);
        //}

        //[TestMethod, TestCategory("Queries")]
        //public void GetArtistsNoAlbums()
        //{
        //    QueryHelper.GetArtistsNoAlbums();
        //}
    }
}
