using System.Data;
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
                
                string sql = "INSERT INTO Immobile (Title, Address, Price, Rooms, `Desc`, Images, Map, externalId, bairroId) WITH BairroNovo AS (" +
                    "SELECT '" + imovel.title + "' AS Title, '" + imovel.address + "' AS Address,  " + imovel.price + " AS Price, " + imovel.rooms + " AS Rooms, '" + imovel.desc + "' AS `Desc`, '" + JsonConvert.SerializeObject(imovel.images) + "' AS Images, '" + imovel.map + "' AS Map, '" + imovel.externalId + "' AS externalId, (SELECT Id FROM Bairro where Name like '" + imovel.bairroId.Trim() +"') AS bairroId)" +
                    "SELECT Title, Address, Price, Rooms, `Desc`, Images, Map, externalId, bairroId FROM BairroNovo";
                    

                    //"VALUES ('" + imovel.title + "', '" + imovel.address + "', " + imovel.price + ", " + imovel.rooms + ", '" + imovel.desc + "', '" + JsonConvert.SerializeObject(imovel.images) + "', '" + imovel.map + "', '" + imovel.externalId + "', (SELECT Id FROM Bairro where Name like '" + imovel.bairroId + "'))";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
                cmd.ExecuteNonQuery();

            }

            catch (Exception e)

            {

                Console.WriteLine(e.Message);

            }

        }
    }
}
