using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Dynamics365Utility utility = new Dynamics365Utility();
            Console.WriteLine("Importing HIMBAP Auto Service Make and Models Data.....");
            
            utility.AddMakers();
            Console.WriteLine("Maker data imported correctly");

            utility.AddModel();
            Console.WriteLine("Models data imported correctly");

            Console.ReadLine();
        }
    }
}
