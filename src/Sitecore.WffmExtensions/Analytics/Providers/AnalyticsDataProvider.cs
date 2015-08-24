using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Abstractions;
using Sitecore.WFFM.Analytics.Model;
using Sitecore.WFFM.Analytics.Providers;
using Sitecore.WFFM.Analytics.Providers.Common;

namespace Sitecore.WffmExtensions.Analytics.Providers
{
    public class AnalyticsDataProvider : IWfmDataProvider
    {
        private readonly string _providerSettingsPrefix = "Sitecore.WffmExtensions.AnalyticsDataProvider.";
        private readonly WFFM.Analytics.Providers.AnalyticsDataProvider _defaultAnalyticsDataProvider = new WFFM.Analytics.Providers.AnalyticsDataProvider();

        public void InsertForm(IFormData form)
        {
            ISettings settings = new SettingsWrapper();

            var autoStoreValues = settings.GetSetting(_providerSettingsPrefix + "AutoStoreValues");
            var autoStoreDisabledReplacementValue = settings.GetSetting(_providerSettingsPrefix + "AutoStoreDisabledReplacementValue");

            bool storeValuesInSitecore = false;

            if (bool.TryParse(autoStoreValues, out storeValuesInSitecore) && !storeValuesInSitecore)
            {
                if (form != null && form.Fields != null && form.Fields.Any())
                {
                    form.Fields.ToList().ForEach(field =>
                    {
                        if (!string.IsNullOrEmpty(field.Data))
                        {
                            field.Data = autoStoreDisabledReplacementValue;
                        }
                        field.Value = autoStoreDisabledReplacementValue;
                    });
                }
            }

            _defaultAnalyticsDataProvider.InsertForm(form);
        }

        public IFormStatistics GetFormStatistics(Guid id)
        {
            return _defaultAnalyticsDataProvider.GetFormStatistics(id);
        }

        public IEnumerable<IFormFieldStatistics> GetFormFieldsStatistics(Guid id)
        {
            return _defaultAnalyticsDataProvider.GetFormFieldsStatistics(id);
        }

        public IEnumerable<IFormContactsResult> GetFormsStatisticsByContact(Guid formId, PageCriteria pageCriteria)
        {
            return _defaultAnalyticsDataProvider.GetFormsStatisticsByContact(formId, pageCriteria);
        }

        public IEnumerable<IFormData> GetFormData(Guid id)
        {
            return _defaultAnalyticsDataProvider.GetFormData(id);
        }
    }
}
