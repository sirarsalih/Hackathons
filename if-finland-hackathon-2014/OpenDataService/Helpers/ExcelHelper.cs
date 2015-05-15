using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Excel;
using OpenDataService.DataManagement;

namespace OpenDataService.Helpers {
    public static class ExcelHelper {
        public static ExcelFormat GetFormat(string filename){
            var lower = filename.ToLower();
            if (lower.EndsWith(".xls")){
                return ExcelFormat.XLS;
            }
            if (lower.EndsWith(".xlsx")) {
                return ExcelFormat.XLSX;
            }
            return ExcelFormat.UNKNOWN;
        }

        private static int ExcCounter = 0;

        public static IExcelDataReader GetReader(Stream inputStream, ExcelFormat format){
            switch (format){
                case ExcelFormat.XLS:
                    return ExcelReaderFactory.CreateBinaryReader(inputStream);
                case ExcelFormat.XLSX:
                    return ExcelReaderFactory.CreateOpenXmlReader(inputStream);
                default:
                    throw new Exception("Unknown file format.");
            }
        }

        public static void ProcessClaims(string path)
        {
            var file = File.OpenRead(path);
            var format = GetFormat(file.Name);
            var reader = GetReader(file, format);
            reader.IsFirstRowAsColumnNames = true;
            var dataset = reader.AsDataSet();
            var table = dataset.Tables[0];
            var autoRand = new Random();
            using (var db = new OpenDataDatabaseEntities())
            {
                for (var i = 0; i < table.Rows.Count; i++) {
                    var claim = db.Claim.FirstOrDefault(e => e.Id == i + 1);
                    if (claim != null) continue;
                    var claim2 = table.Rows[i].RowToClaim(autoRand);
                    if (claim2 == null) continue;
                    claim2.Id = i + 1;
                    db.Claim.Add(claim2);
                }
                db.SaveChanges();
            }
            ExcCounter++;
        }

        private static Claim RowToClaim(this DataRow row, Random rand) {
            try
            {
                var claim = new Claim()
                {
                    Product = (Product) Enum.Parse(typeof (Product), row["Product"].ToString()),
                    Age = row["Age"].ToString().ToNullableInt32(),
                    ClaimType = row["Type"].ToString(),
                    Gender = (Gender) Enum.Parse(typeof (Gender), row["Gender"].ToString()),
                    MunicipalityClaim = row["MunicipalityClaim"].ToString(),
                    Venue = row["Venue"].ToString(),
                    Situation = row["Situation"].ToString(),
                    RoadType = row["RoadType"].ToString(),
                    MunicipalityPolicyholder = row["MunicipalityPolicyholder"].ToString(),
                    Date = DateTime.FromOADate(double.Parse(row["ClaimDate"].ToString()) + double.Parse(row["ClaimTime"].ToString())),
                    Car = new Car()
                    {
                        CylinderVolume = int.Parse(row["CylinderVolume"].ToString()),
                        NumberOfSeats = int.Parse(row["NumberOfSeats"].ToString()),
                        Color = row["Colour"].ToString(),
                        Length = int.Parse(row["Length"].ToString()),
                        Width = int.Parse(row["Width"].ToString()),
                        DistanceBetweenAxes = int.Parse(row["DistanceBetweenAxes"].ToString()),
                        Kilowatts = int.Parse(row["Kilowatts"].ToString()),
                        FuelType = (FuelType)Enum.Parse(typeof(FuelType), row["Fuel"].ToString()),
                        Weight = int.Parse(row["Weight"].ToString()),
                        FirstUseDate = DateTime.ParseExact(row["FirstUseDate"].ToString(), "yyyymmss", CultureInfo.InvariantCulture),
                        Brand = int.Parse(row["Brand"].ToString())
                    },
                    Location = new Location()
                    {
                        Latitude = 60.180805 + (rand.Next(0, 1) + rand.NextDouble())/10,
                        Longitude = 24.832412 + (rand.Next(0, 1) + rand.NextDouble())/10
                    }
                };
                return claim;
            }
            catch (Exception exc) {
                ExcCounter++;
                return null;
            }
        }

        public static int? ToNullableInt32(this string s) {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }
    }



    public enum ExcelFormat{
        XLS,
        XLSX,
        UNKNOWN
    }
}