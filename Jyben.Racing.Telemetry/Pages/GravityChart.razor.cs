using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Jyben.Racing.Telemetry.Pages
{
    public partial class GravityChart : IDisposable
    {
        private Canvas2DContext _context;
        protected BECanvasComponent _canvasReference;

        private float _x, _y;
        private decimal _gX, _gY;
        private bool _isDisposed;

        protected override void OnInitialized()
        {
            ToggleAccelerometer();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _context = await _canvasReference.CreateCanvas2DAsync();
            await _context.SetFillStyleAsync("green");
        }

        public void ToggleAccelerometer()
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (!Accelerometer.Default.IsMonitoring)
                {
                    // Turn on accelerometer
                    Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Default.Start(SensorSpeed.UI);
                }
                else
                {
                    // Turn off accelerometer
                    Accelerometer.Default.Stop();
                    Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                }
            }
        }

        private async void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            _gX = Math.Round((decimal)e.Reading.Acceleration.X, 2);
            _gY = Math.Round((decimal)e.Reading.Acceleration.Y, 2);

            if (_context != null 
                && Math.Round((decimal)_x, 0) != Math.Round(((decimal)e.Reading.Acceleration.X * 10) + 40, 0) 
                && Math.Round((decimal)_y, 0) != Math.Round(((decimal)e.Reading.Acceleration.Y * 10) + 40, 0))
            {
                await _context.ClearRectAsync(0, 0, 150, 150);
                _x = (e.Reading.Acceleration.X * 10) + 40;
                _y = (e.Reading.Acceleration.Y * 10) + 40;
                await _context.FillRectAsync(_x, _y, 15, 15);
            }

            StateHasChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                ToggleAccelerometer();
                Dispose();
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}