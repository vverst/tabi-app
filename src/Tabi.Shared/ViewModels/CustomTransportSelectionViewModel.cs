﻿using System;
using System.Windows.Input;
using Tabi.Shared.Resx;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class CustomTransportSelectionViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private readonly Controls.SelectableObservableCollection<Model.TransportModeItem> _activeItems;

        public CustomTransportSelectionViewModel(INavigation navigation, Controls.SelectableObservableCollection<Model.TransportModeItem> activeItems)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _activeItems = activeItems ?? throw new ArgumentNullException(nameof(activeItems));

            SaveCommand = new Command(async () =>
            {
                if (Text.Contains(","))
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(AppResources.CommaRestrictionText, AppResources.CommaRestrictionTitle, AppResources.OkText);
                }
                activeItems.Add(new Model.TransportModeItem() { Id = Text, Name = Text }, true);
                await _navigation.PopModalAsync();
            });

            CancelCommand = new Command(async () =>
            {
                await _navigation.PopModalAsync();
            });

        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public string Text { get; set; }
    }
}
