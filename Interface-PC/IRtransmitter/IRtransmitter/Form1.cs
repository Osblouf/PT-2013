#region Namespace Inclusions
using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using System.Threading;
using System.IO;
#endregion

namespace IRtransmitter
{
    public partial class Form1 : Form
    {
        #region Enums et Struct declaration 
        // Flag system viewing, non implemented yet
        enum PortMode { Sending, Reading, Waiting };
        #endregion
        #region Variables Locales
        private SerialPort comport = new SerialPort("COM1", 57600, Parity.None, 8); // Le SERIAL PORT huhu
        private PortMode Flags = PortMode.Waiting; // Default standing
        #endregion

        public Form1()
        {
            // Loading User Default Settings...
            //InitializeDefaultUserSettings();

            // Initialize Window
            InitializeComponent();

            // Setting Events on Serial Port
            comport.DataReceived += new SerialDataReceivedEventHandler(port_DataIn);
            comport.PinChanged += new SerialPinChangedEventHandler(comport_PinChanged);
        }

        /// <summary>
        /// Initialize the COM port.
        /// </summary>
        /// <param name="words"></param>
        private void InitializeSerialPort(string [] words)
        {
            try
            {
                comport.Open();
            }
            catch (Exception e)
            {
                this.rtfTerminal.Text += "ALERT : COM Error On Opening \n Text error : " + e.ToString();
            }
            finally
            {
                if (this.comport.IsOpen)
                {
                    this.rtfTerminal.Text += "COMPORT Opened correctly";
                    this.port_Show_Status();
                }
                
                else
                    this.rtfTerminal.Text += "\n \n COMPORT Not Opened";
            }
        }

        private void port_Show_Status()
        {
            try
            {
                this.rtfTerminal.Text += "COMPORT : " + this.comport.ToString();
                this.rtfTerminal.Text += "\n";
            }
            catch (Exception e)
            {
                this.rtfTerminal.Text += e.ToString();
            }
        }

        private void InitializeDefaultUserSettings()
        {
            throw new NotImplementedException();
        }

        private void comport_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void senddata_Click(object sender, EventArgs e)
        {
            // Commandes utilitaires...
            String value = this.command.Text;
            String[] words = value.Split(' ');

            // Debug Zone & visibility
            foreach (string word in words)
                this.rtfTerminal.Text += "UserCommand : " + word + "\n";

            // Action from... ;)
            switch (words[0])
            {
                case "send":
                    this.prompt_send(words);
                    break;
                case "setbaud":
                    this.prompt_setbaud(words);
                    break;
                case "connect":
                    this.prompt_connect(words);
                    break;
                case "echo":
                    this.prompt_echo(words);
                    break;
                case "clear":
                    this.rtfTerminal.Clear();
                    break;
                case "help":
                    this.printHelp();
                    break;
                case "listPortsName":
                    this.prompt_list(words);
                    break;
                case "status":
                    this.prompt_status(words);
                    break;
            }

        }

        
        #region PORT FUNCTIONS SECTION
        private void port_SendData(string dataToBeSent)
        {
            try
            {
                // Convert the user's string of hex digits (ex: B4 CA E2) to a byte array
                byte[] data = HexStringToByteArray(dataToBeSent);

                // Send the binary data out the port
                comport.Write(data, 0, data.Length);

                // Show the hex digits on in the terminal window
                this.rtfTerminal.Text += "Prompt : " + ByteArrayToHexString(data) + "\n";
            }
            catch (FormatException)
            {
                // Inform the user if the hex string was not properly formatted
                this.rtfTerminal.Text += "ALERT : Not properly formatted hex string: " + dataToBeSent + "\n";
            }
        }

        private string port_ReadData()
        {
            String ret = "no new values in buffer \n";
            // If the com port has been closed, do nothing
			if (!comport.IsOpen) return ret;

            // This method will be called when there is data waiting in the port's buffer
            else{
                // Obtain the number of bytes waiting in the port's buffer
                int bytes = comport.BytesToRead;

                // Create a byte array buffer to hold the incoming data
                byte[] buffer = new byte[bytes];

                // Read the data from the port and store it in our buffer
                comport.Read(buffer, 0, bytes);

                // Change buffer ToString
                try{
                ret = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                }
                catch(Exception e){ this.rtfTerminal.Text += e; }
                return ret;
            }
        }

        /// <summary>
        /// Lists Ports Name
        /// </summary>
        /// <returns></returns>
        private string[] OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
        }

        /// <summary> Convert a string of hex digits (ex: E4 CA B2) to a byte array. </summary>
        /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
        /// <returns> Returns an array of bytes. </returns>
        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
        #endregion

        #region PROMPT COMMANDS DETAIL

        /// <summary>
        /// prints a summary of commands
        /// </summary>
        private void printHelp()
        {
            String commandList = "Commands Available :\n send [DATA - String] \n setbaud [RATE Int ] \n connect [Void] \n echo [DATA String] \n clear [Void] \n listPortsName [void] \n status [void] \n";
            this.rtfTerminal.Text += commandList;
        }

        /// <summary>
        /// prints text in rtfbox
        /// </summary>
        /// <param name="w"></param>
        private void print(String w)
        {
            this.rtfTerminal.Text += w + "\n";
        }

        /// <summary>
        /// ECHO basic function - not usefull at all
        /// </summary>
        /// <param name="words"></param>
        private void prompt_echo(string[] words)
        {
            int i = words.Length;

            this.rtfTerminal.Text += "COMMAND : ";

            for (int j = 1; j < i; j++)
            {
                this.rtfTerminal.Text += words[j];
            }
        }

        /// <summary>
        /// This function actually starts the Hello World Routine check if Communication works...
        /// </summary>
        /// <param name="words"></param>
        private void prompt_connect(string[] words)
        {
            // Initialize SerialComPort
            InitializeSerialPort(words);

            // string dataToBeSent = "HW PIC";

            // this.print("Connection test : " + dataToBeSent);

            // sending data through port...
            // this.port_SendData(dataToBeSent);
        }
        
        /// <summary>
        /// Sets up BAUD RS232 COM PORT
        /// </summary>
        /// <param name="words"></param>
        private void prompt_setbaud(string[] words)
        {
            // setting up new baud rate...
            try
            {
                int t = Convert.ToInt32(words[1]);
                if (t > 57600 || t < 1200) throw new Exception("Error Baud rate too high/low");
                else
                    comport.BaudRate = t;
            }
            catch (Exception e)
            {
                this.print(e.ToString());
            }
        }

        /// <summary>
        /// Sends the string[] words through com port
        /// </summary>
        /// <param name="words"></param>
        private void prompt_send(string[] words)
        {
            int i = 1;
            string dataToBeSent = "";
            for (; i < words.Length; i++)
            {
                dataToBeSent += words[i];
            }

            // sending data through port...
            this.port_SendData(dataToBeSent);
        }

        /// <summary>
        /// Shows current status of COMPORTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prompt_list(string[] words)
        {
            string[] t = this.OrderedPortNames();

            foreach (string v in t)
            {
                this.print(v);
            }
        }

        /// <summary>
        /// Prints status of opened PORT
        /// </summary>
        /// <param name="words"></param>
        private void prompt_status(string[] words)
        {
            this.port_Show_Status();
        }
        #endregion

        


        private void label2_Click(object sender, EventArgs e)
        {
            
        }



        private void port_DataIn(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
