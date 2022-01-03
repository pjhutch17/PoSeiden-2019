// Program: PoSeiden
// Purpose: Point of Sale program
// By P J Hutchison
// 21 Jul 2019


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PoSeiden_2019
{
    public partial class Form1 : Form
    {
        Button[] POS_btn;
        string[] POS_Text, POS_Name,  POS_Code;
        double[] POS_Price;
        double SaleTotal = 0;
        string CurrencySymbol = "£";


        public Form1()
        {
            Int32 btn_count;
            string ItemCode = "", ItemName = "", ItemLine;
            string[] ItemList;
            double ItemPrice = 0.0F;
            StreamReader POS_File;
            InitializeComponent();

            // Init arrays
            POS_btn = new Button[41];
            POS_Text = new string[41];
            POS_Name = new string[41];
            POS_Code = new string[41];
            POS_Price = new double[41];

            // Read in POS information from a file
            Console.WriteLine("Directory = " + Directory.GetCurrentDirectory());
            if (File.Exists("..\\..\\POS_Information.txt"))
            {
                //FileOpen(1, "..\..\POS_Information.txt", OpenMode.Input, OpenAccess.Read, OpenShare.LockRead)
                POS_File = new StreamReader("..\\..\\POS_Information.txt");
                btn_count = 1;
                while (!POS_File.EndOfStream)
                {
                    //Input ItemCode, ItemName, ItemPrice from file.
                    ItemLine = POS_File.ReadLine();
                    ItemList = ItemLine.Split(',');

                    ItemCode = (ItemList[0] + "").Trim();
                    ItemName = ItemList[1] + "";
                    ItemPrice = Convert.ToDouble(ItemList[2]+"");
                    POS_Text[btn_count] = ItemCode + Convert.ToChar(13) + ItemName + Convert.ToChar(13) + ItemPrice.ToString("c");
                    Console.WriteLine(POS_Text[btn_count]);
                    POS_Name[btn_count] = ItemName;
                    POS_Price[btn_count] = ItemPrice;
                    POS_Code[btn_count] = ItemCode;
                    btn_count = btn_count + 1;
                }
                POS_File.Close();
            }
            else
            {
                MessageBox.Show("POS Information file not found!");
                return;

            }

            // Draw buttons on the form
            Add_Buttons_to_Form();

            // Add handler for Num Pad buttons
            cmd_Num1.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num2.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num3.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num4.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num5.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num6.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num7.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num8.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num9.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Num0.Click += new System.EventHandler(this.NumPad_Click);
            cmd_NumDot.Click += new System.EventHandler(this.NumPad_Click);
            cmd_Enter.Click += new System.EventHandler(this.NumPad_Click);
            cmd_ClrNumEntry.Click += new System.EventHandler(this.NumPad_Click);
        }


        string TrimNum(string value)
        {
            string Result;
            Int16 n;

            Result = "";
            for (n=0; n < (value.Length); n++)
            {
                if (value[n] >= '0' & value[n] <= '9')
                    Result = Result + value[n];
            }
            return (Result);
        }
        void Add_Buttons_to_Form()
        {
            Int32 x, y, btn_count;

            btn_count = 1;
            for (y=1; y<=4; y++)
            {
                for (x=1; x<=10; x++)
                {
                    // Add a button
                    POS_btn[btn_count] = new Button();
                    POS_btn[btn_count].Left = 7 + (x - 1) * 100;
                    POS_btn[btn_count].Top = 10 + (y - 1) * 100;
                    POS_btn[btn_count].Width = 95;
                    POS_btn[btn_count].Height = 95;
                    POS_btn[btn_count].Text = POS_Text[btn_count];
                    POS_btn[btn_count].ForeColor = Color.Black;
                    POS_btn[btn_count].Tag = btn_count; // Stores ID of button
                    // Add an event handler
                    POS_btn[btn_count].Click += new System.EventHandler(btn_Click);
                    // Add button to the form
                    this.Controls.Add(POS_btn[btn_count]);
                    btn_count = btn_count + 1;
                    if (btn_count > POS_btn.GetUpperBound(0))
                        break; // exit for x loop
                }
                if (btn_count > POS_btn.GetUpperBound(0))
                    break; // exit for y loop
            }
        }

        void btn_Click (object sender, EventArgs e)
        {
            Button button = new Button();
            Int32 idx = 0;

            // Convert object to a button type so I can access attributes
            button = (Button)sender;

            idx = Convert.ToInt32(button.Tag);
            Console.WriteLine("A button was pressed. Button #" + idx.ToString());
            SaleTotal = SaleTotal + Convert.ToDouble(POS_Price[idx]);
            // Converts numbers to £##.## values
            lblClickResult.Text = SaleTotal.ToString("##.##");
            listBox1.Items.Add(POS_Name[idx]);
            listBox2.Items.Add(POS_Price[idx].ToString("##.##"));
        }

        void cmd_ClearAll_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            SaleTotal = 0;
            lblClickResult.Text = SaleTotal.ToString();
        }

        void cmd_ClrEntry_Click(object sender, EventArgs e)
        {
            Int32 item;
            string itemName;
            double itemPrice;

            item = listBox1.SelectedIndex;
            if (item == -1)  return;
            itemName = listBox1.Items[item].ToString();
            itemPrice = Convert.ToDouble(listBox2.Items[item]);
            itemPrice = -itemPrice;
            // Add item to list but as a removal
            listBox1.Items.Add(itemName);
            listBox2.Items.Add(itemPrice);
            // subtract item price
            SaleTotal = SaleTotal + itemPrice;
            lblClickResult.Text = SaleTotal.ToString();
        }

        void NumPad_Click(object sender, EventArgs e)
        {
            Button NumButton;
            string NumValue;
            double CashTend, Change;

            // Get button pressed
            NumButton = (Button)sender;
            // Get value of button from Text field
            NumValue = NumButton.Text;
            // If a digit or dot then add it to the CashTend field
            if (NumValue[0] >= '0' & NumValue[0] <= '9')
                txt_CashTend.Text = txt_CashTend.Text + NumValue;
            else if (NumValue[0] == '.' & (txt_CashTend.Text).IndexOf (".") == -1 )
                txt_CashTend.Text = txt_CashTend.Text + NumValue;
            else if (NumValue == "Enter")
            {
                // Work out the change from cash sale
                CashTend = Convert.ToDouble (txt_CashTend.Text);
                Change = CashTend - SaleTotal;
                lbl_Change.Text = CurrencySymbol + Change.ToString();
            }
            else if (NumValue == "CE")
            {
                CashTend = 0;
                Change = 0;
                txt_CashTend.Text = "";
                lbl_Change.Text = "";
            }
        }
    }

    
}
