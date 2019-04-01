using DatabaseManager.TestData;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager.ViewModel
{
    public class MainViewModel : BindableBase
    {
        #region props
        #endregion

        #region Commands
        public DelegateCommand AllAlbumsFromArtist { get; private set; }
        public DelegateCommand LatestAlbumCommand { get; private set; }
        public DelegateCommand BandFormedCommand { get; private set; }
        public DelegateCommand NoAlbumsCommand { get; private set; }

        private void InitalizeCommands()
        {
            AllAlbumsFromArtist = new DelegateCommand(OnAllAlbumbsFromArtist);
            RaisePropertyChanged(nameof(AllAlbumsFromArtist));

            LatestAlbumCommand = new DelegateCommand(OnLatestAlbum);
            RaisePropertyChanged(nameof(LatestAlbumCommand));

            BandFormedCommand = new DelegateCommand(OnBandFormed);
            RaisePropertyChanged(nameof(BandFormedCommand));

            NoAlbumsCommand = new DelegateCommand(OnNoAlbums);
            RaisePropertyChanged(nameof(NoAlbumsCommand));
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
            TestDataReader.GetArtists();
            InitalizeCommands();
        }
    }
}
