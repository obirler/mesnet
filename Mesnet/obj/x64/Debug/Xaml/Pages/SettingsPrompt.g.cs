#pragma checksum "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6B7B1013F2E608F734C7DD804B6965D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// SettingsPrompt
    /// </summary>
    public partial class SettingsPrompt : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton englishbtn;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton turkishbtn;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox calculationcbx;
        
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
            System.Uri resourceLocater = new System.Uri("/Mesnet;component/xaml/pages/settingsprompt.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
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
            this.englishbtn = ((System.Windows.Controls.RadioButton)(target));
            
            #line 11 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
            this.englishbtn.Checked += new System.Windows.RoutedEventHandler(this.englishbtn_Checked);
            
            #line default
            #line hidden
            return;
            case 2:
            this.turkishbtn = ((System.Windows.Controls.RadioButton)(target));
            
            #line 12 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
            this.turkishbtn.Checked += new System.Windows.RoutedEventHandler(this.turkishbtn_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.calculationcbx = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\..\..\..\Xaml\Pages\SettingsPrompt.xaml"
            this.calculationcbx.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.calculationcbx_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

