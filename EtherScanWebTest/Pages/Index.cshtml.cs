using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace EtherScanWebTest.Pages
{
    public class IndexModel : PageModel
    {


        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Symbol { get; set; }
        [BindProperty]
        public string Contract_address { get; set; }
        [BindProperty]
        public int Total_supply { get; set; }
        [BindProperty]
        public int Total_holders { get; set; }






        [BindProperty]
        public string Name_edit { get; set; } = "";
        [BindProperty]
        public string Symbol_edit { get; set; } = "";
        [BindProperty]
        public string Contract_address_edit { get; set; } = "";
        [BindProperty]
        public int Total_supply_edit { get; set; } =0;
        [BindProperty]
        public int Total_holders_edit { get; set; } = 0;

        private readonly ILogger<IndexModel> _logger;
        public string Message { get; private set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }




        string conn= "datasource=127.0.0.1;port=3306;uid=root;pw=;database=etherscan";
        string Query= "SELECT * FROM `token` ";
        public void OnGet()
        {

            getDatabaseData(1);
        }


        public void getDatabaseData(int page)
        {
            try
            {
                FetchData = new List<TableModel>();
                MySqlConnection MyConn2 = new MySqlConnection(conn);
                MyConn2.Open();
                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;

                using (MySqlDataReader reader = MyCommand2.ExecuteReader())
                {
                    StringBuilder sb = new StringBuilder();
                    while (reader.Read())
                    {

                        var TableData = new TableModel();
                        TableData.id = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
                        TableData.Symbol = reader.GetValue(reader.GetOrdinal("symbol")).ToString();
                        TableData.Name = reader.GetValue(reader.GetOrdinal("name")).ToString();
                        TableData.Contact = reader.GetValue(reader.GetOrdinal("contract_address")).ToString();
                        TableData.Holder = int.Parse(reader.GetValue(reader.GetOrdinal("total_holders")).ToString());
                        TableData.Supply = int.Parse(reader.GetValue(reader.GetOrdinal("total_supply")).ToString());

                        FetchData.Add(TableData);

                    }
                    int total = 0;
                    for (int i= 0; i < FetchData.Count; i++)
                    {
                        total = total + FetchData[i].Supply;
                    }
                    for (int i = 0; i < FetchData.Count; i++)
                    {
                     double percentTotal  =Convert.ToDouble(FetchData[i].Supply) / Convert.ToDouble(total) *100;
                        FetchData[i].SupplyPercent = percentTotal;
                    }

                    tableList = GetPage(FetchData, page);
                 
                }


                MyConn2.Close();








            }
            catch (Exception ex)
            {

            }
        }


        public void getDatabaseData()
        {
            try
            {
                FetchData = new List<TableModel>();
                MySqlConnection MyConn2 = new MySqlConnection(conn);
                MyConn2.Open();
                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;

                using (MySqlDataReader reader = MyCommand2.ExecuteReader())
                {
                    StringBuilder sb = new StringBuilder();
                    while (reader.Read())
                    {

                        var TableData = new TableModel();
                        TableData.id = int.Parse(reader.GetValue(reader.GetOrdinal("id")).ToString());
                        TableData.Symbol = reader.GetValue(reader.GetOrdinal("symbol")).ToString();
                        TableData.Name = reader.GetValue(reader.GetOrdinal("name")).ToString();
                        TableData.Contact = reader.GetValue(reader.GetOrdinal("contract_address")).ToString();
                        TableData.Holder = int.Parse(reader.GetValue(reader.GetOrdinal("total_holders")).ToString());
                        TableData.Supply = int.Parse(reader.GetValue(reader.GetOrdinal("total_supply")).ToString());

                        FetchData.Add(TableData);

                    }
                    int total = 0;
                    for (int i = 0; i < FetchData.Count; i++)
                    {
                        total = total + FetchData[i].Supply;
                    }
                    for (int i = 0; i < FetchData.Count; i++)
                    {
                        double percentTotal = Convert.ToDouble(FetchData[i].Supply) / Convert.ToDouble(total) * 100;
                        FetchData[i].SupplyPercent = percentTotal;
                    }

                

                }


                MyConn2.Close();








            }
            catch (Exception ex)
            {

            }
        }

        public List<TableModel> tableList { get; set; } = new List<TableModel>();
        public List<TableModel> FetchData { get; set; } = new List<TableModel>();
        public List<string> ChartValue { get; set; }

        public void OnPost()
        {
        }    
        public void OnPostSaveButton(IFormCollection data)
        {
            getDatabaseData();
            TableModel find = new TableModel();
            try
            {
                find = FetchData.Single(s => s.Name == Name);

                UpdateDB(find.id);


            }catch(Exception e)
            {
                if (e.ToString().Contains("no matching"))
                {
              
               
                    string queryInsert = "INSERT INTO etherscan.token (name, symbol, total_supply, contract_address,total_holders) VALUES('" + Name + "', '" + Symbol + "', " + Total_supply + ", '" + Contract_address + "'," + Total_holders + ") ";
                    insertDB(queryInsert);



                }
            }
            getDatabaseData(1);
        }  
        public void OnPostResetButton(IFormCollection data)
        {
            getDatabaseData(1);

        }
        public int total_supply_percent(int tokenPercent, int totalSupply)
        {
            return (tokenPercent / totalSupply*100);
        }
        public ActionResult OnGetChartData()
        {

            getDatabaseData(1);

            var test = new { c = new object[] { new { v = "" }, new { v = 0 } } };
            var list = new[] { test }.ToList();

            foreach(var data in tableList)
            {
                list.Add(new { c = new object[] { new { v = data.Name }, new { v = data.Supply } } });
            }

           

            var chart = new Chart
            {
                cols = new object[]
                {
            new { id = "Name", type = "string", label = "Name" },
            new { id = "totalsupply", type = "number", label = "total supply" }
                },
                rows = new object[]
                {


                }
            };
            chart.rows = list.ToArray();

            return new JsonResult(chart);
        }
        public static List<T> CreateList<T>(params T[] elements)
        {
            return new List<T>(elements);
        }
        List<TableModel> GetPage(IList<TableModel> list, int pageNumber, int pageSize = 10)
        {
            return list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        public void insertDB(string query)
        {
         
                try
                {
               
                   
                    MySqlConnection MyConn2 = new MySqlConnection(conn);
                    MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
                    MySqlDataReader MyReader2;
                    MyConn2.Open();
                    MyReader2 = MyCommand2.ExecuteReader();  
                    while (MyReader2.Read())
                    {
                    }
                    MyConn2.Close();
                }
                catch (Exception ex)
                {
               
                }
        }
        public void UpdateDB(int ID)
        {
            string query= 
                @"UPDATE `etherscan`.`token` SET 
`symbol` = '" + Symbol + @"',
`name` = '" + Name + @"',
`total_supply` =" + Total_supply + @",
`contract_address` = '" + Contract_address + @"',
`total_holders` = " + Total_holders + @"
 WHERE `id` = " + ID + @";
            ";

            MySqlConnection MyConn2 = new MySqlConnection(conn);
            MySqlCommand MyCommand2 = new MySqlCommand(query, MyConn2);
            MySqlDataReader MyReader2;
            MyConn2.Open();
            MyReader2 = MyCommand2.ExecuteReader();   
                                                      
            while (MyReader2.Read())
            {
            }
            MyConn2.Close();


        }

        public void OnPostView(int id)
        {
            getDatabaseData();
            var find = FetchData.Single(s => s.id == id);

            Name_edit = find.Name;
            Symbol_edit = find.Symbol;
            Contract_address_edit = find.Contact;
            Total_supply_edit = find.Supply;
            Total_holders_edit = find.Holder;
            getDatabaseData(1);
         
        }

        public FileResult OnPostCSV()
        {
            getDatabaseData();
            return csvBuilder();
        

        }

        public FileResult csvBuilder()
        {


            List<string> CSVdata = new List<string>();

            foreach (var data in FetchData) {

                string strData = data.id + "," + data.Name + "," + data.Symbol + "," + data.Supply + "," + data.Contact + "," + data.Holder+ "\r\n";
                CSVdata.Add(strData);
                 


            }




            StringBuilder sb = new StringBuilder();
      
          
            for (int i = 0; i < FetchData.Count; i++)
            {
                string[] csvD = CSVdata.ToArray();
               
            
                    
                    sb.Append(csvD[i]);
               

                //Append new line character.
        

            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Grid.csv");
        }
    }

    public class Chart
    {
        public object[] cols { get; set; }
        public object[] rows { get; set; }
    }

    public class TableModel
    {

        public int id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public int Holder { get; set; }
        public int Supply { get; set; }
        public double SupplyPercent { get; set; }

    

    }



}
