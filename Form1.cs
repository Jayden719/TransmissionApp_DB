using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DbTransMission
{
    public partial class Form1 : Form
    {
        private Point _imageLocation = new Point(20, 4);
        private Point _imgHitArea = new Point(20, 4);
        Image closeImage;

        //팩스 전송
        bool addrchk = false; // 주소록 체크
        bool reschk = false; // 예약 체크
        bool refchk = false; // 치환문자 체크

        //문자 전송
        bool Maddrchk = false; // 주소록 체크
        bool Mreschk = false; // 예약 체크
        bool Mrefchk = false; // 치환문자 체크
        bool Mmms = false;

        string Mconts = ""; // TXT형식 문자내용
        string Mconte = ""; // Richtextbox 문자내용

        string Maddr = ""; // Textbox 수신번호
        string MaddrID = ""; // 주소록 아이디
        string Msfrom = ""; // 발신번호
      
        string Mref1 = "";
        string Mref2 = "";

        string saddrs = "";
        string sfrom = "";
        string isaddrs = "";
        string addrID = "";
        string sconts ="";
        string sref1 ="";
        string sbanner = "";
        int scontstype = 0;

        // 팩스 전송 예약시간
        string Fdate = "";
        string Ldate = "";
        string insertDate ="";

        // 문자 전송 예약시간
        string MFdate = "";
        string MLdate = "";
        string MinsertDate = "";

        





        public Form1()
        {
            InitializeComponent();
        }

        private void 팩스전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tabControl1.Controls.Contains(tab_fax))
            {
                tabControl1.Controls.Add(tab_fax);
                tabControl1.SelectedTab = tab_fax;
            }
            else
            {
                tabControl1.SelectedTab = tab_fax;
            }
        }

        private void 문자전송ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tabControl1.Controls.Contains(tab_msg))
            {
                tabControl1.Controls.Add(tab_msg);
                tabControl1.SelectedTab = tab_msg;
            }
            else
            {
                tabControl1.SelectedTab = tab_msg;
            }
        }

        private void 전송결과ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("전송프로그램_DB연동 ver_{0}", Application.ProductVersion);
            this.tabControl1.TabPages.Remove(this.tab_fax);
            this.tabControl1.TabPages.Remove(this.tab_msg);
            this.tabControl1.TabPages.Remove(this.tab_res);

            closeImage = Properties.Resources.red_cancel;
            tabControl1.Padding = new Point(15, 4);

            string[] combo = { ".doc |.txt |.hwp", ".tiff" };
            comboBox1.Items.AddRange(combo);
            comboBox1.SelectedIndex = 0;

            listView1.View = View.Details;
            listView1.Columns.Add("잡아이디", 140, HorizontalAlignment.Center);
            listView1.Columns.Add("내용", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("발신번호", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("전송시간", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("완료시간", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("결과코드", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("결과내용", 80, HorizontalAlignment.Center);
        }

        private void 전송결과조회ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tabControl1.Controls.Contains(tab_res))
            {
                tabControl1.Controls.Add(tab_res);
                tabControl1.SelectedTab = tab_res;
            }
            else
            {
                tabControl1.SelectedTab = tab_res;
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Image img = new Bitmap(closeImage);
            Rectangle r = e.Bounds;
            r = this.tabControl1.GetTabRect(e.Index);
            r.Offset(2, 2);
            Brush TitleBrush = new SolidBrush(Color.Black);
            Font f = this.Font;
            string title = this.tabControl1.TabPages[e.Index].Text;
            e.Graphics.DrawString(title, f, TitleBrush, new PointF(r.X, r.Y));
            e.Graphics.DrawImage(img, new Point(r.X + (this.tabControl1.GetTabRect(e.Index).Width - _imageLocation.X), _imageLocation.Y));
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            Point p = e.Location;
            int _tabWidth = 0;
            _tabWidth = this.tabControl1.GetTabRect(tabControl.SelectedIndex).Width - (_imgHitArea.X);
            Rectangle r = this.tabControl1.GetTabRect(tabControl.SelectedIndex);
            r.Offset(_tabWidth, _imgHitArea.Y);
            r.Width = 16;
            r.Height = 16;
            if (tabControl1.SelectedIndex >= 0)
            {
                if (r.Contains(p))
                {
                    TabPage tabPage = (TabPage)tabControl.TabPages[tabControl.SelectedIndex];
                    tabControl.TabPages.Remove(tabPage);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(addrchk == true)
            {
                if(textBox6.Text == "" || textBox6.Text.Length == 0)
                {
                    MessageBox.Show("주소록 아이디를 입력해주세요");
                    return;
                }
                else
                {

                    addrID = textBox6.Text;
                }
            }
            else
            {
                if(textBox1.Text == "" || textBox1.Text.Length == 0)
                {
                    MessageBox.Show("수신번호를 입력해주세요");
                    return;
                }
                else
                {
                saddrs = textBox1.Text.Replace("-","").Replace(" ","");

                }

            }
            if(textBox2.Text==""|| textBox2.Text.Length == 0)
            {
                MessageBox.Show("발송문서를 등록해주세요");
                return;
            }
            else
            {
            sconts = textBox2.Text;

            }

            if (refchk == true)
            {
               
            }
            if(reschk == true)
            {
                Fdate= dateTimePicker1.Value.ToString("yyyy-MM-dd").Replace(" ", "");
                Ldate= dateTimePicker2.Value.ToString("HH:mm:ss").Replace(" ", "");
                insertDate = Fdate + " " + Ldate;
            }
            else
            {
                insertDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            if(textBox8.Text =="" || textBox8.Text.Length == 0)
            {
                MessageBox.Show("발송번호를 입력해주세요");
                return;
            }
            else
            {
                sfrom = textBox8.Text.Replace("-", "").Replace(" ", "");
            }
            if(addrchk != true)
            {
                if(refchk == true)
                {
                    MessageBox.Show("한 사람에게 보낼때는 치환체크를 해지해주세요");
                }
            }

            string faxConn = "server=192.168.0.34;uid=pwi719;pwd=k4152030!!; database=UMS_DB";
            using (SqlConnection conn = new SqlConnection(faxConn))
            {
                //Console.WriteLine("insertDate : " + insertDate);
                //return;
                string sql = "";
                conn.Open();
                // 여러 사람 
                if(addrchk == true)
                {
                    // 치환문자 사용
                    if(refchk == true)
                    {
                        
                    }
                    else
                    {
                        sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                            "values (1, 1, '{0}', 0, '{1}', '{2}',convert(datetime, '{3}', 120))", addrID, sconts, sfrom, insertDate);
                    }
                }
                // 한 사람
                else
                {
                    sql = string.Format("Insert into t_send(nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                        "values(1, 0, '{0}', {1}, '{2}', {3}, convert(datetime, '{4}', 120))", saddrs, scontstype, sconts, sfrom, insertDate);

                }
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            label12.Text = "접수 완료";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (addrchk == true)
            {
                if (textBox3.Text == "" || textBox3.Text.Length == 0)
                {
                    MessageBox.Show("주소 아이디를 입력해주세요");
                    return;
                }
                if(textBox4.Text ==""|| textBox4.Text.Length == 0)
                {
                    MessageBox.Show("수신번호를 입력해주세요");
                    return;
                }
                if(refchk == true)
                {
                    
                }
                else
                {
                  
                }
                addrID = textBox3.Text.Replace(" ", "");
                isaddrs = textBox4.Text.Replace("-","");

                string addrConn = "server=192.168.0.34;uid=pwi719;pwd=k4152030!!; database=UMS_DB";
                using (SqlConnection conn = new SqlConnection(addrConn))
                {
                    conn.Open();
                    string sql = "";
                    if(refchk == true)
                    {
                        sql = string.Format("Insert into t_addrs(saddrid, saddrs, sref1) values('{0}','{1}','{2}')", addrID, isaddrs, sref1);
                    }
                    else
                    {
                        sql = string.Format("Insert into t_addrs(saddrid, saddrs) values('{0}','{1}')", addrID, isaddrs);
                    }

                    SqlCommand comm = new SqlCommand(sql, conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
                label25.Text = addrID + " 접수 완료";
            }
            else
            {
                MessageBox.Show("주소록 체크박스 클릭해주세요");
                return;
            }
            


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                addrchk = true;
            }
            else
            {
                addrchk = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                reschk = true;
            }
            else
            {
                reschk = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "일반 문서|*.doc; *.hwp; *.txt|TIFF 파일|*.tiff";
            ofd.Multiselect = true;
            ofd.ShowDialog();

            string[] filePaths = ofd.FileNames;
            string filePath = "";
            string bfilePath = "";
               
            if(filePaths.Length > 1)
            {      
                foreach(string s in filePaths)
                {
                    filePath = Path.GetFileName(s);
                    bfilePath += filePath + ";";                 
                }
                filePath = bfilePath.Substring(0, bfilePath.Length - 1);    
            }
            else
            {            
                filePath = Path.GetFileName(ofd.FileName);
            }
            textBox2.Text = filePath;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedItem.ToString() == ".doc |.txt |.hwp")
            {
                scontstype = 0;
            }
            else
            {
                scontstype = 1;
            }          
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
            int i = richTextBox1.SelectionStart;
            richTextBox1.Text = richTextBox1.Text.Substring(0, i) + "[수신1]" + richTextBox1.Text.Substring(i);     
            richTextBox1.SelectionStart = i;
        }

        private void button9_Click(object sender, EventArgs e)
        {
         
            int i = richTextBox1.SelectionStart;
            richTextBox1.Text = richTextBox1.Text.Substring(0, i) + "[수신2]" + richTextBox1.Text.Substring(i);
            richTextBox1.SelectionStart = i;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox4.Checked == true)
            {
                Maddrchk = true;
            }
            else
            {
                Maddrchk = false;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Maddrchk == true)
            {              
                if (textBox10.Text == "" || textBox10.Text.Length == 0)
                {
                    MessageBox.Show("수신 번호를 입력해주세요");
                    return;
                }
                else
                {
                    Maddr = textBox10.Text.Replace("-", "").Replace(" ", "");
                }
                if (textBox11.Text == "" || textBox11.Text.Length == 0)
                {
                    MessageBox.Show("주소명을 입력해주세요");
                    return;
                }
                else
                {
                    MaddrID = textBox11.Text.Replace(" ", "");
                }
                if (Mrefchk == true)
                {
                    if ((textBox12.Text == "" || textBox12.Text.Length == 0) && (textBox13.Text == "" || textBox13.Text.Length == 0))
                    {
                        MessageBox.Show("치환문자를 입력해주세요");
                        return;
                    }
                    else
                    {
                        Mref1 = textBox12.Text;
                        Mref2 = textBox13.Text;
                    }
                }
                else
                {
                    if ((textBox12.Text != "" || textBox12.Text.Length != 0) || (textBox13.Text != "" || textBox13.Text.Length != 0))
                    {
                        MessageBox.Show("치환 기능을 클릭해주세요");
                        return;
                    }
                }

                string AddrString = "server=192.168.0.34;uid=pwi719;pwd=k4152030!!; database=UMS_DB";
                using (SqlConnection conn = new SqlConnection(AddrString))
                {
                    conn.Open();
                    string sql = "";
                    if (Mrefchk == true)
                    {
                        sql = string.Format("Insert into t_addrs(saddrid, saddrs, sref1, sref2) values('{0}','{1}','{2}', '{3}')", MaddrID, Maddr, Mref1, Mref2);
                    }
                    else
                    {
                        sql = string.Format("Insert into t_addrs(saddrid, saddrs) values('{0}','{1}')", MaddrID, Maddr);
                    }

                    SqlCommand comm = new SqlCommand(sql, conn);
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    conn.Close();
                }
                label24.Text = MaddrID + " " + "접수 완료";
            }
            else
            {
                MessageBox.Show("주소록 기능을 체크해주세요");
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox6.Checked == true)
            {
                Mrefchk = true;
            }
            else
            {
                Mrefchk = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Maddr = textBox10.Text.Replace(" ","").Replace("-","");
            Mconte = richTextBox1.Text;
            Mconts = textBox14.Text;

            if(textBox9.Text =="" || textBox9.Text.Length == 0)
            {
                MessageBox.Show("발송번호를 입력해주세요");
                return;
            }
            else
            {
                Msfrom = textBox9.Text.Replace(" ", "").Replace("-", "");
            }

            if(Mreschk == true)
            {
                MFdate = dateTimePicker1.Value.ToString("yyyy-MM-dd").Replace(" ", "");
                MLdate = dateTimePicker2.Value.ToString("HH:mm:ss").Replace(" ", "");
                MinsertDate = Fdate + " " + Ldate;
            }else
            {
                MinsertDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            string MsgString = "server=192.168.0.34;uid=pwi719;pwd=k4152030!!; database=UMS_DB";
            using (SqlConnection conn = new SqlConnection(MsgString))
            {
                conn.Open();
                string sql = "";
                // MMS전송
                if (Mmms == true)
                {
                    // 주소록
                    if (Maddrchk == true)
                    {
                        // 치환 
                        if (Mrefchk == true)
                        {
                            sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                "values (6, 1, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", MaddrID, Mconts, MinsertDate); 
                        }
                        else
                        {
                            sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                "values (6, 1, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", MaddrID, Mconts, Msfrom, MinsertDate);
                        }
                    }
                    // 개별
                    else
                    {                       
                        sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                            "values (6, 0, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", Maddr, Mconts, Msfrom, MinsertDate);
                    }
                }
                // SMS LMS전송
                else
                {
                  
                    if (label22.Text == "SMS")
                    {
                        
                        // 주소록
                        if (Maddrchk == true)
                        {
                            // 치환 
                            if (Mrefchk == true)
                            {
                                Console.WriteLine("여기로 오냐");
                                MaddrID = textBox11.Text.Replace(" ", "");
                                sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                    "values (3, 1, '{0}', 0, '{1}','{2}', convert(datetime, '{3}', 120))", MaddrID, Mconte, Msfrom, MinsertDate);
                            }
                            else
                            {
                                if (richTextBox1.Text == "" || richTextBox1.Text.Length == 0)
                                {
                                    MessageBox.Show("문자내용을 입력해주세요");
                                    return;
                                }
                                else
                                {
                                    Mconte = richTextBox1.Text;
                                    sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                        "values (3, 1, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", MaddrID, Mconte, Msfrom, MinsertDate);
                                }
                            }
                        }
                        // 개별
                        else
                        {
                            if (richTextBox1.Text == "" || richTextBox1.Text.Length == 0)
                            {
                                MessageBox.Show("문자내용을 입력해주세요");
                                return;
                            }
                            else
                            {
                                Mconte = richTextBox1.Text;
                                sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                "values (3, 0, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", Maddr, Mconte, Msfrom, MinsertDate);

                            }
                        }
                    }
                    else if(label22.Text == "LMS")
                    {
                     
                        // 주소록
                        if (Maddrchk == true)
                        {
                            // 치환 
                            if (Mrefchk == true)
                            {
                                sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                 "values (5, 1, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", MaddrID, Mconts, MinsertDate);
                            }
                            else
                            {
                                if (richTextBox1.Text == "" || richTextBox1.Text.Length == 0)
                                {
                                    MessageBox.Show("문자내용을 입력해주세요");
                                    return;
                                }
                                else
                                {
                                    sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                               "values (5, 1, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", MaddrID, Mconts, Msfrom, MinsertDate);
                                }
                            }
                        }
                        // 개별
                        else
                        {
                                                            
                                sql = string.Format("insert into t_send (nsvctype, naddrtype, saddrs, ncontstype, sconts, sfrom, dtstarttime) " +
                                 "values (5, 0, '{0}', 0, '{1}', '{2}', convert(datetime, '{3}', 120))", Maddr, Mconts, Msfrom, MinsertDate);                    
                        }
                    }
                    
                }
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
                label23.Text = "접수완료";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "이미지 파일|*.jpg; *.png; *.jpeg|텍스트 파일|*.txt";
            ofd1.Multiselect = true;
            ofd1.ShowDialog();

            string[] filePaths = ofd1.FileNames;
            string filePath = "";
            string bfilePath = "";

            if (filePaths.Length > 1)
            {
                foreach (string s in filePaths)
                {
                    filePath = Path.GetFileName(s);
                    bfilePath += filePath + ";";
                }
                filePath = bfilePath.Substring(0, bfilePath.Length - 1);
            }
            else
            {
                filePath = Path.GetFileName(ofd1.FileName);
            }
            textBox14.Text = filePath;
            string m1 = ".jpg";
            string m2 = ".png";
            string m3 = ".jpeg";
            if (textBox14.Text.Contains(m1) || textBox14.Text.Contains(m2) || textBox14.Text.Contains(m3))
            {
                label22.Text = "MMS";
                Mmms = true;
            }
            else
            {
                label22.Text = "LMS";
                Mmms = false;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string rtext = richTextBox1.Text;
            char[] tempChr = rtext.ToCharArray();
            BchkMethod(rtext, tempChr);
        }

        private void BchkMethod(string text, char[] tempChr)
        {
            int bCnt = 0;

            foreach (char c in tempChr)
            {
                int chr = (int)c;
                if (chr > 122)
                {
                    bCnt += 2;
                }
                else
                {
                    bCnt += 1;
                }
            }
            label20.Text = bCnt.ToString();

            if (Mmms == true)
            {
                label22.Text = "MMS";
            }
            else
            {
                if (0 < bCnt && bCnt <= 90)
                {
                    label22.Text = "SMS";

                }
                else if (bCnt > 90 && bCnt <= 2000)
                {
                    label22.Text = "LMS";
                }
                else if (bCnt > 2000)
                {
                    MessageBox.Show("글자수 초과되었습니다");
                    text = text.Substring(0, text.Length - 1);
                    richTextBox1.Text = text;
                    richTextBox1.Select(richTextBox1.Text.Length, 0);
                    char[] cChr = text.ToCharArray();
                    BchkMethod(text, cChr);
                }
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
           if(textBox14.Text == "" || textBox14.Text.Length == 0)
            {
                Mmms = false;
                string rtext = richTextBox1.Text;
                char[] tempChr = rtext.ToCharArray();
                BchkMethod(rtext, tempChr);
            }
            else
            {   
                string m1 = ".jpg";
                string m2 = ".png";
                string m3 = ".jpeg";
                if(textBox14.Text.Contains(m1)|| textBox14.Text.Contains(m2)|| textBox14.Text.Contains(m3))
                {
                    label22.Text = "MMS";
                    Mmms = true;
                }
                else
                {
                    label22.Text = "LMS";
                    Mmms = false;
                }

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox6.Text = "";
            textBox8.Text = "";
            textBox2.Text = "";
           
            textBox3.Text = "";
            textBox4.Text = "";
         
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox9.Text = "";
            textBox10.Text = "";
            textBox14.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            richTextBox1.Text = "";
            label23.Text = "";
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                Mreschk = true;
                MessageBox.Show("예약전송을 적용하였습니다");
            }
            else
            {
                Mreschk = false;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string sjobid = "";
            string sconts = "";
            string sfrom = "";
            string dtStartTime = "";
            string dtEndTime = "";
            string nResult = "";
            string sResult = "";
            
            string sFrom = "";
            string fDate = "";
            string tDate = "";
            string fTime = "";
            string tTime = "";

            sFrom = textBox15.Text.Replace("-", "").Replace(" ", "");
            fDate = dateTimePicker5.Value.ToString("yyyy-MM-dd").Replace(" ","");
            fTime = dateTimePicker6.Value.ToString("HH:mm:ss").Replace(" ", "");
            fDate = fDate + " " + fTime;

            tDate = dateTimePicker7.Value.ToString("yyyy-MM-dd").Replace(" ","");
            tTime = dateTimePicker8.Value.ToString("HH:mm:ss").Replace(" ","");
            tDate = tDate + " " + tTime;

          

            if(sFrom == "" || sFrom.Length == 0)
            {
                MessageBox.Show("발신번호를 입력해주세요");
            }
            string resSql = "server=192.168.0.34;uid=pwi719;pwd=k4152030!!; database=UMS_DB";
            using (SqlConnection conn = new SqlConnection(resSql))
            {
                conn.Open();
                string res_sql = string.Format("select sjobid, replace(replace(sconts, char(13), ''), char(10), '') as conts, sfrom, dtstarttime, dtendtime, nresult, sresult from " +
                    "t_send with(nolock) where sfrom='{0}' and dtstarttime between Convert(datetime,'{1}',120) and Convert(datetime,'{2}',120)", sFrom, fDate, tDate);

                SqlCommand comm = new SqlCommand(res_sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                ListViewItem lvi;
               
                listView1.BeginUpdate();

                while (sr.Read())
                {

                    sjobid = sr["sjobid"].ToString();
                    sconts = sr["conts"].ToString();
                    sfrom = sr["sfrom"].ToString();
                    dtStartTime = sr["dtstarttime"].ToString();
                    dtEndTime = sr["dtendtime"].ToString();
                    nResult = sr["nresult"].ToString();
                    sResult = sr["sresult"].ToString();

                    lvi = new ListViewItem(sjobid);
                    lvi.SubItems.Add(sconts);
                    lvi.SubItems.Add(sfrom);
                    lvi.SubItems.Add(dtStartTime);
                    lvi.SubItems.Add(dtEndTime);
                    lvi.SubItems.Add(nResult);
                    lvi.SubItems.Add(sResult);

                    listView1.Items.Add(lvi);
                }

                listView1.EndUpdate();
                sr.Close();
                comm.Dispose();
                conn.Close();
            }
        }
    }
}
