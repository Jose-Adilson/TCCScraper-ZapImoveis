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
    internal class VivaReal
    {
        public void ScriptViva()
        {
            int i = 1;
            WindowSwitch change = new WindowSwitch();
            List<Imovel> imoveis = new();
            dbconnect con = new dbconnect();
            var driver = new ChromeDriver();
            while (true)
            {

                var html = new HtmlDocument();


                driver.Navigate().GoToUrl($"https://www.vivareal.com.br/aluguel/parana/curitiba/apartamento_residencial/?pagina={i}#ordenar-por=preco-total:ASC&preco-ate=7000&tipos=apartamento_residencial,casa_residencial,condominio_residencial,kitnet_residencial,sobrado_residencial,");
                html.LoadHtml(driver.PageSource);

                var lista = html.DocumentNode.SelectNodes("//div[@class='results-list js-results-list']//div[@id]");

                foreach (var imovel in lista)
                {
                    try
                    {
                        var id = imovel.GetAttributeValue<string>("id", string.Empty);
                        var htmlImovel = new HtmlDocument();
                        var actions = new Actions(driver);

                        actions.MoveToElement(driver.FindElement(By.XPath($"(//article[@class='property-card__container js-property-card'])[2]")));
                        actions.Perform();
                        try
                        {
                            driver.FindElement("xpath", $"//div[@id='{id}']").Click();
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
                        var address = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='title__address js-address']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='title__address js-address']").InnerText : "";
                        var price = "";
                        price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//h3[@class='price__price-info js-price-sale']").InnerText, @"[^\d\.\,]", "");

                        var rooms = htmlImovel.DocumentNode.SelectSingleNode("//li[@title='Quartos']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//li[@title='Quartos']").InnerText : "0";
                        List<string> images = new();

                        if (htmlImovel.DocumentNode.SelectNodes("//li[@class='carousel__slide js-carousel-item-wrapper']/img/@src") != null)
                        {
                            foreach (var image in htmlImovel.DocumentNode.SelectNodes("//li[@class='carousel__slide js-carousel-item-wrapper']/img/@src"))
                            {
                                if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", string.Empty)))
                                    images.Add(image.GetAttributeValue<string>("src", string.Empty));
                            }
                        }
                        var bairroId = htmlImovel.DocumentNode.SelectSingleNode("//*[@id=\"js-site-main\"]/div[2]/div[1]/div[1]/div/ol/li[3]/ol/li[3]/a") != null ? htmlImovel.DocumentNode.SelectSingleNode("//*[@id=\"js-site-main\"]/div[2]/div[1]/div[1]/div/ol/li[3]/ol/li[3]/a").InnerText : "0";

                        actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='js-fullscreen-lead-vue fullsized-lead vue-lead-form']")));
                        actions.Perform();
                        try
                        {
                            driver.FindElement(By.XPath("//button[normalize-space()='Navegue pela região']")).Click();
                            Thread.Sleep(1000);
                            htmlImovel.LoadHtml(driver.PageSource);
                            var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//iframe[@class='embed__iframe']").GetAttributeValue<string>("src", string.Empty);
                            var desc = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='description__text']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='description__text']").InnerText : "";
                            var matches = Regex.Matches(rooms, @"\d+");
                            var quarts = "";
                            foreach (var match in matches)
                            {
                                quarts += match;
                            }
                            imoveis.Add(new Imovel(id, title, address, price, quarts, desc, images, mapUrl, id, bairroId));
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
        }
        /*
        void VivaReal ()
{
    dbconnect con = new dbconnect();
    var driver = new ChromeDriver();
    List<Imovel> imoveis = new();
    for (int i = 1; i <= 2; i++)
    {
        HtmlDocument html = new HtmlDocument();

        driver.Navigate().GoToUrl($"https://www.vivareal.com.br/aluguel/parana/curitiba/apartamento_residencial/?pagina={i}#ordenar-por=preco-total:ASC&preco-ate=7000&tipos=apartamento_residencial,casa_residencial,condominio_residencial,kitnet_residencial,sobrado_residencial,");
        html.LoadHtml(driver.PageSource);

        var lista = html.DocumentNode.SelectNodes("//div[@class='results-list js-results-list']//div[@id]");

        foreach (var imovel in lista)
        {
            var id = imovel.GetAttributeValue<string>("id", String.Empty);
            var htmlImovel = new HtmlDocument();
            var actions = new Actions(driver);

            actions.MoveToElement(driver.FindElement(By.XPath($"(//article[@class='property-card__container js-property-card'])[2]")));
            actions.Perform();
            driver.FindElement("xpath", $"//div[@id='{id}']").Click();
            Thread.Sleep(1000);
            SwitchToWindow(x => x.Url.Contains(id), driver);
            htmlImovel.LoadHtml(driver.PageSource);

            var title = htmlImovel.DocumentNode.SelectSingleNode("//h1") != null ? htmlImovel.DocumentNode.SelectSingleNode("//h1").InnerText : "";
            var address = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='title__address js-address']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='title__address js-address']").InnerText : "";
            var price = "";
            price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//h3[@class='price__price-info js-price-sale']").InnerText, @"[^\d\.\,]", "");
            
            var rooms = htmlImovel.DocumentNode.SelectSingleNode("//li[@title='Quartos']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//li[@title='Quartos']").InnerText : "0";
            List<string> images = new();

            if (htmlImovel.DocumentNode.SelectNodes("//li[@class='carousel__slide js-carousel-item-wrapper']/img/@src") != null)
            {
                foreach (var image in htmlImovel.DocumentNode.SelectNodes("//li[@class='carousel__slide js-carousel-item-wrapper']/img/@src"))
                {
                    if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", String.Empty)))
                        images.Add(image.GetAttributeValue<string>("src", String.Empty));
                }
            }

            actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='js-fullscreen-lead-vue fullsized-lead vue-lead-form']")));
            actions.Perform();
            driver.FindElement(By.XPath("//button[normalize-space()='Navegue pela região']")).Click();
            Thread.Sleep(1000);
            htmlImovel.LoadHtml(driver.PageSource);
            var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//iframe[@class='embed__iframe']").GetAttributeValue<string>("src", String.Empty);
            var desc = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='description__text']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='description__text']").InnerText : "";
            var matches = Regex.Matches(rooms, @"\d+");
            var quarts = "";
            foreach (var match in matches)
            {
                quarts += match;
            }
            imoveis.Add(new Imovel(id, title, address, price, quarts, desc, images, mapUrl));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }

    }

    foreach (var imovel in imoveis)
    {
        con.MainForm(imovel);
    }
    var imoveisJson = JsonConvert.SerializeObject(imoveis); 
    File.WriteAllText(@"C:\Users\Chron\Documents\VivaReal.json", imoveisJson);
}*/
    }
}
