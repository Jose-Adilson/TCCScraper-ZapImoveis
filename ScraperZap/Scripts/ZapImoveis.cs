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
    internal class ZapImoveis
    {
        public void ScriptZap(int i, int fim)
        {
            WindowSwitch change = new WindowSwitch();
            List<Imovel> imoveis = new();
            dbconnect con = new dbconnect();
            var driver = new ChromeDriver();
            while (i <= fim)
            {

                var html = new HtmlDocument();


                driver.Navigate().GoToUrl($"https://www.zapimoveis.com.br/aluguel/apartamentos/pr+curitiba/?onde=,Paran,Curitiba,,,,,city,BR>Parana>NULL>Curitiba,-25.437238,-49.269973,%2Faluguel%2Fimoveis%2Fpr%2Bcuritiba%2F&transacao=Aluguel&tipo=Imóvel%20usado&tipos=apartamento_residencial,studio_residencial,kitnet_residencial,casa_residencial,condominio_residencial,casa-vila_residencial,cobertura_residencial,flat_residencial,loft_residencial,lote-terreno_residencial,granja_residencial&pagina={i}");
            
                html.LoadHtml(driver.PageSource);

                var lista = html.DocumentNode.SelectNodes("//*[@class='card-container js-listing-card']");

                foreach (var imovel in lista)
                {
                    try
                    {
                        var id = imovel.GetAttributeValue<string>("data-id", string.Empty);
                        var htmlImovel = new HtmlDocument();
                        var actions = new Actions(driver);

                        actions.MoveToElement(driver.FindElement(By.XPath($"//body/main[@id='app']/section[@class='results__section']/div[@class='results__wrapper']/div[@class='results__list js-results']/div[@class='listings__wrapper']/div[@class='listings__container']/div[1]/div[1]/div[2]/div[1]")));
                        actions.Perform();
                        try
                        {
                            driver.FindElement("xpath", $"//div[@data-id = '{id}']").Click();
                        }
                        catch (Exception e)
                        {
                            driver.Navigate().Refresh();
                            continue;
                        }
                        Thread.Sleep(1000);
                        change.SwitchToWindow(x => x.Url.Contains(id), driver);
                        htmlImovel.LoadHtml(driver.PageSource);

                        var title = htmlImovel.DocumentNode.SelectSingleNode("//h1") != null ? htmlImovel.DocumentNode.SelectSingleNode("//h1").InnerText : "";
                        var address = htmlImovel.DocumentNode.SelectSingleNode("//span[@class='link']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//span[@class='link']").InnerText : "";
                        var price = "";
                        try
                        {
                            price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//ul[@class='main-prices']/li/strong").InnerText, @"[^\d\.\,]", "");
                        }
                        catch
                        {
                            price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//ul[@class='main-prices hybrid-business']/li[2]/strong").InnerText, @"[^\d\.\,]", "");
                        }
                        var rooms = htmlImovel.DocumentNode.SelectSingleNode("//span[@itemprop='numberOfRooms']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//span[@itemprop='numberOfRooms']").InnerText : "0";
                        List<string> images = new();

                        if (htmlImovel.DocumentNode.SelectNodes("//li[@class='js-carousel-item carousel__item']/img/@src") != null)
                        {
                            foreach (var image in htmlImovel.DocumentNode.SelectNodes("//li[@class='js-carousel-item carousel__item']/img/@src"))
                            {
                                if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", string.Empty)))
                                    images.Add(image.GetAttributeValue<string>("src", string.Empty));
                            }
                        }

                        var bairroId = htmlImovel.DocumentNode.SelectSingleNode("//nav[@class='breadcrumb__nav']/ol[1]/li[4]/a[1]") != null ? htmlImovel.DocumentNode.SelectSingleNode("//nav[@class='breadcrumb__nav']/ol[1]/li[4]/a[1]").InnerText : "";

                        actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='report-listing__container container']")));
                        actions.Perform();
                        //driver.FindElement(By.XPath("//article[@class='map__wrapper']/div[2]/button")).Click();
                        try
                        {
                            driver.FindElement(By.XPath("//button[contains(text(),'Explore o mapa')]")).Click();
                            Thread.Sleep(1000);
                            htmlImovel.LoadHtml(driver.PageSource);
                            var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//iframe[@class='map-embed__iframe']/@src").GetAttributeValue<string>("src", string.Empty);
                            var desc = htmlImovel.DocumentNode.SelectSingleNode("//div[@class='amenities__description text-regular text-margin-zero']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//div[@class='amenities__description text-regular text-margin-zero']").InnerText : "";
                            var matches = Regex.Matches(rooms, @"\d+");
                            var quarts = "";
                            var url = driver.Url;
                            foreach (var match in matches)
                            {
                                quarts += match;
                            }
                            imoveis.Add(new Imovel(id, title, address, price, quarts, desc, images, mapUrl, id, bairroId, url));
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles.Last());
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
