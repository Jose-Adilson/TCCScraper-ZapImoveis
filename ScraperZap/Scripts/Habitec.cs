using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using ScraperZap.Shared;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

namespace ScraperZap.Scripts
{
    internal class Habitec
    {
        public void ScriptHabitec(int i, int fim)
        {
            int cont;
            WindowSwitch change = new WindowSwitch();
            List<Imovel> imoveis = new();
            dbconnect con = new dbconnect();
            var driver = new ChromeDriver();
            while (i <= fim)
            {

                var html = new HtmlDocument();

                cont = 1;
                driver.Navigate().GoToUrl($"https://habitec.com.br/alugar-imovel/?cidade=Curitiba%2C%20PR&tipo_imovel=Apartamento&pg={i}");
                driver.Manage().Window.Maximize();
                Thread.Sleep(3000);
                html.LoadHtml(driver.PageSource);

                var lista = html.DocumentNode.SelectNodes("//*[@class='box-pesquisa']");

                foreach (var imovel in lista)
                {
                    try
                    {
                        var id = imovel.GetAttributeValue<string>("data-imovel", string.Empty);
                        var htmlImovel = new HtmlDocument();
                        var actions = new Actions(driver);
                        System.Diagnostics.Debug.WriteLine("ID - " + id);

                        //actions.MoveToElement(driver.FindElement(By.XPath($"(//article[@class='property-card__container js-property-card'])[2]")));
                        //actions.Perform();
                        var title = html.DocumentNode.SelectSingleNode($"(//article[@data-imovel]//div[@class='box-descri']//div//div//div//div//h2//a)[{cont}]") != null ? html.DocumentNode.SelectSingleNode($"(//article[@data-imovel]//div[@class='box-descri']//div//div//div//div//h2//a)[{cont}]").InnerText : "";
                        cont++;
                        System.Diagnostics.Debug.WriteLine("Title - " + title);
                        System.Diagnostics.Debug.WriteLine("Cont - " + cont);
                        try
                        {
                            driver.FindElement("xpath", $"//article[@data-imovel='{id}']").Click();
                        }
                        catch (Exception e)
                        {
                            driver.Navigate().Refresh();
                            continue;
                        }
                        Thread.Sleep(1000);
                        change.SwitchToWindow(x => x.Url.Contains(id), driver);
                        htmlImovel.LoadHtml(driver.PageSource);

                        //var title = htmlImovel.DocumentNode.SelectSingleNode("//div[@class='box-descri']//h2") != null ? htmlImovel.DocumentNode.SelectSingleNode("//div[@class='box-descri']//h2").InnerText : "";
                        var address = htmlImovel.DocumentNode.SelectSingleNode("//div[@class='box-descri']//p") != null ? htmlImovel.DocumentNode.SelectSingleNode("//div[@class='box-descri']//p").InnerText : "";
                        var price = "";
                        price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//div[@class='box-descri']//h3").InnerText, @"[^\d\.\,]", "");

                        var rooms = htmlImovel.DocumentNode.SelectSingleNode("(//span[@class='num'])[1]") != null ? htmlImovel.DocumentNode.SelectSingleNode("(//span[@class='num'])[1]").InnerText : "0";
                        List<string> images = new();

                        if (htmlImovel.DocumentNode.SelectNodes("//div[@role='listbox']//div//img[@src]") != null)
                        {
                            foreach (var image in htmlImovel.DocumentNode.SelectNodes("//div[@role='listbox']//div//img[@src]"))
                            {
                                if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", string.Empty)))
                                    images.Add(image.GetAttributeValue<string>("src", string.Empty));
                            }
                        }
                        //var bairroId = htmlImovel.DocumentNode.SelectSingleNode("//*[@id=\"js-site-main\"]/div[2]/div[1]/div[1]/div/ol/li[3]/ol/li[3]/a") != null ? htmlImovel.DocumentNode.SelectSingleNode("//*[@id=\"js-site-main\"]/div[2]/div[1]/div[1]/div/ol/li[3]/ol/li[3]/a").InnerText : "0";
                        var bairroId = Regex.Replace(title, @"(?=[\|]).*", "");
                        System.Diagnostics.Debug.WriteLine("Bairro - " + bairroId);

                        //actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='js-fullscreen-lead-vue fullsized-lead vue-lead-form']")));
                        //actions.Perform();
                        try
                        {
                            driver.FindElement(By.XPath("//button[@id='btn-mapa']")).Click();
                            Thread.Sleep(1000);
                            htmlImovel.LoadHtml(driver.PageSource);
                            var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//a[@title='Abrir esta área no Google Maps (abre uma nova janela)']").GetAttributeValue<string>("href", string.Empty);
                            var desc = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='p-mais-descri']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='p-mais-descri']").InnerText : "";
                            var url = driver.Url;
                            imoveis.Add(new Imovel(id, title, address, price, rooms, desc, images, mapUrl, id, bairroId, url));                            
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.Last());

                            System.Diagnostics.Debug.WriteLine("MapURL - " + mapUrl);
                            System.Diagnostics.Debug.WriteLine("desc - " + desc);
                        }
                        catch (Exception e)
                        {
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.Last());
                        }

                        
                    }
                    catch (NullReferenceException e)
                    {
                        driver.Close();
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                        continue;
                    }                    
                }

                foreach (var imovel in imoveis)
                {
                    con.MainForm(imovel);
                }
                i++;                

            }
            driver.Close();
        }
    }
}
