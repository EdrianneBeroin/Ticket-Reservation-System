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

namespace Ticket_Reservation_System
{
    public partial class Form1 : Form
    {
        SqlConnection cn = new SqlConnection();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();
        public Form1()
        {
            InitializeComponent();
        }
        private void tabPage2_Enter(object sender, EventArgs e)
        {
            frmseats frm = new frmseats();
            frm.ShowDialog();
            tabControl1.SelectedIndex = 0;
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp" ;
            open.Multiselect = false;
            if (open.ShowDialog() == DialogResult.OK)
            {

                moviepicturebox.Image = new Bitmap(open.FileName);

                txtimageloc.Text = open.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
          

            displaydg();



        }
        public int movieid;
        private void displaydg()
        {
            dg1.Rows.Clear();
            cn.Open();
            com = new SqlCommand("select * from Movie", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {              
                    dg1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToString("MM/dd/yyyy"), reader.GetInt32(6).ToString());
                }
            }
            cn.Close();
        }
        byte[] ConvertImageToBytes(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        public Image ConvertByteArrayToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
              
                return Image.FromStream(ms);
            }
        }
        SqlCommand com;
        public void btnSave_Click(object sender, EventArgs e)
        {
            if(btnSave.Text=="ADD")
            {
                formcontrolls("true");
                btnSave.Text = "SAVE";
            }

           else if (btnSave.Text == "SAVE")
            {
                InsertMovie();
                formcontrolls("false");
                displaydg();
            }


          
        }

