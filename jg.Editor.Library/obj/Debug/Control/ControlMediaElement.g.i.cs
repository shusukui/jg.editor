﻿#pragma checksum "..\..\..\Control\ControlMediaElement.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CCD9C1A282DEF7895AD115CBDA14A90E34E62C3C"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
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
using System.Windows.Forms.Integration;
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
using jg.Editor.Library.Control;


namespace jg.Editor.Library.Control {
    
    
    /// <summary>
    /// ControlMediaElement
    /// </summary>
    public partial class ControlMediaElement : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 73 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement mediaElement;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridControl;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar progressBar;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal jg.Editor.Library.Control.ControlSlider slider;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btnZoom;
        
        #line default
        #line hidden
        
        
        #line 156 "..\..\..\Control\ControlMediaElement.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPlay;
        
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
            System.Uri resourceLocater = new System.Uri("/jg.Editor.Library;component/control/controlmediaelement.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Control\ControlMediaElement.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.mediaElement = ((System.Windows.Controls.MediaElement)(target));
            
            #line 74 "..\..\..\Control\ControlMediaElement.xaml"
            this.mediaElement.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.mediaElement_PreviewMouseUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gridControl = ((System.Windows.Controls.Grid)(target));
            
            #line 78 "..\..\..\Control\ControlMediaElement.xaml"
            this.gridControl.MouseEnter += new System.Windows.Input.MouseEventHandler(this.progressBar_MouseEnter);
            
            #line default
            #line hidden
            
            #line 79 "..\..\..\Control\ControlMediaElement.xaml"
            this.gridControl.MouseLeave += new System.Windows.Input.MouseEventHandler(this.progressBar_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 3:
            this.progressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 4:
            this.slider = ((jg.Editor.Library.Control.ControlSlider)(target));
            return;
            case 5:
            this.btnZoom = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            
            #line 105 "..\..\..\Control\ControlMediaElement.xaml"
            this.btnZoom.Click += new System.Windows.RoutedEventHandler(this.btnZoom_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnPlay = ((System.Windows.Controls.Button)(target));
            
            #line 157 "..\..\..\Control\ControlMediaElement.xaml"
            this.btnPlay.Click += new System.Windows.RoutedEventHandler(this.btnPlay_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

