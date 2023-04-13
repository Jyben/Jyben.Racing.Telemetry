
using System;
using Jyben.Racing.Telemetry.Services;
using Jyben.Racing.Telemetry.Services.Impl;
using Microsoft.AspNetCore.Components;

namespace Jyben.Racing.Telemetry.Pages
{
    public partial class Telemetrie
    {
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public ITelemetrieArrierePlanService TelemetrieArrierePlanService { get; set; }

        private bool _isDisposed;
        private bool _estTelemetrieActivee;
        private string _textToggle = "Démarrer";
        private string _nomPilote;

        private void ToggleTelemetrie()
        {
            if (_estTelemetrieActivee)
            {
                TelemetrieArrierePlanService.Stop();
                _estTelemetrieActivee = false;
                _textToggle = "Démarrer";
            }
            else
            {
                TelemetrieArrierePlanService.Start(_nomPilote);
                _estTelemetrieActivee = true;
                _textToggle = "Arrêter";
            }
        }
    }
}