package com.jetbrains.rider.plugins.git.options

import com.jetbrains.rider.plugins.git.OptionPagesBundle
import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class GitPluginSettingsPage: SimpleOptionsPage(
    name = OptionPagesBundle.message("configurable.name.optionpages.options.title"),
    pageId = "GitPluginSettingsPage"// Must be in sync with SamplePage.PID
) {
    override fun getId(): String {
        return "GitPluginSettingsPage"
    }
}
