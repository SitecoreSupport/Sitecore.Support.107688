namespace Sitecore.Support.ExperienceExplorer.Business.Pipelines.HttpRequest.EnableExperienceModePipeline
{
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceExplorer.Business.Constants;
    using Sitecore.ExperienceExplorer.Business.Helpers;
    using Sitecore.ExperienceExplorer.Business.Managers;
    using Sitecore.ExperienceExplorer.Business.Utilities;
    using Sitecore.Pipelines.HttpRequest;
    using Sitecore.Publishing;
    using Sitecore.Sites;
    using Sitecore.Web;
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class InjectExperienceExplorerControlPipeline 
    {
        public void Process(HttpRequestArgs args)
        {
            if (SettingsHelper.ExperienceModePipelineEnabled && Context.Item != null)
            {
                bool flag = ModuleManager.IsExpButtonClicked || PageModeHelper.IsExperienceMode;
                if (flag && !ExperienceExplorerUtil.CurrentTicketIsValid())
                {
                    PageModeHelper.RedirectToLoginPage();
                }
                if (!Context.IsLoggedIn)
                {
                    PreviewManager.RestoreUser();
                }
                if (!Context.IsLoggedIn && WebUtil.GetQueryStringOrCookie(SettingsHelper.AddOnQueryStringKey) == "1")
                {
                    SiteContext site = Factory.GetSite("login");
                    WebUtil.Redirect(site.VirtualFolder);
                }
                else if (SettingsHelper.IsEnabledForCurrentSite && Context.Site.DisplayMode == DisplayMode.Normal)
                {
                    try
                    {
                        WebUtil.SetCookieValue(SettingsHelper.AddOnQueryStringKey, flag ? "1" : "0");
                        if (!flag)
                        {
                            goto IL_0142;
                        }
                        SettingsHelper.ExplorerWasAccessed = true;
                        Control control = WebUtil.FindControlOfType(Context.Page.Page, typeof(HtmlForm));
                        if (control != null)
                        {
                            Control child = Context.Page.Page.LoadControl(Paths.Module.Controls.GlobalHeaderPath);
                            control.Controls.AddAt(0, child);
                            Sitecore.ExperienceExplorer.Business.WebControls.ExperienceExplorer child2 = new Sitecore.ExperienceExplorer.Business.WebControls.ExperienceExplorer();
                            control.Controls.Add(child2);
                            ModuleManager.IsExpButtonClicked = false;
                            HttpContext.Current.Items["IsExperienceMode"] = null;
                            EnsureFirstLoad();
                            if (!UserHelper.IsVirtualUser(Context.User.Name))
                            {
                                UserHelper.AuthentificateVirtualUser(Context.User.Name);
                            }
                            goto IL_0142;
                        }
                        goto end_IL_0083;
                        IL_0142:
                        if (Context.PageMode.IsPreview || Context.PageMode.IsPageEditor || Context.PageMode.IsDebugging)
                        {
                            UserHelper.AuthentificateRealUser();
                        }
                        end_IL_0083:;
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("code blocks"))
                        {
                            Log.Error("Inject Experience Explorer Control: ", ex, this);
                        }
                    }
                }
            }

        }
        private void EnsureFirstLoad()
        {
            if (HttpContext.Current.Session["IsFirstTime"] == null && !UserHelper.IsVirtualUser(Context.User.Name))
            {
                HttpContext.Current.Session["IsFirstTime"] = true;
            }
            else
            {
                HttpContext.Current.Session["IsFirstTime"] = false;
            }
        }

    }
}