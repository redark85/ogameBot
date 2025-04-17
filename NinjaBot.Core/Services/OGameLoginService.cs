using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools;
using System.Text.Json;
using System.Text;
using NinjaBot.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NinjaBot.Shared.Dtos.Requests;
using NinjaBot.Domain.Interfaces.Services;
using NinjaBot.Domain.Entities;
using SeleniumExtras.WaitHelpers;
using NinjaBot.Domain.Dtos;

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
        private static readonly HttpClient client = new HttpClient();
        public OGameLoginService(
            IWebDriver driver,
            IServiceScopeFactory serviceScopeFactory)
        {
            _driver = driver;
            _serviceScopeFactory = serviceScopeFactory;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        public async Task<bool> LoginWithCredentialsAsync(LoginRequestDto dto)
        {
            _devTools = _driver as IDevTools;
            _session = _devTools?.GetDevToolsSession();

            if (_session == null)
            {
                throw new InvalidOperationException("No se pudo inicializar Chrome DevTools");
            }
            var networkDomain = _session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V135.DevToolsSessionDomains>().Fetch;

            await networkDomain.Enable(new OpenQA.Selenium.DevTools.V135.Fetch.EnableCommandSettings());

            do
            {                
                networkDomain.RequestPaused += async (sender, e) =>
                {                    
                    if (e.Request.Url.Contains("/api/v1/auth/thin/sessions") &&
                        e.Request.Method == "POST")
                    {
                        Console.WriteLine($"Response: {e}");

                        try
                        {
                            if (!string.IsNullOrEmpty(e.Request.PostData))
                            {
                                _interceptedPayload = JsonSerializer.Deserialize<OGameLoginPayload>(e.Request.PostData);
                                Console.WriteLine($"Intercepted and parsed payload: {JsonSerializer.Serialize(_interceptedPayload, new JsonSerializerOptions { WriteIndented = true })}");
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing payload: {ex.Message}");
                        }
                    }                    
                    try
                    {
                        await networkDomain.ContinueRequest(new OpenQA.Selenium.DevTools.V135.Fetch.ContinueRequestCommandSettings
                        {
                            RequestId = e.RequestId
                        });
                    }
                    catch (Exception)
                    {

                        
                    }
                   
                };

                if (_interceptedPayload == null)
                {
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
                }
            }
            while (_interceptedPayload == null);

            if (_interceptedPayload != null)
            {
               return await Login(_interceptedPayload!, dto.Universe);
            }
            return false;
            
        }

        public async Task<bool> CreateUser(OGameLoginPayload dto, string password, string universe, string token)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dataService = scope.ServiceProvider.GetRequiredService<IAppDataService>();
                
                var user = new User
                {
                    Blackbox = dto.Blackbox,
                    Email = dto.Identity,
                    Language = dto.Language,
                    Password = dto.Password,
                    Token = token,
                    Universe = universe,
                    Locale = dto.Locale
                };

                dataService.User.Add(user);
                await dataService.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> SolveCaptchaAsync(string challengeId, string locale)
        {
            var baseUrl = $"https://image-drop-challenge.gameforge.com/challenge/{challengeId}/{locale}";

            // 1. GET
            var getResponse = await client.GetAsync(baseUrl);
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var getJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(getContent);

            if (!getJson.ContainsKey("status") || getJson["status"].ToString() != "presented")
            {
                throw new Exception("El captcha no está disponible para resolver.");
            }

            // 2. POST con respuesta "0"
            var postData = new { answer = 0 };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            var postContent = new StringContent(json, Encoding.UTF8, "application/json");

            var postResponse = await client.PostAsync(baseUrl, postContent);
            var postString = await postResponse.Content.ReadAsStringAsync();

            var postJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(postString);

            if (postJson.ContainsKey("status") && postJson["status"].ToString() == "solved")
            {
                Console.WriteLine("Captcha resuelto correctamente.");
                return true;
            }
            else
            {
                Console.WriteLine("Reintentando captcha...");
                return await SolveCaptchaAsync(challengeId, locale); // Recursividad
            }
        }

        public async Task<bool> Login(OGameLoginPayload dto, string universe)
        {
            var loginData = new
            {
                blackbox = dto.Blackbox,
                gameEnvironmentId = dto.GameEnvironmentId,
                gfLang = dto.Language,
                identity = dto.Identity,
                locale = dto.Locale,
                password = dto.Password,
                platformGameId = dto.PlatformGameId,
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // POST request
            HttpResponseMessage response = await client.PostAsync("https://gameforge.com/api/v1/auth/thin/sessions", content);
            Thread.Sleep(1000);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                TokenResponse loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(responseString)!;
                return await CreateUser(dto, dto.Password, universe, loginResponse.Token);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var challengeId = response.Headers.GetValues("gf-challenge-id").FirstOrDefault();
                if (await SolveCaptchaAsync(challengeId!.Split(';')[0], dto.Locale))
                {
                    return await Login(dto, universe);
                }
                ;
                return false;

            }
            return false;
        }

        public void Dispose()
        {
        }
    }
}
