﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using CSharpAchiever.VSX;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Strokes.BasicAchievements;
using Strokes.Core;
using Strokes.Core.Integration;
using Strokes.Data;
using Strokes.GUI;
using System.Linq;
using Strokes.VSX.Trackers;

namespace Strokes.VSX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]

    [ProvideAutoLoad("{adfc4e64-0397-11d1-9f4e-00a0c911004f}")]
    
    [ProvideToolWindow(typeof(AchievementStatisticsToolWindow), Style = VsDockStyle.MDI)]
    [ProvideService(typeof(IAchevementLibraryService))]

    [Guid(GuidList.guidCSharpAchiever_Achiever_VSIXPkgString)]
    public sealed class StrokesVsxPackage : Package, IAchevementLibraryService
    {
        public StrokesVsxPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        //Solution build manager service, and it's cookie.
        public IVsSolutionBuildManager2 Sbm = null;
        private uint _updateSolutionEventsCookie;

        public IVsStatusbar statusBar = null;

        private BuildTracker _buildTracker;

        //DTE object
        private DTE dte;

        #region VSX Initialization

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();
            
            //Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.guidCSharpAchiever_Achiever_VSIXCmdSet, (int)PkgCmdIDList.showAchievementIndex);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }

            //Obtain DTE service.
            dte = (DTE)GetService(typeof(DTE));

            //Subscribe to Build events
            Sbm = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
            if (Sbm != null)
            {
                _buildTracker = new BuildTracker(dte);
                Sbm.AdviseUpdateSolutionEvents(_buildTracker, out _updateSolutionEventsCookie);
            }

            //Statusbar
            statusBar = GetService(typeof (SVsStatusbar)) as IVsStatusbar;

            //Promote the Achievement Library service
            var serviceContainer = (IServiceContainer)this;
            serviceContainer.AddService(typeof (IAchevementLibraryService), this, true);

            //Initialize Basic achievements
            RegisterAchievementAssembly(typeof(Strokes_BasicAchievements_VSXPackage).Assembly);

            GuiInitializer.Initialize();
            AchievementContext.AchievementClicked += new AchievementContext.AchievementClickedHandler(AchievementContext_AchievementClicked);
            DetectionDispatcher.DetectionCompleted += new EventHandler<DetectionCompletedEventArgs>(DetectionDispatcher_DetectionCompleted);
        }

        void DetectionDispatcher_DetectionCompleted(object sender, DetectionCompletedEventArgs e)
        {
            statusBar.SetText(string.Format("{0} achievements tested in {1} milliseconds", e.AchievementsTested, e.ElapsedMilliseconds));
        }

        void AchievementContext_AchievementClicked(object sender, AchievementClickedEventArgs args)
        {
            dte.ItemOperations.OpenFile(args.AchievementDescriptor.CodeLocation.FileName, EnvDTE.Constants.vsViewKindCode);
            //TODO: Make this also ensure that the lines that unlocks the achievement becomes visible within the ActiveDocument (scroll to it).
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Unadvise all events 
            if (Sbm != null && _updateSolutionEventsCookie != 0)
                Sbm.UnadviseUpdateSolutionEvents(_updateSolutionEventsCookie);
        } 

        #endregion

        #region Tools menu item and Achievement Pane activation

        /// <summary>
        /// Called when the Tools -> C# Achievements menu is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ShowAchievementPane(true);
        }

        private void ShowAchievementPane(bool activate)
        {
            var wnd = CreateToolWindow();
            var windowFrame = (IVsWindowFrame)wnd.Frame;
            if (activate)
                windowFrame.Show();
            else
                windowFrame.ShowNoActivate();
        }

        private AchievementStatisticsToolWindow CreateToolWindow()
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var window = (AchievementStatisticsToolWindow)this.FindToolWindow(typeof(AchievementStatisticsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Can't show!");
            }

            return window;
        }

        #endregion

        /// <summary>
        /// Called by third party extensions to register their achievement assemblies.
        /// </summary>
        /// <param name="assembly">Assembly to scan for achievements</param>
        public void RegisterAchievementAssembly(Assembly assembly)
        {
            var achievementDescriptorRepository = new AchievementDescriptorRepository(); //TODO: Resolve with IoC
            achievementDescriptorRepository.LoadFromAssembly(assembly);
        }
    }
}
