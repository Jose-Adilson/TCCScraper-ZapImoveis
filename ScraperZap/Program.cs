using ConsoleApp1;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

ScraperZap.dbconnect con = new ScraperZap.dbconnect();

VivaReal();

void VivaReal ()
{
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
}

void ZapImoveis ()
{
    var driver = new ChromeDriver();
    List<Imovel> imoveis = new();
    for (int i = 1; i <= 1; i++)
    {
        var html = new HtmlDocument();


        driver.Navigate().GoToUrl($"https://www.zapimoveis.com.br/aluguel/imoveis/pr+curitiba/?onde=,Paraná,Curitiba,,,,,city,BR>Parana>NULL>Curitiba,-25.437238,-49.269973,&transacao=Aluguel&tipo=Imóvel%20usado&pagina={i}");
        html.LoadHtml(driver.PageSource);

        var lista = html.DocumentNode.SelectNodes("//*[@class='card-container js-listing-card']");

        foreach (var imovel in lista)
        {
            var id = imovel.GetAttributeValue<string>("data-id", String.Empty);
            var htmlImovel = new HtmlDocument();
            var actions = new Actions(driver);

            actions.MoveToElement(driver.FindElement(By.XPath($"//body/main[@id='app']/section[@class='results__section']/div[@class='results__wrapper']/div[@class='results__list js-results']/div[@class='listings__wrapper']/div[@class='listings__container']/div[1]/div[1]/div[2]/div[1]")));
            actions.Perform();
            driver.FindElement("xpath", $"//div[@data-id = '{id}']").Click();
            Thread.Sleep(1000);
            SwitchToWindow(x => x.Url.Contains(id), driver);
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
                    if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", String.Empty)))
                        images.Add(image.GetAttributeValue<string>("src", String.Empty));
                }
            }

            actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='report-listing__container container']")));
            actions.Perform();
            driver.FindElement(By.XPath("//article[@class='map__wrapper']/div[2]/button")).Click();
            Thread.Sleep(1000);
            htmlImovel.LoadHtml(driver.PageSource);
            var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//iframe[@class='map-embed__iframe']/@src").GetAttributeValue<string>("src", String.Empty);
            var desc = htmlImovel.DocumentNode.SelectSingleNode("//div[@class='amenities__description text-regular text-margin-zero']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//div[@class='amenities__description text-regular text-margin-zero']").InnerText : "";
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
    File.WriteAllText(@"C:\Users\Chron\Documents\ZapImoveis.json", imoveisJson);
}

void SwitchToWindow(Expression<Func<IWebDriver, bool>> predicateExp, ChromeDriver driver)
{
    var predicate = predicateExp.Compile();
    foreach (var handle in driver.WindowHandles)
    {
        driver.SwitchTo().Window(handle);
        if (predicate(driver))
        {
            return;
        }
    }

    throw new ArgumentException(string.Format("Unable to find window with condition: '{0}'", predicateExp.Body));
}
