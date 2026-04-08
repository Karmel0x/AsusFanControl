using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AsusFanControl;
using AsusFanControlGUI.Models;

namespace AsusFanControlGUI.Services
{
    /// <summary>
    /// Background daemon that polls CPU temperature every 2 seconds, interpolates the
    /// active fan curve, and applies the result via AsusControl.
    /// </summary>
    public class FanCurveDaemon
    {
        private readonly AsusControl _asusControl;
        private CancellationTokenSource _cts;
        private Task _task;

        /// <summary>The profile applied by the daemon. Set before calling Start().</summary>
        public FanCurveProfile ActiveProfile { get; set; }

        /// <summary>
        /// When true and the interpolated RPM is 0, the daemon sends percent=0 which
        /// exits manual mode and lets ASUS firmware control the fan.
        /// When false, the daemon physically enforces 0% duty cycle.
        /// </summary>
        public bool LetAsusControlAtLowTemps { get; set; } = true;

        public bool IsRunning => _task != null && !_task.IsCompleted;

        public const float MaxRpm = 7000f;

        /// <summary>Fired on the thread pool with (cpuTempC, cpuFanPercent, cpuFanRpm).</summary>
        public event Action<float, float, float> StatsUpdated;

        /// <summary>Fired on the thread pool when an error occurs in the loop.</summary>
        public event Action<string> ErrorOccurred;

        public FanCurveDaemon(AsusControl asusControl)
        {
            _asusControl = asusControl;
        }

        public void Start()
        {
            if (IsRunning) return;
            if (ActiveProfile == null)
                throw new InvalidOperationException("ActiveProfile must be set before starting the daemon.");

            _cts = new CancellationTokenSource();
            _task = Task.Run(() => Loop(_cts.Token), _cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            try { _task?.Wait(3000); } catch { }
            _task = null;
        }

        private void Loop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // ── Read CPU temp ────────────────────────────────────────────────
                    float cpuTemp = (float)_asusControl.Thermal_Read_Cpu_Temperature();

                    // ── Interpolate CPU fan target ───────────────────────────────────
                    var cpuCurve = ActiveProfile?.CpuCurve;
                    if (cpuCurve != null && cpuCurve.Count > 0)
                    {
                        float targetRpm = InterpolateRpm(cpuCurve, cpuTemp);
                        int percent = (int)Math.Round(targetRpm / MaxRpm * 100f);
                        percent = Math.Max(0, Math.Min(100, percent));

                        if (targetRpm == 0 && LetAsusControlAtLowTemps)
                            _asusControl.SetFanSpeeds(0);  // 0 → exits test mode
                        else
                            _asusControl.SetFanSpeeds(percent);

                        // ── Read back actual RPM (fan 0) ─────────────────────────────
                        float actualRpm = 0;
                        try
                        {
                            var speeds = _asusControl.GetFanSpeeds();
                            if (speeds != null && speeds.Count > 0)
                                actualRpm = speeds[0];
                        }
                        catch { }

                        StatsUpdated?.Invoke(cpuTemp, percent, actualRpm);
                    }

                    // ── GPU fan (only if 2+ fans detected) ──────────────────────────
                    try
                    {
                        int fanCount = _asusControl.HealthyTable_FanCounts();
                        if (fanCount > 1)
                        {
                            var gpuCurve = ActiveProfile?.GpuCurve;
                            if (gpuCurve != null && gpuCurve.Count > 0)
                            {
                                // GPU temp is not directly readable via AsusWinIO64 in this
                                // rev of the DLL, so we skip GPU-specific interpolation and
                                // mirror CPU percent to GPU fan as a safe approximation.
                                // If GPU temp becomes available, plug it in here.
                                float gpuTargetRpm = InterpolateRpm(gpuCurve, cpuTemp);
                                int gpuPercent = (int)Math.Round(gpuTargetRpm / MaxRpm * 100f);
                                gpuPercent = Math.Max(0, Math.Min(100, gpuPercent));
                                // Set fan index 1 specifically
                                if (gpuTargetRpm == 0 && LetAsusControlAtLowTemps)
                                {
                                    _asusControl.SetFanSpeed(0, 1);
                                }
                                else
                                {
                                    _asusControl.SetFanSpeed(gpuPercent, 1);
                                }
                            }
                        }
                    }
                    catch { /* GPU fan optional — never crash the loop */ }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(ex.Message);
                }

                // Wait 2 seconds or until cancelled
                try { Task.Delay(2000, ct).Wait(); }
                catch (AggregateException ae) when (ae.InnerException is TaskCanceledException) { break; }
            }
        }

        /// <summary>Linear interpolation across sorted CurvePoint list.</summary>
        public static float InterpolateRpm(List<CurvePoint> curve, float tempC)
        {
            if (curve == null || curve.Count == 0) return 0;
            if (tempC <= curve[0].TempC) return curve[0].Rpm;
            if (tempC >= curve[curve.Count - 1].TempC) return curve[curve.Count - 1].Rpm;

            for (int i = 0; i < curve.Count - 1; i++)
            {
                var p1 = curve[i];
                var p2 = curve[i + 1];
                if (tempC >= p1.TempC && tempC <= p2.TempC)
                {
                    float t = (tempC - p1.TempC) / (p2.TempC - p1.TempC);
                    return p1.Rpm + (p2.Rpm - p1.Rpm) * t;
                }
            }

            return curve[curve.Count - 1].Rpm;
        }
    }
}
