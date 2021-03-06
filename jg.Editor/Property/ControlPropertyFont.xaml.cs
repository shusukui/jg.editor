﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls.Primitives;

namespace jg.Editor.Property
{
    /// <summary>
    /// ControlPropertyFont.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPropertyFont : UserControl
    {
        FontCollection fontCollection = new FontCollection();

        public event RoutedPropertyChangedEventHandler<FontFamily> PropertyFontFamilyChanged = null;
        public event RoutedPropertyChangedEventHandler<double> PropertyFontSizeChanged = null;
        public event RoutedPropertyChangedEventHandler<bool> PropertyFontBoldChanged = null;
        public event RoutedPropertyChangedEventHandler<bool> PropertyFontItalicChanged = null;
        Control.ControlTextEditor controlTextEditor = null;

        private DesignerItem _source = null;
        public DesignerItem Source
        {
            get { return _source; }
            set
            {
                ToolboxItem toolBoxItem;
                
                _source = value;
                itemFontFamily = _source.FontFamily;
                itemFontSize = _source.FontSize;
                itemFontBold = _source.FontWeight == FontWeights.Bold ? true : false;
                itemFontItalic = _source.FontStyle == FontStyles.Italic ? true : false;

                toolBoxItem = _source.Content as ToolboxItem;

                if (toolBoxItem == null) return;
                controlTextEditor = null;
                if (toolBoxItem.Content is Control.ControlTextEditor)
                {
                    controlTextEditor = toolBoxItem.Content as Control.ControlTextEditor;
                    toggleBold.Visibility = System.Windows.Visibility.Collapsed;
                    toggleItalic.Visibility = System.Windows.Visibility.Collapsed;

                    btnCut.Visibility = System.Windows.Visibility.Visible;
                    btnCopy.Visibility = System.Windows.Visibility.Visible;
                    btnPaste.Visibility = System.Windows.Visibility.Visible;
                    btnUndo.Visibility = System.Windows.Visibility.Visible;
                    btnRedo.Visibility = System.Windows.Visibility.Visible;
                    btnBold.Visibility = System.Windows.Visibility.Visible;
                    btnItalic.Visibility = System.Windows.Visibility.Visible;
                    btnUnderline.Visibility = System.Windows.Visibility.Visible;
                    btnIncreaseFontSize.Visibility = System.Windows.Visibility.Visible;
                    btnDecreaseFontSize.Visibility = System.Windows.Visibility.Visible;
                    btnToggleBullets.Visibility = System.Windows.Visibility.Visible;
                    btnToggleNumbering.Visibility = System.Windows.Visibility.Visible;
                    btnAlignLeft.Visibility = System.Windows.Visibility.Visible;
                    btnAlignCenter.Visibility = System.Windows.Visibility.Visible;
                    btnAlignRight.Visibility = System.Windows.Visibility.Visible;
                    btnIncreaseIndentation.Visibility = System.Windows.Visibility.Visible;
                    btnDecreaseIndentation.Visibility = System.Windows.Visibility.Visible;

                    btnCut.CommandTarget = controlTextEditor.mainRTB;
                    btnCopy.CommandTarget = controlTextEditor.mainRTB;
                    btnPaste.CommandTarget = controlTextEditor.mainRTB;
                    btnUndo.CommandTarget = controlTextEditor.mainRTB;
                    btnRedo.CommandTarget = controlTextEditor.mainRTB;
                    btnBold.CommandTarget = controlTextEditor.mainRTB;
                    btnItalic.CommandTarget = controlTextEditor.mainRTB;
                    btnUnderline.CommandTarget = controlTextEditor.mainRTB;
                    btnIncreaseFontSize.CommandTarget = controlTextEditor.mainRTB;
                    btnDecreaseFontSize.CommandTarget = controlTextEditor.mainRTB;
                    btnToggleBullets.CommandTarget = controlTextEditor.mainRTB;
                    btnToggleNumbering.CommandTarget = controlTextEditor.mainRTB;
                    btnAlignLeft.CommandTarget = controlTextEditor.mainRTB;
                    btnAlignCenter.CommandTarget = controlTextEditor.mainRTB;
                    btnAlignRight.CommandTarget = controlTextEditor.mainRTB;
                    btnIncreaseIndentation.CommandTarget = controlTextEditor.mainRTB;
                    btnDecreaseIndentation.CommandTarget = controlTextEditor.mainRTB;
                }
                else
                {
                    toggleBold.Visibility = System.Windows.Visibility.Visible;
                    toggleItalic.Visibility = System.Windows.Visibility.Visible;

                    btnCut.Visibility = System.Windows.Visibility.Collapsed;
                    btnCopy.Visibility = System.Windows.Visibility.Collapsed;
                    btnPaste.Visibility = System.Windows.Visibility.Collapsed;
                    btnUndo.Visibility = System.Windows.Visibility.Collapsed;
                    btnRedo.Visibility = System.Windows.Visibility.Collapsed;
                    btnBold.Visibility = System.Windows.Visibility.Collapsed;
                    btnItalic.Visibility = System.Windows.Visibility.Collapsed;
                    btnUnderline.Visibility = System.Windows.Visibility.Collapsed;
                    btnIncreaseFontSize.Visibility = System.Windows.Visibility.Collapsed;
                    btnDecreaseFontSize.Visibility = System.Windows.Visibility.Collapsed;
                    btnToggleBullets.Visibility = System.Windows.Visibility.Collapsed;
                    btnToggleNumbering.Visibility = System.Windows.Visibility.Collapsed;
                    btnAlignLeft.Visibility = System.Windows.Visibility.Collapsed;
                    btnAlignCenter.Visibility = System.Windows.Visibility.Collapsed;
                    btnAlignRight.Visibility = System.Windows.Visibility.Collapsed;
                    btnIncreaseIndentation.Visibility = System.Windows.Visibility.Collapsed;
                    btnDecreaseIndentation.Visibility = System.Windows.Visibility.Collapsed;
                }
                

            }
        }

        #region FontFamily

        public FontFamily itemFontFamily
        {
            get
            {
                if (_source == null)
                    return null;
                else
                    return _source.FontFamily;
            }
            set
            {
                if (_source == null) return;
                FontFamily family = _source.FontFamily;
                cmbFontList.SelectionChanged -= new SelectionChangedEventHandler(cmbFontList_SelectionChanged);
                foreach (FontFamily v in cmbFontList.Items)
                {
                    if (v.ToString() == value.ToString()) { cmbFontList.SelectedItem = v; break; }
                }

                cmbFontList.SelectionChanged += new SelectionChangedEventHandler(cmbFontList_SelectionChanged);
            }
        }

        private void cmbFontList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (_source == null) return;

            if (controlTextEditor != null)
            {                
                if (cmb == null) return;
                if (cmb.SelectedItem == null) return;

                string fontfamily = cmb.SelectedItem.ToString();
                if (fontfamily == "") return;
                controlTextEditor.mainRTB.Selection.ApplyPropertyValue(Run.FontFamilyProperty, fontfamily);
            }
            else
            {
                _source.FontFamily = (FontFamily)e.AddedItems[0];

                if (PropertyFontFamilyChanged == null) return;
                PropertyFontFamilyChanged(_source, new RoutedPropertyChangedEventArgs<FontFamily>((FontFamily)e.RemovedItems[0], (FontFamily)e.AddedItems[0]));
            }
        }

