using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AirportPlates
{
   public class PagesException : Exception
   {
      public IEnumerable<Exception> Exceptions { get; set; }
      public PagesException()
      {
      }
      public PagesException(IEnumerable<Exception> exceptions)
      {
         Exceptions = exceptions;
      }

      public PagesException(string message) : base(message) { }
      public PagesException(string message, IEnumerable<Exception> exceptions) : base(message)
      {
         Exceptions = exceptions;
      }
   }
}
