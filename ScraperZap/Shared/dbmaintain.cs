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


        public void MainForm(Imovel imovel)

        {
            //define o dataset
            mDataSet = new DataSet();
            //define string de conexao e cria a conexao
            mConn = new MySqlConnection(" Persist Security Info=False;server=localhost;port=3307;database=tcc2;uid=root;server = localhost; database = tcc2; uid = root; pwd = root");
            /*string consulta = "UPDATE Immobile SET in_use = false WHERE ;
            MySqlCommand cmd1 = new MySqlCommand(consulta, mConn);
            var retorno = cmd1.ExecuteScalar();
            System.Diagnostics.Debug.WriteLine("Consulta - " + consulta);*/
        }
    }
}
