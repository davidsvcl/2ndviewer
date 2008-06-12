using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using libsecondlife;

namespace _2ndviewer
{
    public partial class ObjectForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private SecondLife client_;

        public ObjectForm()
        {
            InitializeComponent();
        }

        public void SetClient(SecondLife client)
        {
            client_ = client;
        }

        private void refresh_button_Click(object sender, EventArgs e)
        {
            client_.Network.CurrentSim.ObjectsPrimitives.ForEach(new Action<Primitive>(delegate(Primitive prim)
            {
                if (prim.ParentID == 0) //root prims only
                {
                    System.Diagnostics.Trace.WriteLine(prim.Text + "," + prim.PropertiesFamily.Name + "," + prim.PropertiesFamily.Description);
                    //client_.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
                }
            }
            ));
        }

        private void Objects_OnObjectPropertiesFamily(Simulator sim,LLObject.ObjectPropertiesFamily family)
        {
            //System.Diagnostics.Trace.WriteLine(family.Name + "," + family.Description);
        }
    }
}
