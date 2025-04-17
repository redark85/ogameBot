using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V135.Fetch;
using System.Text.Json;
using System.Text;
using NinjaBot.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NinjaBot.Shared.Dtos.Requests;
using NinjaBot.Domain.Interfaces.Services;
using NinjaBot.Domain.Entities;
using SeleniumExtras.WaitHelpers;

namespace NinjaBot.Core.Services
{
    public class OGameLoginService : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IDevTools? _devTools;
        private DevToolsSession? _session;
        private OGameLoginPayload? _interceptedPayload;

        public OGameLoginService(
            IWebDriver driver,
            IServiceScopeFactory serviceScopeFactory)
        {
            _driver = driver;
            _serviceScopeFactory = serviceScopeFactory;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        public async Task<OGameLoginPayload?> LoginWithCredentialsAsync(LoginRequestDto dto)
        {
            _devTools = _driver as IDevTools;
            _session = _devTools?.GetDevToolsSession();

            if (_session == null)
            {
                throw new InvalidOperationException("No se pudo inicializar Chrome DevTools");
            }
            var networkDomain = _session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V135.DevToolsSessionDomains>().Fetch;

            await networkDomain.Enable(new OpenQA.Selenium.DevTools.V135.Fetch.EnableCommandSettings());

            networkDomain.RequestPaused += async (sender, e) =>
            {
                Console.WriteLine($"Request: {e.Request.Url}");

                if (e.Request.Url.Contains("/api/v1/auth/thin/sessions") &&
                    e.Request.Method == "POST")
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(e.Request.PostData))
                        {
                            _interceptedPayload = JsonSerializer.Deserialize<OGameLoginPayload>(e.Request.PostData);
                            Console.WriteLine($"Intercepted and parsed payload: {JsonSerializer.Serialize(_interceptedPayload, new JsonSerializerOptions { WriteIndented = true })}");
                            if (_interceptedPayload != null)
                            {
                                await CreateUser(_interceptedPayload, dto);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing payload: {ex.Message}");
                    }
                }
                await networkDomain.ContinueRequest(new OpenQA.Selenium.DevTools.V135.Fetch.ContinueRequestCommandSettings
                {
                    RequestId = e.RequestId
                });
            };

            _driver.Navigate().GoToUrl("https://lobby.ogame.gameforge.com/es_ES");

            // Esperar contenedor de login
            var loginContainer = _wait.Until(
                ExpectedConditions.ElementIsVisible(By.ClassName("loginRegister"))
            );

            // Tabs container
            Console.WriteLine("Looking for tabs container...");
            var tabsContainer = loginContainer.FindElement(By.ClassName("tabs"));

            // Buscar los tabs
            Console.WriteLine("Looking for login tab...");
            var loginTab = tabsContainer.FindElement(By.XPath(".//li[contains(text(), 'Iniciar')]"));
            var registerTab = tabsContainer.FindElement(By.XPath(".//li[contains(text(), 'Registrarse')]"));

            // Cambiar a tab de login si está activo el de registro
            if (registerTab.GetAttribute("class")!.Contains("active"))
            {
                Console.WriteLine("Clicking login tab...");
                loginTab.Click();
                Thread.Sleep(1000); // Esperar por la animación
            }

            // Esperar por el formulario de login
            Console.WriteLine("Waiting for login form...");
            var loginForm = _wait.Until(
                ExpectedConditions.ElementIsVisible(By.Id("loginForm"))
            );

            // Campos de email y password
            Console.WriteLine("Finding form fields...");
            var emailWrapper = loginForm.FindElement(By.ClassName("inputWrap"));
            var emailInput = emailWrapper.FindElement(By.Name("email"));
            var passwordWrapper = emailWrapper.FindElement(By.XPath("following-sibling::div[@class='inputWrap']"));
            var passwordInput = passwordWrapper.FindElement(By.Name("password"));

            // Llenar formulario
            Console.WriteLine("Filling login form...");
            emailInput.Clear();
            emailInput.SendKeys(dto.Email);
            passwordInput.Clear();
            passwordInput.SendKeys(dto.Password);

            // Botón de login
            Console.WriteLine("Waiting for submit button to be clickable...");
            var submitButton = _wait.Until(
                ExpectedConditions.ElementToBeClickable(By.CssSelector("button.button-primary.button-lg[type='submit']"))
            );

            // Scroll y clic con JS
            Console.WriteLine("Scrolling to button...");
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            Thread.Sleep(5000);

            Console.WriteLine("Clicking submit button using JavaScript...");
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);
            Thread.Sleep(2000);



            return _interceptedPayload;

        }

        public async Task<bool> CreateUser(OGameLoginPayload dto, LoginRequestDto userData)
        {
            // Crear un nuevo scope para el DbContext
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
                
                var user = new User
                {
                    Blackbox = dto.Blackbox,
                    Email = userData.Email,
                    Language = userData.Language,
                    Password = userData.Password,
                    Token = "",
                    Universe = userData.Universe
                };

                dataService.User.Add(user);
                await dataService.SaveChangesAsync();
                return true;
            }
        }
        public void Dispose()
        {
        }
    }
}
