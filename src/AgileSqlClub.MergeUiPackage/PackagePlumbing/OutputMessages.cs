using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace AgileSqlClub.MergeUi.PackagePlumbing
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

    }
}
