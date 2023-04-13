using System;
namespace Jyben.Racing.Telemetry.Services
{
	public interface ITelemetrieArrierePlanService
	{
        void Start(string nomPilote);
        void Stop();
    }
}

