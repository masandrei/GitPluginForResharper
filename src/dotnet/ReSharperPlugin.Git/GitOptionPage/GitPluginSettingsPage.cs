using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Daemon.OptionPages;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.Rider.Model.UIAutomation;

namespace ReSharperPlugin.Git;

[OptionsPage(PID, PageTitle, typeof(UnitTestingThemedIcons.Session), ParentId = CodeInspectionPage.PID)]
public class GitPluginSettingsPage : BeSimpleOptionsPage
{
    private const string PID = nameof(GitPluginSettingsPage);
    private const string PageTitle = "ReSharper SDK";

    private readonly Lifetime _lifetime;

    public GitPluginSettingsPage(Lifetime lifetime,
        OptionsPageContext optionsPageContext,
        OptionsSettingsSmartContext optionsSettingsSmartContext)
        : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
    {
        _lifetime = lifetime;

        AddIntOption((GitPluginSettings s) => s.LastGitCommits, "Number of last git commits to show");
    }

    private BeControl AddIntOption<TKeyClass>(Expression<Func<TKeyClass, int>> lambdaExpression, string description)
    {
        var property = new Property<int>(description);
        OptionsSettingsSmartContext.SetBinding(_lifetime,lambdaExpression, property);
        var control = new BeTextBox(new BeTextBoxSettings())
        {
            Text =  {Value = property.Value.ToString()}
        };
        
        control.Text.Change.Advise(_lifetime, newValue =>
        {
            if (int.TryParse(newValue, out int value))
            {
                value = Math.Max(0, value);
                control.Text.Value = value.ToString();
                property.Value = value;
            }
        });
        
        AddControl(control.WithDescription(description, _lifetime));
        return control;
    }
    
}