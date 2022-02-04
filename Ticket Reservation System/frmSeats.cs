using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace Ticket_Reservation_System
{
    public partial class frmseats : Form
    {

        SqlConnection cn = new SqlConnection();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand com;
        private static Excel.Workbook MyBook = null;
        private static Excel.Application MyApp = null;
        private static Excel.Worksheet MySheet = null;



        public frmseats()
        {
            InitializeComponent();
        }
        public int space = 65;
        public int standardwidth = 246;
        public int spaceheight = 87;
        public int standardheight = 116;

        public bool checkseatsreserved(string seatname)
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            cn.Open();
            com = new SqlCommand("select * from reservemovie where seats = '"+seatname+ "' and timeschedule ='" + cmbtime.Text + "' and datereserved ='" + dateTimePicker1.Value.ToShortDateString() + "' ", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                if (reader.Read())
                {
                    return true;
                }
            }
            cn.Close();
            return false;

        }
        public int movieid;
        List<string> seats = new List<string>();
        List<string> allofseats = new List<string>();
        void button_Click(object sender, EventArgs e)
        {

        }
        public void button_click(object sender,EventArgs e)
        {
            if (cmbtime.Text == "")
            {
                MessageBox.Show("Please select a time schedule");
                return;
            }

            Button btn = sender as Button;
            if(checkseatsreserved(btn.Name))
            {
  
                if (MessageBox.Show("Cancel", "Do you want to cancel this seats", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    cn.Close();
                    cn.Open();
                    com = new SqlCommand("update reservemovie set status ='CANCELLED' where seats= '" + btn.Name + "'and timeschedule ='" + cmbtime.Text + "' and datereserved ='" + dateTimePicker1.Value.ToShortDateString() + "' ", cn);
                    com.ExecuteNonQuery();
                    cn.Close();
                    resetbutton();
                    loadbutton();
                    return;
                }

            }
          
            var match = seats.FirstOrDefault(stringToCheck => stringToCheck.Contains(btn.Name));

            if (match == null)
            {
                rtb1.Clear();



                seats.Add(btn.Name);

                for (int i = 0; i < seats.Count; i++)
                {
                    rtb1.Text += seats[i] + ", ";
                }




                //if (rtb1.Text == "")
                //    rtb1.Text += btn.Name;
                //else

                //    rtb1.Text += ", " + btn.Name;
                btn.BackgroundImage = global::Ticket_Reservation_System.Properties.Resources.reserved;
            }
            else
            {
                btn.BackgroundImage = global::Ticket_Reservation_System.Properties.Resources.seats;
                for (int i = 0; i < seats.Count; i++)
                {
                    // if it is List<String>
                    if (seats[i].Equals(btn.Name))
                    {
                        seats.RemoveAt(i);
                    }
                }
                rtb1.Clear();
                for (int i = 0; i < seats.Count; i++)
                {
                    rtb1.Text += seats[i] + ", ";
                }




            }







        }
        private void addnewbutton(int x)
         {
            string name = string.Empty;
            if (x == 1)
                name = "a";
            if (x == 2)
                name = "b";
            if (x == 3)
                name = "c";
            if (x == 4)
                name = "d";


            for (int y = 1; y <= 5; y++)
            {
                Button a = new Button();
                a.BackgroundImage = global::Ticket_Reservation_System.Properties.Resources.seats;
                a.BackgroundImageLayout = ImageLayout.Stretch;
                a.Location = new System.Drawing.Point(standardwidth, standardheight);
                a.Name = name + y;
                a.Width = 71;
                a.Height = 67;
            
                a.TabIndex = 3;
                a.UseVisualStyleBackColor = true;
                standardwidth += space;
                this.Controls.Add(a);
                allofseats.Add(name + y);
            }
            standardwidth = 246;
            standardheight += 65;
        }
        private void frmSeats_Load(object sender, EventArgs e)
        {
            loadmovie();
            for (int x=1;x<5;x++)
            {

                        addnewbutton(x);  
            }
            //PrintReciept();
            
        }
        private void loadmovie()
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            cn.Open();
            com = new SqlCommand("select * from Movie", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    cmbmovie.Items.Add(reader[1].ToString());
                  
                }
            }
            cn.Close();


            cn.Open();
            com = new SqlCommand("update reservemovie set status = 'USED' where datereserved<'" +dateTimePicker1.Value.ToShortDateString()+"' and status ='RESERVED' and timeschedule > '"+ DateTime.Now.ToString("HH:mm") +"'", cn);
            com.ExecuteNonQuery();
            cn.Close();



        }
        private void resetbutton()
        {
            rtb1.Text = "";
           for(int i =0; i< allofseats.Count;i++ )
            {
             
             this.Controls[allofseats[i]].BackgroundImage = global::Ticket_Reservation_System.Properties.Resources.seats;
            }

        }
        private void loadbutton()
        {
            int seatcount = 0;
            string reserveseats = string.Empty;
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            cn.Open();
            com = new SqlCommand("select * from reservemovie where movieid ='"+movieid + "' and timeschedule ='" + cmbtime.Text + "' and datereserved ='"+dateTimePicker1.Value.ToShortDateString()+ "' and status ='RESERVED' ", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    reserveseats = reader[1].ToString();
                    this.Controls[reserveseats].BackgroundImage =  global::Ticket_Reservation_System.Properties.Resources.nositting;
                    seatcount++;
                }
            }
            cn.Close();
            seats.Clear();
            lblseats.Text = "Total available seats=" + (20-seatcount);



        }
        public  int movieidbyname(string moviename)
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            cn.Open();
            com = new SqlCommand("select * from movie where mov_name='"+moviename+"'", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            cn.Close();
            return 0;
        }
        private void cmbmovie_SelectedIndexChanged(object sender, EventArgs e)
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;

            int movid = movieidbyname(cmbmovie.Text);
            cn.Close();
            movieid = movieidbyname(cmbmovie.Text);
            cn.Close();
            cn.Open();
            cmbtime.Items.Clear();
            com = new SqlCommand("select * from movietime where movieid="+ movid + "", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    cmbtime.Items.Add(reader[1].ToString());
                }
            }
            cn.Close();
            cn.Open();
            com = new SqlCommand("select * from movie where id=" + movid + "", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    pictureBox1.Image = ByteToImage((byte[])(reader[5]));
                }
            }
            cn.Close();

            if (cmbtime.Text!="")
            {
                resetbutton();
                loadbutton();
            }
       


        }
        private void label9_Click(object sender, EventArgs e)
        {

        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        private void cmbtime_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetbutton();
            loadbutton();
        }
        private void btnBook_Click(object sender, EventArgs e)
        {
            if (cmbmovie.Text == "" && cmbtime.Text == "")
            {
                MessageBox.Show("Please select a movie and a time schedule");
                return;
            }
            if (txtName.Text == "")
            {
                MessageBox.Show("Please add a name");
                return;
            }
            cn.Open();
            for (int i = 0; i < seats.Count; i++)
            {
                com = new SqlCommand("insert into reservemovie(seats,datereserved,movieid,custname,status,timeschedule) values ('"
                    +seats[i]+ "','"+dateTimePicker1.Value.ToShortDateString() +"','"+ movieid + "','"+txtName.Text+"','RESERVED','"+cmbtime.Text+"')", cn);
                com.ExecuteNonQuery();
            }
            cn.Close();
            PrintReciept();
            loadbutton();
           
         

             

        }

        private void PrintReciept()
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            resetbutton();
            loadbutton();
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            MyApp = new Excel.Application();

            MyBook = (MyApp.Workbooks.Add(""));

            MySheet = String.IsNullOrEmpty("Sheet1") ? (Excel.Worksheet)MyBook.ActiveSheet : (Excel.Worksheet)MyBook.Worksheets["Sheet1"];
            if (cmbmovie.Text == "")
            {
                MessageBox.Show("Please select a movie");
                return;
            }

            SqlCommand com2;
            SqlDataReader reader;
            SqlDataReader reader2;
            SqlConnection cn2 = new SqlConnection();
            cn2.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
            string name = string.Empty;
            string seatsbyname = string.Empty;
            int rowcounter = 1;
            int iDataColumn = 1;
            string filePath = "";
            string status = "";
            string dateandtime = "";

                cn.Open();
                com = new SqlCommand("SELECT DISTINCT STATUS,custname  FROM RESERVEMOVIE where movieid ='" + movieid + "' and timeschedule  like '%" + cmbtime.Text + "%' and datereserved ='" + dateTimePicker1.Value.ToShortDateString() + "'",cn);

                using ( reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader[1].ToString();
                        cn2.Open();
                        com2 = new SqlCommand("select * from RESERVEMOVIE where  custname='"+name+ "' and status = '"+reader[0].ToString()+"' and timeschedule  like '%" + cmbtime.Text + "%' and datereserved ='" + dateTimePicker1.Value.ToShortDateString() + "'", cn2);
                        using ( reader2 = com2.ExecuteReader())
                        {
                            while (reader2.Read())
                            {
                                seatsbyname += reader2[1].ToString() + ", "; 
                            status= reader2[5].ToString();
                            dateandtime = reader2[2].ToString() + "." + reader2[6].ToString();
                            }
                        }
                        cn2.Close();
                        

                       MySheet.Cells[rowcounter, 1] = dateandtime;
                       MySheet.Cells[rowcounter, 2] = name;
                       MySheet.Cells[rowcounter, 3] = seatsbyname;
                       MySheet.Cells[rowcounter, 4] = status;
                    seatsbyname = "";
                        rowcounter++;
                    }
                }
                cn.Close();




            //MySheet.Cells[rowcounter, iDataColumn] = header[iDataRow].ToString();
            //iDataRow++;
            //iDataColumn++;
            string FileName = string.Empty;
            SaveFileDialog dlg;
            using (dlg = new SaveFileDialog())
            {
                dlg.Title = "Reports";
                dlg.Filter = "Excel Files (*.xls)|*.xls";
                dlg.FilterIndex = 3;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {

                    FileName = dlg.FileName;
                }

            }


            try
            {

                MyBook.SaveAs(FileName, Excel.XlFileFormat.xlWorkbookNormal,
                                   System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                                   Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                                   System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                MyApp.Visible = true;

            }
            catch
            {
                MessageBox.Show("Report not saved successfully");
            }

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (seats.Count == 0)
                return;
            int docuheight = 10;
            for (int x = 0; x < seats.Count; x++)
            {
                e.Graphics.DrawString("TICKET", new Font(this.Font.Name, 15), new SolidBrush(Color.Black), new Point(10, docuheight)); docuheight += 20; 
                e.Graphics.DrawString(cmbmovie.Text, new Font(this.Font.Name, 10), new SolidBrush(Color.Black), new Point(10, docuheight)); docuheight += 20;
                e.Graphics.DrawString(cmbtime.Text +"  "+ dateTimePicker1.Value.ToShortDateString(), new Font(this.Font.Name, 10), new SolidBrush(Color.Black), new Point(10, docuheight)); docuheight += 20;
                e.Graphics.DrawString(seats[x], new Font(this.Font.Name, 10), new SolidBrush(Color.Black), new Point(10, docuheight)); docuheight += 20;
                e.Graphics.DrawString(DateTime.Now.ToString("f"), new Font(this.Font.Name, 10), new SolidBrush(Color.Black), new Point(10, docuheight)); docuheight += 20;

                docuheight += 40;
            }
        }
    }
}
