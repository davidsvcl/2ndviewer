using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using OpenMetaverse;

namespace _2ndviewer
{
    /// <summary>
    /// ミニマップウィンドウクラス
    /// ミニマップ画面表示を行います。
    /// </summary>
    public partial class MinimapForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>前にいたシム名</summary>
        private string oldSim_;
        /// <summary>イメージ</summary>
        private System.Drawing.Image mMapImage;
        /// <summary>フィルタ</summary>
        public int filter_selected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MinimapForm()
        {
            InitializeComponent();
            this.filter_comboBox.SelectedIndex = 0;
            filter_selected = 0;
        }

        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
        }

        /// <summary>更新ボタン</summary>
        private void refresh_button_Click(object sender, EventArgs e)
        {
            printMap(true);
        }

        /// <summary>ミニマップ描画</summary>
        public void printMap(bool reqweb)
        {
            Bitmap map = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Font font = new Font("Tahoma", 8, System.Drawing.FontStyle.Bold);
            Pen pen = new Pen(Brushes.GreenYellow,1);
            Brush brush = new SolidBrush(Color.Green);
            string strInfo = string.Empty;

            // 必要なときだけ背景マップ取得
            if (mMapImage == null || oldSim_ != client_.Network.CurrentSim.Name)
            {
                mMapImage = DownloadWebMapImage();
                oldSim_ = client_.Network.CurrentSim.Name;
            }
            // リフレッシュボタンを押したときは強制的に取得
            if (reqweb == true)
            {
                mMapImage = DownloadWebMapImage();
                oldSim_ = client_.Network.CurrentSim.Name;
            }

            if (mMapImage == null) {
                System.Diagnostics.Trace.WriteLine("mMapImage is NULL");
                return;
            }
            Graphics g = Graphics.FromImage(map);
            try
            {
                g.DrawImage(mMapImage, new Rectangle(0, 0, 256, 256), 0, 0, 256, 256, GraphicsUnit.Pixel);
            }
            catch {
            }
            // avatars
            System.Collections.Generic.List<Vector3> avatars = client_.Network.CurrentSim.AvatarPositions;
            for (int i = 0; i < avatars.Count; i++)
            {
                Rectangle rect = new Rectangle((int)Math.Round(avatars[i].X,0) -2, 255 -((int)Math.Round(avatars[i].Y,0) -2), 4, 4);
                g.FillEllipse(brush, rect);
                g.DrawEllipse(pen, rect);
            }

            // self
            Rectangle myrect = new Rectangle((int)Math.Round(client_.Self.SimPosition.X, 0) - 3, 255 - ((int)Math.Round(client_.Self.SimPosition.Y, 0) - 3), 6, 6);
            g.FillEllipse(new SolidBrush(Color.Yellow), myrect);
            g.DrawEllipse(new Pen(Brushes.Goldenrod ,1), myrect);

            strInfo = client_.Network.CurrentSim.Name + "/" +
                Math.Round(client_.Self.SimPosition.X, 0) + "," +
                Math.Round(client_.Self.SimPosition.Y, 0) + "," +
                Math.Round(client_.Self.SimPosition.Z, 0) + ",av:" +
                avatars.Count;
            g.DrawString(strInfo, font, Brushes.DarkOrange, 4, 4);

            world.BackgroundImage = map;
        }

        /// <summary>ミニマップをダウンロードする</summary>
        public System.Drawing.Image DownloadWebMapImage()
        {
            System.Net.HttpWebRequest request = null;
            System.Net.HttpWebResponse response = null;
            string imageURL = "";
            GridRegion currRegion;

            client_.Grid.GetGridRegion(client_.Network.CurrentSim.Name, GridLayerType.Terrain, out currRegion);
            try
            {
                imageURL = "http://secondlife.com/apps/mapapi/grid/map_image/" +
                    currRegion.X.ToString() + "-" +
                    (1279 - currRegion.Y).ToString() + "-1-0";
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageURL);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 20000;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                return System.Drawing.Image.FromStream(response.GetResponseStream());
            } catch(Exception e) {
                System.Diagnostics.Trace.WriteLine("Error Downloading Web Map Image"+e.ToString());
                return null;
            }
        }

        /// <summary>フィルタの選択変更時に呼ばれるメソッド</summary>
        private void filter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter_selected = this.filter_comboBox.SelectedIndex;
        }

        /// <summary>未使用</summary>
        private void world_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("auto move:");
        }

        /// <summary>クリック時:auto move</summary>
        private void world_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("auto move:" + e.X + "," + e.Y);
            client_.Self.AutoPilotCancel();
            // axis-Y negative
            int z = (int)client_.Self.SimPosition.Z;
            client_.Self.AutoPilotLocal(e.X, 255-e.Y, z);
        }

        /// <summary>ダブルクリック時:teleport</summary>
        private void world_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("teleport:" + e.X + "," + e.Y);
            client_.Self.AutoPilotCancel();
            // axis-Y negative
            int z = (int)client_.Self.SimPosition.Z;
            System.Threading.Thread process = new System.Threading.Thread(
                delegate()
                {
                    try
                    {
                        client_.Self.Teleport(client_.Network.CurrentSim.Name, new Vector3(e.X, 255 - e.Y, z));
                    }
                    catch
                    {
                        MessageBox.Show(StringResource.failedTeleport, "Error");
                    }
                });
            process.Start();
        }


    }
}
