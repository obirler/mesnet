﻿#pragma checksum "..\..\..\..\Xaml\User Controls\Beam.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "321B4DD159880B9FCD8A171C8D835894"
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


namespace Mesnet.Xaml.User_Controls {
    
    
    /// <summary>
    /// Beam
    /// </summary>
    public partial class Beam : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.RotateTransform rotateTransform;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid contentgrid;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas upcanvas;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas directionarrow;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle core;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas downcanvas;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line startcirclepoint;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line endcirclepoint;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Line centerbeampoint;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse startcircle;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\Xaml\User Controls\Beam.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse endcircle;
        
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
            System.Uri resourceLocater = new System.Uri("/Mesnet;component/xaml/user%20controls/beam.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Xaml\User Controls\Beam.xaml"
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
            this.rotateTransform = ((System.Windows.Media.RotateTransform)(target));
            return;
            case 2:
            this.contentgrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.upcanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.directionarrow = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.core = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 6:
            this.downcanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.startcirclepoint = ((System.Windows.Shapes.Line)(target));
            return;
            case 8:
            this.endcirclepoint = ((System.Windows.Shapes.Line)(target));
            return;
            case 9:
            this.centerbeampoint = ((System.Windows.Shapes.Line)(target));
            return;
            case 10:
            this.startcircle = ((System.Windows.Shapes.Ellipse)(target));
            return;
            case 11:
            this.endcircle = ((System.Windows.Shapes.Ellipse)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
