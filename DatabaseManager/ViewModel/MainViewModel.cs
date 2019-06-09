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
using System.Collections.ObjectModel;

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

        private ObservableCollection<Artist> m_Artists =
            new ObservableCollection<Artist>();
        private ObservableCollection<Album> m_ArtistAlbums =
            new ObservableCollection<Album>();
        private ObservableCollection<Artist> m_ArtistsQueryFour =
            new ObservableCollection<Artist>();


        private Artist m_ArtistQueryOne;
        private int m_LastRealeaseYear;
        private int m_FoundingYear;
        private Artist m_ArtistQueryThree;

        public int FoundingYear
        {
            get { return m_FoundingYear; }
            set { SetProperty(ref m_FoundingYear, value); }
        }

        public ObservableCollection<Artist> ArtistsQueryFour
        {
            get { return m_ArtistsQueryFour; }
            set { SetProperty(ref m_ArtistsQueryFour, value); }
        }

        public Artist ArtistQueryThree
        {
            get { return m_ArtistQueryThree; }
            set { SetProperty(ref m_ArtistQueryThree, value); }
        }

        public Artist ArtistQueryOne
        {
            get { return m_ArtistQueryOne; }
            set { SetProperty(ref m_ArtistQueryOne, value); }
        }

        public int LastReleaseYear
        {
            get { return m_LastRealeaseYear; }
            set { SetProperty(ref m_LastRealeaseYear, value); }
        }

        public ObservableCollection<Album> ArtistAlbums
        {
            get { return m_ArtistAlbums; }
            set { SetProperty(ref m_ArtistAlbums, value); }
        }

        public ObservableCollection<Artist> Artists
        {
            get { return m_Artists; }
            set { SetProperty(ref m_Artists, value); }
        }

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

        public DelegateCommand UpdateQueryOneCommand { get; private set; }
        public DelegateCommand UpdateQueryTwoCommand { get; private set; }
        public DelegateCommand UpdateQueryThreeCommand { get; private set; }
        public DelegateCommand UpdateQueryFourCommand { get; private set; }

        public DelegateCommand ExportCSVCommand { get; private set; }

        private void InitalizeCommands()
        {
            InitDBCommand = new DelegateCommand(OnInitDB);
            RaisePropertyChanged(nameof(InitDBCommand));

            InitQueryCommand = new DelegateCommand(OnInitQuery);
            RaisePropertyChanged(nameof(InitQueryCommand));

            UpdateQueryOneCommand = new DelegateCommand(OnQueryOne);
            RaisePropertyChanged(nameof(UpdateQueryOneCommand));

            UpdateQueryTwoCommand = new DelegateCommand(OnQueryTwo);
            RaisePropertyChanged(nameof(UpdateQueryTwoCommand));

            UpdateQueryThreeCommand = new DelegateCommand(OnQueryThree);
            RaisePropertyChanged(nameof(UpdateQueryThreeCommand));

            UpdateQueryFourCommand = new DelegateCommand(OnQueryFour);
            RaisePropertyChanged(nameof(UpdateQueryFourCommand));

            ExportCSVCommand = new DelegateCommand(OnExportCSV);
            RaisePropertyChanged(nameof(ExportCSVCommand));
        }

        private void OnExportCSV()
        {
            m_QueryHelper.ExportCSV();
        }

        private void OnQueryFour()
        {
            ArtistsQueryFour = new ObservableCollection<Artist>(m_QueryHelper.GetArtistsNoAlbums());
        }

        private void OnQueryThree()
        {
            if(m_ArtistQueryThree == null)
            {
                return;
            }

            FoundingYear = m_QueryHelper.GetArtistFoundingYear(m_ArtistQueryThree.Id);
        }

        private void OnQueryTwo()
        {
            LastReleaseYear = m_QueryHelper.GetLatestAlbumRelease();
        }

        private void OnQueryOne()
        {
            if(m_ArtistQueryOne == null)
            {
                return;
            }

            ArtistAlbums = new ObservableCollection<Album>(m_QueryHelper.GetAllAlbumsByArtistId(m_ArtistQueryOne.Id));
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

            InitSpecialQueries();
        }

        private void InitSpecialQueries()
        {
            Artists = new ObservableCollection<Artist>(m_QueryHelper.ReadAllArtists());

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
        #endregion

        public MainViewModel()
        {
            InitalizeCommands();
        }
    }
}
