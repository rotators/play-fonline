using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PlayFO
{
    // https://stackoverflow.com/questions/17615105/getting-text-entered-in-textbox-of-other-applications-using-c-sharp
    public static class Win32
    {
        // Delegate we use to call methods when enumerating child windows.
        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        // Callback method used to collect a list of child windows we need to capture text from.
        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            // Creates a managed GCHandle object from the pointer representing a handle to the list created in GetChildWindows.
            var gcHandle = GCHandle.FromIntPtr(pointer);

            // Casts the handle back back to a List<IntPtr>
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }

            // Adds the handle to the list.
            list.Add(handle);

            return true;
        }

        /*public static IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle)
        {
            FindWindowEx(parentHandle, childAfter, className, windowTitle);
        }*/

        // Returns an IEnumerable<IntPtr> containing the handles of all child windows of the parent window.
        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            // Create list to store child window handles.
            var result = new List<IntPtr>();

            // Allocate list handle to pass to EnumChildWindows.
            var listHandle = GCHandle.Alloc(result);

            try
            {
                // Enumerates though all the child windows of the parent represented by IntPtr parent, executing EnumChildWindowsCallback for each. 
                EnumChildWindows(parent, EnumChildWindowsCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                // Free the list handle.
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            // Return the list of child window handles.
            return result;
        }

        // Gets text text from a control by it's handle.
        private static string GetText(IntPtr handle)
        {
            const uint WM_GETTEXTLENGTH = 0x000E;
            const uint WM_GETTEXT = 0x000D;

            // Gets the text length.
            var length = (int)SendMessage(handle, WM_GETTEXTLENGTH, IntPtr.Zero, null);

            // Init the string builder to hold the text.
            var sb = new StringBuilder(length + 1);

            // Writes the text from the handle into the StringBuilder
            SendMessage(handle, WM_GETTEXT, (IntPtr)sb.Capacity, sb);

            // Return the text as a string.
            return sb.ToString();
        }


        public static bool WindowContainsTextString(IntPtr handle, string text)
        {
            var childWindows = GetChildWindows(handle);
            foreach (var childWindowText in childWindows.Select(GetText))
            {
                if (childWindowText.Contains(text))
                    return true;
            }
            return false;
        }

        public static bool WindowContainsTextString(string windowTitle, string text)
        {
            var sb = new StringBuilder();
            var windowHWnd = FindWindowByCaption(IntPtr.Zero, windowTitle);
            var childWindows = GetChildWindows(windowHWnd);
            foreach (var childWindowText in childWindows.Select(GetText))
            {
                if (childWindowText.Contains(text))
                    return true;
            }

            return false;
        }

        // Wraps everything together. Will accept a window title and return all text in the window that matches that window title.
        public static string GetAllTextFromWindowByTitle(string windowTitle)
        {
            var sb = new StringBuilder();
            // Find the main window's handle by the title.
            var windowHWnd = FindWindowByCaption(IntPtr.Zero, windowTitle);

            // Loop though the child windows, and execute the EnumChildWindowsCallback method
            var childWindows = GetChildWindows(windowHWnd);

            // For each child handle, run GetText
            foreach (var childWindowText in childWindows.Select(GetText))
            {
                // Append the text to the string builder.
                sb.Append(childWindowText);
            }

            // Return the windows full text.
            return sb.ToString();
        }
    }
}
