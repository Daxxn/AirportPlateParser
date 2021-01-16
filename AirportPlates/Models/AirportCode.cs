using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportPlates.Models
{
   public class AirportCode
   {
      #region - Fields & Properties
      public string CityState { get; set; }
      public string CountryCode { get; set; }
      public string IATACode { get; set; }
      public string Other { get; set; }
      #endregion

      #region - Constructors
      public AirportCode() { }
      #endregion

      #region - Methods
      public override string ToString()
      {
         return $"({CountryCode}{IATACode}) {CityState}{(Other != null ? $" - {Other}" : "")}";
      }

      public void QuickClean()
      {
         CityState = CityState.Trim('-', ' ');
         CityState = CityState.Trim();
         CityState = CityState.Replace("  ", " ");

         IATACode = IATACode.Trim();

         if (Other != null)
         {
            Other = Other.Trim();
            Other = Other.Replace(" - ", null);
            Other = Other.Replace("  ", " ");
         }
      }
      #endregion

      #region - Full Properties
      public string FullCode
      {
         get
         {
            return $"{CountryCode}{IATACode}";
         }
      }
      #endregion
   }
}
