using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PAN.Models;
using PAN.Services;
using System.Globalization;

namespace PAN.ViewModels
{
    [QueryProperty(nameof(IdEvenement), "idEvenement")]
    public partial class EventDetailViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IEvenementService evenementService)
        : BaseViewModel(dialogService, navigationService)
    {
        private readonly IEvenementService _evenementService = evenementService;

        [ObservableProperty]
        private int idEvenement;

        [ObservableProperty]
        private EvenementDetailItem? evenement;

        [ObservableProperty]
        private string mapUrl = string.Empty;

        public bool HasCoordinates =>
            Evenement?.Latitude != null && Evenement?.Longitude != null;

        public bool HasNoCoordinates => !HasCoordinates;

        public string MouvantText =>
            Evenement?.EstMouvant == true ? "Oui" : "Non";

        partial void OnIdEvenementChanged(int value)
        {
            if (value > 0)
                _ = LoadAsync();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            try
            {
                Evenement = await _evenementService.GetByIdAsync(IdEvenement);

                if (Evenement == null)
                {
                    MapUrl = string.Empty;
                    await Shell.Current.DisplayAlert("Erreur", "Événement introuvable.", "OK");
                    return;
                }

                if (HasCoordinates)
                {
                    string lat = Evenement.Latitude!.Value.ToString(CultureInfo.InvariantCulture);
                    string lng = Evenement.Longitude!.Value.ToString(CultureInfo.InvariantCulture);
                    string ville = Uri.EscapeDataString(Evenement.Ville ?? "Observation GEIPAN");

#if WINDOWS
                    MapUrl = $"ms-appx-web:///Resources/Raw/map.html?lat={lat}&lng={lng}&ville={ville}";
#elif ANDROID
                    MapUrl = $"file:///android_asset/map.html?lat={lat}&lng={lng}&ville={ville}";
#else
                    MapUrl = $"map.html?lat={lat}&lng={lng}&ville={ville}";
#endif
                }
                else
                {
                    MapUrl = string.Empty;
                }

                OnPropertyChanged(nameof(HasCoordinates));
                OnPropertyChanged(nameof(HasNoCoordinates));
                OnPropertyChanged(nameof(MouvantText));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Erreur",
                    $"Impossible de charger le détail : {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task UpVoteAsync()
        {
            if (Evenement == null)
                return;

            bool ok = await _evenementService.AddUpVoteAsync(Evenement.IdEvenement);

            if (!ok)
                return;

            Evenement.UpVote++;
            OnPropertyChanged(nameof(Evenement));

            await Shell.Current.DisplayAlert("Info", "Upvote ajouté", "OK");
        }

        [RelayCommand]
        private async Task OpenExternalMapAsync()
        {
            if (!HasCoordinates || Evenement == null)
                return;

            string lat = Evenement.Latitude!.Value.ToString(CultureInfo.InvariantCulture);
            string lng = Evenement.Longitude!.Value.ToString(CultureInfo.InvariantCulture);

            string url = $"https://www.openstreetmap.org/?mlat={lat}&mlon={lng}#map=15/{lat}/{lng}";
            await Launcher.Default.OpenAsync(url);
        }
    }
}