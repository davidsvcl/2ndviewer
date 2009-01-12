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
    /// オブジェクトウィンドウクラス
    /// オブジェクト画面表示を行います。
    /// </summary>
    public partial class ObjectForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        /// <summary>Second Lifeグリッド通信ライブラリ</summary>
        private GridClient client_;
        /// <summary>PrimsWaiting配列</summary>
        private Dictionary<UUID, Primitive> PrimsWaiting = new Dictionary<UUID, Primitive>();
        /// <summary></summary>
        System.Threading.AutoResetEvent AllPropertiesReceived = new System.Threading.AutoResetEvent(false);
        /// <summary>オブジェクト配列</summary>
        System.Collections.Generic.List<Primitive> object_array_ = new System.Collections.Generic.List<Primitive>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObjectForm()
        {
            InitializeComponent();
        }

        /// <summary>通信ライブラリをセットする</summary>
        public void SetClient(GridClient client)
        {
            client_ = client;
            client_.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
        }

        /// <summary>Objects_OnObjectProperties</summary>
        private void Objects_OnObjectProperties(Simulator simulator, Primitive.ObjectProperties properties)
        {
            lock (PrimsWaiting)
            {
                Primitive prim;
                if (PrimsWaiting.TryGetValue(properties.ObjectID, out prim))
                {
                    prim.Properties = properties;
                }
                PrimsWaiting.Remove(properties.ObjectID);

                if (PrimsWaiting.Count == 0)
                {
                    AllPropertiesReceived.Set();
                }
            }
        }

        /// <summary>RequestObjectProperties</summary>
        private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
        {
            uint[] localids = new uint[objects.Count];
            lock (PrimsWaiting)
            {
                PrimsWaiting.Clear();
                for (int i = 0; i < objects.Count; ++i)
                {
                    localids[i] = objects[i].LocalID;
                    PrimsWaiting.Add(objects[i].ID, objects[i]);
                }
            }
            client_.Objects.SelectObjects(client_.Network.CurrentSim, localids);
            return AllPropertiesReceived.WaitOne(2000 + msPerRequest * objects.Count, false);
        }

        /// <summary>検索</summary>
        private void searchObjects(string searchString)
        {
            float radius = float.Parse("20");
            object_array_.Clear();
            this.listBox1.Items.Clear();

            Vector3 location = client_.Self.SimPosition;
            List<Primitive> prims = client_.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    Vector3 pos = prim.Position;
                    //System.Diagnostics.Trace.WriteLine(prim.LocalID);
                    return ((prim.ParentID == 0) && (pos != Vector3.Zero) && (Vector3.Distance(pos, location) < radius));
                    //return (prim.ParentID == 0);
                }
            );

            bool complete = RequestObjectProperties(prims, 250);

            foreach (Primitive p in prims)
            {
                string name = p.Properties.Name;//色々
                if ((name != null) && (name.Contains(searchString)))
                {
                    //System.Diagnostics.Trace.WriteLine(name + "," + p.Position.X+","+p.Position.Y+ "," + p.ID);//どのIDだ
                    object_array_.Add(p);
                    this.listBox1.Items.Add(name);
                }
            }
            if (!complete)
            {
                foreach (UUID uuid in PrimsWaiting.Keys)
                {
                    System.Diagnostics.Trace.WriteLine(uuid);
                }
            }
            System.Diagnostics.Trace.WriteLine("Done");
        }

        /// <summary>検索</summary>
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                searchObjects(textBox1.Text);
            }
        }

        /// <summary>リフレッシュ</summary>
        private void refresh_button_Click(object sender, EventArgs e)
        {
            searchObjects("");

//            client_.Network.CurrentSim.ObjectsPrimitives.ForEach(new Action<Primitive>(delegate(Primitive prim)
//            {
//                if (prim.ParentID == 0) //root prims only
//                {
//                    System.Diagnostics.Trace.WriteLine(prim.Text + "," + prim.PropertiesFamily.Name + "," + prim.PropertiesFamily.Description);
//                    //client_.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
//                }
//            }
//            ));
        }

        /// <summary>座るボタン</summary>
        private void sit_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.RequestSit(object_array_[index].ID, Vector3.Zero);
            client_.Self.Sit();
        }

        /// <summary>
        /// 触るボタン
        /// 実際に触れてない？
        /// </summary>
        private void touch_button_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index <= -1) return;
            client_.Self.Touch(object_array_[index].LocalID);
        }

//        private void Objects_OnObjectPropertiesFamily(Simulator sim,LLObject.ObjectPropertiesFamily family)
//        {
//            //System.Diagnostics.Trace.WriteLine(family.Name + "," + family.Description);
//        }
    }
}
