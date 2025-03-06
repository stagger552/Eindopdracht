using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for JsonMessage.xaml
    /// </summary>
    public partial class JsonMessage : Window
    {
        public JsonMessage(string jsonData)
        {
            InitializeComponent();

            // Set the JSON string to the TextBox
            txtJsonData.Text = jsonData;
        }
    }
}
