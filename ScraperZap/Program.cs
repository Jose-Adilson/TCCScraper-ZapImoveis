using ConsoleApp1;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

List<Imovel> imoveis = new();
var driver = new ChromeDriver();

for (int i = 1; i <= 3; i++)
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

        actions.MoveToElement(driver.FindElement(By.XPath($"//div[@data-id = '{id}']")));
        actions.Perform();
        driver.FindElement("xpath", $"//div[@data-id = '{id}']").Click();
        SwitchToWindow(x => x.Url.Contains(id));
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
        var rooms = htmlImovel.DocumentNode.SelectSingleNode("//span[@itemprop='numberOfRooms']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//span[@itemprop='numberOfRooms']").InnerText : "";
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

        imoveis.Add(new Imovel(title, address, price, rooms, desc, images, mapUrl));
        driver.Close();
        driver.SwitchTo().Window(driver.WindowHandles.Last());
    }

}
var imoveisJson = JsonConvert.SerializeObject(imoveis);
File.WriteAllText(@"C:\Users\Chron\Documents\ZapImoveis.json", imoveisJson);

void SwitchToWindow(Expression<Func<IWebDriver, bool>> predicateExp)
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
