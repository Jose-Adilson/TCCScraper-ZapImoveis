using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ScraperZap.Shared
{
    internal class dbconnect
    {


        private MySqlConnection mConn;

        private MySqlDataAdapter mAdapter;

        private DataSet mDataSet;


        public void MainForm(Imovel imovel)

        {
            //define o dataset

            mDataSet = new DataSet();


            //define string de conexao e cria a conexao

            mConn = new MySqlConnection(" Persist Security Info=False;server=localhost;port=3307;database=tcc2;uid=root;server = localhost; database = tcc2; uid = root; pwd = root");


            try
            {

                //abre a conexao

                mConn.Open();

                string consulta = "SELECT externalId from Immobile WHERE externalId = " + imovel.externalId;
                MySqlCommand cmd1 = new MySqlCommand(consulta, mConn);
                var retorno = cmd1.ExecuteScalar();
                System.Diagnostics.Debug.WriteLine("Consulta - " + consulta);
                System.Diagnostics.Debug.WriteLine("Retorno - " + retorno);

                if (retorno == null)
                {
                    imovel.bairroId = imovel.bairroId.ToLower();
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([à-å])", "a");
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([ç])", "c");
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([è-ë])", "e");
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([ì-ï])", "i");
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([ò-ö])", "o");
                    imovel.bairroId = Regex.Replace(imovel.bairroId, "([ù-ü])", "u");
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    imovel.bairroId = textInfo.ToTitleCase(imovel.bairroId);

                    imovel.price = Regex.Replace(imovel.price, "[\\.]", "");
                    imovel.price = Regex.Replace(imovel.price, "[\\,]", ".");

                    string sql = "INSERT INTO Immobile (Title, Address, Price, Rooms, `Desc`, Images, Map, externalId, bairroId, site_url) WITH BairroNovo AS (" +
                        "SELECT '" + imovel.title + "' AS Title, '" + imovel.address + "' AS Address,  " + imovel.price + " AS Price, " + imovel.rooms + " AS Rooms, '" + imovel.desc + "' AS `Desc`, '" + JsonConvert.SerializeObject(imovel.images) + "' AS Images, '" + imovel.map + "' AS Map, '" + imovel.externalId + "' AS externalId, (SELECT Id FROM Bairro where Name like '" + imovel.bairroId.Trim() + "' ) AS bairroId, '" + imovel.siteUrl + "' AS site_url)" +
                        "SELECT Title, Address, Price, Rooms, `Desc`, Images, Map, externalId, bairroId, site_url FROM BairroNovo";
                    MySqlCommand cmd = new MySqlCommand(sql, mConn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string update = "SET @@session.time_zone='-03:00'; UPDATE Immobile SET webscraping_date = timestamp(current_timestamp()), in_use = true WHERE externalId = " + imovel.externalId;
                    MySqlCommand cmd2 = new MySqlCommand(update, mConn);
                    var retorno2 = cmd2.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Update - " + update);
                    System.Diagnostics.Debug.WriteLine("Retorno - " + retorno2);
                }

                
                mConn.Close();
            }
            
            catch (Exception e)

            {

                Console.WriteLine(e.Message);

            }

        }
    }
}