        #endregion

        #region FontSize

        private double oldValue = 12;

        public double itemFontSize
        {
            get
            {
                if (_source == null)
                    return 9;
                else
                    return _source.FontSize;
            }
            set
            {
                if (_source == null) return;
                txtFontSize.TextChanged -= txtFontSize_TextChanged;
                // _source.FontSize = value;
                txtFontSize.Text = _source.FontSize.ToString();
                txtFontSize.TextChanged += txtFontSize_TextChanged;

            }
        }

        void txtFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            double fontsize;
            if (_source != null && txt != null)
            {
                if (double.TryParse(txt.Text, out fontsize))
                {
                    if (controlTextEditor != null)
                    {
                        controlTextEditor.mainRTB.Selection.ApplyPropertyValue(Run.FontSizeProperty, fontsize);
                    }
                    else
                    {
                        _source.FontSize = fontsize;

                        if (PropertyFontSizeChanged != null)
                            PropertyFontSizeChanged(_source, new RoutedPropertyChangedEventArgs<double>(oldValue, fontsize));
                    }
                }
                else
                {
                    txtFontSize.Text = oldValue.ToString();
                }
            }
        }

        private void txtFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
                double.TryParse(txt.Text, out oldValue);
        }

        #endregion

        #region Bold

        public bool itemFontBold
        {
            get
            {
                if (_source == null)
                    return false;
                else
                {
                    if (_source.FontWeight == FontWeights.Bold)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                if (_source == null) return;
                if (value)
                {
                    toggleBold.IsChecked = true;
                    // _source.FontWeight = FontWeights.Bold;
                }
                else
                {
                    toggleBold.IsChecked = false;
                    // _source.FontWeight = FontWeights.Normal;
                }

            }
        }

        private void toggleBold_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            
            if (tb == null) return;
            if (_source == null) return;

            if (tb.IsChecked == true)
            {
                _source.FontWeight = FontWeights.Bold;
                if (PropertyFontBoldChanged == null) return;
                PropertyFontBoldChanged(_source, new RoutedPropertyChangedEventArgs<bool>(tb.IsChecked == true ? false : true, tb.IsChecked == true ? true : false));
            }
            else
            {
                _source.FontWeight = FontWeights.Normal;
                if (PropertyFontBoldChanged == null) return;
                PropertyFontBoldChanged(_source, new RoutedPropertyChangedEventArgs<bool>(tb.IsChecked == true ? false : true, tb.IsChecked == true ? true : false));
            }
        }

        #endregion

        #region Italic

        public bool itemFontItalic
        {
            get
            {
                if (_source == null) 
                    return false;
                else
                {
                    if (_source.FontStyle == FontStyles.Italic)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                if (_source == null) return;
                if (value)
                {
                    toggleItalic.IsChecked = true;
                    // _source.FontStyle = FontStyles.Italic;
                }
                else
                {
                    toggleItalic.IsChecked = false;
                    // _source.FontStyle = FontStyles.Normal;
                }
            }
        }

        private void toggleItalic_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;

            if (tb == null) return;
            if (_source == null) return;

            if (tb.IsChecked == true)
            {
                _source.FontStyle = FontStyles.Italic;
                if (PropertyFontItalicChanged == null) return;
                PropertyFontItalicChanged(_source, new RoutedPropertyChangedEventArgs<bool>(tb.IsChecked == true ? false : true, tb.IsChecked == true ? true : false));
            }
            else
            {
                _source.FontStyle = FontStyles.Normal;
                if (PropertyFontItalicChanged == null) return;
                PropertyFontItalicChanged(_source, new RoutedPropertyChangedEventArgs<bool>(tb.IsChecked == true ? false : true, tb.IsChecked == true ? true : false));
            }
        }

        #endregion

        public ControlPropertyFont()
        {
            InitializeComponent();

            foreach (FontFamily v in fontCollection)
            {
                cmbFontList.Items.Add(v);
            }
        }
    }
}