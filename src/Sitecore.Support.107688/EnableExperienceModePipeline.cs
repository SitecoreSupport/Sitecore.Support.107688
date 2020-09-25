namespace Sitecore.Support.ExperienceExplorer.Business.Pipelines.HttpRequest.EnableExperienceModePipeline
{
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.HttpRequest;
    using Sitecore.Sites;
    using System;
    using Sitecore.Web;
    using Sitecore.ExperienceExplorer.Business.Helpers;
    using Sitecore.ExperienceExplorer.Business.Managers;
    using Sitecore.ExperienceExplorer.Business.Utilities;
    using Sitecore.ExperienceExplorer.Business.Constants;

    public class EnableExperienceModePipeline : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            WebUtil.SetCookieValue(SettingsHelper.AddOnQueryStringKey, "0");
            Assert.ArgumentNotNull(args, "args");
            if (!SettingsHelper.ExperienceModePipelineEnabled)
            {
                return;
            }
            if (!SettingsHelper.IsEnabledForCurrentSite)
            {
                if (Context.Site.DisplayMode == DisplayMode.Normal)
                {
                    Context.Site.SetDisplayMode(DisplayMode.Edit, DisplayModeDuration.Remember);
                }
                return;
            }
            bool flag = PageModeHelper.IsExperienceMode || (PageModeHelper.HasPermission && (args.LocalPath == Paths.Local.Controls.Editor.AbsolutePath || args.LocalPath == Paths.Local.Controls.Viewer.AbsolutePath || args.LocalPath.StartsWith(Paths.Local.Services.AbsolutePath)));
            string database = SiteContext.GetSite("website").SiteInfo.Database;
            if (!string.IsNullOrEmpty(database))
            {
                bool flag2 = ModuleManager.IsExpButtonClicked && string.Compare(Context.Database.Name, database, StringComparison.InvariantCultureIgnoreCase) == 0;
                if (flag || flag2)
                {
                    Database database2 = ExperienceExplorerUtil.ResolveContextDatabase();
                    Context.Site.Database = database2;
                    Context.Database = database2;
                    WebUtil.SetCookieValue(SettingsHelper.AddOnQueryStringKey, "1");
                }
            }
        }
    }
}