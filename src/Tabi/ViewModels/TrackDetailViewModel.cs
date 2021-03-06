﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.Controls;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Model;
using Tabi.Pages;
using Tabi.Resx;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Position = Xamarin.Forms.Maps.Position;

namespace Tabi.ViewModels
{
    public class TrackDetailViewModel : ObservableObject
    {
        private readonly IRepoManager _repoManager;
        private readonly INavigation _navigation;
        private readonly TrackEntry _trackEntry;
        private readonly IMotiveConfiguration _motiveConfig;

        private ListItem _motiveListItem;


        public TrackDetailViewModel(IRepoManager repoManager, INavigation navigation, IMotiveConfiguration motiveConfig, TrackEntry trackEntry)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _trackEntry = trackEntry ?? throw new ArgumentNullException(nameof(trackEntry));
            _motiveConfig = motiveConfig ?? throw new ArgumentNullException(nameof(Motive));

            TransportationModeEntry modeEntry = _repoManager.TransportationModeRepository.GetLastWithTrackEntry(_trackEntry.Id);

            TransportModeSelectionCommand = new Command(async () =>
            {
                await _navigation.PushModalAsync(new NavigationPage(new TransportSelectionPage(_trackEntry)));
            });

            OpenTrackMotiveCommand = new Command(async () =>
            {
                TrackDetailMotivePage motivePage = new TrackDetailMotivePage(Motive);
                await _navigation.PushModalAsync(new NavigationPage(motivePage));
            });

            DataItems = new ObservableRangeCollection<ListItem>();

            if (_motiveConfig.Tracks)
            {
                PrepareForMotive();
            }

            CompletedTransport = modeEntry != null;

        }

        private void PrepareForMotive()
        {
            // Find an existing Motive for the current track.
            Motive stopMotive = _repoManager.MotiveRepository.GetByTrackId(_trackEntry.Id);
            // Initialize a new motive since the ViewModel needs one.
            stopMotive = stopMotive ?? new Motive() { TrackId = _trackEntry.Id };

            Motive = new TrackMotiveViewModel(stopMotive, _motiveConfig);

            _motiveListItem = new ListItem()
            {
                Name = AppResources.TrackMotiveLabel,
                Subtitle = Motive.ConvertedText ?? AppResources.SetTrackMotiveHint,
                Command = OpenTrackMotiveCommand
            };

            DataItems.Add(_motiveListItem);

            Motive.PropertyChanged += Motive_PropertyChanged; ;
        }

        public ICommand TransportModeSelectionCommand { get; protected set; }

        public ICommand OpenTrackMotiveCommand { get; set; }

        public ObservableRangeCollection<ListItem> DataItems { get; private set; }

        public TrackMotiveViewModel Motive { get; set; }

        void Motive_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                _motiveListItem.Subtitle = Motive.ConvertedText ?? AppResources.SetTrackMotiveHint;
            }
        }

        public MapSpan AveragePosition()
        {
            MapSpan result = MapSpan.FromCenterAndRadius(new Position(52.092876, 5.104480), Distance.FromKilometers(30));
            List<PositionEntry> positions = _repoManager.PositionEntryRepository.FilterPeriodAccuracy(_trackEntry.StartTime, _trackEntry.EndTime, 100);

            if (positions.Count() > 0)
            {
                PositionEntry avg = Util.AveragePosition(positions);
                System.Diagnostics.Debug.WriteLine($"AVG {avg.Latitude} {avg.Longitude}");

                double distance = _trackEntry.DistanceTravelled < 50000 ? _trackEntry.DistanceTravelled : 50000;

                result = MapSpan.FromCenterAndRadius(new Position(avg.Latitude, avg.Longitude), Distance.FromMeters(distance));
            }

            return result;
        }

        private bool completedTransport;

        public bool CompletedTransport {
            get => completedTransport;
            set => SetProperty(ref completedTransport, value);
        }

        public (Line, Pin, Pin) GetMapData()
        {
            Line line = new Line();
            Pin startPin = null;
            Pin endPin = null;

            List<PositionEntry> positions = _repoManager.PositionEntryRepository.FilterPeriodAccuracy(_trackEntry.StartTime, _trackEntry.EndTime, 100);
            foreach (PositionEntry pe in positions)
            {
                line.Positions.Add(new Position(pe.Latitude, pe.Longitude));
            }

            if (positions.Count >= 2)
            {
                PositionEntry firstPos = positions.First();
                PositionEntry lastPos = positions.Last();

                startPin = new Pin()
                {
                    Position = new Position(
                    firstPos.Latitude, firstPos.Longitude),
                    Label = "Start"
                };
                endPin = new Pin()
                {
                    Position = new Position(
                    lastPos.Latitude, lastPos.Longitude),
                    Label = "End",
                    Type = PinType.Generic
                };
            }

            return (line, startPin, endPin);
        }
    }
}
