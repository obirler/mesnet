﻿#pragma checksum "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A1F3433CF52BC835A599B6CE3B5942DB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Mesnet.Xaml.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Mesnet.Xaml.Pages {
    
    
    /// <summary>
    /// MessagePrompt
    /// </summary>
    public partial class MessagePrompt : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Message;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button yesbtn;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button nobtn;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelbtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Mesnet;component/xaml/pages/messageprompt.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Message = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.yesbtn = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
            this.yesbtn.Click += new System.Windows.RoutedEventHandler(this.yesbtn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.nobtn = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
            this.nobtn.Click += new System.Windows.RoutedEventHandler(this.nobtn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cancelbtn = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\..\..\Xaml\Pages\MessagePrompt.xaml"
            this.cancelbtn.Click += new System.Windows.RoutedEventHandler(this.cancelbtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

