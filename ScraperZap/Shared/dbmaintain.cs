using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ScraperZap.Shared
{
    internal class dbmaintain
    {
        private MySqlConnection mConn;

        private MySqlDataAdapter mAdapter;

        private DataSet mDataSet;


        public void MainForm()

        {            
            //define o dataset
            mDataSet = new DataSet();
            //define string de conexao e cria a conexao
            mConn = new MySqlConnection(" Persist Security Info=False;server=localhost;port=3307;database=tcc2;uid=root;server = localhost; database = tcc2; uid = root; pwd = root");

            try
            {
                //abre a conexao
                mConn.Open();
            
                string consulta = "UPDATE Immobile SET in_use = false WHERE DATE(webscraping_date) < DATE(timestamp(current_timestamp()))";
                MySqlCommand cmd = new MySqlCommand(consulta, mConn);
                cmd.ExecuteNonQuery();
                mConn.Close();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            
            
        }
    }
}
