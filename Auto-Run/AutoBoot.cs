using Stressless_Service.Database;
using Stressless_Service.Models;
using System.Diagnostics;

namespace Stressless_Service.Auto_Run
{
    public class AutoBoot : IDisposable
    {
        private static DateTime LastSynced;
        private async Task<bool> CheckTime(bool IsWorkingTime = false)
        {
            // App is crashing here...
            // Check if configuration exists first? 

            using (database database = new database()) 
            {
                ConfigurationModel configuration = await database.GetConfiguration();

                if (!string.IsNullOrEmpty(configuration.day_Start) && !string.IsNullOrEmpty(configuration.day_End))
                {
                    DateTime[] Times = await database.GetShift();

                    // Check if the current system time is within the range of the users specified shift pattern
                    if (System.DateTime.Now >= Times[0] && System.DateTime.Now <= Times[1])
                    {
                        IsWorkingTime = true;
                    }
                }
            }

            return IsWorkingTime;
        }

        public async Task initializeFront()
        {
            if (await CheckTime()) // True
            {
                if (Process.GetProcessesByName("notepad.exe").Length > 0) {
                    // Process already exists... Do nothing...

                    using (AutoBootTimer abTimer = new AutoBootTimer()) {
                        await abTimer.StartABTimer();
                    }
                }
                else {
                //True: If program was last booted + 2 hour ago...

                    if (await CheckLastBoot())
                    {
                        // BOOT: Process.Start("APP_NAME");

                        LastSynced = DateTime.Now;

                        using (AutoBootTimer abTimer = new AutoBootTimer())
                        {
                            await abTimer.StartABTimer();
                        }
                    }
                }

            }
        }

        private async Task<bool> CheckLastBoot(bool IsLater = false)
        {
            TimeSpan Difference = System.DateTime.Now - LastSynced;

            if (Difference.TotalHours >= 2) {
                IsLater = true;
            }

            return IsLater;
        }

        public void Dispose() => GC.Collect();
    }
}
