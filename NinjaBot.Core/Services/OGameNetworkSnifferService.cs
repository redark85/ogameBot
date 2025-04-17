using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V135;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaBot.Core.Services
{
    public class OGameNetworkSnifferService
    {
        public async Task StartSniffing()
        {
            var options = new ChromeOptions();
            options.AddArgument("--auto-open-devtools-for-tabs");
            var driver = new ChromeDriver(options);

            var devTools = driver.GetDevToolsSession();
            var network = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V135.DevToolsSessionDomains>().Network;

            await network.Enable(new OpenQA.Selenium.DevTools.V135.Network.EnableCommandSettings());

            network.RequestWillBeSent += async (sender, e) =>
            {
                var request = e.Request;

                // Detectamos la URL de login
                if (request.Url.Contains("https://gameforge.com/api/v1/auth/thin/sessions"))
                {
                    Console.WriteLine("📡 Interceptada solicitud de login:");
                    Console.WriteLine($"URL: {request.Url}");
                    Console.WriteLine("Payload:");
                    Console.WriteLine(request.PostData); // Acá está el JSON con email, password, etc
                }
            };

            driver.Navigate().GoToUrl("https://lobby.ogame.gameforge.com/");

            Console.WriteLine("🧭 Navegador abierto, hacé el login normalmente...");

            // Quedarse abierto para espiar
            Console.ReadLine();

            driver.Quit();
        }
    }
}
