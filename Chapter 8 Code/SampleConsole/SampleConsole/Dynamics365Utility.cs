using System;
using System.Collections.Generic;

using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.IO;
using System.Data;

namespace SampleConsole
{
    class Dynamics365Utility
    {
        IOrganizationService service = null;

        public Dynamics365Utility()
        {
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = ConfigurationManager.AppSettings["UserName"];
            credentials.UserName.Password = ConfigurationManager.AppSettings["Password"]; 
            Uri serviceUri = new Uri(ConfigurationManager.AppSettings["OrgURL"]);

            OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            service = (IOrganizationService)proxy;
        }


        public void AddModel()
        {
            //get model file path
            DataTable models = GetCSVFile("Model");
            //process medle file records
            foreach (DataRow row in models.Rows)
            {
                if (row["Make"].ToString() != "" && row["Model\r"].ToString() != "")
                {
                    //create model
                    Entity model = new Entity("him_model");
                    model["him_name"] = row["Model\r"].ToString();
                    //get make record id
                    Guid makeId = GetMake(row["Make"].ToString());
                    if (makeId != Guid.Empty)
                    {
                        model["him_make"] = new EntityReference("him_make", GetMake(row["Make"].ToString()));
                    }
                    service.Create(model);

                }
            }
        }
        private Guid GetMake(string make)
        {
            EntityCollection results = null;
            Guid Id = Guid.Empty;
            QueryExpression query = new QueryExpression()
            {

                EntityName = "him_make",
                ColumnSet = new ColumnSet(new string[] { "him_name" }),

                Criteria =
                            {
                                Filters =
                                {
                                    new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                        {
                                          new ConditionExpression("him_name",ConditionOperator.Equal,make)
                                        },
                                    }
                                }
                            }
            };
           results=service.RetrieveMultiple(query);
           if(results.Entities.Count>0)
                Id=results.Entities.FirstOrDefault().Id;
            return Id;
        }
        public static string GetRowValue(string value)
        {
            //check for \r\n
            if (value.Contains('\r') || value.Contains('\n'))
            {
                //remove \r or \n
                value = value.Substring(0, value.Length - 1);
            }

            return value;
        }
        public void AddMakers()
        {
            //get csv file data into data table
            DataTable makers = GetCSVFile("Makers");
           //loop all rows and get their data
            foreach (DataRow row in makers.Rows)
            {
                if(row["Name\r"].ToString()!="")
                {
                    //Create Make record
                    Entity make = new Entity("him_make");
                    make["him_name"] = GetRowValue(row["Name\r"].ToString());
                    service.Create(make);
                }

            }
        }
        private DataTable GetCSVFile(string keyname)
        {
            string path = ConfigurationManager.AppSettings[keyname];
            string fileText = string.Empty;
            DataTable source = new DataTable();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        fileText = sr.ReadToEnd().ToString();   
                        string[] rows = fileText.Split('\n'); 
                        for (int i = 0; i < rows.Count() - 1; i++)
                        {
                            string[] rowValues = rows[i].Split(','); 
                            {
                               
                                if (i == 0)
                                {
                                    for (int j = 0; j < rowValues.Count(); j++)
                                    {
                                        source.Columns.Add(rowValues[j]);  
                                    }
                                }
                                else
                                {
                                    DataRow dr = source.NewRow();
                                    for (int k = 0; k < rowValues.Count(); k++)
                                    {
                                        dr[k] = rowValues[k].ToString();
                                    }
                                    source.Rows.Add(dr);  
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return source;
        }

    }
}
