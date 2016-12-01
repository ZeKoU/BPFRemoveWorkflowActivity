using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace BPFRemoveWorkflowActivity
{

    
    /**
     * In order to remove Business Process Flow (BPF) from existing records, it is not enough to just deactivate BPF.
     * Even when we deactivate BPF, records that are associated with it will still show it with warning message that it is deactivated.
     * 
     * It is true, if you delete BPF from Processes in CRM, they will be removed (not showed) on the records that were associated with that BPF.
     * However, what if you don't want to delete default BPF, like on system entities - Leads, Opportunities and Accounts?
     * 
     * This code activity is responsible for removing existing association of the Opportunity entity record with BPF.
     * Opportunity is picked just as an example, but same principle can be used with other entities.
     * 
     **/
    public sealed class BPFRemoveWorkflowActivity : CodeActivity
    {

        /*Context record GUID*/
        private Guid PrimaryEntityID;

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            #region Get required params and services

            // Create the tracing service
            ITracingService tracingService = context.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered WF Activity Execute() method, Activity Instance Id: {0}, Workflow Instance Id: {1}",
                context.ActivityInstanceId,
                context.WorkflowInstanceId);

            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

            if (workflowContext == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("Correlation Id: {0}, Initiating User: {1}",
                workflowContext.CorrelationId,
                workflowContext.InitiatingUserId);

            PrimaryEntityID = workflowContext.PrimaryEntityId;
            tracingService.Trace("Primary Entity ID: " + PrimaryEntityID);

            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            #endregion


            #region Get entity's BPF Process ID
            // Create a column set to define which attributes should be retrieved.
            ColumnSet attributes = new ColumnSet(new string[] { "name", "ownerid", "processid", "opportunityid" });
            Entity opportunity = service.Retrieve("opportunity", PrimaryEntityID, attributes);

            //log all attributes
            tracingService.Trace("Retreived Opportunity entity...");
            tracingService.Trace("Printing attributes... ");
            foreach (KeyValuePair<String, Object> attribute in opportunity.Attributes)
            {
                tracingService.Trace(attribute.Key + ": " + attribute.Value);
            }
            #endregion


            #region Update records ProcessId to remove association with BPF
            tracingService.Trace("Removing BPF association from record - setting 'processid' attribute to null!");
            //Set ProcessId attribute to null
            opportunity["processid"] = null;
            //Update record in CRM
            service.Update(opportunity);
            tracingService.Trace("Record updated");
            #endregion


        }
    }
}