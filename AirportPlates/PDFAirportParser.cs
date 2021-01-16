using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AirportPlates.Models;
using IronPdf;
using JsonReaderLibrary;

namespace AirportPlates
{
   public enum PlateType
   {
      AirportDiagram,
      ArrivalDeparture,
      Approach
   }

   public class AirportMetaData
   {
      public static readonly Dictionary<string, string> RegionCodes = new Dictionary<string, string>
      {
         { "AK", "AK" },
         { "CN", "CN" },
         { "EC-1", "EC1" },
         { "EC-2", "EC2" },
         { "EC-3", "EC3" },
         { "NC-1", "NC1" },
         { "NC-2", "NC2" },
         { "NC-3", "NC3" },
         { "NE-1", "NE1" },
         { "NE-2", "NE2" },
         { "NE-3", "NE3" },
         { "NE-4", "NE4" },
         { "NW-1", "NW1" },
         { "SC-1", "SC1" },
         { "SC-2", "SC2" },
         { "SC-3", "SC3" },
         { "SC-4", "SC4" },
         { "SC-5", "SC5" },
         { "SE-1", "SE1" },
         { "SE-2", "SE2" },
         { "SE-3", "SE3" },
         { "SE-4", "SE4" },
         { "SW-1", "SW1" },
         { "SW-2", "SW2" },
         { "SW-3", "SW3" },
         { "SW-4", "SW4" }
      };
      public static readonly Dictionary<string, string> ApproachTypes = new Dictionary<string, string>
      {
         { "R", "RNAV" },
         { "IL", "ILSLOC" },
         { "L", "LOC" },
         { "VGA", "VORGPSA" },
         { "HIL", "HiILSLOC" },
         { "HT", "HiTACAN" },
         { "T", "TACAN" },
         { "VD", "VORDME" },
         { "N", "NDB" },
         { "HR", "HiRNAV" },
         { "V", "VOR" },
         { "I", "ILS" },
         { "RR", "RNAVRNP" },
         { "TC", "TACANC" },
         { "VA", "VORA" },
         { "VDB", "VORDMEB" },
         { "HVT", "HiVORTACAN" },
         { "LDAD", "LDADME" },
         { "VB", "VORB" },
         { "IPRM", "ILSPRM" },
         { "RPRM", "RNAVGPSPRM" },
         { "ILD", "ILSLOCDME" },
         { "HVDT", "HiVORDMETACAN" },
         { "TA", "TACANA" },
         { "VG", "VORGPS" },
         { "LBC", "LOCBC" },
         { "LDAPRM", "LDAPRM" },
      };
      public static readonly string[] ApproachOptions = new string[]
      {
         "Y",
         "Z",
         "X",
         "W",
      };
      public int FAANumber { get; set; }
      public string RegionCode { get; set; }
      public string AirportCode { get; set; }
      public PlateType Type { get; set; } = PlateType.ArrivalDeparture;
      public string PlateData { get; set; }
      public string Runway { get; set; }
      public string ApproachOption { get; set; }
      public string OtherData { get; set; }

      public static Dictionary<int, string> AirportFileIds = new Dictionary<int, string>();

      public override string ToString()
      {
         StringBuilder bd = new StringBuilder(!String.IsNullOrEmpty(AirportCode) ? $"{AirportCode} " : "NA- ");
         bd.Append(!String.IsNullOrEmpty(RegionCode) ? $"{RegionCode} " : "NA- ");
         bd.Append(Type);
         bd.Append(!String.IsNullOrEmpty(PlateData) ? $" {PlateData}" : " NA-");
         bd.Append(!String.IsNullOrEmpty(Runway) ? $" {Runway}" : " NA-");
         bd.Append(!String.IsNullOrEmpty(ApproachOption) ? $" {ApproachOption}" : " NA-");
         bd.Append(!String.IsNullOrEmpty(OtherData) ? $" {OtherData}" : " NA-");
         //return $"{AirportCode_2} {RegionCode} {Type} {PlateData}{(!String.IsNullOrEmpty(Runway) ? $" {Runway}" : "")}";
         return bd.ToString();
      }
      //public string AirportCode_2
      //{
      //   get
      //   {
      //      if (AirportCodes.ContainsKey(FAANumber))
      //      {
      //         return AirportCodes[FAANumber];
      //      }
      //      else
      //      {
      //         return "NA-";
      //      }
      //   }
      //}
   }

