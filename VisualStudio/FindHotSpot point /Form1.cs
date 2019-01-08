using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System.IO;

namespace FindHotSpot
{

    public partial class srcpictureBoxIpl : Form
    {
        //画素値の平均は射影変換後にとった方がいい
        //元画像
        private IplImage org_com_1;
        private IplImage org_com_2;
        private IplImage org_com_3;
        //入力グレースケール画像
        private IplImage gray_in_com_1;
        private IplImage gray_in_com_3;
        private IplImage gray_in_com_2;
        // 入力画像　射影変換後白黒
        private IplImage H_org_1_com;
        private IplImage H_org_2_com;
        private IplImage H_org_3_com;
        //二値化後の画像格納
        private IplImage dst_dst;
        private IplImage dst_dst2;
        private IplImage dst_dst3;
        //画像の大きさ
        private int Height_com = 237;//47.4 *5
        private int Width_com = 319;//63.8*5

        //係数共通
        private int R_com = 40;
        private int blockSize_com = 131;
        //画像初期化ヒント
        //IplImage image = Cv.CreateImage(new CvSize(Width_com, Height_com), BitDepth.U8, 1);
        //最終処理結果(トリミング済)
        // 描画用Graphicsオブジェクト  
        private Graphics g = null;
        // クリック位置の描画用座標
        private System.Drawing.Point[] point = new System.Drawing.Point[4];
        private int clickcnt = 0;
        public srcpictureBoxIpl()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //
        //Left
        //
        private void Open_1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                org_com_1 = Cv.LoadImage(openFileDialog1.FileName);
                gray_in_com_1 = Cv.CreateImage(new CvSize(org_com_1.Width, org_com_1.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_1, gray_in_com_1, ColorConversion.BgrToGray);//グレースケール変換
                Cv.Not(gray_in_com_1, gray_in_com_1);
                dst1pic.ImageIpl = gray_in_com_1;
                resized_orgpic.ImageIpl = resize(org_com_1);
                action.Text = "左　入力画像 左上下右下上-四隅指定";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "左方向  元画像";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
            }
        }
        //
        //Right
        //
        private void Open_2_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                org_com_3 = Cv.LoadImage(openFileDialog3.FileName);
                gray_in_com_3 = Cv.CreateImage(new CvSize(org_com_3.Width, org_com_3.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_3, gray_in_com_3, ColorConversion.BgrToGray);//グレースケール変換
                Cv.Not(gray_in_com_3, gray_in_com_3);
                dst3pic.ImageIpl = gray_in_com_3;
                resized_org2pic.ImageIpl = resize(org_com_3);
                action.Text = "";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
                action3.Text = "正面　入力画像 左上下右下上-四隅指定";
                action.Text = "左　入力画像 左上下右下上-四隅指定";
                action2.Text = "右　入力画像 左上下右下上-四隅指定";
                rightsrc_name.Text = "右方向  元画像";
            }
        }
        //
        // Center
        //
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                org_com_2 = Cv.LoadImage(openFileDialog2.FileName);
                gray_in_com_2 = Cv.CreateImage(new CvSize(org_com_2.Width, org_com_2.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_2, gray_in_com_2, ColorConversion.BgrToGray);//グレースケール変換
                Cv.Not(gray_in_com_2, gray_in_com_2);
                HotSpotpic.ImageIpl = gray_in_com_2;
                resized_Center_orgpic.ImageIpl = resize(org_com_2);
                action.Text = "";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
                action3.Text = "正面　入力画像 左上下右下上-四隅指定";
                action.Text = "左　入力画像 左上下右下上-四隅指定";
                centersrc_name.Text = "正面方向  元画像";
            }
        }

        //
        //544射影変換ボタン
        //
        private void Homo_GO_Click(object sender, EventArgs e)
        {
            H_org_1_com = Homo(gray_in_com_1);
            resized_src1.ImageIpl = resize(H_org_1_com);
            L_Ave.Text = Ave_H_Pixel_Get(H_org_1_com).ToString();
            Ave_com_text.Text = L_Ave.Text;
            action.Text = "";
            action3.Text = "";
            action2.Text = "";
            leftsrc_name.Text = "";
            centersrc_name.Text = "";
            rightsrc_name.Text = "";
            lefthomo_name.Text = "左方向　射影変換";
            centerhomo_name.Text = "";
            righthomo_name.Text = "";
            leftbin_name.Text = "";
            centerbin_name.Text = "";
            rightbin_name.Text = "";
        }

        private void Homo_GO3_Click(object sender, EventArgs e)
        {
            H_org_3_com = Homo(gray_in_com_3);
            resized_src3.ImageIpl = resize(H_org_3_com);
            R_Ave.Text = Ave_H_Pixel_Get(H_org_3_com).ToString();
            action.Text = "";
            action3.Text = "";
            action2.Text = "";
            leftsrc_name.Text = "";
            centersrc_name.Text = "";
            rightsrc_name.Text = "";
            lefthomo_name.Text = "";
            centerhomo_name.Text = "";
            righthomo_name.Text = "";
            leftbin_name.Text = "";
            centerbin_name.Text = "";
            rightbin_name.Text = "";
            righthomo_name.Text = "右方向　射影変換";
        }
        //
        //　合成ボタン　今回ここに画像合成を
        //
        private void Bin_Click(object sender, EventArgs e)
        {
            //
            // 二値化処理
            // 画素の平均値を引数として渡せば良いのではないか
            
            Cv.EqualizeHist(H_org_1_com, H_org_1_com);//正規化
            Cv.EqualizeHist(H_org_2_com, H_org_2_com);//正規化
            Cv.EqualizeHist(H_org_3_com, H_org_3_com);//正規化
            
            dst_dst = dstget(H_org_1_com);
            dst_dst2 = dstget(H_org_2_com);
            dst_dst3 = dstget(H_org_3_com);
            //リサイズ版

            bin1_resized.ImageIpl = resize(dst_dst);
            bin3_resized.ImageIpl = resize(dst_dst2);
            bin2_resized.ImageIpl = resize(dst_dst3);

            //表示版
            dst1pic.ImageIpl = dst_dst;
            HotSpotpic.ImageIpl = dst_dst2;
            dst3pic.ImageIpl = dst_dst3;



            action.Text = "";
            action3.Text = "";
            action2.Text = "";
            leftsrc_name.Text = "";
            centersrc_name.Text = "";
            rightsrc_name.Text = "";
            lefthomo_name.Text = "";
            centerhomo_name.Text = "";
            righthomo_name.Text = "";
            leftbin_name.Text = "";
            centerbin_name.Text = "";
            rightbin_name.Text = "";
            action.Text = "";
            action3.Text = "画像合成";
            action2.Text = "";

            leftbin_name.Text = "";
            centerbin_name.Text = "画像合成";
            rightbin_name.Text = "";
        }
        //
        //ホットスポットを見つけるボタン
        //
        private void Bin_Click_1(object sender, EventArgs e)
        {
            //三枚で共通して黒いところだけ残す
            HotSpotpic.ImageIpl = mix_ave(dst_dst, dst_dst2, dst_dst3);
          //  int cou;
          // int cou = pixcount(HotSpotpic.ImageIpl)
            //テキストボックスに表示するためにストリング型に
            Hottext.Text = pixcount().ToString();
            //trimming(HotSpotpic.ImageIpl, 0, 0, Width, Height)).SaveImage("result.bmp");
            action3.Text = "ver" + vertext.Text + " : No-" + comboBox4.Text + " " + comboBox1.Text + "月" + comboBox2.Text + "日" + comboBox3.Text + comboBox5.Text + "時" + ":B-" + blocksize_text.Text + ":R-" + Sikii.Text + ":K-" + Ksikii.Text;
            action.Text = "左 正面 共通部分";
            action2.Text = "右 正面 共通部分";
            leftsrc_name.Text = "左方向  元画像";
            lefthomo_name.Text = "左方向　射影変換";
            centersrc_name.Text = "正面方向  元画像";
            centerhomo_name.Text = "正面方向　射影変換";
            rightsrc_name.Text = "右方向  元画像";
            righthomo_name.Text = "右方向　射影変換";
            leftbin_name.Text = "左 二値化";
            centerbin_name.Text = "中央 二値化";
            rightbin_name.Text = "右 二値化";

        }

        //
        //全画素の平均値で合成する関す
        //

        //IplImageからCvMatに変換する
        CvMat IplImageToMat(IplImage image)
        {
            return Cv.GetMat(image);
        }
        //CvMatからIplImageへ変換する
        IplImage CvMatToIplImage(CvMat cvmat)
        {
            return Cv.GetImage(cvmat);
        }
        //Mat からIplImageへ変換する もっといい方法があると思います.
        IplImage MatToIplImage(Mat Before_Mat)
        {
            Bitmap Bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Before_Mat);
            IplImage After_IplImage = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(Bitmap);
            return After_IplImage;
        }

        //
        //★★★全画素アクセス★★★
        //


        /*
        //
        //bitmap iplimage
        //
        // Bitmap to IplImage
        IplImage iplOriginal = (OpenCvSharp.IplImage)BitmapConverter.ToIplImage(beforeBitmap);

        // IplImage to Bitmap
        Bitmap afterBitmap = BitmapConverter.ToBitmap(iplOriginal);
        */

        //
        //                  ★★★エラーの原因？intをbyteに変えてキャストする必要があると思う★★★
        //                  自作の関数なのでガバガバだと思います

        IplImage mix_ave(IplImage Mix_1, IplImage Mix_2, IplImage Mix_3)
        {

            /*
            //
            //CvMatだとこれ...CvMatだとmat.At使えない
            //
            CvMat Mix_1_Mat = ToMat(Mix_1);
            CvMat Mix_2_Mat = ToMat(Mix_2);
            CvMat Mix_3_Mat = ToMat(Mix_3);
            CvMat Mixed_Mat = Mix_1_Mat;
            */


            //
            //Mat だとこれ
            //
            using (Mat Mix_1_Mat = new Mat(Mix_1))
            using (Mat Mix_2_Mat = new Mat(Mix_2))
            using (Mat Mix_3_Mat = new Mat(Mix_3))
            using (Mat Mixed_Mat = new Mat(Mix_1_Mat.Height, Mix_2_Mat.Width, MatType.CV_8UC1)) //同じ大きさ、同じ型であれば良い.
            {
               

                //CvMat a = new CvMat(Mix_1); だめ
                //CvMat a = Cv2.CvArrToMat(Mix_1); だめ
                //CvMat a = Cv.cvGetImage(Mix_1); だめ

                //CvMat a = Cv.GetMat(Mix_1); //OK
                int x, y;
                for (y = 0; y < Mix_1.Height; y++)
                {
                    for (x = 0; x < Mix_1.Width; x++)
                    {
                        //BGR 青　緑　赤　グレースケール画像なのでどの要素も画素数は等しい
                        // int intensity = Mixed_Mat.At<Vecd>(y, x);

                        int pix_mixed = Mixed_Mat.At<int>(y, x);
                        int pix_1 = Mix_1_Mat.At<int>(y, x);
                        int pix_2 = Mix_2_Mat.At<int>(y, x);
                        int pix_3 = Mix_3_Mat.At<int>(y, x);

                        //   if (pix_1 > 200 || pix_2 > 200 || pix_3 > 200)
                        //     { pix_mixed = 255;  }
                        //   else  {
                        //
                        //三枚で共通して黒いところだけ残す　全画素アクセス苦労した
                        //
                        if ((pix_1 + pix_2 + pix_3) == 0)
                        {
                            pix_mixed = 0;
                        }
                        else
                        {
                            pix_mixed = 255;//(pix_1 + pix_2 + pix_3) / 3;　なぜかこれ変になる

                        }
                        //if (pix_mixed > 165) pix_mixed = 255;
                        //     }

                        // (pix_mixed  !=0 ) { pix_mixed = 255; }// 三枚とも重なっているところだけ0黒 255白
                        Mixed_Mat.Set<int>(y, x, pix_mixed);
                    }


                }
                //IplImage Mixed = new IplImage(Mixed_Mat); //だめ
                IplImage Mixed = MatToIplImage(Mixed_Mat);
                // Cv.ShowImage("a",Mixed);
                return Mixed;
            }
        }

        //
        //  黒が出力されているピクセル数を数える ホットスポットだけカウントしたいので枠を除外-30　289(319)*207(237)　59823　が全ピクセル数　★★★ここもbyteをintに★★★
        //  自作関数なのでガバガバです

        int pixcount()
        {

            int i, j;
            int count = 0;
            using (IplImage count_tori = trimming(HotSpotpic.ImageIpl, 0, 0, Width, Height))
            using (Mat Mat_pix_count = new Mat(count_tori))
            {
                for (i = 0; i < Width; i++)
                {
                    for (j = 0; j < Height; j++)
                    {
                        byte tmppix_count = Mat_pix_count.At<byte>(j, i);
                        int pix_count = (int)tmppix_count;
                        if (pix_count == 0)
                        {
                            count++;
                        }
                    }
                }
               

            }
            return count;

        }
        private void SAVE1_Click(object sender, EventArgs e)
        {
            //IplImage save_gray = grayget();
            dst1pic.ImageIpl= H_org_1_com;
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = "01Left_homo";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する


                dst1pic.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                action.Text = "保存しました。";
            }

        }

        private void SAVE2_Click(object sender, EventArgs e)
        {
            //IplImage save_gray = grayget();
            HotSpotpic.ImageIpl= H_org_2_com;
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = "02Center_homo";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する


                HotSpotpic.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                action3.Text = "保存しました。";
            }
        }

        private void SAVE3_Click(object sender, EventArgs e)
        {
            //IplImage save_gray = grayget();
            dst3pic.ImageIpl = H_org_3_com;
            //SaveFileDialogクラスのインスタンスを作成
            SaveFileDialog sfd = new SaveFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            sfd.FileName = "03Right_homo";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            sfd.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;

            //ダイアログを表示する
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する


                dst3pic.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                action2.Text = "保存しました。";
            }
        }
        private void Point_get(object sender, MouseEventArgs e)
        {
            // クリックした座標を取得
            point[clickcnt].X = e.X;
            point[clickcnt].Y = e.Y;

            //System.Diagnostics.Debug.WriteLine("({0},{1}) {2}", point[clickcnt].X, point[clickcnt].Y, clickcnt);

            // クリックした位置に点を描画
            g = dst1pic.CreateGraphics();
            g.FillEllipse(Brushes.Aqua, point[clickcnt].X, point[clickcnt].Y, 10, 10);

            // 点の間でLineを引く
            if (clickcnt != 0)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[clickcnt - 1]);
                p.Dispose();
            }
            if (clickcnt == point.Length - 1)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[(clickcnt + 1) % point.Length]);
                p.Dispose();
            }

            g.Dispose();

            clickcnt = (clickcnt + 1) % point.Length;
        }

        private void Point_get2(object sender, MouseEventArgs e)
        {
            // クリックした座標を取得
            point[clickcnt].X = e.X;
            point[clickcnt].Y = e.Y;

            //System.Diagnostics.Debug.WriteLine("({0},{1}) {2}", point[clickcnt].X, point[clickcnt].Y, clickcnt);

            // クリックした位置に点を描画
            g = HotSpotpic.CreateGraphics();
            g.FillEllipse(Brushes.Aqua, point[clickcnt].X, point[clickcnt].Y, 10, 10);

            // 点の間でLineを引く
            if (clickcnt != 0)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[clickcnt - 1]);
                p.Dispose();
            }
            if (clickcnt == point.Length - 1)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[(clickcnt + 1) % point.Length]);
                p.Dispose();
            }

            g.Dispose();

            clickcnt = (clickcnt + 1) % point.Length;
        }

        private void Point_get3(object sender, MouseEventArgs e)
        {
            // クリックした座標を取得
            point[clickcnt].X = e.X;
            point[clickcnt].Y = e.Y;

            //System.Diagnostics.Debug.WriteLine("({0},{1}) {2}", point[clickcnt].X, point[clickcnt].Y, clickcnt);

            // クリックした位置に点を描画
            g = dst3pic.CreateGraphics();
            g.FillEllipse(Brushes.Aqua, point[clickcnt].X, point[clickcnt].Y, 10, 10);

            // 点の間でLineを引く
            if (clickcnt != 0)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[clickcnt - 1]);
                p.Dispose();
            }
            if (clickcnt == point.Length - 1)
            {
                Pen p = new Pen(Color.Aqua, 3);
                g.DrawLine(p, point[clickcnt], point[(clickcnt + 1) % point.Length]);
                p.Dispose();
            }

            g.Dispose();

            clickcnt = (clickcnt + 1) % point.Length;
        }
        //
        //画像取得546
        //


        //
        //画像取得548
        //


        //
        //graygetとあるが正規化のためのメゾット 画素値と係数の関係が明らかになれば不要になる。
        //

        /*
        IplImage grayget3()
        {
            IplImage src3 = get3();
            IplImage equal_in3 = Cv.CreateImage(new CvSize(src3.Width, src3.Height), BitDepth.U8, 1);//正規化後の格納用引数
            IplImage gray_in3 = Cv.CreateImage(new CvSize(src3.Width, src3.Height), BitDepth.U8, 1);

            Cv.CvtColor(src3, gray_in3, ColorConversion.BgrToGray);//グレースケール変換
            Cv.EqualizeHist(gray_in3, equal_in3);//正規化
                                               //graypictureBoxIpl.ImageIpl = equal_in;
            action.Text = "グレースケールに変更しました。";
            return equal_in3; // return equal_in;//正規化したものを返す

        }
        */
        //
        //二値化
        //
        IplImage dstget(IplImage src_dst_in)
        {
            Max_Pixel_Get();
            Ave_Pixel_Get();

            Cv.EqualizeHist(src_dst_in, src_dst_in);
            try
            {
                string ks = Ksikii.Text;
                string str = Sikii.Text;
                string str2 = blocksize_text.Text;
                double k = double.Parse(ks);
                int R = int.Parse(str);
                int blockSize = int.Parse(str2);
                OpenCvSharp.Extensions.Binarizer.SauvolaFast(src_dst_in, src_dst_in, blockSize, k, R);
            }
            catch (FormatException)
            {
                error_text.Text = "エラーが発生しました。";
            }
            return src_dst_in;
        }
        //
        //  トリミング関数
        //
        IplImage trimming(IplImage src, int x, int y, int width, int height)
        {
            IplImage dest = new IplImage(width, height, src.Depth, src.NChannels);
            Cv.SetImageROI(src, new CvRect(x, y, width, height));
            dest = Cv.CloneImage(src);
            Cv.ResetImageROI(src);
            return dest;
        }

        //
        //画像小さく
        //
        IplImage resize(IplImage src_size)
        {
            CvSize size = new CvSize(src_size.Width / 3, src_size.Height / 3);
            IplImage resize_out = new IplImage(size, src_size.Depth, src_size.NChannels);
            // Cv.Resize(src_size, resize_out, Interpolation.Cubic);
            Cv.Resize(src_size, resize_out, Interpolation.Lanczos4);
            return resize_out;
        }
        //
        //射影変換　変換する座標指定したい
        //

        private IplImage Homo(IplImage src_homo)
        {
            
                // クリックで取得した座標
                double[] a = new double[]{
                                point[0].X,point[0].Y,
                                point[1].X,point[1].Y,
                                point[2].X,point[2].Y,
                                point[3].X,point[3].Y
                                };

                // 変換先の頂点の座標
                double[] b = new double[]{
                                0, 0,
                                0, 237,
                                319, 237,
                                319, 0
                                };

            using (CvMat Mata = new CvMat(4, 2, MatrixType.F64C1, a))
            using (CvMat Matb = new CvMat(4, 2, MatrixType.F64C1, b))
            {


                // Bitmap bmp = Homo_motopic.Image;
                // IplImage src = BitmapConverter.ToIplImage(bmp);

                IplImage dst = Cv.CreateImage(Cv.Size(512, 512), src_homo.Depth, src_homo.NChannels);

                //homography matrix
                using (CvMat homography = new CvMat(3, 3, MatrixType.F64C1)) {
                   

                    Cv.FindHomography(Mata, Matb, homography);
                    Cv.WarpPerspective(src_homo, dst, homography, Interpolation.Cubic);
                    //dst = trimming(dst, 0, 0, 319, 237);

                    // homography.Dispose();
                }
                return dst;
            }
            // pictureboxを更新
            // UpdateForm(BitmapConverter.ToBitmap(dst));
        }
  
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void src_homo1_Click(object sender, EventArgs e)
        {

        }

        private void src_homo2_Click(object sender, EventArgs e)
        {

        }



        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        //
        //
        //
        //ホットスポットを見つける関数　アルファブレンディング
        //二値化
        //
        //
        //
        IplImage FindHotSpot_1(IplImage src1, IplImage src2)
        {
            IplImage mix = Cv.CreateImage(new CvSize(src1.Width, src1.Height), BitDepth.U8, 1);
            IplImage HotSpot = Cv.CreateImage(new CvSize(src1.Width, src1.Height), BitDepth.U8, 1);

            double alpha = 0.5;
            double beta = 0.5;

            Cv.AddWeighted(src1, alpha, src2, beta, 0, mix);
            Cv.Threshold(mix, HotSpot, 100, 255, ThresholdType.Binary);
            return HotSpot;
        }



        private void resized_dst3pic_Click(object sender, EventArgs e)
        {

        }

        private void HotSpotpic_Click(object sender, EventArgs e)
        {

        }



        private void resized_org_Click(object sender, EventArgs e)
        {

        }

        private void resized_org2_Click(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            H_org_2_com = Homo(gray_in_com_2);
            //H_org2picにできなかった。
            resized_src2.ImageIpl = resize(H_org_2_com);
            C_Ave.Text = Ave_H_Pixel_Get(H_org_2_com).ToString();
            action.Text = "";
            action3.Text = "";
            action2.Text = "";
            leftsrc_name.Text = "";
            centersrc_name.Text = "";
            rightsrc_name.Text = "";
            lefthomo_name.Text = "";
            centerhomo_name.Text = "";
            righthomo_name.Text = "";
            leftbin_name.Text = "";
            centerbin_name.Text = "";
            rightbin_name.Text = "";
            centerhomo_name.Text = "正面方向　射影変換";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Sikii_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void blocksize_text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Bin_Click(sender, e);
                Bin_Click_1(sender, e);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void blocksize_text_TextChanged(object sender, EventArgs e)
        {

        }

        private void Sikii_text_TextChanged(object sender, EventArgs e)
        {
            Bin_Click(sender, e);
            Bin_Click_1(sender, e);
        }

        private void Ksikii_TextChanged(object sender, EventArgs e)
        {
            Bin_Click(sender, e);
            Bin_Click_1(sender, e);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Add("1");
            comboBox1.Items.Add("2");
            comboBox1.Items.Add("3");
            comboBox1.Items.Add("4");
            comboBox1.Items.Add("5");
            comboBox1.Items.Add("6");
            comboBox1.Items.Add("7");
            comboBox1.Items.Add("8");
            comboBox1.Items.Add("9");
            comboBox1.Items.Add("10");
            comboBox1.Items.Add("11");
            comboBox1.Items.Add("12");
        }

        private void comboBox2_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox2.Items.Add("1");
            comboBox2.Items.Add("2");
            comboBox2.Items.Add("3");
            comboBox2.Items.Add("4");
            comboBox2.Items.Add("5");
            comboBox2.Items.Add("6");
            comboBox2.Items.Add("7");
            comboBox2.Items.Add("8");
            comboBox2.Items.Add("9");
            comboBox2.Items.Add("10");
            comboBox2.Items.Add("11");
            comboBox2.Items.Add("12");
            comboBox2.Items.Add("13");
            comboBox2.Items.Add("14");
            comboBox2.Items.Add("15");
            comboBox2.Items.Add("16");
            comboBox2.Items.Add("17");
            comboBox2.Items.Add("18");
            comboBox2.Items.Add("19");
            comboBox2.Items.Add("20");
            comboBox2.Items.Add("21");
            comboBox2.Items.Add("22");
            comboBox2.Items.Add("23");
            comboBox2.Items.Add("24");
            comboBox2.Items.Add("25");
            comboBox2.Items.Add("26");
            comboBox2.Items.Add("27");
            comboBox2.Items.Add("28");
            comboBox2.Items.Add("29");
            comboBox2.Items.Add("30");
            comboBox2.Items.Add("31");

        }

        private void comboBox3_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox3.Items.Add("AM");
            comboBox3.Items.Add("PM");
        }

        private void comboBox4_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox4.Items.Add("1");
            comboBox4.Items.Add("2");
            comboBox4.Items.Add("3");
            comboBox4.Items.Add("4");
            comboBox4.Items.Add("5");
            comboBox4.Items.Add("6");
            comboBox4.Items.Add("7");
            comboBox4.Items.Add("8");
            comboBox4.Items.Add("9");
            comboBox4.Items.Add("10");
            comboBox4.Items.Add("11");
            comboBox4.Items.Add("12");
            comboBox4.Items.Add("13");
            comboBox4.Items.Add("14");
            comboBox4.Items.Add("15");
            comboBox4.Items.Add("16");
            comboBox4.Items.Add("17");
            comboBox4.Items.Add("18");
            comboBox4.Items.Add("19");
            comboBox4.Items.Add("20");
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox5.Items.Add("1");
            comboBox5.Items.Add("2");
            comboBox5.Items.Add("3");
            comboBox5.Items.Add("4");
            comboBox5.Items.Add("5");
            comboBox5.Items.Add("6");
            comboBox5.Items.Add("7");
            comboBox5.Items.Add("8");
            comboBox5.Items.Add("9");
            comboBox5.Items.Add("10");
            comboBox5.Items.Add("11");
            comboBox5.Items.Add("12");
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox6.Items.Add("1");
            comboBox6.Items.Add("2");
            comboBox6.Items.Add("3");
            comboBox6.Items.Add("4");
            comboBox6.Items.Add("5");
            comboBox6.Items.Add("6");
            comboBox6.Items.Add("7");
            comboBox6.Items.Add("8");
            comboBox6.Items.Add("9");
            comboBox6.Items.Add("10");
            comboBox6.Items.Add("11");
            comboBox6.Items.Add("12");
            comboBox6.Items.Add("13");
            comboBox6.Items.Add("14");
            comboBox6.Items.Add("15");
            comboBox6.Items.Add("16");
            comboBox6.Items.Add("17");
            comboBox6.Items.Add("18");
            comboBox6.Items.Add("19");
            comboBox6.Items.Add("20");
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //
        //射影変換後画素の最大値を求める関数
        //トリミング()　合成(addweitedで)　全画素アクセス

        int Max_Pixel_Get()
        {
            using (IplImage Max_mix = trimming(Mix_add(H_org_1_com, H_org_2_com, H_org_3_com), 0, 0, 319, 237))
            using (Mat Max_1_Mat = new Mat(Max_mix))
            {
                int Max = 0;

                //
                //      トリミング関数を使うことで周りの余計な画像を消す
                //

                //
                //      Mat.atでやりたかったが値が14億??????→　unsighedchar　が　byte
                //


               // textに出力する場合のメモ


                //  Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                //  StreamWriter writer =
                //   new StreamWriter(@"C:\Users\Rih0z\Google ドライブ\5S\吉田ラボ\画像合成\アプリ 実験 Sauvola 係数\test documents\Test.txt", true, sjisEnc);

                //C:\Users\Rih0z\Google ドライブ\5S\吉田ラボ\画像合成\アプリ 実験 Sauvola 係数\test documents
                // Cv2.CvtColor(Max_1_Mat, Max_1_Mat, ColorConversion.BgrToGray);

                int x, y;

                for (y = 15; y < Max_1_Mat.Height - 15; y++)
                {
                    for (x = 15; x < Max_1_Mat.Width - 15; x++)
                    {

                        byte tmppix_Max_1 = Max_1_Mat.At<byte>(y, x);
                        int pix_Max_1 = (int)tmppix_Max_1;

                        if (pix_Max_1 > Max) Max = pix_Max_1;
                        /*
                        if ((pix_Max_1 / 10) == 0)
                        {
                            writer.WriteLine("  "+pix_Max_1.ToString());

                        }

                        else if ((pix_Max_1 / 100) == 0)
                        {
                           writer.WriteLine(" " + pix_Max_1.ToString());
                        }

                        else if ((pix_Max_1 / 1000) == 0)
                        {
                           writer.WriteLine(pix_Max_1.ToString());
                        }
                        */
                    }
                    // writer.WriteLine("\n");
                }
                //writer.Close();

                Max_text.Text = Max.ToString();

                return Max;
            }
        }

        int Ave_Pixel_Get()
        {
            using (IplImage Ave_mix = trimming(Mix_add(H_org_1_com, H_org_2_com, H_org_3_com), 0, 0, 319, 237))
            using (Mat Ave_1_Mat = new Mat(Ave_mix))
            {
                int Ave = 0;
                int Sum = 0;
                //
                //      トリミング　関数を使うことで周りの余計な画像を消す
                //


                //
                //      Mat.atでやりたかったが値が14億??????→　unsighedchar　が　byte
                //
                


                int x, y;

                for (y = 0; y < Ave_1_Mat.Height ; y++)
                {
                    for (x = 0; x < Ave_1_Mat.Width ; x++)
                    {

                        byte tmppix_Max_1 = Ave_1_Mat.At<byte>(y, x);
                        int pix_Ave_1 = (int)tmppix_Max_1;
                        Sum += pix_Ave_1;
                    }

                }
                Ave = Sum / (Ave_1_Mat.Height * Ave_1_Mat.Width);

                Ave_text.Text = Ave.ToString();

                return Ave;
            }
        }
        int Ave_Pixel_Get_ind(IplImage Ave_ind)
        {
            using (Mat Ave_1_Mat = new Mat(trimming(Ave_ind,0,0,319,237)))
            {
                int Ave = 0;
                int Sum = 0;
                //
                //      トリミング　関数を使うことで周りの余計な画像を消す
                //


                //
                //      Mat.atでやりたかったが値が14億??????→　unsighedchar　が　byte
                //



                int x, y;

                for (y = 0; y < Ave_1_Mat.Height; y++)
                {
                    for (x = 0; x < Ave_1_Mat.Width; x++)
                    {

                        byte tmppix_Max_1 = Ave_1_Mat.At<byte>(y, x);
                        int pix_Ave_1 = (int)tmppix_Max_1;
                        Sum += pix_Ave_1;
                    }

                }
                Ave = Sum / (Ave_1_Mat.Height * Ave_1_Mat.Width);

                Ave_text.Text = Ave.ToString();

                return Ave;
            }
        }
        //
        //射影変換した直後の平均値を求める
        //
        int Ave_H_Pixel_Get(IplImage tmpAve_H)
        {
            tmpAve_H = trimming(tmpAve_H, 15, 15, Width_com - 15, Height_com - 15);
            
                using (Mat Ave_H = new Mat(tmpAve_H))
                {

                    int Ave_H_pixel = 0;
                    int Sum_H = 0;
                    int x, y;

                    for (y = 0; y < Ave_H.Height; y++)
                    {
                        for (x = 0; x < Ave_H.Width; x++)
                        {

                            byte tmppix_Max_H = Ave_H.At<byte>(y, x);
                            int pix_Max_H = (int)tmppix_Max_H;
                            Sum_H += pix_Max_H;
                        }

                    }
                    Ave_H_pixel = Sum_H / (Ave_H.Height * Ave_H.Width);

                    return Ave_H_pixel;
                }
            
        }
        //
        //三枚の共通部分のみを残す　OpenCVの関数に頼る
        //自作関数なのでガバガバです。
        //privateで宣言した関数に処理結果画像を入れるのが良いのか、返し値として画像を返すのが良いのか　放置
        IplImage Mix_add(IplImage add_1, IplImage add_2, IplImage add_3)
        {
            using (IplImage add_out1 = Cv.CreateImage(new CvSize(add_1.Width, add_1.Height), BitDepth.U8, 1))
            {
                IplImage add_out2 = Cv.CreateImage(new CvSize(add_1.Width, add_1.Height), BitDepth.U8, 1);
                double alpha_1 = 0.5;
                double beta_1 = 0.5;
                double alpha_2 = 0.66;
                double beta_2 = 0.33;
                Cv.AddWeighted(add_1, alpha_1, add_2, beta_1, 0, add_out1);
                Cv.AddWeighted(add_out1, alpha_2, add_3, beta_2, 0, add_out2);
                return add_out2;
            }
        }

        private void HPixel_text_TextChanged(object sender, EventArgs e)
        {
            if (Hottext.Text == null)
            {
                HS_Result_text.Text = "error";
            }
            else {
                int HP = int.Parse(Hottext.Text);
                int HP_s = int.Parse(HPixel_text.Text);
                if (HP <= HP_s)
                {
                    HS_Result_text.Text = "HS無し";
                }
                else if (HP > HP_s)
                {
                    HS_Result_text.Text = "HS有り";

                }
            }
        }

        private void Max_text_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                org_com_1 = Cv.LoadImage(openFileDialog1.FileName);
                H_org_1_com = Cv.CreateImage(new CvSize(org_com_1.Width, org_com_1.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_1, H_org_1_com, ColorConversion.BgrToGray);
                resized_src1.ImageIpl = resize(H_org_1_com);
                L_Ave.Text = Ave_H_Pixel_Get(H_org_1_com).ToString();
                action.Text = "";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "左方向  元画像";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
            }
        
    }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                org_com_2 = Cv.LoadImage(openFileDialog2.FileName);
                H_org_2_com = Cv.CreateImage(new CvSize(org_com_2.Width, org_com_2.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_2, H_org_2_com, ColorConversion.BgrToGray);
                resized_src2.ImageIpl = resize(H_org_2_com);
                C_Ave.Text = Ave_H_Pixel_Get(H_org_2_com).ToString();
                action.Text = "";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
                action3.Text = "正面　入力画像 左上下右下上-四隅指定";
                action.Text = "左　入力画像 左上下右下上-四隅指定";
                centersrc_name.Text = "正面方向  元画像";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                org_com_3 = Cv.LoadImage(openFileDialog3.FileName);
                H_org_3_com = Cv.CreateImage(new CvSize(org_com_3.Width, org_com_3.Height), BitDepth.U8, 1);
                Cv.CvtColor(org_com_3, H_org_3_com, ColorConversion.BgrToGray);
                resized_src3.ImageIpl = resize(H_org_3_com);
                R_Ave.Text = Ave_H_Pixel_Get(H_org_3_com).ToString();
                action.Text = "";
                action3.Text = "";
                action2.Text = "";
                leftsrc_name.Text = "";
                centersrc_name.Text = "";
                rightsrc_name.Text = "";
                lefthomo_name.Text = "";
                centerhomo_name.Text = "";
                righthomo_name.Text = "";
                leftbin_name.Text = "";
                centerbin_name.Text = "";
                rightbin_name.Text = "";
                action3.Text = "正面　入力画像 左上下右下上-四隅指定";
                action.Text = "左　入力画像 左上下右下上-四隅指定";
                action2.Text = "右　入力画像 左上下右下上-四隅指定";
                rightsrc_name.Text = "右方向  元画像";
            }
        }

        private void L_K_KeyDown(object sender, KeyEventArgs e)
        {
            if (L_K.Text != "" || L_K.Text != "0.")
            {
                try
                {
                    error_text.Text = "Left二値化処理";
                    double kl = double.Parse(L_K.Text);
                    int Rl = int.Parse(L_R.Text);
                    int blockSize = int.Parse(L_B.Text);
                    dst_dst = Cv.CreateImage(new CvSize(H_org_1_com.Width, H_org_1_com.Height), BitDepth.U8, 1);
                    OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_1_com, dst_dst, blockSize, kl, Rl);
                }
                catch (FormatException)
                {
                    error_text.Text = "エラーが発生しました。";
                }
                dst1pic.ImageIpl = dst_dst;
                bin1_resized.ImageIpl = resize(dst_dst);
            }
        }


        private void L_K_TextChanged(object sender, EventArgs e)
        {
            error_text.Text = "Left_Kの値が変化";
        }

        private void C_K_KeyDown(object sender, KeyEventArgs e)
        {
            if (C_K.Text != "" || C_K.Text != "0.")
            {
                try
                {
                    error_text.Text = "Left二値化処理";
                    double kc = double.Parse(C_K.Text);
                    int Rc = int.Parse(C_R.Text);
                    int blockSizec = int.Parse(C_B.Text);
                    dst_dst2 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
                    OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_2_com, dst_dst2, blockSizec, kc, Rc);
                }
                catch (FormatException)
                {
                    error_text.Text = "エラーが発生しました。";
                }
                HotSpotpic.ImageIpl = dst_dst2;
                bin2_resized.ImageIpl = resize(dst_dst2);
            }
        }

        private void R_K_KeyDown(object sender, KeyEventArgs e)
        {
            if (R_K.Text != "" || R_K.Text != "0.")
            {
                try
                {
                    error_text.Text = "Left二値化処理";
                    double kr = double.Parse(R_K.Text);
                    int Rr = int.Parse(R_R.Text);
                    int blockSizer = int.Parse(R_B.Text);
                    dst_dst3 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
                    OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_3_com, dst_dst3, blockSizer, kr, Rr);
                }
                catch (FormatException)
                {
                    error_text.Text = "エラーが発生しました。";
                }
                dst3pic.ImageIpl = dst_dst3;
                bin3_resized.ImageIpl = resize(dst_dst3);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            KeyEventArgs a = null;
            L_K_KeyDown(sender,a);
            C_K_KeyDown(sender, a);
            R_K_KeyDown(sender, a);
            //ave = -315.2k + 148.66     k = -1*((ave - 148.66)/315.2)


        }
        //Excelの近似線機能
        private void button7_Click(object sender, EventArgs e)
        {
            double ave_all = (double.Parse(L_Ave.Text) + double.Parse(C_Ave.Text) + double.Parse(R_Ave.Text))/3;
            double k_all = -1 * ((ave_all - 148.66) / 315.2);
            dst_dst = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            dst_dst2 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            dst_dst3 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_1_com, dst_dst, blockSize_com, k_all, R_com);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_2_com, dst_dst2, blockSize_com, k_all, R_com);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_3_com, dst_dst3, blockSize_com, k_all, R_com);
            dst1pic.ImageIpl = dst_dst;
            HotSpotpic.ImageIpl = dst_dst2;
            dst3pic.ImageIpl = dst_dst3;
            bin1_resized.ImageIpl = resize(dst_dst);
            bin2_resized.ImageIpl = resize(dst_dst2);
            bin3_resized.ImageIpl = resize(dst_dst3);

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //個別に設定するための式はまた別にExcelで計算して出さないといけないと思う
        private void Bin_ind_Click(object sender, EventArgs e)
        {
            // k = -0.0042*double.Parse(L_Ave.Text)  + 0.6089
            double k_L = ((((-1) * (double.Parse(L_Ave.Text) * 4.2)) + 608.9) / 1000);
            double k_C = ((((-1) * (double.Parse(C_Ave.Text) * 4.2)) + 608.9) / 1000);
            double k_R =  ((    (   (-1) * (double.Parse(R_Ave.Text) * 4.2  ) )+608.9) / 1000);
            Auto_L.Text = k_L.ToString();
            Auto_C.Text = k_C.ToString();
            Auto_R.Text = k_R.ToString();
            dst_dst = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            dst_dst2 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            dst_dst3 = Cv.CreateImage(new CvSize(H_org_2_com.Width, H_org_2_com.Height), BitDepth.U8, 1);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_1_com, dst_dst, blockSize_com, k_L, R_com);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_2_com, dst_dst2, blockSize_com, k_C, R_com);
            OpenCvSharp.Extensions.Binarizer.SauvolaFast(H_org_3_com, dst_dst3, blockSize_com, k_R, R_com);
            dst1pic.ImageIpl = dst_dst;
            HotSpotpic.ImageIpl = dst_dst2;
            dst3pic.ImageIpl = dst_dst3;
            bin1_resized.ImageIpl = resize(dst_dst);
            bin2_resized.ImageIpl = resize(dst_dst2);
            bin3_resized.ImageIpl = resize(dst_dst3);
        }

        private void L_Ave_TextChanged(object sender, EventArgs e)
        {

        }
        //private int Height_com = 237;//47.4 *5
        //private int Width_com = 319;//63.8*5
        private void button8_Click(object sender, EventArgs e)
        {
            error_text.Text = "Noiseボタン";
            using (IplImage test = HotSpotpic.ImageIpl)
            {
                //test.SaveImage("1butten");
                //Cv.ShowImage("1butten", test);
                HotSpotpic.ImageIpl = Noise_div(test);

            }


        }
        //窓処理でノイズを削除したかった
        //作成途中　なぜか落ちる　PCの性能不足かプログラムが悪いのかたぶんプログラムが悪い
        //
        IplImage Noise_div(IplImage tmpNoise_img)
        {
            
            using (Mat Noise_img = new Mat(tmpNoise_img))
            using (Mat Noise_out = new Mat(Noise_img.Height, Noise_img.Width, MatType.CV_8UC1))
            {
                
                int Height_div = Height_com / 5;
                int Width_div = Width_com / 5;
                int x_div, y_div, x_pro, y_pro;
                //5分割した窓
                for (y_div = 0; y_div < Height_com; y_div += Height_div)
                {
                    for (x_div = 0; x_div < Width_com; x_div += Width_div)
                    {
                        
                        //窓ごとの処理
                        for (y_pro = y_div; y_pro < (y_div += Height_div); y_pro++)
                        {
                            for (x_pro = x_div; x_pro < (x_div += Width_div); x_pro++)
                            {
                                if ((y_div <= 0 || y_div >= 47) && (x_div <= 0 || x_div >= 63))
                                {
                                    Noise_out.Set<int>(y_pro, x_pro);

                                }
                                else
                                {
                                    Noise_out.Set<int>(y_pro, x_pro, 255);
                                    // tmpNoise_img.SaveImage("5処理");
                                    //Noise_out.Set<int>(y_pro, x_pro, 255);
                                }

                            }

                        }

                    }
                }
                Cv.ShowImage("5pro", tmpNoise_img);
                tmpNoise_img = MatToIplImage(Noise_out);

            }
           // tmpNoise_img.SaveImage("8終わり");
            error_text.Text = "Noise関数落ちませんでした";
                return tmpNoise_img;
            

            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Ave_com_text_KeyDown(object sender, KeyEventArgs e)
        {
            k_get_Click(sender, e);


        }

        private void k_get_Click(object sender, EventArgs e)
        {
           if (Ave_com_text.Text != "")
            {
                double k_com = ((((-1) * (double.Parse(Ave_com_text.Text) * 4.2)) + 608.9) / 1000);
            
                Ksikii.Text = k_com.ToString();
                L_K.Text = k_com.ToString();
                C_K.Text = k_com.ToString();
                R_K.Text = k_com.ToString();
            }
        }
    }
}
                                    