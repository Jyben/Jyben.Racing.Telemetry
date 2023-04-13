using System;
using System.Text.Json;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using GeolocatorPlugin;
using GeolocatorPlugin.Abstractions;
using GoogleGson;
using Jyben.Racing.Telemetry.Helpers;
using Jyben.Racing.Telemetry.Infrastructure.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Jyben.Racing.Telemetry.Services.Impl
{
    [Service(Name = "com.companyname.jyben.racing.telemetry.TelemetrieArrierePlanService")]
    public class TelemetrieArrierePlanService : Service, ITelemetrieArrierePlanService
    {
        private Position _positionPrecedente;
        private Position _positionPrecedenteNonFixee;
        private List<Position> _positions = new();
        private HubConnection _hubConnection;
        private TelemetryPilote _telemetriePilote = new();
        private int _viaranceDeLaMesure = 50;
        private int _varianceDuProcess = 5;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            AfficherNotification();

            _telemetriePilote.Nom = "Jyben";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://jyben-racing-dashboard.azurewebsites.net/signalr/telemetry")
                .Build();

            // on démarre la télémétrie dès qu'on reçoit le signal
            _hubConnection.On<bool>("BasculerTelemetrie", (estTelemetrieActivee) =>
            {
                ToggleGPS(estTelemetrieActivee);
                if (estTelemetrieActivee)
                {
                    Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));
                }
                else
                {
                    Vibration.Default.Vibrate(TimeSpan.FromSeconds(3));
                }
            });

            // déclenche une vribration dès qu'un meilleur temps est réalisé
            _hubConnection.On("NotifierMeilleurTemps", () =>
            {
                Vibration.Default.Vibrate(TimeSpan.FromSeconds(3));
            });

            _hubConnection.StartAsync();

            // on indique au serveur qu'on est connecté et prêt à écouter les signaux
            string nomPilote = intent.GetStringExtra("nomPilote");
            _telemetriePilote.Nom = nomPilote;
            _hubConnection.SendAsync("Connexion", nomPilote);

            return StartCommandResult.Sticky;
        }

        private void AfficherNotification()
        {
            String NOTIFICATION_CHANNEL_ID = "com.Your.project.id";

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);

            Notification notification = notificationBuilder.SetOngoing(true)
                .SetContentTitle("Télémétrie")
                .SetContentText("FONCE GUITOU !")
                .SetSmallIcon(Resource.Drawable.ic_clock_black_24dp)
                .SetOngoing(true)
                .Build();

            const int Service_Running_Notification_ID = 936;
            StartForeground(Service_Running_Notification_ID, notification);
        }   

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            ToggleGPS(false);
            base.OnDestroy();
        }

        public void Start(string nomPilote)
        {
            var intent = new Intent(Android.App.Application.Context, typeof(TelemetrieArrierePlanService));
            intent.PutExtra("nomPilote", nomPilote);
            Android.App.Application.Context.StartForegroundService(intent);
        }

        public void Stop()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(TelemetrieArrierePlanService));
            Android.App.Application.Context.StopService(intent);
        }

        public async void ToggleGPS(bool toggleOn)
        {
            if (toggleOn)
            {
                if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromMilliseconds(200), 1, true, new ListenerSettings
                {
                    ActivityType = ActivityType.AutomotiveNavigation,
                    AllowBackgroundUpdates = true,
                    DeferLocationUpdates = false,
                    ListenForSignificantChanges = false,
                    PauseLocationUpdatesAutomatically = false,

                }))
                {
                    CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                }
            }
            else
            {
                if (await CrossGeolocator.Current.StopListeningAsync())
                {
                    CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                }
            }
        }

        private async void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e)
        {
            double vitesse = 0;

            // permet d'éviter les doublons : actuellement ça envoi 2 fois la même position en même temps
            if (_positionPrecedenteNonFixee != null && _positionPrecedenteNonFixee.Longitude == e.Position.Longitude && _positionPrecedenteNonFixee.Latitude == e.Position.Latitude)
            {
                _positionPrecedenteNonFixee = e.Position;
                return;
            }

            _positionPrecedenteNonFixee = e.Position;

            if (_positionPrecedente != null)
            {
                vitesse = CalculerVitesse(_positionPrecedente, e.Position);
                if (vitesse > 0)
                {
                    vitesse = Math.Round(vitesse, 1);
                    Console.WriteLine("Vitesse : " + vitesse + " km/h");
                }
            }

            _positionPrecedente = e.Position;

            _telemetriePilote.Trace = new()
            {
                Latitude = _positionPrecedente.Latitude,
                Longitude = Math.Abs(_positionPrecedente.Longitude),
                Vitesse = vitesse
            };

            var json = JsonSerializer.Serialize(_telemetriePilote);

            await _hubConnection.SendAsync("EnvoyerTelemetry", json);
        }

        public static Position Filter(Position position, KalmanFilter latitudeFilter, KalmanFilter longitudeFilter)
        {
            double filteredLatitude = latitudeFilter.Filter(position.Latitude);
            double filteredLongitude = longitudeFilter.Filter(position.Longitude);

            return new Position(filteredLatitude, filteredLongitude);
        }

        public static double CalculerVitesse(Position previousLocation, Position currentLocation)
        {
            var timeSpan = currentLocation.Timestamp - previousLocation.Timestamp;
            double distanceEnKm = previousLocation.CalculateDistance(currentLocation, GeolocatorUtils.DistanceUnits.Kilometers);
            double heures = timeSpan.TotalHours;
            double vitesse = distanceEnKm / heures;
            return vitesse;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}

