using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CAAP_DTR_PRINTER
{
    public partial class Form1 : Form
    {
        private static printableForm printForm = new printableForm();
        private Panel panel1 = printForm.panel1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|RTF Files|*.rtf|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                if (Path.GetExtension(filePath).ToLower() == ".rtf")
                {
                    richTextBox1.LoadFile(filePath, RichTextBoxStreamType.RichText);
                }
                else
                {
                    richTextBox1.Text = File.ReadAllText(filePath);
                }

                ShowPrintableForm(richTextBox1.Text);
            }
        }

        private void ShowPrintableForm(string content)
        {
            // Split content by new lines
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Ensure there are enough lines for the data structure
            if (lines.Length < 6)
            {
                MessageBox.Show("Invalid file format. Please check the content and try again.");
                return;
            }


            string name = GetValueFromLine(lines[0], "Name:");
            string position = GetValueFromLine(lines[1], "Position:");
            string department = GetValueFromLine(lines[2], "Department:");

            // Extract the daily time records
            int numRecords = lines.Length - 4; // Adjust based on your data structure
            string[,] timeRecords = new string[numRecords, 5]; // Assuming 5 columns per record

            for (int i = 0; i < numRecords; i++)
            {
                var columns = lines[i + 4].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Adjust index based on your data structure
                if (columns.Length >= 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        timeRecords[i, j] = columns[j];
                    }
                }
                else
                {
                    // Handle cases where there are not enough columns
                    for (int j = 0; j < columns.Length; j++)
                    {
                        timeRecords[i, j] = columns[j];
                    }
                    for (int j = columns.Length; j < 5; j++)
                    {
                        timeRecords[i, j] = string.Empty;
                    }
                }
            }

          
            printForm.PopulateForm(name, position, department, timeRecords);
            printForm.Show();
        }

        private string GetValueFromLine(string line, string keyword)
        {
            int index = line.IndexOf(keyword);
            if (index != -1)
            {
                return line.Substring(index + keyword.Length).Trim();
            }
            return string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
           if(richTextBox1.Text == string.Empty)
            {
                MessageBox.Show("Upload a file first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           else
            {

                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.Document = printDocument;

                printPreviewDialog.ShowDialog();
            }
        }

      

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
         
            Bitmap bitmap = new Bitmap(panel1.Width, panel1.Height);
            panel1.DrawToBitmap(bitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));
            e.Graphics.DrawImage(bitmap, e.MarginBounds.Left, e.MarginBounds.Top);

         
            e.HasMorePages = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
         if(richTextBox1.Text == string.Empty)
            {
                MessageBox.Show("Upload a file first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         else
            {

                PdfDocument pdfDocument = new PdfDocument();


                PdfPage page = pdfDocument.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);


                Bitmap bitmap = new Bitmap(printForm.panel1.Width, printForm.panel1.Height);
                printForm.panel1.DrawToBitmap(bitmap, new Rectangle(0, 0, printForm.panel1.Width, printForm.panel1.Height));

                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;


                XImage xImage = XImage.FromStream(memoryStream);


                gfx.DrawImage(xImage, 0, 0);


                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{printForm.textBox1.Text}.pdf");

                pdfDocument.Save(savePath);


                pdfDocument.Close();
                memoryStream.Close();
                bitmap.Dispose();


                MessageBox.Show("PDF saved successfully!");


                //string pdfViewerPath = @"C:\Program Files\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                //if (File.Exists(pdfViewerPath))
                //{
                //    System.Diagnostics.Process.Start(pdfViewerPath, savePath);
                //}
                //else
                //{
                //    MessageBox.Show("PDF viewer application not found. Please install a PDF viewer and try again.");
                //}
            }
        }

    }
}
