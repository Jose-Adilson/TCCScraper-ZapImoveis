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
	internal class KondorImoveis
	{
		public void ScriptKondor(int fim)
		{
			int i = 1;
			int cont;
			WindowSwitch change = new WindowSwitch();
			List<Imovel> imoveis = new();
			dbconnect con = new dbconnect();
			var driver = new ChromeDriver();
            driver.Navigate().GoToUrl($"https://www.kondorimoveis.com.br/alugar/Apartamento/todos-bairros/Curitiba/?typeBussiness=2&tipo_busca=0&endereco=&cidades=3314&bairros%5B%5D=155&bairros%5B%5D=106&bairros%5B%5D=77&bairros%5B%5D=58&bairros%5B%5D=16&bairros%5B%5D=91&bairros%5B%5D=18&bairros%5B%5D=90&bairros%5B%5D=57&bairros%5B%5D=114&bairros%5B%5D=69&bairros%5B%5D=15&bairros%5B%5D=13&bairros%5B%5D=88&bairros%5B%5D=76&bairros%5B%5D=19&bairros%5B%5D=124&bairros%5B%5D=60&bairros%5B%5D=138&bairros%5B%5D=80&bairros%5B%5D=42&bairros%5B%5D=3&bairros%5B%5D=52&bairros%5B%5D=1038&bairros%5B%5D=104&bairros%5B%5D=70&bairros%5B%5D=68&bairros%5B%5D=1267&bairros%5B%5D=1&bairros%5B%5D=20&bairros%5B%5D=1268&bairros%5B%5D=47&bairros%5B%5D=26&bairros%5B%5D=22&bairros%5B%5D=610&bairros%5B%5D=31&bairros%5B%5D=1276&bairros%5B%5D=34&bairros%5B%5D=9&bairros%5B%5D=1258&bairros%5B%5D=782&bairros%5B%5D=8&bairros%5B%5D=141&bairros%5B%5D=33&bairros%5B%5D=28&bairros%5B%5D=123&bairros%5B%5D=86&bairros%5B%5D=97&bairros%5B%5D=1242&bairros%5B%5D=1216&bairros%5B%5D=12&bairros%5B%5D=1252&bairros%5B%5D=75&bairros%5B%5D=79&bairros%5B%5D=50&bairros%5B%5D=63&bairros%5B%5D=2&bairros%5B%5D=45&bairros%5B%5D=4&bairros%5B%5D=118&bairros%5B%5D=23&bairros%5B%5D=94&bairros%5B%5D=14&bairros%5B%5D=103&bairros%5B%5D=82&bairros%5B%5D=274&bairros%5B%5D=61&bairros%5B%5D=38&bairros%5B%5D=161&bairros%5B%5D=73&bairros%5B%5D=54&bairros%5B%5D=965&bairros%5B%5D=21&bairros%5B%5D=144&bairros%5B%5D=84&bairros%5B%5D=44&bairros%5B%5D=72&bairros%5B%5D=66&bairros%5B%5D=98&bairros%5B%5D=78&bairros%5B%5D=95&bairros%5B%5D=10&bairros%5B%5D=1019&bairros%5B%5D=1032&bairros%5B%5D=51&bairros%5B%5D=7&bairros%5B%5D=24&tipoImovel%5B%5D=res_3&tipoImovel%5B%5D=res_1&tipoImovel%5B%5D=res_15&tipoImovel%5B%5D=res_18&precoMin=0&precoMax=999999999&numeroQuartos=&numeroSuites=&numeroVagas=#");
            driver.Manage().Window.Maximize();
            while (i <= fim)
			{
                cont = 1;
                var html = new HtmlDocument();
                Thread.Sleep(3000);
                html.LoadHtml(driver.PageSource);

				var lista = html.DocumentNode.SelectNodes("//div[@id='listagem_cards']//div[@codigo-imovel]");

				foreach (var imovel in lista)
				{
					try
					{
						var id = imovel.GetAttributeValue<string>("codigo-imovel", string.Empty);	
						var refe = Regex.Replace(html.DocumentNode.SelectSingleNode($"(//p[@class='referencia text-truncate']//span)[{cont}]") != null ? html.DocumentNode.SelectSingleNode($"(//p[@class='referencia text-truncate']//span)[{cont}]").InnerText : "", @"[\\.]", "").ToLower().TrimStart();
						refe = Regex.Replace(refe, @"\s", "-");
                        var htmlImovel = new HtmlDocument();
						var actions = new Actions(driver);
						System.Diagnostics.Debug.WriteLine("ID - " + id);
                        System.Diagnostics.Debug.WriteLine("Referencia - " + refe);
                        System.Diagnostics.Debug.WriteLine("Cont - " + cont);
                        
                        try
						{
							//actions.MoveToElement(driver.FindElement(By.XPath($"//*[@id='listagem_cards']/div[{cont+1}]/following-sibling::div")));                            
							//actions.Perform();
							//Thread.Sleep(4000);
							//driver.FindElement("xpath", $"(//a[contains(text(),'Detalhes')])[{cont}]").Click();                            
							var e = driver.FindElement("xpath", $"//*[@codigoimovel='{id}']");
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", e);
                            Thread.Sleep(500);
                            driver.FindElement("xpath", $"(//a[contains(text(),'Detalhes')])[{cont}]").Click();
                            //driver.FindElement("xpath", $"//*[@codigoimovel='{id}']/following-sibling::a").Click();                            
                        }
						catch (Exception e)
						{
							driver.Navigate().Refresh();
                            System.Diagnostics.Debug.WriteLine(e);
                            continue;
						}                        
                        cont++;
						Thread.Sleep(1000);
						change.SwitchToWindow(x => x.Url.Contains(refe), driver);
						htmlImovel.LoadHtml(driver.PageSource);

						
						var title = htmlImovel.DocumentNode.SelectSingleNode("//h1[@class='title']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//h1[@class='title']").InnerText : "";
						var address = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("(//p[@class='descri'])[1]") != null ? htmlImovel.DocumentNode.SelectSingleNode("(//p[@class='descri'])[1]").InnerText : "", @".*[\\:\n]", "");
                        System.Diagnostics.Debug.WriteLine("Title - " + title);
                        System.Diagnostics.Debug.WriteLine("Addres - " + address);
                        var price = "";
						try
						{
							price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//p[@class='preco text-truncate']").InnerText, @"[^\d\.\,]", "");
						}
						catch
						{
							price = Regex.Replace(htmlImovel.DocumentNode.SelectSingleNode("//p[@class='total text-truncate']").InnerText, @"[^\d\.\,]", "");
						}
                        System.Diagnostics.Debug.WriteLine("Price - " + price);
                        var rooms = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='quartos text-truncate']//span") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='quartos text-truncate']//span").InnerText : "0";
                        System.Diagnostics.Debug.WriteLine("Rooms - " + rooms);
                        List<string> images = new();

						if (htmlImovel.DocumentNode.SelectNodes("//div[@class='images']//img") != null)
						{
							foreach (var image in htmlImovel.DocumentNode.SelectNodes("//div[@class='images']//img"))
							{
								if (!string.IsNullOrEmpty(image.GetAttributeValue<string>("src", string.Empty)))
									images.Add(image.GetAttributeValue<string>("src", string.Empty));
							}
						}

                        var bairroId = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='tipoBairro']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='tipoBairro']").InnerText : "";
                        System.Diagnostics.Debug.WriteLine("Bairro - " + bairroId);

                        //actions.MoveToElement(driver.FindElement(By.XPath("//div[@class='report-listing__container container']")));
                        //actions.Perform();
                        //driver.FindElement(By.XPath("//article[@class='map__wrapper']/div[2]/button")).Click();
                        try
						{
							driver.FindElement(By.XPath("//label[normalize-space()='Mapa']")).Click();
							Thread.Sleep(2000);
							htmlImovel.LoadHtml(driver.PageSource);
							var mapUrl = htmlImovel.DocumentNode.SelectSingleNode("//iframe[@id='iframeMapa']").GetAttributeValue<string>("src", string.Empty);
                            System.Diagnostics.Debug.WriteLine("mapUrl - " + mapUrl);
                            var desc = htmlImovel.DocumentNode.SelectSingleNode("//p[@class='texto']") != null ? htmlImovel.DocumentNode.SelectSingleNode("//p[@class='texto']").InnerText : "";
                            System.Diagnostics.Debug.WriteLine("desc - " + desc);
                            var url = driver.Url;
                            imoveis.Add(new Imovel(id, title, address, price, rooms, desc, images, mapUrl, id, bairroId, url));
							driver.Close();
							driver.SwitchTo().Window(driver.WindowHandles.Last());
                            Thread.Sleep(1000);
                        }
						catch (Exception e)
						{
							driver.Close();
							driver.SwitchTo().Window(driver.WindowHandles.Last());
                            //Thread.Sleep(3000);
                        }


					}
					catch (NullReferenceException e)
					{
						driver.Close();
						driver.SwitchTo().Window(driver.WindowHandles.Last());
                        Thread.Sleep(3000);
                        continue;
					}

				}

				foreach (var imovel in imoveis)
				{
					con.MainForm(imovel);
				}
				i++;
                driver.FindElement(By.XPath("//a[@onclick='nextPage()']")).Click();
                Thread.Sleep(3000);


            }
            driver.Close();
        }
	}
}
