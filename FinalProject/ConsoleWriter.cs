using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace wpf_resipe
{
    public class ConsoleWriter : TextWriter
    {
        private TextBox _textBox;

        public ConsoleWriter(TextBox textBox)
        {
            _textBox = textBox;
        }

        public override void Write(char value)
        {
            Application.Current.Dispatcher.Invoke(() => _textBox.AppendText(value.ToString()));
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
