using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.PowerPacks; //匯入VB向量繪圖功能

namespace UDP_Grafiti_Wall
{
    public partial class Form1 : Form
    {
        UdpClient U;    //UDP通訊物件
        Thread Th;  //監聽用執行緒
        public Form1()
        {
            InitializeComponent();
        }


        //啟動監聽按鈕程序
        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;    //忽略跨執行緒錯誤
            Th = new Thread(Listen);    // 建立監聽執行緒，目標副執行緒(Listen)
            Th.Start(); //啟動監聽執行緒
            button1.Enabled = false;    //使按鍵失效，不能(也不需要)重複開啟監聽
        }

        //監聽副程序
        private void Listen()
        {
            int Port = int.Parse(textBox3.Text);    //設定監聽用的通訊埠
            U = new UdpClient(Port);    //監聽UDP監聽器實體

            //建立本機端點資訊
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);

            //持續監聽的無限迴圈：有訊息(True)就處理，無訊息就等待
            while (true)
            {
                byte[] B = U.Receive(ref EP);   //訊息到達時讀取資訊到B[]
                string A = Encoding.Default.GetString(B);   //翻譯B陣列為字串A
                string[] Q = A.Split('/');  //切割座標點資訊
                Point[] R = new Point[Q.Length];    //宣告座標點陣列
                for(int i = 0; i < Q.Length; i++)
                {
                    string[] K = Q[i].Split(',');   //切割X與Y座標
                    R[i].X = int.Parse(K[0]);   //意義第i點X座標
                    R[i].Y = int.Parse(K[1]);   //意義第i點Y座標
                }
                for(int i = 0; i < Q.Length - 1; i++)
                {
                    LineShape L = new LineShape();  //建立線段物件
                    L.StartPoint = R[i];    //線段起點
                    L.EndPoint = R[i + 1];  //線段終點
                    L.Parent = D;   //線段L加入畫布D(遠端使用者繪圖)
                }
            }
        }

        //關閉監聽執行緒
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort(); //關閉監聽執行緒
                U.Close();  //關閉監聽器
            }
            catch
            {
                //忽略錯誤繼續執行
            }
        }

        //繪圖相關變數宣告
        ShapeContainer C;   //畫布物件(本機繪圖用)
        ShapeContainer D;   //畫布物件(遠端繪圖用)
        Point stP;  //繪圖起點
        string p;   //筆畫座標字串

        //表單載入
        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer();   //建立畫布(本機繪圖用)
            this.Controls.Add(C);   //加入畫布C到表單
            D = new ShapeContainer();   //建立畫布(遠端繪圖用)
            this.Controls.Add(D);   //加入畫布D到表單
        }

        //本機端開始繪圖
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location;   //起點
            p = stP.X.ToString() + "," + stP.Y.ToString();    //起點座標紀錄
        }

        //本機繪圖中
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();  //建立線段物件
                L.StartPoint = stP; //線段起點
                L.EndPoint = e.Location;    //線段終點
                L.Parent = C;   //線段加入畫布C
                stP = e.Location;   //終點變起點
                p += "/" + stP.X.ToString() + "," + stP.Y.ToString();   //持續記錄座標
            }
        }

        //送出繪圖動作
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int Port = int.Parse(textBox2.Text);    //設定發送的目標Port
            UdpClient S = new UdpClient(textBox1.Text, Port);   //建立UDP物件
            byte[] B = Encoding.Default.GetBytes(p);    //翻譯p字串為B陣列
            S.Send(B, B.Length);    //發送資料
            S.Close();  //關閉UDP物件
        }
    }
}