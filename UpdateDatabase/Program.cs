using MySqlConnector;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Timers;

namespace UpdateDatabase
{
    class Program
    {
        private static System.Timers.Timer aTimer;
        static void Main(string[] args)
        {
            int a = 1;
        
           
                SetTimer();
                Console.WriteLine("\nPress the Enter key to exit the application...\n");
                Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
                Console.ReadLine();
                aTimer.Stop();
                aTimer.Dispose();

            

        }

        public static double APIcall(string symbol)
        {
            Price price = new Price();
          var client = new RestClient("https://min-api.cryptocompare.com/data/price?fsym="+ symbol+ "&tsyms=USD");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            price = JsonSerializer.Deserialize<Price>(response.Content); 
           // Console.WriteLine(price.USD);

            return price.USD;        
        }

        private static void SetTimer()
        {
    
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(300000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                  e.SignalTime);
            getDatabaseData().ForEach(e =>
            {
                Console.WriteLine(APIcall(e));

                UpdateDB(APIcall(e), e);
            });
        }
        public static void UpdateDB(double price,string symb)
        {
            string conn = "datasource=127.0.0.1;port=3306;uid=root;pw=;database=etherscan";
            string query ="UPDATE `etherscan`.`token` SET `price` = " + price + " WHERE `symbol` = '" + symb+"'" ;

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
        public static List<string> getDatabaseData()
        {
            var FetchData = new List<string>();
            try
            {
                string conn = "datasource=127.0.0.1;port=3306;uid=root;pw=;database=etherscan";
                string Query = "SELECT * FROM `token` ";

              
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
                        FetchData.Add( reader.GetValue(reader.GetOrdinal("symbol")).ToString());
                }




                }


                MyConn2.Close();





                return FetchData;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                  
            }

            return FetchData;
        }
    }

    class Price
    {
        public double USD { get; set; }
    }
}
