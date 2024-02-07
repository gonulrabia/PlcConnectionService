using ActUtlType64Lib;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace PlcConnectionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public bool IsConnected = false;
        public bool IsReadWrite = false;
        public readonly ActUtlType64 plc = new ActUtlType64();


        public Worker(ILogger<Worker> logger, IHostApplicationLifetime appLifetime, IHost host)
        {
            _logger = logger;
            ActUtlType64 plc = new ActUtlType64();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {                 
                    if (IsConnected)
                    {
                        DataReadWriteFromPlc();
                    }
                    if (!IsConnected)
                    {
                        IsConnected = OpenConnection();
                    }                
                    await Task.Delay(2000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " ==> PLC ile bağlantı kurulamadı. Hata Kodu: " + ex.Message);
            }
        }

        public short raporSil, silCounter, raporVar, raporAdet, counter, batchNo, tN, adimNo, siloNo;
        public int receteID, partiID, hammaddeID, alinacak, alinan, shut;
        private bool OpenConnection()
        {
            plc.ActLogicalStationNumber = 1;
            int result = plc.Open(); //bağlantı başarılı ise 0 döner.

            try
            {
                if (result == 0)
                {
                    Console.WriteLine(DateTime.Now + " ==> PLC ile bağlantı kuruldu.");
                    return true;
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " ==> PLC ile bağlantı kurulamadı. Hata Kodu: " + result);
                    plc.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DataReadWriteFromPlc()
        {
            try
            {
                //int plcPortNumber = 1200; 
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                plc.GetDevice2("D1002", out raporVar);
                stopwatch.Stop();
                if (raporVar == 0)
                {
                    double timeOutSeconds = stopwatch.Elapsed.TotalSeconds;
                    if(timeOutSeconds >= 4)
                    {                       
                        Console.WriteLine(DateTime.Now + " ==> Bağlantı Koptu.");
                        plc.Close();
                        IsConnected = false;
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now + " ==> Rapor Yok.");
                    }
                }

                // Okunan değerleri ilgili değişkenlere ata
                short[] deviceValues = new short[9];
                plc.ReadDeviceBlock2("D1000", 9, out deviceValues[0]);
                raporSil = deviceValues[0];
                silCounter = deviceValues[1];
                raporVar = deviceValues[2];
                raporAdet = deviceValues[3];
                counter = deviceValues[4];
                batchNo = deviceValues[5];
                tN = deviceValues[6];
                adimNo = deviceValues[7];
                siloNo = deviceValues[8];

                plc.ReadDeviceBlock("D1009", 2, out receteID);
                plc.ReadDeviceBlock("D1011", 2, out partiID);
                plc.ReadDeviceBlock("D1013", 2, out hammaddeID);
                plc.ReadDeviceBlock("D1015", 2, out alinacak);
                plc.ReadDeviceBlock("D1017", 2, out alinan);
                plc.ReadDeviceBlock("D1019", 2, out shut);
         
                SaveToSql();

                // Veriyi PLC'ye gönderme
                short dataToSend = 1;
                var result = plc.WriteDeviceBlock2("D1000", 1, ref dataToSend); // D1000 adresine 1 değerini yazma
                var result2 = plc.WriteDeviceBlock2("D1001", 1, ref counter); // D1001 adresine counter değerini yazma
                if (result == 0)
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri başarıyla PLC'ye gönderildi.");
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri yazma hatası. Hata Kodu: " + result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(DateTime.Now + " ==> PLC bağlantısı sağlanamadı. Hata:" + ex.Message);
            }
        }

        private void SaveToSql()
        {
            string connectionString = "Server=DESKTOP-5G86BK6; Database=PlcConnection;User Id=sa;Password=sa;Encrypt=true; TrustServerCertificate=true;"; // SQL Server bağlantı dizesi
            string tableName = "PlcConnectionTable"; // Kaydedilecek SQL tablosu adı

            try
            {
                //burda bağlantı açılıyo
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = $"INSERT INTO {tableName} (kayitTarihi,counter,batchNo,tN,adimNo,siloNo,receteID,partiID,hammaddeID,alinacak,alinan,shut) " +
                             "VALUES (@kayitTarihi,@counter,@batchNo, @tN, @adimNo,@siloNo,@receteID, @partiID, @hammaddeID, @alinacak, @alinan, @shut)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        // Parametreleri ekleyerek değerleri atayın

                        insertCommand.Parameters.AddWithValue("@kayitTarihi", DateTime.Now); // Şu anki tarih
                        insertCommand.Parameters.AddWithValue("@counter", Convert.ToInt32(counter));
                        insertCommand.Parameters.AddWithValue("@batchNo", Convert.ToInt32(batchNo));
                        insertCommand.Parameters.AddWithValue("@tN", Convert.ToInt32(tN));
                        insertCommand.Parameters.AddWithValue("@adimNo", Convert.ToInt32(adimNo));
                        insertCommand.Parameters.AddWithValue("@siloNo", Convert.ToInt32(siloNo));
                        insertCommand.Parameters.AddWithValue("@receteID", receteID);
                        insertCommand.Parameters.AddWithValue("@partiID", partiID);
                        insertCommand.Parameters.AddWithValue("@hammaddeID", hammaddeID);
                        insertCommand.Parameters.AddWithValue("@alinacak", alinacak);
                        insertCommand.Parameters.AddWithValue("@alinan", alinan);
                        insertCommand.Parameters.AddWithValue("@shut", shut);

                        // SQL komutunu çalıştır
                        insertCommand.ExecuteNonQuery();

                        _logger.LogInformation(DateTime.Now + " ==> Veriler SQL'e başarıyla eklendi.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"SQL'e kaydetme işleminde hata oluştu. Hata: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}