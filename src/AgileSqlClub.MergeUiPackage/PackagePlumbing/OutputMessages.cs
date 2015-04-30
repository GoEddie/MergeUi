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
        private static Guid _guidWindow;

        static OutputWindowMessage()
        {
            _guidWindow = new Guid();
        } 
    
        public static void WriteMessage(string format, params object[] parameters)
        {
            WriteMessage(string.Format(format, parameters));
        }

      
        public static void WriteMessage(string message)
        {
            try
            {
                var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                
                IVsOutputWindowPane pane;
                var hr = outputWindow.CreatePane(_guidWindow, "MergeUi", 1, 0);
                hr = outputWindow.GetPane(_guidWindow, out pane);
                pane.Activate();
                pane.OutputString(message + "\r\n");
               
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
                
                IVsOutputWindowPane pane;
                var hr = outputWindow.CreatePane(_guidWindow, "MergeUi", 1, 0);
                hr = outputWindow.GetPane(_guidWindow, out pane);
                pane.Activate();
                //pane.OutputString("MergeUi: " + message + "\r\n");
               var item = pane.OutputTaskItemString( string.Format(format, parameters) + "\r\n",
                    VSTASKPRIORITY.TP_NORMAL, VSTASKCATEGORY.CAT_BUILDCOMPILE, "MergeUi", 0, path, (uint)line, string.Format(format, parameters));
                pane.Activate();

                
            }
            catch (Exception e)
            {

            }
        }

    }
}
