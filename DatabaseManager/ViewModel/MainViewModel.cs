using DatabaseManager.Model;
using DatabaseManager.TestData;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManager;
using DatabaseManager.Database;
using System.Threading;

namespace DatabaseManager.ViewModel
{
    public class MainViewModel : BindableBase
    {
        #region props
        private bool m_InitDBProgress = false;
        private bool m_InitQueryProgress = false;
        private bool m_InitDBFinished = false;
        private bool m_InitQueryFinished = false;

        private QueryHelper m_QueryHelper;

        private TimeSpan m_InitDBDuration;
        private TimeSpan m_InitQueryDuration;

        public bool InitQueryFinished
        {
            get { return m_InitQueryFinished; }
            set { SetProperty(ref m_InitQueryFinished, value); }
        }

        public bool InitDBFinished
        {
            get { return m_InitDBFinished; }
            set { SetProperty(ref m_InitDBFinished, value); }
        }

        public TimeSpan InitQueryDuration
        {
            get { return m_InitQueryDuration; }
            set { SetProperty(ref m_InitQueryDuration, value); }
        }

        public TimeSpan InitDBDuration
        {
            get { return m_InitDBDuration; }
            set { SetProperty(ref m_InitDBDuration, value); }
        }

        public bool InitQueryProgress
        {
            get { return m_InitQueryProgress; }
            set { SetProperty(ref m_InitQueryProgress, value); }
        }


        public bool InitDBProgress
        {
            get { return m_InitDBProgress; }
            set { SetProperty(ref m_InitDBProgress, value); }
        }

        #endregion

        #region Commands
        public DelegateCommand InitDBCommand { get; private set; }
        public DelegateCommand InitQueryCommand { get; private set; }

        public DelegateCommand AllAlbumsFromArtist { get; private set; }
        public DelegateCommand LatestAlbumCommand { get; private set; }
        public DelegateCommand BandFormedCommand { get; private set; }
        public DelegateCommand NoAlbumsCommand { get; private set; }

        private void InitalizeCommands()
        {
            InitDBCommand = new DelegateCommand(OnInitDB);
            RaisePropertyChanged(nameof(InitDBCommand));

            InitQueryCommand = new DelegateCommand(OnInitQuery);
            RaisePropertyChanged(nameof(InitQueryCommand));

            AllAlbumsFromArtist = new DelegateCommand(OnAllAlbumbsFromArtist);
            RaisePropertyChanged(nameof(AllAlbumsFromArtist));

            LatestAlbumCommand = new DelegateCommand(OnLatestAlbum);
            RaisePropertyChanged(nameof(LatestAlbumCommand));

            BandFormedCommand = new DelegateCommand(OnBandFormed);
            RaisePropertyChanged(nameof(BandFormedCommand));

            NoAlbumsCommand = new DelegateCommand(OnNoAlbums);
            RaisePropertyChanged(nameof(NoAlbumsCommand));
        }

        private void OnInitQuery()
        {
            Thread t = new Thread(OnInitQueryStart);
            t.Start();
        }

        private void OnInitQueryStart()
        {
            InitQueryProgress = true;
            InitQueryFinished = false;
            var initQueryProgressStart = DateTime.Now;
            m_QueryHelper = new QueryHelper();
            InitQueryEnd();
            InitQueryDuration = DateTime.Now - initQueryProgressStart;
        }

        private void InitQueryEnd()
        {
            InitQueryProgress = false;
            InitQueryFinished = true;
        }

        private void OnInitDB()
        {
            Thread t = new Thread(InitDBStart);
            t.Start();
        }

        private void InitDBStart()
        {
            InitDBProgress = true;
            InitDBFinished = false;
            var initDBProgressStart = DateTime.Now;
            DatabaseHelper.InitDataBase();
            InitDBEnd();
            InitDBDuration = DateTime.Now - initDBProgressStart;
        }

        private void InitDBEnd()
        {
            InitDBProgress = false;
            InitDBFinished = true;
        }

        private void OnAllAlbumbsFromArtist()
        {
            throw new NotImplementedException();
        }

        private void OnLatestAlbum()
        {
            throw new NotImplementedException();
        }

        private void OnBandFormed()
        {
            throw new NotImplementedException();
        }

        private void OnNoAlbums()
        {
            throw new NotImplementedException();
        }
        #endregion

        public MainViewModel()
        {
            InitalizeCommands();
        }
    }
}
