using ActUtlType64Lib;
using Microsoft.Data.SqlClient;

namespace PlcConnectionService

{


    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public bool IsConnected = false;
        public readonly ActUtlType64 plc = new ActUtlType64();


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            ActUtlType64 plc = new ActUtlType64();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int stationNumber = 1;//plc istasyon numaras�

            int result = plc.ActLogicalStationNumber = stationNumber;
            //ba�lant� kontrol
            if (result == 1)
            {
                Console.WriteLine(DateTime.Now + " ==> PLC ile ba�lant� kuruldu.");
            }
            else
            {
                Console.WriteLine(DateTime.Now + " ==> PLC ile ba�lant� kurulamad�. Hata Kodu: " + result);
            }

            plc.Open();

            while (!stoppingToken.IsCancellationRequested)
            {
                IsConnected = CheckConnection();
                await Task.Delay(2000, stoppingToken);

            }

            plc.Close();
        }
        public int raporSil, silCounter, raporVar, raporAdet, counter, batchNo, tN, adimNo, siloNo,
            receteID1, receteID2, partiID1, partiID2, hammaddeID1,
            hammaddeID2, alinacak1, alinacak2, alinan1, alinan2, shut1, shut2, id;

        private bool CheckConnection()
        {
            try
            {
                //string plcIpAddress = "192.168.3.39"; // PLC'nin IP adresini buraya yaz�n
                //int plcPortNumber = 1200; // PLC'nin port numaras�n� buraya yaz�n



                plc.GetDevice("D1000", out raporSil);
                plc.GetDevice("D1001", out silCounter);
                plc.GetDevice("D1002", out raporVar);
                plc.GetDevice("D1003", out raporAdet);


                plc.GetDevice("D1004", out counter);
                plc.GetDevice("D1005", out batchNo);
                plc.GetDevice("D1006", out tN);
                plc.GetDevice("D1007", out adimNo);
                plc.GetDevice("D1008", out siloNo);

                plc.GetDevice("D1009", out receteID1);
                plc.GetDevice("D1010", out receteID2);

                plc.GetDevice("D1011", out partiID1);
                plc.GetDevice("D1012", out partiID2);

                plc.GetDevice("D1013", out hammaddeID1);
                plc.GetDevice("D1014", out hammaddeID2);

                plc.GetDevice("D1015", out alinacak1);
                plc.GetDevice("D1016", out alinacak2);

                plc.GetDevice("D1017", out alinan1);
                plc.GetDevice("D1018", out alinan2);


                plc.GetDevice("D1019", out shut1);
                plc.GetDevice("D1020", out shut2);



                SaveToSql();
                // _logger.LogInformation($"PLC'den okunan de�erler: wRaporSil={wRaporSil}, wSilCounter={wSilCounter}, wRaporVar={wRaporVar}," +
                //  $" wRaporAdet={wRaporAdet}, wCounter={wCounter}, dwReceteID ={dwReceteID},dwPartiID={dwPartiID},dwHammaddeID={dwHammaddeID},dwAlinacak={dwAlinacak}" +
                //   $"dwAlinan={dwAlinan},dwShut={dwShut}");


                // Veriyi PLC'ye g�nderme
                short dataToSend = 42;
                var result = plc.WriteDeviceBlock2("D1001", 1, ref dataToSend); // D1000 adresine bir de�er yazma
                if (result == 0)
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri ba�ar�yla PLC'ye g�nderildi.");
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri yazma hatas�. Hata Kodu: " + result);
                }

                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError(DateTime.Now + " ==> PLC ba�lant�s� sa�lanamad�. Hata:" + ex.Message);
                return false;
            }

        }

        private void SaveToSql()
        {
            string connectionString = "Server=DESKTOP-5G86BK6; Database=PlcConnection;User Id=sa;Password=sa;Encrypt=true; TrustServerCertificate=true;"; // SQL Server ba�lant� dizesi
            string tableName = "PlcConnectionTable"; // Kaydedilecek SQL tablosu ad�


            try
            {
                //burda ba�lant� a��l�yo
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    long receteID = (long)receteID1 * 10000 + receteID2;
                    long partiID = (long)partiID1 * 10000 + partiID2;
                    long hammaddeID = (long)hammaddeID1 * 10000 + hammaddeID2;
                    long alinacak = (long)alinacak1 * 10000 + alinacak2;
                    long alinan = (long)alinan1 * 10000 + alinan2;
                    long shut = (long)shut1 * 10000 + shut2;

                    string insertQuery = $"INSERT INTO {tableName} (kayitTarihi,counter,batchNo,tN,adimNo,siloNo,receteID,partiID,hammaddeID,alinacak,alinan,shut) " +
                             "VALUES (@kayitTarihi,@counter,@batchNo, @tN, @adimNo,@siloNo,@receteID, @partiID, @hammaddeID, @alinacak, @alinan, @shut)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        // Parametreleri ekleyerek de�erleri atay�n

                        insertCommand.Parameters.AddWithValue("@kayitTarihi", DateTime.Now); // �u anki tarihi ekleyin
                        insertCommand.Parameters.AddWithValue("@counter", counter);
                        insertCommand.Parameters.AddWithValue("@batchNo", batchNo);
                        insertCommand.Parameters.AddWithValue("@tN", tN);
                        insertCommand.Parameters.AddWithValue("@adimNo", adimNo);
                        insertCommand.Parameters.AddWithValue("@siloNo", siloNo);
                        insertCommand.Parameters.AddWithValue("@receteID", receteID);
                        insertCommand.Parameters.AddWithValue("@partiID", partiID);
                        insertCommand.Parameters.AddWithValue("@hammaddeID", hammaddeID);
                        insertCommand.Parameters.AddWithValue("@alinacak", alinacak);
                        insertCommand.Parameters.AddWithValue("@alinan", alinan);
                        insertCommand.Parameters.AddWithValue("@shut", shut);

                        // SQL komutunu �al��t�r
                        insertCommand.ExecuteNonQuery();

                        _logger.LogInformation(DateTime.Now + " ==> Veriler SQL'e ba�ar�yla eklendi.");


                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL'e kaydetme i�leminde hata olu�tu. Hata: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }
            }


        }


    }
}
