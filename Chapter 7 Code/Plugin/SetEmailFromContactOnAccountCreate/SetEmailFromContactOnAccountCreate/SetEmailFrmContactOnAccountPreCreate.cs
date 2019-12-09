using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SetEmailFromContactOnAccountCreate
{
    public class SetEmailFrmContactOnAccountPreCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext pluginContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = serviceFactory.CreateOrganizationService(pluginContext.UserId);
            if (pluginContext.InputParameters.Contains("Target") && pluginContext.InputParameters["Target"] is Entity)
            {
                try
                {
                    Entity account = (Entity)pluginContext.InputParameters["Target"];
                    if (account.LogicalName != "account")
                    {
                        return;
                    }
                    if (account.Contains("primarycontactid"))
                    {
                        Entity contact = orgService.Retrieve("contact", account.GetAttributeValue<EntityReference>("primarycontactid").Id, new ColumnSet(new string[] { "emailaddress1" }));
                        if (contact.Contains("emailaddress1"))
                            account.Attributes.Add("emailaddress1", contact.GetAttributeValue<string>("emailaddress1"));
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    tracingService.Trace(ex.Message + " : " + ex.StackTrace);
                    throw ex;
                }
                catch (Exception e)
                {
                    tracingService.Trace(e.Message + " : " + e.StackTrace);
                    throw e;
                }
            }
        }
    }
}
