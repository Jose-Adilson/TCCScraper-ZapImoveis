using System.Data;
using ConsoleApp1;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ScraperZap
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

                string sql = "INSERT INTO Immobile (Title, Address, Price, Rooms, `Desc`, Images, Map) VALUES ('"+ imovel.title + "', '" + imovel.address + "', " + imovel.price + ", " + imovel.rooms + ", '" + imovel.desc + "', '" + JsonConvert.SerializeObject(imovel.images) + "', '" + imovel.map + "')";
                MySqlCommand cmd = new MySqlCommand(sql, mConn);
                cmd.ExecuteNonQuery();

            }

            catch (System.Exception e)

            {

                Console.WriteLine(e.Message);

            }

        }
    }
}
