using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WebAnalyticsViewer
{
    public partial class Form2 : Form
    {
        private Form1 mainForm;

        public Form2(Form1 parentForm)
        {
            InitializeComponent();
            mainForm = parentForm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            mainForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
                mainForm.UpdateDatasource(textBox1.Text);
            mainForm.Show();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DisplayConfirmationData();
        }

        private void DisplayConfirmationData()
        {
            string[] confirmationData = mainForm.ReturnGridSelectionData();
            label7.Text =   confirmationData[0];
            label8.Text =   confirmationData[1];
            label9.Text =   confirmationData[2];
            label11.Text =  confirmationData[3];

        }
    }
}
