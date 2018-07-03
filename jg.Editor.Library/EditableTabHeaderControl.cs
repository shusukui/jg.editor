namespace jg.Editor.Library
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Windows.Data;
    /// <summary>
    /// Header Editable TabItem
    /// </summary>
    [TemplatePart(Name = "PART_TabHeader", Type = typeof(TextBox))]
    public class EditableTabHeaderControl : ContentControl
    {
        /// <summary>
        /// Dependency property to bind EditMode with XAML Trigger
        /// </summary>
        private static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTabHeaderControl));
        public delegate void OnEditEnd(bool IsSubmit,object tvItem);
        public event OnEditEnd EditEnd = null;
        private TextBox textBox;
        private string oldText;
        private DispatcherTimer timer;
        private delegate void FocusTextBox();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)this.GetValue(IsInEditModeProperty);
            }
            set
            {
                this.SetValue(IsInEditModeProperty, value);
                if (string.IsNullOrEmpty(this.textBox.Text))
                {
                    this.textBox.Text = this.oldText;
                }

                this.oldText = this.textBox.Text;
                
                this.timer.Start();
            }
        }

        private static void IsInEditModeProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.textBox = this.Template.FindName("PART_TabHeader", this) as TextBox;
            if (this.textBox != null)
            {
                this.timer = new DispatcherTimer();
                this.timer.Tick += TimerTick;
                this.timer.Interval = TimeSpan.FromMilliseconds(1);
                this.LostFocus += TextBoxLostFocus;
                this.textBox.KeyDown += TextBoxKeyDown;
                
            }
        }

        public EditableTabHeaderControl()
        {
            this.MouseDoubleClick +=new MouseButtonEventHandler(EditableTabHeaderControl_MouseDoubleClick);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(EditableTabHeaderControl_DataContextChanged);
        }

        void EditableTabHeaderControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemInfo info = e.NewValue as TreeViewItemInfo;

            TreeViewItemInfo oldInfo = e.NewValue as TreeViewItemInfo;

            //if (oldInfo != null)
            //    oldInfo.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(info_PropertyChanged);
            //if (info != null)
            //    info.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(info_PropertyChanged);
            if (info == null) return;
            Binding binding = new Binding();
            binding.Source = info;
            binding.Mode = BindingMode.TwoWay;
            binding.Path = new PropertyPath("IsEdit");
            this.SetBinding(IsInEditModeProperty, binding);

        }
        /// <summary>
        /// Sets the IsInEdit mode.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetEditMode(bool value)
        {
            this.IsInEditMode = value;
            this.timer.Start();
        }
        void info_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            TreeViewItemInfo info = sender as TreeViewItemInfo;
            if (info == null) return;
            switch (e.PropertyName)
            {
                case "IsEdit":
                    SetEditMode(info.IsEdit);
                    break;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.MoveTextBoxInFocus();
        }

        private void MoveTextBoxInFocus()
        {
            if (this.textBox.CheckAccess())
            {
                //if (!string.IsNullOrEmpty(this.textBox.Text))
                //{
                    this.textBox.CaretIndex = 0;
                    this.textBox.Focus();
                //}
            }
            else
            {
                this.textBox.Dispatcher.BeginInvoke(DispatcherPriority.Render, new FocusTextBox(this.MoveTextBoxInFocus));
            }
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.textBox.Text = oldText;
                this.IsInEditMode = false;
                if (EditEnd != null) EditEnd(false, this.Parent);
            }
            else if (e.Key == Key.Enter)
            {
                this.IsInEditMode = false;
                if (EditEnd != null) EditEnd(true, this.Parent);
            }
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
        }

        private void EditableTabHeaderControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetEditMode(true);
            }
        }
    }
}