   public static class PDFAirportParser
   {
      #region - Fields & Properties
      private static readonly char[] BadChars = new char[]
      {
         '',
         '',
         '',
         '',
         '',
         ''
      };

      public static List<AirportCode> AirportInfo { get; private set; }
      public static List<string> AirportCodes { get; set; }
      #endregion

      #region - Constructors
      #endregion

      #region - Methods
      public static void LoadAirportCodes(string path)
      {
         try
         {
            AirportInfo = JsonReader.OpenJsonFile<List<AirportCode>>(path);
            AirportCodes = AirportInfo.Select(ap => ap.IATACode).ToList();
         }
         catch (Exception)
         {
            throw;
         }
      }

      public static void Test()
      {
         var paths = Directory.GetFiles(@"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate PDFs\A");
         var plateText = new List<string>();
         foreach (var path in paths)
         {
            AirportMetaData meta = new AirportMetaData();
            var doc = PdfDocument.FromFile(path);
            string rawText = GetAirportCode(meta, doc);
            if (rawText.Contains("AIRPORT DIAGRAM") || rawText.Contains("airport diagram"))
            {
               meta.Type = PlateType.AirportDiagram;
            }
            else if (rawText.Contains("ILS RWY") || rawText.Contains("ils rwy"))
            {
               //meta.Type = PlateType.ILS;
            }
            plateText.Add(rawText);
         }
      }