        private void updatemovie()
        {
            FileStream fs = new FileStream(txtimageloc.Text, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] img3 = br.ReadBytes((int)fs.Length);
            cn.Open();
            com = new SqlCommand("update Movie set mov_name= '" + txtmovname.Text + "', mov_dur = '" + txtmovduration.Text +"',  mov_int= '" + txtMovInt.Text + "',dtrelease= '" + dt1.Value.ToShortDateString() + "',price= " + txtPrice.Text + ", poster= @image  where id='" + movieid+"'", cn);
            com.Parameters.Add(new SqlParameter("@image", img3));
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    dg1.Rows.Add(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToString("MM/dd/yyyy"), reader.GetInt32(6).ToString());
                }
            }
            cn.Close();
        }

        private void formcontrolls(string v)
        {
            txtimageloc.Text = "";
            txtmovduration.Text = "";
            txtMovInt.Text = "";
            txtmovname.Text = "";
            txtPrice.Text = "";

            txtimageloc.Enabled = Convert.ToBoolean(v);
            txtmovduration.Enabled = Convert.ToBoolean(v);
            txtMovInt.Enabled = Convert.ToBoolean(v);
            txtmovname.Enabled = Convert.ToBoolean(v);
            txtPrice.Enabled = Convert.ToBoolean(v);
            dt1.Enabled=Convert.ToBoolean(v);


        }

        private void InsertMovie()
        {
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;


            Image img = moviepicturebox.Image; //moviepicturebox.Image();
            byte[] arr = null ;
            ImageConverter converter = new ImageConverter();
            arr = (byte[])converter.ConvertTo(img, typeof(byte[]));

            MemoryStream ms = new MemoryStream();
            moviepicturebox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] img2 = ms.ToArray();

            FileStream fs = new FileStream(txtimageloc.Text, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] img3 = br.ReadBytes((int)fs.Length);



            if (!Validatetextbox())
            {
                return;
            }


            cn.Open(); /*+ img2/*ConvertImageToBytes(moviepicturebox.Image) +*/
            com = new SqlCommand("insert into movie(mov_name, mov_dur, mov_int, dtrelease, price, poster ) values('" + txtmovname.Text + "','" + txtmovduration.Text + "','" + txtMovInt.Text + "', '" + dt1.Value + "'," + txtPrice.Text + ",  @image)", cn);
            com.Parameters.Add(new SqlParameter("@image", img3));
            using (SqlDataReader reader = com.ExecuteReader())
            {
                MessageBox.Show("movie registered");
            }

            cn.Close();
            int movieid = 0;
            cn.Open();
            com = new SqlCommand("select max(id) from movie", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                if (reader.Read())
                {
                    movieid = Convert.ToInt32(reader[0]);
                }
            }
            cn.Close();

            DateTime am = new DateTime(2019, 05, 09, 10, 0, 0);
            int minutes = 0;
            for (int x = 0; x < 5; x++)
            {

                minutes += int.Parse(txtmovduration.Text) + int.Parse(txtMovInt.Text);

                DateTime timeshowing = am;
                Console.WriteLine(string.Format("{0} ", timeshowing.ToString("H:mm")));





                cn.Open();
                com = new SqlCommand("insert into movietime values('" + movieid + "','" + timeshowing + "')", cn);
                am = am.AddMinutes(minutes);
                using (SqlDataReader reader = com.ExecuteReader())
                {

                }
                cn.Close();
            }


            cn.Close();

        }

        private bool Validatetextbox()
        {
            var controls = new[] { txtmovname, txtPrice, txtMovInt, txtmovduration, txtimageloc };
            foreach (var control in controls.Where(e => String.IsNullOrWhiteSpace(e.Text)))
            {
                MessageBox.Show("Please fill up all information");
                return false ;
            }

            return true;
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
        private void dg1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            Loadnowshowing();
        }

        private void Loadnowshowing()
        {

            //Button a = new Button();
            //a.BackgroundImage = global::Ticket_Reservation_System.Properties.Resources.seats;
            //a.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            //a.Location = new System.Drawing.Point(standardwidth, standardheight);
            //a.Name = name + y;
            //a.Size = new System.Drawing.Size(71, 67);
            //a.Click += new EventHandler(this.button_click);
            //a.TabIndex = 3;
            //a.UseVisualStyleBackColor = true;
            //standardwidth += space;
            //this.Controls.Add(a);
            //allofseats.Add(name + y);  230, 22
            int pictureloc = 2;
            cn.Open();
            SqlCommand com = new SqlCommand("select top 3 * from movie order by dtrelease desc ", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                 


                    PictureBox a = new PictureBox();
           
                    a.BackgroundImageLayout = ImageLayout.Stretch;
                    a.Location = new System.Drawing.Point(pictureloc, 22);
                  //  a.Name = "pictureBox2";
                    a.Size = new System.Drawing.Size(225, 298);
                    a.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    a.BorderStyle = BorderStyle.Fixed3D;
                    a.BackgroundImage = ByteToImage((byte[])(reader[5]));

                    tabPage3.Controls.Add(a);


                    Label price = new Label();
                    price.BackColor = System.Drawing.Color.Crimson;
                    price.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    price.Location = new System.Drawing.Point(pictureloc, 0);

                    price.Size = new System.Drawing.Size(222, 19);
                    price.TabIndex = 1;
                    price.Text = reader[6].ToString();
                    price.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    price.AutoSize = false;
                    tabPage3.Controls.Add(price);




                    Label title = new Label();
                    title.BackColor = System.Drawing.Color.LightSalmon;
                    title.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    title.Location = new System.Drawing.Point(pictureloc, 318);
                 
                    title.Size = new System.Drawing.Size(222, 19);
                    title.TabIndex = 1;
                    title.Text = ":Spider man no way home"+reader[1].ToString();
                    title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    title.AutoSize = false;
                    tabPage3.Controls.Add(title);



                    pictureloc += 227; 
                }
            }

            cn.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "UPDATE")
            {
                updatemovie();
                formcontrolls("false");
                btnSave.Text = "ADD";
                displaydg();
                btnEdit.Text = "EDIT";
            }
            else if (btnEdit.Text == "EDIT")
            {
                formcontrolls("true");
                btnEdit.Text = "UPDATE";
                btnSave.Text = "ADD";


            }
        }

        private void dg1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            formcontrolls("false");
            cn.Open();
            SqlCommand com = new SqlCommand("select * from Movie where mov_name='" + dg1.Rows[e.RowIndex].Cells[0].Value + "' and price =" + dg1.Rows[e.RowIndex].Cells[4].Value + "", cn);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                if (reader.Read())
                {
                    movieid = reader.GetInt32(0);
                    txtmovname.Text = reader.GetString(1);
                    txtmovduration.Text = reader.GetString(2);
                    txtMovInt.Text = reader.GetString(3);
                    txtPrice.Text = reader.GetInt32(6).ToString();
                    dt1.Value = Convert.ToDateTime(reader.GetDateTime(4));
                    moviepicturebox.Image = ByteToImage((byte[])(reader[5]));

                    //moviepicturebox.Image = ConvertByteArrayToImage((byte[])(reader[5]));

                    //byte[] pic = (byte[])(reader[5]);
                    //MemoryStream ms = new MemoryStream(pic);

                    //pictureBox1.Image=Image.FromStream(ms);
                    //moviepicturebox.Image = Image.FromStream(ms);
                    btnEdit.Text = "EDIT";
                }
            }

            cn.Close();
        }
    }
}
