using MVVMLibraryFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AirportPlates.ViewModels
{
   public class MainViewModel : ViewModel
   {
      #region - Fields & Properties
      public BaseCommand OpenTestCmd { get; private set; }
      public BaseCommand ParsePDFCmd { get; private set; }
      public string PDFPath { get; set; } = @"B:\Games\OtherGames\FS 2020\Airport Plates\A-K_Pages\page_0.pdf";
      #endregion

      #region - Constructors
      public MainViewModel()
      {
         OpenTestCmd = new BaseCommand(PdfOpenTest);
         ParsePDFCmd = new BaseCommand(PdfParse);
      }
      #endregion

      #region - Methods
      public void PdfParse(object p)
      {
         try
         {
            PDFAirportParser.LoadAirportCodes(@"B:\Games\OtherGames\FS 2020\Airport Plates\USAAirportCodes.json");
            PDFAirportParser.ParsePDFs(
                  @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate PDFs\C",
                  @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate Images Test\C"
               );
            Console.WriteLine("C Done...");
            PDFAirportParser.ParsePDFs(
                  @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate PDFs\D",
                  @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate Images Test\D"
               );
            Console.WriteLine("D Done...");
         }
         catch (PagesException exe)
         {
            PrintExceptionList(exe);
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
      }
      public void PdfOpenTest(object p)
      {
         //PDFAirportParser.ParsePDFs(
         //      @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate PDFs\A",
         //      @"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate Images Test\A"
         //   );
         PDFAirportParser.LoadAirportCodes(@"B:\Games\OtherGames\FS 2020\Airport Plates\USAAirportCodes.json");
         PDFAirportParser.ParsePDFsTest(@"B:\Games\OtherGames\FS 2020\Airport Plates\All Plate PDFs\A\00007HR22.PDF");
      }
      public void PrintExceptionList(PagesException exe)
      {
         StringBuilder bd = new StringBuilder($"PDF Copy exception was thrown, {exe.Message}\n");
         foreach (var ex in exe.Exceptions)
         {
            bd.AppendLine(ex.Message);
         }
         MessageBox.Show(bd.ToString());
      }
      #endregion

      #region - Full Properties

      #endregion
   }
}
