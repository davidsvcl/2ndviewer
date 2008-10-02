using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

using OpenMetaverse;
using OpenMetaverse.Rendering;
using OpenMetaverse.StructuredData;
using OpenMetaverse.Imaging;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace _2ndviewer
{
    public partial class RenderForm : Form
    {
        private GridClient client_;
        private MainForm mainForm_;
        private bool textureDownload_;

        const float DEG_TO_RAD = 0.0174532925f;
        const uint TERRAIN_START = (uint)Int32.MaxValue + 1;
        Dictionary<uint, Primitive> RenderFoliageList = new Dictionary<uint, Primitive>();
        Dictionary<uint, RenderablePrim> RenderPrimList = new Dictionary<uint, RenderablePrim>();

        Camera Camera;
        EventHandler IdleEvent;

        int TotalPrims;

        // Textures
        TexturePipeline TextureDownloader;
        Dictionary<UUID, TextureInfo> Textures = new Dictionary<UUID, TextureInfo>();

        // Terrain
        float MaxHeight = 0.1f;
        TerrainPatch[,] Heightmap;
        HeightmapLookupValue[] LookupHeightTable;

        // Picking globals
        bool Clicked = false;
        int ClickX = 0;
        int ClickY = 0;
        uint LastHit = 0;

        Vector3 PivotPosition = Vector3.Zero;
        bool Pivoting = false;
        Point LastPivot;
        //
        const int SELECT_BUFSIZE = 512;
        uint[] SelectBuffer = new uint[SELECT_BUFSIZE];

        //
        NativeMethods.Message msg;
        private bool AppStillIdle
        {
            get { return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0); }
        }

        public RenderForm()
        {
            InitializeComponent();

            // Get a list of rendering plugins
            List<string> renderers = RenderingLoader.ListRenderers(".");

            foreach (string r in renderers)
            {
                DialogResult result = MessageBox.Show(
                    String.Format("Use renderer {0}?", r), "Select Rendering Plugin", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Render.Plugin = RenderingLoader.LoadRenderer(r);
                    break;
                }
            }

            if (Render.Plugin == null)
            {
                MessageBox.Show("No valid rendering plugin loaded, exiting...");
                Application.Exit();
            }

            textureDownload_ = false;
            if (DialogResult.Yes == MessageBox.Show("Do you want to download textures", "Question", MessageBoxButtons.YesNo))
            {
                textureDownload_ = true;
            }

            // Setup OpenGL
            glControl.InitializeContexts();
            glControl.SwapBuffers();
            glControl.MouseWheel += new MouseEventHandler(glControl_MouseWheel);


            IdleEvent = new EventHandler(Application_Idle);
            Application.Idle += IdleEvent;

            // Show a flat sim before login so the screen isn't so boring
            InitHeightmap();
            InitOpenGL();
            InitCamera();

            glControl_Resize(null, null);

            // ほんとはログイン直前に
            // Re-initialize everything
            InitializeObjects();
        }

        public void SetClient(GridClient client)
        {
            client_ = client;
            client_.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client_.Terrain.OnLandPatch += new TerrainManager.LandPatchCallback(Terrain_OnLandPatch);
            client_.Objects.OnNewFoliage += new ObjectManager.NewFoliageCallback(Objects_OnNewFoliage);
            //
            // Initialize the texture download pipeline
            if (TextureDownloader != null)
                TextureDownloader.Shutdown();
            TextureDownloader = new TexturePipeline(client_);
            TextureDownloader.OnDownloadFinished += new TexturePipeline.DownloadFinishedCallback(TextureDownloader_OnDownloadFinished);
            //TextureDownloader.OnDownloadProgress += new TexturePipeline.DownloadProgressCallback(TextureDownloader_OnDownloadProgress);
        }

        public void SetMainForm(MainForm mainForm)
        {
            mainForm_ = mainForm;
        }

        public void TextureDownloaderReset()
        {
            TextureDownloader.OnDownloadFinished -= TextureDownloader_OnDownloadFinished;
            //
            // Initialize the texture download pipeline
            if (TextureDownloader != null)
                TextureDownloader.Shutdown();

            TextureDownloader = new TexturePipeline(client_);
            TextureDownloader.OnDownloadFinished += new TexturePipeline.DownloadFinishedCallback(TextureDownloader_OnDownloadFinished);
            //TextureDownloader.OnDownloadProgress += new TexturePipeline.DownloadProgressCallback(TextureDownloader_OnDownloadProgress);
        }

        public void InitLists()
        {
            TotalPrims = 0;

            lock (Textures)
            {
                foreach (TextureInfo tex in Textures.Values)
                {
                    int id = tex.ID;
                    Gl.glDeleteTextures(1, ref id);
                }

                Textures.Clear();
            }

            lock (RenderPrimList) RenderPrimList.Clear();
            lock (RenderFoliageList) RenderFoliageList.Clear();
        }

        private void InitializeObjects()
        {
            InitLists();


            // Initialize the camera object
            InitCamera();

            // Setup the libsl camera to match our Camera struct
            UpdateCamera();
            glControl_Resize(null, null);

            /*
            // Enable lighting
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            float[] lightPosition = { 128.0f, 64.0f, 96.0f, 0.0f };
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPosition);

            // Setup ambient property
            float[] ambientLight = { 0.2f, 0.2f, 0.2f, 0.0f };
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambientLight);

            // Setup specular property
            float[] specularLight = { 0.5f, 0.5f, 0.5f, 0.0f };
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, specularLight);
            */
        }

        private void InitOpenGL()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);

            Gl.glClearDepth(1.0f);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
        }

        private void InitCamera()
        {
            Camera = new Camera();
            Camera.Position = new Vector3(128f, -192f, 90f);
            Camera.FocalPoint = new Vector3(128f, 128f, 0f);
            Camera.Zoom = 1.0d;
            Camera.Far = 512.0d;
        }

        private void UpdateCamera()
        {
            if (client_ != null)
            {
                client_.Self.Movement.Camera.LookAt(Camera.Position, Camera.FocalPoint);
                client_.Self.Movement.Camera.Far = (float)Camera.Far;
            }

            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            SetPerspective();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
        }

        private void InitHeightmap()
        {
            // Initialize the heightmap
            Heightmap = new TerrainPatch[16, 16];
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Heightmap[y, x] = new TerrainPatch();
                    Heightmap[y, x].Heightmap = new float[16 * 16];
                }
            }

            // Speed up terrain exports with a lookup table
            LookupHeightTable = new HeightmapLookupValue[256 * 256];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    LookupHeightTable[i + (j * 256)] = new HeightmapLookupValue(i + (j * 256), ((float)i * ((float)j / 127.0f)));
                }
            }
            Array.Sort<HeightmapLookupValue>(LookupHeightTable);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            Gl.glClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            Gl.glViewport(0, 0, glControl.Width, glControl.Height);

            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            SetPerspective();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();

            // Set the center of the glControl as the default pivot point
            LastPivot = glControl.PointToScreen(new Point(glControl.Width / 2, glControl.Height / 2));
        }
        private void SetPerspective()
        {
            Glu.gluPerspective(50.0d * Camera.Zoom, 1.0d, 0.1d, Camera.Far);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                RenderScene();
            }
        }

        private void RenderScene()
        {
            Camera.FocalPoint = client_.Self.SimPosition;
            UpdateCamera();

            try
            {
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                Gl.glLoadIdentity();
                Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
                Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

                if (Clicked)
                    StartPicking(ClickX, ClickY);

                // Setup wireframe or solid fill drawing mode
                Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);

                // Position the camera
                Glu.gluLookAt(
                    Camera.Position.X, Camera.Position.Y, Camera.Position.Z,
                    Camera.FocalPoint.X, Camera.FocalPoint.Y, Camera.FocalPoint.Z,
                    0f, 0f, 1f);

                RenderSkybox();

                // Push the world matrix
                Gl.glPushMatrix();

                RenderTerrain();
                RenderPrims();
                RenderAvatars();

                Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
                Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);

                if (Clicked)
                {
                    Clicked = false;
                    StopPicking();
                }

                // Pop the world matrix
                Gl.glPopMatrix();
                Gl.glFlush();

                glControl.Invalidate();
            }
            catch (Exception)
            {
            }
        }
        private void RenderSkybox()
        {
            //Gl.glTranslatef(0f, 0f, 0f);
        }

        private void RenderTerrain()
        {
            if (Heightmap != null)
            {
                int i = 0;

                // No texture
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

                for (int hy = 0; hy < 16; hy++)
                {
                    for (int hx = 0; hx < 16; hx++)
                    {
                        uint patchName = (uint)(TERRAIN_START + i);
                        Gl.glPushName(patchName);
                        ++i;

                        // Check if this patch is currently selected
                        bool selected = (LastHit == patchName);

                        for (int y = 0; y < 15; y++)
                        {
                            Gl.glBegin(Gl.GL_TRIANGLE_STRIP);

                            for (int x = 0; x < 15; x++)
                            {
                                // Vertex 0
                                float height = Heightmap[hy, hx].Heightmap[y * 16 + x];
                                float color = height / MaxHeight;
                                float red = (selected) ? 1f : color;

                                Gl.glColor3f(red, color, color);
                                Gl.glTexCoord2f(0f, 0f);
                                Gl.glVertex3f(hx * 16 + x, hy * 16 + y, height);

                                // Vertex 1
                                height = Heightmap[hy, hx].Heightmap[y * 16 + (x + 1)];
                                color = height / MaxHeight;
                                red = (selected) ? 1f : color;

                                Gl.glColor3f(red, color, color);
                                Gl.glTexCoord2f(1f, 0f);
                                Gl.glVertex3f(hx * 16 + x + 1, hy * 16 + y, height);

                                // Vertex 2
                                height = Heightmap[hy, hx].Heightmap[(y + 1) * 16 + x];
                                color = height / MaxHeight;
                                red = (selected) ? 1f : color;

                                Gl.glColor3f(red, color, color);
                                Gl.glTexCoord2f(0f, 1f);
                                Gl.glVertex3f(hx * 16 + x, hy * 16 + y + 1, height);

                                // Vertex 3
                                height = Heightmap[hy, hx].Heightmap[(y + 1) * 16 + (x + 1)];
                                color = height / MaxHeight;
                                red = (selected) ? 1f : color;

                                Gl.glColor3f(red, color, color);
                                Gl.glTexCoord2f(1f, 1f);
                                Gl.glVertex3f(hx * 16 + x + 1, hy * 16 + y + 1, height);
                            }

                            Gl.glEnd();
                        }

                        Gl.glPopName();
                    }
                }
            }
        }

        int[] CubeMapDefines = new int[]
        {
            Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X_ARB,
            Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_X_ARB,
            Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_Y_ARB,
            Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_Y_ARB,
            Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_Z_ARB,
            Gl.GL_TEXTURE_CUBE_MAP_NEGATIVE_Z_ARB
        };

        private void RenderPrims()
        {
            if (RenderPrimList != null && RenderPrimList.Count > 0)
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);

                lock (RenderPrimList)
                {
                    bool firstPass = true;
                    Gl.glDisable(Gl.GL_BLEND);
                    Gl.glEnable(Gl.GL_DEPTH_TEST);

                StartRender:

                    foreach (RenderablePrim render in RenderPrimList.Values)
                    {
                        RenderablePrim parentRender = RenderablePrim.Empty;
                        Primitive prim = render.Prim;

                        if (prim.ParentID != 0)
                        {
                            // Get the parent reference
                            if (!RenderPrimList.TryGetValue(prim.ParentID, out parentRender))
                            {
                                // Can't render a child with no parent prim, skip it
                                continue;
                            }
                        }

                        Gl.glPushName(prim.LocalID);
                        Gl.glPushMatrix();

                        if (prim.ParentID != 0)
                        {
                            // Child prim
                            Primitive parent = parentRender.Prim;

                            // Apply parent translation and rotation
                            Gl.glMultMatrixf(Math3D.CreateTranslationMatrix(parent.Position));
                            Gl.glMultMatrixf(Math3D.CreateRotationMatrix(parent.Rotation));
                        }

                        // Apply prim translation and rotation
                        Gl.glMultMatrixf(Math3D.CreateTranslationMatrix(prim.Position));
                        Gl.glMultMatrixf(Math3D.CreateRotationMatrix(prim.Rotation));

                        // Scale the prim
                        Gl.glScalef(prim.Scale.X, prim.Scale.Y, prim.Scale.Z);

                        // Draw the prim faces
                        for (int j = 0; j < render.Mesh.Faces.Count; j++)
                        {
                            Face face = render.Mesh.Faces[j];
                            FaceData data = (FaceData)face.UserData;
                            Color4 color = face.TextureFace.RGBA;
                            bool alpha = false;
                            int textureID = 0;

                            if (color.A < 1.0f)
                                alpha = true;

                            #region Texturing
                            // 2008/09/20 tuna テクスチャはキャッシュシステムを構築するまでおあずけ
                            TextureInfo info;
                            if (Textures.TryGetValue(face.TextureFace.TextureID, out info))
                            {
                                if (info.Alpha)
                                    alpha = true;

                                textureID = info.ID;

                                // Enable texturing for this face
                                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                            }
                            else
                            {
                                if (face.TextureFace.TextureID == Primitive.TextureEntry.WHITE_TEXTURE ||
                                    face.TextureFace.TextureID == UUID.Zero)
                                {
                                  Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);
                                }
                                else
                                {
                                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);
                                }
                            }

                            if (firstPass && !alpha || !firstPass && alpha)
                            {
                                // Color this prim differently based on whether it is selected or not
                                if (LastHit == prim.LocalID || (LastHit != 0 && LastHit == prim.ParentID))
                                {
                                    Gl.glColor4f(1f, color.G * 0.3f, color.B * 0.3f, color.A);
                                }
                                else
                                {
                                  Gl.glColor4f(color.R, color.G, color.B, color.A);
                                }

                                // Bind the texture
                                Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID);

                                Gl.glTexCoordPointer(2, Gl.GL_FLOAT, 0, data.TexCoords);
                                Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, data.Vertices);
                                Gl.glDrawElements(Gl.GL_TRIANGLES, data.Indices.Length, Gl.GL_UNSIGNED_SHORT, data.Indices);
                            }

                            #endregion Texturing
                        }

                        Gl.glPopMatrix();
                        Gl.glPopName();
                    }

                    if (firstPass)
                    {
                        firstPass = false;
                        Gl.glEnable(Gl.GL_BLEND);
                        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                        //Gl.glDisable(Gl.GL_DEPTH_TEST);

                        goto StartRender;
                    }
                }

                Gl.glEnable(Gl.GL_DEPTH_TEST);
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
        }

        private void RenderAvatars()
        {
            if (client_ != null && client_.Network.CurrentSim != null)
            {
                Gl.glColor3f(0f, 1f, 0f);

                client_.Network.CurrentSim.ObjectsAvatars.ForEach(
                    delegate(Avatar avatar)
                    {
                        Gl.glPushMatrix();
                        Gl.glTranslatef(avatar.Position.X, avatar.Position.Y, avatar.Position.Z);

                        Glu.GLUquadric quad = Glu.gluNewQuadric();
                        Glu.gluSphere(quad, 1.0d, 10, 10);
                        Glu.gluDeleteQuadric(quad);

                        Gl.glPopMatrix();
                    }
                );

                Gl.glColor3f(1f, 1f, 1f);
            }
        }

        #region Texture Downloading

        private void TextureDownloader_OnDownloadFinished(UUID id, bool success)
        {
            bool alpha = false;
            ManagedImage imgData = null;
            byte[] raw = null;

            try
            {
                // Load the image off the disk
                if (success)
                {
                    ImageDownload download = TextureDownloader.GetTextureToRender(id);
                    if (OpenJPEG.DecodeToImage(download.AssetData, out imgData))
                    {
                        raw = imgData.ExportRaw();

                        if ((imgData.Channels & ManagedImage.ImageChannels.Alpha) != 0)
                            alpha = true;
                    }
                    else
                    {
                        success = false;
                        Console.WriteLine("Failed to decode texture");
                    }
                }

                // Make sure the OpenGL commands run on the main thread
                BeginInvoke(
                       (MethodInvoker)delegate()
                       {
                           if (success)
                           {
                               int textureID = 0;

                               try
                               {
                                   Gl.glGenTextures(1, out textureID);
                                   Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID);

                                   Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST); //Gl.GL_NEAREST);
                                   Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                                   Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
                                   Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
                                   Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE); //Gl.GL_FALSE);

                                   //Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, bitmap.Width, bitmap.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE,
                                   //    bitmapData.Scan0);
                                   //int error = Gl.glGetError();

                                   int error = Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA, imgData.Width, imgData.Height, Gl.GL_BGRA,
                                       Gl.GL_UNSIGNED_BYTE, raw);

                                   if (error == 0)
                                   {
                                       Textures[id] = new TextureInfo(textureID, alpha);
                                       Console.WriteLine("Created OpenGL texture for " + id.ToString());
                                   }
                                   else
                                   {
                                       Textures[id] = new TextureInfo(0, false);
                                       Console.WriteLine("Error creating OpenGL texture: " + error);
                                   }
                               }
                               catch (Exception ex)
                               {
                                   Console.WriteLine(ex);
                               }
                           }

                           // Remove this image from the download listbox
                           //lock (DownloadList)
                           //{
                           //    GlacialComponents.Controls.GLItem item;
                           //    if (DownloadList.TryGetValue(id, out item))
                           //    {
                           //        DownloadList.Remove(id);
                           //        try { lstDownloads.Items.Remove(item); }
                           //        catch (Exception) { }
                           //        lstDownloads.Invalidate();
                           //    }
                           //}
                       });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion Texture Downloading

        private void StartPicking(int cursorX, int cursorY)
        {
            int[] viewport = new int[4];

            Gl.glSelectBuffer(SELECT_BUFSIZE, SelectBuffer);
            Gl.glRenderMode(Gl.GL_SELECT);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
            Glu.gluPickMatrix(cursorX, viewport[3] - cursorY, 5, 5, viewport);

            SetPerspective();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Gl.glInitNames();
        }

        private void StopPicking()
        {
            int hits;

            // Resotre the original projection matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glFlush();

            // Return to normal rendering mode
            hits = Gl.glRenderMode(Gl.GL_RENDER);

            // If there are hits process them
            if (hits != 0)
            {
                ProcessHits(hits, SelectBuffer);
            }
            else
            {
                LastHit = 0;
                glControl.ContextMenu = null;
            }
        }

        private void ProcessHits(int hits, uint[] selectBuffer)
        {
            uint names = 0;
            uint numNames = 0;
            uint minZ = 0xffffffff;
            uint ptr = 0;
            uint ptrNames = 0;

            for (uint i = 0; i < hits; i++)
            {
                names = selectBuffer[ptr];
                ++ptr;
                if (selectBuffer[ptr] < minZ)
                {
                    numNames = names;
                    minZ = selectBuffer[ptr];
                    ptrNames = ptr + 2;
                }

                ptr += names + 2;
            }

            ptr = ptrNames;

            for (uint i = 0; i < numNames; i++, ptr++)
            {
                LastHit = selectBuffer[ptr];
            }

            if (LastHit >= TERRAIN_START)
            {
                // Terrain was clicked on, turn off the context menu
                // 2008/09/20 tuna for Tachikawa-kun comments out
                //glControl.ContextMenu = ExportTerrainMenu;
            }
            else
            {
                RenderablePrim render;
                if (RenderPrimList.TryGetValue(LastHit, out render))
                {
                    if (render.Prim.ParentID == 0)
                    {
                        //Camera.FocalPoint = render.Prim.Position;
                        //UpdateCamera();
                        //cient_.Self.AutoPilotすればいい？
                    }
                    else
                    {
                        // See if we have the parent
                        RenderablePrim renderParent;
                        if (RenderPrimList.TryGetValue(render.Prim.ParentID, out renderParent))
                        {
                            // Turn on the context menu
                            // 2008/09/20 tuna for Tachiakwa-kun comments out
                            //glControl.ContextMenu = ExportPrimMenu;

                            // Change the clicked on prim to the parent. Camera position stays on the
                            // clicked child but the highlighting is applied to all the children
                            LastHit = renderParent.Prim.LocalID;

                            //Camera.FocalPoint = renderParent.Prim.Position + render.Prim.Position;
                            //UpdateCamera();
                            //cient_.Self.AutoPilotすればいい？
                        }
                        else
                        {
                            Console.WriteLine("Clicked on a child prim with no parent!");
                            LastHit = 0;
                        }
                    }
                }
            }
        }

        void Terrain_OnLandPatch(Simulator simulator, int x, int y, int width, float[] data)
        {
            if (client_ != null && client_.Network.CurrentSim == simulator)
            {
                Heightmap[y, x].Heightmap = data;
            }

            // Find the new max height
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > MaxHeight)
                    MaxHeight = data[i];
            }
        }

        void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            try
            {
                RenderablePrim render = new RenderablePrim();
                render.Prim = prim;
                render.Mesh = Render.Plugin.GenerateFacetedMesh(prim, DetailLevel.High);

                // Create a FaceData struct for each face that stores the 3D data
                // in a Tao.OpenGL friendly format
                for (int j = 0; j < render.Mesh.Faces.Count; j++)
                {
                    Face face = render.Mesh.Faces[j];
                    FaceData data = new FaceData();

                    // Vertices for this face
                    data.Vertices = new float[face.Vertices.Count * 3];
                    for (int k = 0; k < face.Vertices.Count; k++)
                    {
                        data.Vertices[k * 3 + 0] = face.Vertices[k].Position.X;
                        data.Vertices[k * 3 + 1] = face.Vertices[k].Position.Y;
                        data.Vertices[k * 3 + 2] = face.Vertices[k].Position.Z;
                    }

                    // Indices for this face
                    data.Indices = face.Indices.ToArray();

                    // Texture transform for this face
                    Primitive.TextureEntryFace teFace = prim.Textures.GetFace((uint)j);
                    Render.Plugin.TransformTexCoords(face.Vertices, face.Center, teFace);

                    // Texcoords for this face
                    data.TexCoords = new float[face.Vertices.Count * 2];
                    for (int k = 0; k < face.Vertices.Count; k++)
                    {
                        data.TexCoords[k * 2 + 0] = face.Vertices[k].TexCoord.X;
                        data.TexCoords[k * 2 + 1] = face.Vertices[k].TexCoord.Y;
                    }

                    // Texture for this face
                    if (textureDownload_ == true &&
                        teFace.TextureID != UUID.Zero &&
                        teFace.TextureID != Primitive.TextureEntry.WHITE_TEXTURE)
                    { 
                        lock (Textures)
                        {
                            if (!Textures.ContainsKey(teFace.TextureID))
                            {
                                // We haven't constructed this image in OpenGL yet, get ahold of it
                                TextureDownloader.RequestTexture(teFace.TextureID);
                            }
                        }
                    }

                    // Set the UserData for this face to our FaceData struct
                    face.UserData = data;
                    render.Mesh.Faces[j] = face;
                }

                lock (RenderPrimList) RenderPrimList[prim.LocalID] = render;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("OnNewPrim:"+e);
            }
        }

        void Objects_OnNewFoliage(Simulator simulator, Primitive foliage, ulong regionHandle, ushort timeDilation)
        {
            lock (RenderFoliageList)
                RenderFoliageList[foliage.LocalID] = foliage;
        }

        void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                // Calculate the distance to move to/away
                float dist = (float)(e.Delta / 120) * 10.0f;

                if (Vector3.Distance(Camera.Position, Camera.FocalPoint) > dist)
                {
                    // Move closer or further away from the focal point
                    Vector3 toFocal = Camera.FocalPoint - Camera.Position;
                    toFocal.Normalize();

                    toFocal = toFocal * dist;

                    Camera.Position += toFocal;
                    UpdateCamera();
                }
            }
        }

        private void RenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client_.Objects.OnNewPrim -= Objects_OnNewPrim;
            client_.Terrain.OnLandPatch -= Terrain_OnLandPatch;
            client_.Objects.OnNewFoliage -= Objects_OnNewFoliage;
            TextureDownloader.OnDownloadFinished -= TextureDownloader_OnDownloadFinished;
            // Shutdown the texture downloader
            if (TextureDownloader != null)
            {
                TextureDownloader.Shutdown();
                TextureDownloader.Abort();
            }

            Application.Idle -= IdleEvent;
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Alt) != 0 && LastHit > 0)
            {
                // Alt is held down and we have a valid target
                Pivoting = true;
                PivotPosition = Camera.FocalPoint;

                Control control = (Control)sender;
                LastPivot = control.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop pivoting if we were previously
            Pivoting = false;
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Pivoting)
            {
                float a, x, y, z;

                Control control = (Control)sender;
                Point mouse = control.PointToScreen(new Point(e.X, e.Y));

                // Calculate the deltas from the center of the control to the current position
                int deltaX = (int)((mouse.X - LastPivot.X) * -0.5d);
                int deltaY = (int)((mouse.Y - LastPivot.Y) * -0.5d);

                // Translate so the focal point is the origin
                Vector3 altered = Camera.Position - Camera.FocalPoint;

                // Rotate the translated point by deltaX
                a = (float)deltaX * DEG_TO_RAD;
                x = (float)((altered.X * Math.Cos(a)) - (altered.Y * Math.Sin(a)));
                y = (float)((altered.X * Math.Sin(a)) + (altered.Y * Math.Cos(a)));

                altered.X = x;
                altered.Y = y;

                // Rotate the translated point by deltaY
                a = (float)deltaY * DEG_TO_RAD;
                y = (float)((altered.Y * Math.Cos(a)) - (altered.Z * Math.Sin(a)));
                z = (float)((altered.Y * Math.Sin(a)) + (altered.Z * Math.Cos(a)));

                altered.Y = y;
                altered.Z = z;

                // Translate back to world space
                altered += Camera.FocalPoint;

                // Update the camera
                Camera.Position = altered;
                UpdateCamera();

                // Update the pivot point
                LastPivot = mouse;
            }
        }

        private void glControl_MouseClick(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Alt) == 0 && e.Button == MouseButtons.Left)
            {
                // Only allow clicking if alt is not being held down
                ClickX = e.X;
                ClickY = e.Y;
                Clicked = true;
            }
        }

        private void glControl_Resize_1(object sender, EventArgs e)
        {
            Gl.glClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            Gl.glViewport(0, 0, glControl.Width, glControl.Height);

            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            SetPerspective();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();

            // Set the center of the glControl as the default pivot point
            LastPivot = glControl.PointToScreen(new Point(glControl.Width / 2, glControl.Height / 2));
        }
    }

    public struct RenderablePrim
    {
        public Primitive Prim;
        public FacetedMesh Mesh;

        public readonly static RenderablePrim Empty = new RenderablePrim();
    }

    public struct Camera
    {
        public Vector3 Position;
        public Vector3 FocalPoint;
        public double Zoom;
        public double Far;
    }
    public struct NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr HWnd;
            public uint Msg;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public System.Drawing.Point Point;
        }

        //[System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
    }

    public struct TextureInfo
    {
        /// <summary>OpenGL Texture ID</summary>
        public int ID;
        /// <summary>True if this texture has an alpha component</summary>
        public bool Alpha;

        public TextureInfo(int id, bool alpha)
        {
            ID = id;
            Alpha = alpha;
        }
    }
    public struct HeightmapLookupValue : IComparable<HeightmapLookupValue>
    {
        public int Index;
        public float Value;

        public HeightmapLookupValue(int index, float value)
        {
            Index = index;
            Value = value;
        }

        public int CompareTo(HeightmapLookupValue val)
        {
            return Value.CompareTo(val.Value);
        }
    }

    public static class Math3D
    {
        // Column-major:
        // |  0  4  8 12 |
        // |  1  5  9 13 |
        // |  2  6 10 14 |
        // |  3  7 11 15 |

        public static float[] CreateTranslationMatrix(Vector3 v)
        {
            float[] mat = new float[16];

            mat[12] = v.X;
            mat[13] = v.Y;
            mat[14] = v.Z;
            mat[0] = mat[5] = mat[10] = mat[15] = 1;

            return mat;
        }

        public static float[] CreateRotationMatrix(Quaternion q)
        {
            float[] mat = new float[16];

            // Transpose the quaternion (don't ask me why)
            q.X = q.X * -1f;
            q.Y = q.Y * -1f;
            q.Z = q.Z * -1f;

            float x2 = q.X + q.X;
            float y2 = q.Y + q.Y;
            float z2 = q.Z + q.Z;
            float xx = q.X * x2;
            float xy = q.X * y2;
            float xz = q.X * z2;
            float yy = q.Y * y2;
            float yz = q.Y * z2;
            float zz = q.Z * z2;
            float wx = q.W * x2;
            float wy = q.W * y2;
            float wz = q.W * z2;

            mat[0] = 1.0f - (yy + zz);
            mat[1] = xy - wz;
            mat[2] = xz + wy;
            mat[3] = 0.0f;

            mat[4] = xy + wz;
            mat[5] = 1.0f - (xx + zz);
            mat[6] = yz - wx;
            mat[7] = 0.0f;

            mat[8] = xz - wy;
            mat[9] = yz + wx;
            mat[10] = 1.0f - (xx + yy);
            mat[11] = 0.0f;

            mat[12] = 0.0f;
            mat[13] = 0.0f;
            mat[14] = 0.0f;
            mat[15] = 1.0f;

            return mat;
        }

        public static float[] CreateScaleMatrix(Vector3 v)
        {
            float[] mat = new float[16];

            mat[0] = v.X;
            mat[5] = v.Y;
            mat[10] = v.Z;
            mat[15] = 1;

            return mat;
        }
    }

    public struct FaceData
    {
        public float[] Vertices;
        public ushort[] Indices;
        public float[] TexCoords;
        public int TexturePointer;
        public System.Drawing.Image Texture;
        // TODO: Normals / binormals?
    }

    public static class Render
    {
        public static IRendering Plugin;
    }


    class TaskInfo
    {
        public UUID RequestID;
        public int RequestNbr;


        public TaskInfo(UUID reqID, int reqNbr)
        {
            RequestID = reqID;
            RequestNbr = reqNbr;
        }
    }

    /// <summary>
    /// Texture request download handler, allows a configurable number of download slots
    /// </summary>
    public class TexturePipeline
    {
        private static GridClient Client;

        // queue for requested images
        private Queue<UUID> RequestQueue;

        // list of current requests in process
        private Dictionary<UUID, int> CurrentRequests;

        private static AutoResetEvent[] resetEvents;

        private static int[] threadpoolSlots;

        /// <summary>
        /// For keeping track of active threads available/downloading textures
        /// </summary>
        public static int[] ThreadpoolSlots
        {
            get { lock (threadpoolSlots) { return threadpoolSlots; }}
            set { lock (threadpoolSlots) { threadpoolSlots = value; } }
        }

        // storage for images ready to render
        private Dictionary<UUID, ImageDownload> RenderReady;

        // maximum allowed concurrent requests at once
        const int MAX_TEXTURE_REQUESTS = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="success"></param>
        public delegate void DownloadFinishedCallback(UUID id, bool success);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="received"></param>
        /// <param name="total"></param>
        public delegate void DownloadProgressCallback(UUID image, int recieved, int total);

        /// <summary>Fired when a texture download completes</summary>
        public event DownloadFinishedCallback OnDownloadFinished;
        /// <summary></summary>
        public event DownloadProgressCallback OnDownloadProgress;

        private Thread downloadMaster;
        private bool Running;

        private AssetManager.ImageReceivedCallback DownloadCallback;
        private AssetManager.ImageReceiveProgressCallback DownloadProgCallback;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="client">Reference to <code>SecondLife</code> client</param>
        public TexturePipeline(GridClient client)
        {
            Running = true;

            RequestQueue = new Queue<UUID>();
            CurrentRequests = new Dictionary<UUID, int>(MAX_TEXTURE_REQUESTS);

            RenderReady = new Dictionary<UUID, ImageDownload>();

            resetEvents = new AutoResetEvent[MAX_TEXTURE_REQUESTS];
            threadpoolSlots = new int[MAX_TEXTURE_REQUESTS];

            // pre-configure autoreset events/download slots
            for (int i = 0; i < MAX_TEXTURE_REQUESTS; i++)
            {
                resetEvents[i] = new AutoResetEvent(false);
                threadpoolSlots[i] = -1;
            }

            Client = client;

            DownloadCallback = new AssetManager.ImageReceivedCallback(Assets_OnImageReceived);
            DownloadProgCallback = new AssetManager.ImageReceiveProgressCallback(Assets_OnImageReceiveProgress);
            Client.Assets.OnImageReceived += DownloadCallback;
            Client.Assets.OnImageReceiveProgress += DownloadProgCallback;

            // Fire up the texture download thread
            downloadMaster = new Thread(new ThreadStart(DownloadThread));
            downloadMaster.Start();
        }

        public void Shutdown()
        {
            Client.Assets.OnImageReceived -= DownloadCallback;
            Client.Assets.OnImageReceiveProgress -= DownloadProgCallback;

            RequestQueue.Clear();

            for (int i = 0; i < resetEvents.Length; i++)
                if (resetEvents[i] != null)
                    resetEvents[i].Set();

            Running = false;
        }

        public void Abort()
        {
            downloadMaster.Abort();
            // waiting a thread abort
            downloadMaster.Join();
        }

        /// <summary>
        /// Request a texture be downloaded, once downloaded OnImageRenderReady event will be fired
        /// containing texture key which can be used to retrieve texture with GetTextureToRender method
        /// </summary>
        /// <param name="textureID">id of Texture to request</param>
        public void RequestTexture(UUID textureID)
        {
            if (Client.Assets.Cache.HasImage(textureID))
            {
                // Add to rendering dictionary
                lock (RenderReady)
                {
                    if (!RenderReady.ContainsKey(textureID))
                    {
                        RenderReady.Add(textureID, Client.Assets.Cache.GetCachedImage(textureID));

                        // Let any subscribers know about it
                        if (OnDownloadFinished != null)
                        {
                            OnDownloadFinished(textureID, true);
                        }
                    }
                    else
                    {
                        // This image has already been served up, ignore this request
                    }
                }
            }
            else
            {
                lock (RequestQueue)
                {
                    // Make sure we aren't already downloading the texture
                    if (!RequestQueue.Contains(textureID) && !CurrentRequests.ContainsKey(textureID))
                    {
                        RequestQueue.Enqueue(textureID);
                    }
                }
            }
        }

        /// <summary>
        /// retrieve texture information from dictionary
        /// </summary>
        /// <param name="textureID">Texture ID</param>
        /// <returns>ImageDownload object</returns>
        public ImageDownload GetTextureToRender(UUID textureID)
        {
            ImageDownload renderable = new ImageDownload();
            lock (RenderReady)
            {
                if (RenderReady.ContainsKey(textureID))
                {
                    renderable = RenderReady[textureID];
                }
                else
                {
                    Logger.Log("Requested texture data for texture that does not exist in dictionary", Helpers.LogLevel.Warning);
                }
                return renderable;
            }
        }

        /// <summary>
        /// Remove no longer necessary texture from dictionary
        /// </summary>
        /// <param name="textureID"></param>
        public void RemoveFromPipeline(UUID textureID)
        {
            lock (RenderReady)
            {
                if (RenderReady.ContainsKey(textureID))
                    RenderReady.Remove(textureID);
            }
        }

        /// <summary>
        /// Master Download Thread, Queues up downloads in the threadpool
        /// </summary>
        private void DownloadThread()
        {
            int reqNbr;

            try
            {
                while (Running)
                {
                    if (RequestQueue.Count > 0)
                    {
                        reqNbr = -1;
                        // find available slot for reset event
                        for (int i = 0; i < threadpoolSlots.Length; i++)
                        {
                            if (threadpoolSlots[i] == -1)
                            {
                                threadpoolSlots[i] = 1;
                                reqNbr = i;
                                break;
                            }
                        }

                        if (reqNbr != -1)
                        {
                            UUID requestID;
                            lock (RequestQueue)
                                requestID = RequestQueue.Dequeue();

                            Logger.DebugLog(String.Format("Sending Worker thread new download request {0}", reqNbr));
                            ThreadPool.QueueUserWorkItem(new WaitCallback(textureRequestDoWork), new TaskInfo(requestID, reqNbr));

                            continue;
                        }
                    }

                    // Queue was empty, let's give up some CPU time
                    Thread.Sleep(500);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                System.Diagnostics.Trace.WriteLine("Abort");
            }
        }

        void textureRequestDoWork(Object threadContext)
        {
            TaskInfo ti = (TaskInfo)threadContext;

            lock (CurrentRequests)
            {
                if (CurrentRequests.ContainsKey(ti.RequestID))
                {
                    threadpoolSlots[ti.RequestNbr] = -1;
                    return;
                }
                else
                {
                    CurrentRequests.Add(ti.RequestID, ti.RequestNbr);
                }
            }

            Logger.DebugLog(String.Format("Worker {0} Requesting {1}", ti.RequestNbr, ti.RequestID));

            resetEvents[ti.RequestNbr].Reset();
            Client.Assets.RequestImage(ti.RequestID, ImageType.Normal);

            // don't release this worker slot until texture is downloaded or timeout occurs
            if (!resetEvents[ti.RequestNbr].WaitOne(30 * 1000, false))
            {
                // Timed out
                Logger.Log("Worker " + ti.RequestNbr + " Timeout waiting for Texture " + ti.RequestID + " to Download", Helpers.LogLevel.Warning);

                lock (CurrentRequests)
                    CurrentRequests.Remove(ti.RequestID);
            }

            // free up this download slot
            threadpoolSlots[ti.RequestNbr] = -1;
        }

        private void Assets_OnImageReceived(ImageDownload image, AssetTexture asset)
        {
            // Free up this slot in the ThreadPool
            lock (CurrentRequests)
            {
                int requestNbr;
                if (asset != null && CurrentRequests.TryGetValue(image.ID, out requestNbr))
                {
                    Logger.DebugLog(String.Format("Worker {0} Downloaded texture {1}", requestNbr, image.ID));
                    resetEvents[requestNbr].Set();
                    CurrentRequests.Remove(image.ID);
                }
            }

            if (image.Success)
            {
                lock (RenderReady)
                {
                    if (!RenderReady.ContainsKey(image.ID))
                    {
                        // Add to rendering dictionary
                        RenderReady.Add(image.ID, image);
                    }
                }
            }
            else
            {
                Console.WriteLine(String.Format("Download of texture {0} failed. NotFound={1}", image.ID, image.NotFound));
            }

            // Let any subscribers know about it
            if (OnDownloadFinished != null)
            {
                OnDownloadFinished(image.ID, image.Success);
            }
        }

        private void Assets_OnImageReceiveProgress(UUID image, int recieved, int total)
        {
            if (OnDownloadProgress != null)
            {
                OnDownloadProgress(image, recieved, total);
            }
        }
    }

}