      public static void ParsePDFs(string pdfDir, string imgDir)
      {
         try
         {
            var paths = Directory.GetFiles(pdfDir);
            var errors = new List<Exception>();

            foreach (var path in paths)
            {
               try
               {
                  var doc = PdfDocument.FromFile(path);
                  var rawText = doc.ExtractAllText();
                  var meta = new AirportMetaData();

                  ParseFileName(meta, Path.GetFileNameWithoutExtension(path));
                  meta.AirportCode = GetAirportCode_2(rawText, meta.FAANumber);
                  meta.RegionCode = GetAirportRegion(rawText);
                  //(meta.AirportCode, meta.RegionCode) = GetAirportCodeAndRegion(doc, meta.FAANumber);

                  if (meta.FAANumber > 0)
                  {
                     if (!AirportMetaData.AirportFileIds.ContainsKey(meta.FAANumber) && meta.AirportCode != "NA-")
                     {
                        AirportMetaData.AirportFileIds.Add(meta.FAANumber, meta.AirportCode);
                     }
                  }

                  ConvertToImage(imgDir, doc, meta);
               }
               catch (Exception e)
               {
                  errors.Add(e);
               }
            }

            if (errors.Count > 0)
            {
               throw new PagesException("Airport parse errors occured.", errors);
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      public static void ParsePDFsTest(string testFile)
      {
         try
         {
            //var paths = Directory.GetFiles(pdfDir);
            var paths = new string[] { testFile };
            var errors = new List<Exception>();

            foreach (var path in paths)
            {
               try
               {
                  var doc = PdfDocument.FromFile(path);
                  var meta = new AirportMetaData();

                  ParseFileName(meta, Path.GetFileNameWithoutExtension(path));
                  //(meta.AirportCode, meta.RegionCode) = GetAirportCodeAndRegion(doc, meta.FAANumber);
                  string rawText = doc.ExtractAllText();
                  meta.AirportCode = GetAirportCode(rawText, meta.FAANumber);
                  meta.RegionCode = GetAirportRegion(rawText);

                  if (meta.FAANumber > 0)
                  {
                     if (!AirportMetaData.AirportFileIds.ContainsKey(meta.FAANumber) && meta.AirportCode != "NA-")
                     {
                        AirportMetaData.AirportFileIds.Add(meta.FAANumber, meta.AirportCode);
                     }
                  }

                  //ConvertToImage(imgDir, doc, meta);
               }
               catch (Exception e)
               {
                  errors.Add(e);
               }
            }

            if (errors.Count > 0)
            {
               throw new PagesException("Airport parse errors occured.", errors);
            }
         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Not used
      /// </summary>
      private static string GetAirportCode(AirportMetaData meta, PdfDocument doc)
      {
         string rawText = doc.ExtractAllText();
         string[] lines = rawText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
         foreach (var line in lines)
         {
            if (line.Contains("(") && line.Contains(")"))
            {
               string[] findAirportCode = line.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
               foreach (var subLine in findAirportCode)
               {
                  if (subLine.Trim().Length == 3 && !subLine.Contains("FAA"))
                  {
                     meta.AirportCode = subLine;
                  }
               }
            }
         }

         return rawText;
      }

      /// <summary>
      /// Not used
      /// </summary>
      private static string GetAirportCode(PdfDocument doc, int currentAirportCode)
      {
         string rawText = doc.ExtractAllText();
         string[] lines = rawText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
         string airportCode = "";
         if (!AirportMetaData.AirportFileIds.ContainsKey(currentAirportCode))
         {
            foreach (var line in lines)
            {
               if (line.Contains("(") && line.Contains(")"))
               {
                  string[] findAirportCode = line.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                  foreach (var subLine in findAirportCode)
                  {
                     if (subLine.Trim().Length == 3 && !subLine.Contains("FAA"))
                     {
                        airportCode = subLine;
                        break;
                     }
                  }
                  if (airportCode != "")
                  {
                     break;
                  }
               }
            }
         }
         else
         {
            return AirportMetaData.AirportFileIds[currentAirportCode];
         }

         return airportCode;
      }
      
      /// <summary>
      /// Not Used
      /// </summary>
      private static (string airportCode, string regionCode) GetAirportCodeAndRegion(PdfDocument doc, int currentAirportCode)
      {
         string rawText = doc.ExtractAllText();
         string[] lines = rawText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
         string airportCode = "NA-";
         string regionCode = "NA-";
         if (!AirportMetaData.AirportFileIds.ContainsKey(currentAirportCode))
         {
            if (!BadChars.Any(c => rawText.Contains(c)))
            {
               foreach (var line in lines)
               {
                  if (line.Contains("(") && line.Contains(")"))
                  {
                     string[] findAirportCode = line.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                     foreach (var subLine in findAirportCode)
                     {
                        if ((subLine.Trim().Length == 3 || subLine.Trim().Length == 4) && !subLine.Contains("FAA"))
                        {
                           airportCode = subLine;
                           break;
                        }
                     }
                  }

                  foreach (var regKey in AirportMetaData.RegionCodes.Keys)
                  {
                     if (line.Contains(regKey))
                     {
                        regionCode = AirportMetaData.RegionCodes[regKey];
                     }
                  }

                  if (airportCode != "NA-" && regionCode != "NA-")
                  {
                     break;
                  }
               }
            }
         }
         else
         {
            return (AirportMetaData.AirportFileIds[currentAirportCode], regionCode);
         }

         return (airportCode, regionCode);
      }

      public static string GetAirportRegion(string docText)
      {
         string airportRegion = "NA-";
         if (!BadChars.Any(c => docText.Contains(c)))
         {
            string[] lines = docText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
               foreach (var regKey in AirportMetaData.RegionCodes.Keys)
               {
                  if (line.Contains(regKey))
                  {
                     airportRegion = AirportMetaData.RegionCodes[regKey];
                  }
               }

               if (airportRegion != "NA-")
               {
                  break;
               }
            }
         }
         return airportRegion;
      }

      private static string GetAirportCode(string docText, int currentNum)
      {
         string airportCode = "NA-";
         if (!BadChars.Any(c => docText.Contains(c)))
         {
            try
            {
               string[] lines = docText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
               foreach (var line in lines)
               {
                  foreach (var code in AirportCodes)
                  {
                     if (line.Contains(code))
                     {
                        airportCode = code;
                        break;
                     }
                  }

                  if (airportCode != "NA-")
                  {
                     break;
                  }
               }

               if (currentNum != 0 && airportCode != "NA-")
               {
                  if (!AirportMetaData.AirportFileIds.ContainsKey(currentNum))
                  {
                     AirportMetaData.AirportFileIds.Add(currentNum, airportCode);
                  }
                  else
                  {
                     AirportMetaData.AirportFileIds[currentNum] = airportCode;
                  }
               }
            }
            catch (Exception)
            {
               throw;
            }
            return airportCode;
         }
         else
         {
            return null;
         }
      }
      private static string GetAirportCode_2(string docText, int currentNum)
      {
         string airportCode = "NA-";
         if (!BadChars.Any(c => docText.Contains(c)))
         {
            try
            {
               var foundCodes = new List<string>();
               string[] lines = docText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
               foreach (var line in lines)
               {
                  foreach (var code in AirportCodes)
                  {
                     if (line.Contains($"({code})"))
                     {
                        foundCodes.Add(code);
                     }
                  }

                  if (airportCode != "NA-")
                  {
                     break;
                  }
               }

               if (foundCodes.Count > 1)
               {
                  var result = from id in foundCodes
                     group id by id into g
                     orderby g.Count() descending
                     select new { code = g.Key, Count = g.Count() };
                  airportCode = result.First().code;
               }
               else if (foundCodes.Count == 1)
               {
                  airportCode = foundCodes[0];
               }

               if (currentNum != 0 && airportCode != "NA-")
               {
                  if (!AirportMetaData.AirportFileIds.ContainsKey(currentNum))
                  {
                     AirportMetaData.AirportFileIds.Add(currentNum, airportCode);
                  }
                  else
                  {
                     AirportMetaData.AirportFileIds[currentNum] = airportCode;
                  }
               }
            }
            catch (Exception)
            {
               throw;
            }
            return airportCode;
         }
         else
         {
            return null;
         }
      }

      private static void ParseFileName(AirportMetaData meta, string rawTitle)
      {
         if (String.IsNullOrEmpty(rawTitle))
         {
            return;
         }

         string fileTypeCodes = rawTitle.Substring(5);
         string apNumber = rawTitle.Substring(0, 5);

         bool success = int.TryParse(apNumber, out int airportNumber);
         if (success)
         {
            meta.FAANumber = airportNumber;
         }

         if (fileTypeCodes == "AD")
         {
            meta.Type = PlateType.AirportDiagram;
         }
         else if (!fileTypeCodes.Any(c => Char.IsDigit(c)) && fileTypeCodes.Length >= 5)
         {
            meta.Type = PlateType.ArrivalDeparture;
            meta.PlateData = fileTypeCodes;
         }
         else
         {
            foreach (var key in AirportMetaData.ApproachTypes.Keys)
            {
               if (fileTypeCodes.StartsWith(key))
               {
                  meta.Type = PlateType.Approach;
                  meta.PlateData = AirportMetaData.ApproachTypes[key];
                  string runwaySub = fileTypeCodes.Substring(key.Length);
                  if (!String.IsNullOrEmpty(runwaySub))
                  {
                     var parsedRunway = "";
                     var extras = "";
                     if (AirportMetaData.ApproachOptions.Any(o => o == runwaySub[0].ToString()))
                     {
                        meta.ApproachOption = runwaySub[0].ToString();
                        runwaySub = runwaySub.Remove(0, 1);
                     }
                     for (int i = 0; i < runwaySub.Length; i++)
                     {
                        if (i < 3)
                        {
                           if (Char.IsDigit(runwaySub[i]) || runwaySub[i] == 'L' || runwaySub[i] == 'R')
                           {
                              parsedRunway += runwaySub[i];
                           }
                           else
                           {
                              extras += runwaySub[i];
                           }
                        }
                        else
                        {
                           extras += runwaySub[i];
                        }
                     }
                     meta.Runway = parsedRunway;
                     meta.OtherData = extras;
                  }
                  break;
               }
            }
         }
      }

      public static void ConvertToImage(string imgDir, PdfDocument doc, AirportMetaData meta)
      {
         string savePath = Path.Combine(imgDir, meta.ToString() + ".png");
         doc.ToPngImages(savePath, 180);
      }

      //public static void ConvertToImages(string pdfDir, string imgDir)
      //{
      //   try
      //   {
      //      var errors = new List<Exception>();
      //      var paths = Directory.GetFiles(pdfDir);
      //      for (int i = 0; i < paths.Length; i++)
      //      {
      //         try
      //         {
      //            var doc = PdfDocument.FromFile(paths[i]);
      //            doc.ToPngImages(Path.Combine(imgDir, $"page_{i}.png"), 170);
      //         }
      //         catch (Exception e)
      //         {
      //            errors.Add(e);
      //         }
      //      }

      //      if (errors.Count > 0)
      //      {
      //         throw new PagesException("Page Copy Exceptions", errors);
      //      }
      //   }
      //   catch (Exception)
      //   {
      //      throw;
      //   }
      //}

      //public static void ConvertToImageTest()
      //{
      //   var doc = PdfDocument.FromFile(@"B:\Games\OtherGames\FS 2020\Airport Plates\A-K_Pages\page_81.pdf");
      //   doc.ToPngImages(@"B:\Games\OtherGames\FS 2020\Airport Plates\page_test_2.png", 968, 1685, 180);
      //}

      #endregion

      #region - Full Properties

      #endregion
   }
}
