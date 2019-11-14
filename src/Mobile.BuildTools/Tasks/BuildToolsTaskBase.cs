﻿using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Tasks.Utils;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public abstract class BuildToolsTaskBase : Task, IBuildConfiguration
    {
        public bool BuildingInsideVisualStudio { get; set; }

        [Required]
        public string ConfigurationPath { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public string SdkShortFrameworkIdentifier { get; set; }

        IDictionary<string, string> IBuildConfiguration.GlobalProperties => BuildEngine.GetGlobalProperties();

        ILog IBuildConfiguration.Logger =>
            (BuildHostLoggingHelper)Log;

        BuildToolsConfig IBuildConfiguration.Configuration =>
            ConfigHelper.GetConfig(ConfigurationPath);

        Platform IBuildConfiguration.Platform =>
            SdkShortFrameworkIdentifier.GetTargetPlatform();

        public sealed override bool Execute()
        {
            try
            {
                ExecuteInternal(this);
            }
            catch (Exception ex)
            {
                Log.LogError($"Unhandled error while executing {GetType().Name}");
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }

        void IBuildConfiguration.SaveConfiguration() =>
            ConfigHelper.SaveConfig(((IBuildConfiguration)this).Configuration, ConfigurationPath);

        internal abstract void ExecuteInternal(IBuildConfiguration config);
    }
}