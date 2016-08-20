using Ex.Audit.ExternalSources.InitialStaticInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources._STARTUP_
{
    public class AuditExternalSourceStartUp
    {
        public static void Main(string[] args)
        {
            KeyInformation.Collect();
            TryExcuteProcedures.Try();


            Console.ReadLine();
        }

    }
}