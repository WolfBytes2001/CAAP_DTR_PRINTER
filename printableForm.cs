using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAAP_DTR_PRINTER
{
    public partial class printableForm : Form
    {
        public printableForm()
        {
            InitializeComponent();
        }
        public void PopulateForm(string name, string position, string department, string[,] timeRecords)
        {
            textBox1.Text = name;
            textBox2.Text = position;
            textBox3.Text = department;

            
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Date", "Date");
            dataGridView1.Columns.Add("TimeIn", "Time In AM");
            dataGridView1.Columns.Add("LunchOut", "Time Out AM");
            dataGridView1.Columns.Add("LunchIn", "Time In PM");
            dataGridView1.Columns.Add("TimeOut", "Time Out PM");

            
            dataGridView1.Rows.Clear();

            for (int i = 0; i < timeRecords.GetLength(0); i++)
            {
                if (timeRecords.GetLength(1) >= 4) // Ensure at least 4 columns exist per row
                {
                    // Extract individual time components
                    string date = timeRecords[i, 0];
                    string timeIn = timeRecords[i, 1];
                    string lunchOut = timeRecords[i, 2];
                    string lunchIn = timeRecords[i, 3];
                    string timeOut = timeRecords[i, 4];

                    
                    dataGridView1.Rows.Add(date, timeIn, lunchOut, lunchIn, timeOut);
                }
                else
                {
                    
                    dataGridView1.Rows.Add(); 
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
