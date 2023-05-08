using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.PowerPacks; //�פJVB�V�qø�ϥ\��

namespace UDP_Grafiti_Wall
{
    public partial class Form1 : Form
    {
        UdpClient U;    //UDP�q�T����
        Thread Th;  //��ť�ΰ����
        public Form1()
        {
            InitializeComponent();
        }


        //�Ұʺ�ť���s�{��
        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;    //�������������~
            Th = new Thread(Listen);    // �إߺ�ť������A�ؼаư����(Listen)
            Th.Start(); //�Ұʺ�ť�����
            button1.Enabled = false;    //�ϫ��䥢�ġA����(�]���ݭn)���ƶ}�Һ�ť
        }

        //��ť�Ƶ{��
        private void Listen()
        {
            int Port = int.Parse(textBox3.Text);    //�]�w��ť�Ϊ��q�T��
            U = new UdpClient(Port);    //��ťUDP��ť������

            //�إߥ������I��T
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);

            //�����ť���L���j��G���T��(True)�N�B�z�A�L�T���N����
            while (true)
            {
                byte[] B = U.Receive(ref EP);   //�T����F��Ū����T��B[]
                string A = Encoding.Default.GetString(B);   //½ĶB�}�C���r��A
                string[] Q = A.Split('/');  //���ήy���I��T
                Point[] R = new Point[Q.Length];    //�ŧi�y���I�}�C
                for(int i = 0; i < Q.Length; i++)
                {
                    string[] K = Q[i].Split(',');   //����X�PY�y��
                    R[i].X = int.Parse(K[0]);   //�N�q��i�IX�y��
                    R[i].Y = int.Parse(K[1]);   //�N�q��i�IY�y��
                }
                for(int i = 0; i < Q.Length - 1; i++)
                {
                    LineShape L = new LineShape();  //�إ߽u�q����
                    L.StartPoint = R[i];    //�u�q�_�I
                    L.EndPoint = R[i + 1];  //�u�q���I
                    L.Parent = D;   //�u�qL�[�J�e��D(���ݨϥΪ�ø��)
                }
            }
        }

        //������ť�����
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort(); //������ť�����
                U.Close();  //������ť��
            }
            catch
            {
                //�������~�~�����
            }
        }

        //ø�Ϭ����ܼƫŧi
        ShapeContainer C;   //�e������(����ø�ϥ�)
        ShapeContainer D;   //�e������(����ø�ϥ�)
        Point stP;  //ø�ϰ_�I
        string p;   //���e�y�Цr��

        //�����J
        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer();   //�إߵe��(����ø�ϥ�)
            this.Controls.Add(C);   //�[�J�e��C����
            D = new ShapeContainer();   //�إߵe��(����ø�ϥ�)
            this.Controls.Add(D);   //�[�J�e��D����
        }

        //�����ݶ}�lø��
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location;   //�_�I
            p = stP.X.ToString() + "," + stP.Y.ToString();    //�_�I�y�Ь���
        }

        //����ø�Ϥ�
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();  //�إ߽u�q����
                L.StartPoint = stP; //�u�q�_�I
                L.EndPoint = e.Location;    //�u�q���I
                L.Parent = C;   //�u�q�[�J�e��C
                stP = e.Location;   //���I�ܰ_�I
                p += "/" + stP.X.ToString() + "," + stP.Y.ToString();   //����O���y��
            }
        }

        //�e�Xø�ϰʧ@
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int Port = int.Parse(textBox2.Text);    //�]�w�o�e���ؼ�Port
            UdpClient S = new UdpClient(textBox1.Text, Port);   //�إ�UDP����
            byte[] B = Encoding.Default.GetBytes(p);    //½Ķp�r�ꬰB�}�C
            S.Send(B, B.Length);    //�o�e���
            S.Close();  //����UDP����
        }
    }
}