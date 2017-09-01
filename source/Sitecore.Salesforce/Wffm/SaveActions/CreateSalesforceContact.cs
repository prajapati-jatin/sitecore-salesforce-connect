using Sitecore.WFFM.Actions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.WFFM.Abstractions.Actions;
using Sitecore.Configuration;
using Sitecore.Integration.Common.Providers;
using Sitecore.Salesforce.Client.Data;

namespace Sitecore.Salesforce.Wffm.SaveActions
{
    public class CreateSalesforceContact : WffmSaveAction
    {
        public struct Fields
        {
            public static String FirstName = "{1E2FD989-3C64-4E6F-8848-140868D39D99}";
            public static String LastName = "{5B87388F-77CD-4861-B49D-50B6B6637C61}";
            public static String Email = "{203DF09C-3E30-4FC0-924B-51906EFD5354}";
            public static String JobTitle = "{517C783B-C137-4D06-910A-4CAE63DE03F9}";
            public static String BirthDate = "{92E94BF7-211A-402E-A0DC-830AC8DA511F}";
            public static String Phone = "{19737B71-6EDE-4E6C-A546-C63F6F780E8D}";
        }

        public override void Execute(ID formId, AdaptedResultList adaptedFields, ActionCallContext actionCallContext = null, params object[] data)
        {
            AdaptedControlResult firstName = adaptedFields.GetEntry(Fields.FirstName, "First name");
            AdaptedControlResult lastName = adaptedFields.GetEntry(Fields.LastName, "Last name");
            AdaptedControlResult email = adaptedFields.GetEntry(Fields.Email, "Email");
            AdaptedControlResult jobTitle = adaptedFields.GetEntry(Fields.JobTitle, "Job title");
            AdaptedControlResult birthDate = adaptedFields.GetEntry(Fields.BirthDate, "Birth date");
            AdaptedControlResult phone = adaptedFields.GetEntry(Fields.Phone, "Phone");

            ProviderHelper<SalesforceProvider, ProviderCollection<SalesforceProvider>>.DefaultProvider = new SalesforceProvider();
            var configuration = SalesforceManager.GetConfiguration("salesforce");

            var salesForceClient = configuration.Client;
            var contactsApi = configuration.Api.ContactsApi;
            var fieldMapping = configuration.FieldMapping;

            var newContactProperties = new Dictionary<String, object>
            {
                { "FirstName", firstName.Value },
                { "LastName", lastName.Value },
                { "Email", email.Value },
                { "Phone", phone.Value },
                { "Title", jobTitle.Value },
                { "Birthdate", Sitecore.DateUtil.ParseDateTime(birthDate.Value, DateTime.MinValue).ToUniversalTime().ToString("s") },
                { "SC_Password__c", "12345" },
                { "SC_PasswordQuestion__c", "Password questions" },
                { "SC_PasswordAnswer__c", "Password answer" },
                { "SC_IsApproved__c", false }
            };

            OperationResult result = null;
            try
            {
                result = contactsApi.Create(newContactProperties);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(ex.ToString(), this);
            }
            if (result.Success)
            {
                Sitecore.Diagnostics.Log.Info("contact created", this);
            }
        }
    }
}
