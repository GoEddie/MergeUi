using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace AgileSqlClub.MergeUI.PackagePlumbing
{
    public static class OutputWindowMessage
    {
        public static void WriteMessage(string format, params object[] parameters)
        {
            WriteMessage(string.Format(format, parameters));
        }

      
        public static void WriteMessage(string message)
        {
            try
            {
                var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

                var guidGeneral = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
                IVsOutputWindowPane pane;
                var hr = outputWindow.CreatePane(guidGeneral, "General", 1, 0);
                hr = outputWindow.GetPane(guidGeneral, out pane);
                pane.Activate();
                pane.OutputString("MergeUI: " + message + "\r\n");
               
                pane.Activate();



            }
            catch (Exception e)
            {

            }
        }

        public static void WriteWarning(string path, int line, string format, params object[] parameters)
        {
            try
            {
                var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                
                var guidGeneral = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
                IVsOutputWindowPane pane;
                var hr = outputWindow.CreatePane(guidGeneral, "General", 1, 0);
                hr = outputWindow.GetPane(guidGeneral, out pane);
                pane.Activate();
                //pane.OutputString("MergeUI: " + message + "\r\n");
               var item = pane.OutputTaskItemString("MergeUI: " + string.Format(format, parameters) + "\r\n",
                    VSTASKPRIORITY.TP_NORMAL, VSTASKCATEGORY.CAT_BUILDCOMPILE, "MergeUI", 0, path, (uint)line, "MergeUI: " + string.Format(format, parameters));
                pane.Activate();

                
            }
            catch (Exception e)
            {

            }
        }

    }
}
