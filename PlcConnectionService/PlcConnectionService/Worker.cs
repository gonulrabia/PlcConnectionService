using ActUtlType64Lib;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using ActSupportMsg64Lib;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using PlcConnectionService.DAL.Modules;
using PlcConnectionService.Entities.Entities;
using PlcConnectionService.DATA;
using PlcConnectionService.DAL;
using System;
using Microsoft.EntityFrameworkCore;
using Serilog;


namespace PlcConnectionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public bool IsConnected = false;
        public bool IsReadWrite = false;
        public readonly ActUtlType64 plc = new ActUtlType64();
        public readonly ActSupportMsg64 sup = new ActSupportMsg64();

        public int port;

        public SerialPort spCOM3 = new SerialPort("COM3");
        public SerialPort spCOM4 = new SerialPort("COM4");
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
            ActUtlType64 plc = new ActUtlType64();
            ActSupportMsg64 sup = new ActSupportMsg64();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                /*  spCOM3 ??= new SerialPort("COM3"); //spCOM3 null ise yeniden new()leyerek olu�turur
                  if (!spCOM3.IsOpen)
                  {
                      RefreshComport();
                  }
                  spCOM4 ??= new SerialPort("COM4"); //spCOM4 null ise yeniden new()leyerek olu�turur
                  if (!spCOM4.IsOpen)
                  {
                      RefreshComport();
                  }*/

                //SaveToSql();

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        IsConnected = OpenConnection();
                    }
                    if (IsConnected)
                    {
                        DataReadWriteFromPlc();
                    }
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(2000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " ==> ExecuteAsync(). Hata: " + ex.Message);
            }
        }

        public short raporSil, silCounter, raporVar, raporAdet, counter, batchNo, tN, adimNo, siloNo;
        public int receteID, partiID, hammaddeID, alinacak, alinan, shut;

        /*public void RefreshComport()
        {
            try
            {
                spCOM3.Close();
                spCOM3.Dispose();
                spCOM4.Close();
                spCOM4.Dispose();

                spCOM3 ??= new SerialPort("COM3"); //spCOM3 null ise yeniden new leyerek olu�turur
                if (!spCOM3.IsOpen)
                {
                    spCOM3.Open();
                }
                spCOM4 ??= new SerialPort("COM4"); //spCOM4 null ise yeniden new leyerek olu�turur
                if (!spCOM4.IsOpen)
                {
                    spCOM4.Open();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now + " ==> RefreshComport(). Hata: " + ex.Message);
            }
        }*/

        private bool OpenConnection()
        {
            try
            {
                plc.ActLogicalStationNumber = 1;
                int result = plc.Open(); //ba�lant� ba�ar�l� ise 0 d�ner.
                int errorCode = result;
                string errorMessage;
                sup.GetErrorMessage(result, out errorMessage);

                if (result == 0)
                {
                    Console.WriteLine(DateTime.Now + " ==> PLC ile ba�lant� kuruldu.");                    
                    return true;
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " ==> PLC ile ba�lant� kurulamad�.");
                    Console.WriteLine(DateTime.Now + $"Hata Kodu: {errorCode}, Hata �letisi: {errorMessage}");
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
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                plc.GetDevice2("D1002", out raporVar);
                stopwatch.Stop();
                if (raporVar == 0)
                {
                    double timeOutSeconds = stopwatch.Elapsed.TotalSeconds;
                    if (timeOutSeconds >= 4)
                    {
                        Console.WriteLine(DateTime.Now + " ==> Ba�lant� Koptu.");
                        plc.Close();

                        //RefreshComport();
                        
                        IsConnected = false;
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now + " ==> Rapor Yok.");
                    }
                }

                // Okunan de�erleri ilgili de�i�kenlere ata
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

                // Veriyi PLC'ye g�nderme
                short dataToSend = 1;
                var result = plc.WriteDeviceBlock2("D1000", 1, ref dataToSend); // D1000 adresine 1 de�erini yazma
                var result2 = plc.WriteDeviceBlock2("D1001", 1, ref counter); // D1001 adresine counter de�erini yazma
                if (result == 0)
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri ba�ar�yla PLC'ye g�nderildi.");
                }
                else
                {
                    Console.WriteLine(DateTime.Now + " ==> Veri yazma hatas�. Hata Kodu: " + result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(DateTime.Now + " ==> PLC ba�lant�s� sa�lanamad�. Hata:" + ex.Message);
            }
        }

       /* private void SaveToSql()
        {
            string connectionString = "Server=DESKTOP-5G86BK6; Database=PlcConnection;User Id=sa;Password=sa;Encrypt=true; TrustServerCertificate=true;"; // SQL Server ba�lant� dizesi
            string tableName = "PlcConnectionTable"; // Kaydedilecek SQL tablosu ad�

            try
            {
                //burda ba�lant� a��l�yo
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = $"INSERT INTO {tableName} (kayitTarihi,counter,batchNo,tN,adimNo,siloNo,receteID,partiID,hammaddeID,alinacak,alinan,shut) " +
                             "VALUES (@kayitTarihi,@counter,@batchNo, @tN, @adimNo,@siloNo,@receteID, @partiID, @hammaddeID, @alinacak, @alinan, @shut)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        // Parametreleri ekleyerek de�erleri atay�n

                        insertCommand.Parameters.AddWithValue("@kayitTarihi", DateTime.Now); // �u anki tarih
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
        }*/
        private void SaveToSql()
        {
            try
            {
                /*using(BaseDbContext context = new BaseDbContext())
                {

                }*/

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
                    // DbContext kullanarak i�lemleri ger�ekle�tirin

                    PlcData plcData = new PlcData();

                    //burada verileri doldurur
                    plcData.KayitTarihi = DateTime.Now;
                    plcData.Counter = Convert.ToInt32(counter);
                    plcData.BatchNo = Convert.ToInt32(batchNo);
                    plcData.TN = Convert.ToInt32(tN);
                    plcData.AdimNo = Convert.ToInt32(adimNo);
                    plcData.SiloNo = Convert.ToInt32(siloNo);

                    plcData.ReceteID = receteID;
                    plcData.PartiID = partiID;
                    plcData.HammaddeID = hammaddeID;
                    plcData.Alinacak = alinacak;
                    plcData.Alinan = alinan;
                    plcData.Shut = shut;

                    PlcDataManagement plcDataMan = new PlcDataManagement(dbContext);

                    var response = plcDataMan.PostPlcData(plcData);


                    _logger.LogInformation(DateTime.Now + " ==> Veriler SQL'e ba�ar�yla eklendi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"SQL'e kaydetme i�leminde hata olu�tu. Hata: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogInformation($"�� Hata: {ex.InnerException.Message}");
                }
            }
        }
    }
}