using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fenetre
{
    public partial class Form1 : Form
    {
        private System.IO.Ports.SerialPort port_serie;

        public Form1()
        {
            InitializeComponent();

            // création de la class d'utilisation du port série
            port_serie = new System.IO.Ports.SerialPort();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //configuration du port série
            this.port_serie.PortName = this.textBox_Nom.Text;
            this.port_serie.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            this.port_serie.Parity = SetPortParity(_serialPort.Parity);
            this.port_serie.DataBits = SetPortDataBits(_serialPort.DataBits);
            this.port_serie.StopBits = SetPortStopBits(_serialPort.StopBits);
            this.port_serie.Handshake = SetPortHandshake(_serialPort.Handshake);
            this.port_serie.ReadTimeout = 500;
            this.port_serie.WriteTimeout = 500;

            //ouverture du port série
            this.port_serie.Open();

            //écriture du message sur le port série
            this.port_serie.WriteLine(String.Format(" ", ));

            //fermeture du port série
            this.port_serie.Close();
        }
    }
}
