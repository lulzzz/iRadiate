using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using System.Windows.Controls;

namespace iRadiate.Desktop.Common.Tools
{
    public interface ITool
    {
      
        /// <summary>
        /// Name of the tool to appear 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Instructions for the User
        /// </summary>
        string ToolTipText { get; }

        /// <summary>
        /// Saves changes made to the tool
        /// </summary>
        RelayCommand SaveCommand { get; set; }

        void UIThreadInitialise();

        void NonUIThreadInitialise();

        [Obsolete]
        bool Available { get; set; }

        /// <summary>
        /// This is the tool itself
        /// </summary>
        /// <remarks>
        /// Will usually be a button with the IconContent inside
        /// but could be a toggle button, or a drop down button
        /// or anything
        /// </remarks>
        ContentControl ToolContent { get; }
    }

   

   
}
