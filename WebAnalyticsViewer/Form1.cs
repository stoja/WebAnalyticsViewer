using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WebAnalyticsViewer
{
    public partial class Form1 : Form
    {
        private DataSet result;
        private Helper_Data dataHelper;
        private BindingSource bindingSource;
        private string[] ignoreList;
        private string searchBy;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureIgnoreList();
            dataHelper = new Helper_Data();
            dataHelper.EstablishConnection();
            bindingSource = new BindingSource();
            result = dataHelper.ReturnDataset();
            groupBox2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
		{
            PopulateGridView(false);
            textBox1.Clear();
            radioButton1.Checked = true;
            groupBox2.Enabled = true;
		}

        private void radioButton1_Click(object sender, EventArgs e)
        {
            PopulateGridView(false);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            PopulateGridView(true);
        }

        private void ConfigureIgnoreList()
        {
            string[] blackList = {"c50", "purchaseID"};
            ignoreList = blackList;
        }

        public void UpdateDatasource(string newBaseValue)
        {
            dataGridView1.CurrentCell.Value = newBaseValue;
            bindingSource.EndEdit();
            result.AcceptChanges();

            if (dataGridView1.CurrentRow.Cells[9].Value != DBNull.Value)
                dataHelper.UpdateRowExpValue(Convert.ToInt32(dataGridView1.CurrentRow.Cells[9].Value), null, null, newBaseValue);
            else
                dataHelper.UpdateRowExpValue(null, Convert.ToInt32(dataGridView1.CurrentRow.Cells[8].Value), Convert.ToInt32(dataGridView1.CurrentRow.Cells[7].Value), newBaseValue);

            dataHelper.EstablishConnection();
            result = dataHelper.ReturnDataset();
        }

        private void PopulateGridView(bool filteredResult)
        {
            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null)
            {
                int i;
                int mismatchedCount = 0;
                searchBy = comboBox2.SelectedItem.ToString() + "." + comboBox1.SelectedItem.ToString();

                bindingSource.DataSource = result.Tables[0];
                bindingSource.Filter = "host = '" + searchBy + "'";
                dataGridView1.DataSource = bindingSource;

                // We dont want to show certain columns
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[9].Visible = false;
                dataGridView1.Columns[11].Visible = false;

                // We dont want people to select rows, just cells.
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;

                if (!filteredResult)
                {
                    for (i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];
                        row.Visible = true;

                        if (ignoreList.Contains(row.Cells[4].Value))
                        {
                            row.Visible = false;
                        }

                        else if (!row.Cells[5].Value.Equals(row.Cells[6].Value))
                        {
                            row.DefaultCellStyle.BackColor = Color.Red;
                            mismatchedCount++;
                        }
                    }
                }
                else
                {
                    dataGridView1.CurrentCell = null;
                    for (i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];
                        row.Visible = false;
                        if (!row.Cells[5].Value.Equals(row.Cells[6].Value) && !ignoreList.Contains(row.Cells[5].Value))
                        {
                            row.Visible = true;
                            row.DefaultCellStyle.BackColor = Color.Red;
                            mismatchedCount++;

                        }
                    }
                }
                dataGridView1.Refresh();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                
                // Set total count in toolstrip
                toolStripStatusLabel2.Text = dataGridView1.Rows.Count.ToString();
                toolStripStatusLabel2.Visible = true;
                toolStripStatusLabel5.Text = mismatchedCount.ToString();
                toolStripStatusLabel5.Visible = true;

                if (mismatchedCount > 0)
                {
                    label3.Visible = true;
                    label4.Visible = false;
                    pictureBox2.Visible = true;
                    pictureBox1.Visible = false;
                }
                else
                {
                    label4.Visible = true;
                    label3.Visible = false;
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Please select a valid site and environment before attempting to retrieve results.");
            }

        }

        public string[] ReturnGridSelectionData()
        {
            string[] selectionData = new string[4];
            selectionData[0] = comboBox2.SelectedItem.ToString() + "." + comboBox1.SelectedItem.ToString();
            selectionData[1] = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            selectionData[2] = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            selectionData[3] = dataGridView1.CurrentRow.Cells[10].Value.ToString();

            return selectionData;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                Form2 varEditDialog = new Form2(this);
                this.Hide();
                varEditDialog.Show(this);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 6)
            {
                dataGridView1.CurrentCell.Selected = false;
                dataGridView1.CurrentRow.Cells[6].Selected = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox2.Enabled = false;
            string validInput = Regex.Replace(textBox1.Text, @"[\^$.|?*+()\]\[]", "");

            if (!radioButton1.Checked)
            {
                radioButton1.Checked = true;
            }

            if (textBox1.Text != string.Empty)
            {
                int i = 0;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                bindingSource.Filter = "host = '" + searchBy + "' AND page_name LIKE '%" + validInput + "%'";
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Visible = true;
                    if (!row.Cells[5].Value.Equals(row.Cells[6].Value))
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
                dataGridView1.Refresh();
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                textBox2.Enabled = true;
                PopulateGridView(false);
                dataGridView1.Refresh();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Enabled = false;
            string validInput = Regex.Replace(textBox2.Text, @"[\^$.|?*+()\]\[]", "");

            if (!radioButton1.Checked)
            {
                radioButton1.Checked = true;
            }

            if (textBox2.Text != string.Empty)
            {
                int i = 0;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                bindingSource.Filter = "host = '" + searchBy + "' AND var_name LIKE '%" + validInput + "%'";
                for (i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Visible = true;
                    if (!row.Cells[5].Value.Equals(row.Cells[6].Value))
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
                dataGridView1.Refresh();
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                textBox1.Enabled = true;
                PopulateGridView(false);
                dataGridView1.Refresh();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_MouseHover_1(object sender, EventArgs e)
        {
            ToolTip infoMsg = new ToolTip();
            infoMsg.UseFading = true;
            infoMsg.ToolTipTitle = "About Relevancy Filter";
            infoMsg.SetToolTip(pictureBox3, "The relevancy filter hides dynamic analytics data from the results. \nThese results cannot be programmatically verified due to their dynamic nature.\n Current ignore list contains " + ignoreList.Count() + " entries.");
            
        }

    }
}
