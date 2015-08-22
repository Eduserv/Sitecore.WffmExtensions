using System.Linq;
using Sitecore.Abstractions;
using Sitecore.WFFM.Analytics.Model;

namespace Sitecore.WffmExtensions.Analytics.Providers
{
    public class AnalyticsDataProvider : WFFM.Analytics.Providers.AnalyticsDataProvider
    {
        private const string ProviderSettingsPrefix = "Sitecore.WffmExtensions.AnalyticsDataProvider.";

        public new void InsertForm(IFormData form)
        {
            ISettings settings = new SettingsWrapper();

            var autoStoreValues = settings.GetSetting(ProviderSettingsPrefix + "AutoStoreValues");
            var autoStoreDisabledReplacementValue = settings.GetSetting(ProviderSettingsPrefix + "AutoStoreDisabledReplacementValue");

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

            base.InsertForm(form);
        }
    }
}
