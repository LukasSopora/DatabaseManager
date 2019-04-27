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
        private QueryHelper m_QueryHelper;

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
            InitQueryProgress = true;
            Thread t = new Thread(OnInitQueryStart);
            t.Start();
        }

        private void OnInitQueryStart()
        {
            m_QueryHelper = new QueryHelper();
            InitQueryEnd();
        }

        private void InitQueryEnd()
        {
            InitQueryProgress = false;
        }

        private void OnInitDB()
        {
            InitDBProgress = true;
            Thread t = new Thread(InitDBStart);
            t.Start();
        }

        private void InitDBStart()
        {
            DatabaseHelper.InitDataBase();
            InitDBEnd();
        }

        private void InitDBEnd()
        {
            InitDBProgress = false;
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
