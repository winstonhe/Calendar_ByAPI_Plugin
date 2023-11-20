using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;

namespace CalendarPlugin
{

    public class WorkHourPlugin : IPlugin
    {
       
        void IPlugin.Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService organizationProxy = factory.CreateOrganizationService(context.UserId);

            // Get the required configuration values (replace with your actual values).
            string clientId = "e47f53fe-c419-44e8-8526-62ebb4001eb1";
            string clientSecret = "#####";
            string tenantId = "d98fb82e-3c2c-4e5f-9c1d-c29bdd0be80c";
            string resource = "https://winston.crm5.dynamics.com";
            string apiUrl = "https://winston.crm5.dynamics.com/api/data/v9.1/calendars"; // Adjust the URL as needed


            // Create an instance of Dynamics365ApiService.
            var dynamics365ApiService = new CalendarService(clientId, clientSecret, tenantId, resource);

            //// Get the access token.
            //string accessToken = dynamics365ApiService.GetAccessToken().Result;

            // Perform actions with the access token (e.g., make API calls).
            string workHours = dynamics365ApiService.CreateWorkingHours().Result;

        }
    }
}
