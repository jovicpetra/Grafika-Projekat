// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Windows.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 13000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        Cylinder cylinder;
        private const string font = "Tahoma";
        private double ballSize;
        private double goalHeight;
        private double speedOfRotation;
        private bool ballUp;
        private bool isBallBouncing;
        private float ballX;
        private float ballY;
        private float ballZ;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer3;
        private enum TextureObjects { Grass = 0, WhitePlastic };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images//grass.jpg", "..//..//images//whiteplastic.jpg" };

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public double BallSize
        {
            get { return ballSize; }
            set { ballSize = value; }
        }

        public double GoalHeight
        {
            get { return goalHeight; }
            set { goalHeight = value; }
        }

        public double SpeedOfRotation
        {
            get { return speedOfRotation; }
            set { speedOfRotation = value; }
        }

        public bool BallUp
        {
            get { return ballUp; }
            set { ballUp = value; }
        }


        public DispatcherTimer Timer1
        {
            get => timer1;
            set => timer1 = value;
        }

        public DispatcherTimer Timer2
        {
            get => timer2;
            set => timer2 = value;
        }

        public DispatcherTimer Timer3
        {
            get => timer3;
            set => timer3 = value;
        }

        public float BallX
        {
            get => ballX;
            set => ballX = value;
        }

        public float BallY
        {
            get => ballY;
            set => ballY = value;
        }

        public float BallZ
        {
            get => ballZ;
            set => ballZ = value;
        }

        public bool IsBallBouncing
        {
            get => isBallBouncing;
            set => isBallBouncing = value;
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_NORMALIZE);

            InitializeTexture(gl);

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(5);
            timer1.Tick += new EventHandler(MoveBall);
            timer1.Start();
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(5f);
            timer2.Tick += new EventHandler(ChangeDirection);
            timer2.Start();

            BallX = 0.0f;
            BallY = -2500f;
            BallZ = -2000f;
            BallUp = true;
            IsBallBouncing = true;

            m_scene.LoadScene();
            m_scene.Initialize();

            cylinder = new Cylinder();
            GoalHeight = 6500;
            BallSize = 0.5;
        }
       
        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Viewport(0, 0, m_width, m_height);
            // perspektiva
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (double)m_width / m_height, 0.5f, 100000f);
            //kamera 
            gl.LookAt(0f, 0f, m_sceneDistance + 1, 0f, 0f, m_sceneDistance, 0f, 1f, 0f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            //tackasti izvor svjetlosti
            gl.PushMatrix();
            float[] light0pos = new float[] { 1300f, 700.0f, 0, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.7f, 0.7f, 0.7f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.PopMatrix();

            //reflektor iznad lopte
            gl.PushMatrix();
            float[] light1pos = new float[] { 0, 3300, -200, 1.0f };
            float[] light1ambient = new float[] { 0.1f, 0f, 0f, 1.0f };
            float[] light1diffuse = { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] light1specular = new float[] { 1.0f, 1.0f, 0f, 1.0f };
            float[] light1direction = new float[] { 0f, -1f, 0f, 0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1direction);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.PopMatrix();

            DrawBase(gl);
            DrawGoal(gl);
            DrawBall(gl);
            DrawText(gl);
            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.5f, 100000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                
        }

        private void DrawBase(OpenGL gl)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(2, 2, 2);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.0f, 0.3f, 0.0f);
            gl.Normal(FindFaceNormal(8000.0f, -2000.0f, -m_sceneDistance, -8000.0f, -2000.0f, -m_sceneDistance,
               -8000.0f, -5000.0f, m_sceneDistance));
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(8000.0f, -2000.0f, -m_sceneDistance);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-8000.0f, -2000.0f, -m_sceneDistance);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-8000.0f, -5000.0f, m_sceneDistance);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(8000.0f, -5000.0f, m_sceneDistance);
            gl.End();
            gl.PopMatrix();
        }

        private void DrawGoal(OpenGL gl)
        {
            // sredina
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhitePlastic]);
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.Color(0.5, 0.5, 0.5);
            cylinder.CreateInContext(gl);
            cylinder.Height = 4500;
            cylinder.TopRadius = 100f;
            cylinder.BaseRadius = 100f;
            cylinder.TextureCoords = true;
            cylinder.NormalGeneration = Normals.Smooth;
            cylinder.NormalOrientation = Orientation.Outside;
            gl.Translate(0.0f, GoalHeight - 8500f, -m_sceneDistance);
            gl.Rotate(-90f, 1.0f, 0.0f, 0.0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            // horizontalna
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhitePlastic]);
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            cylinder.CreateInContext(gl);
            cylinder.Height = 5000;
            cylinder.TopRadius = 80f;
            cylinder.BaseRadius = 80f;
            cylinder.TextureCoords = true;
            cylinder.NormalGeneration = Normals.Smooth;
            cylinder.NormalOrientation = Orientation.Outside;
            gl.Translate(2500.0f, GoalHeight - 4000f, -m_sceneDistance);
            gl.Rotate(-90f, 0.0f, 1.0f, 0.0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.PopMatrix();


            // lijeva vodoravna
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhitePlastic]);
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            cylinder.CreateInContext(gl);
            cylinder.Height = 6500;
            cylinder.TopRadius = 80f;
            cylinder.BaseRadius = 80f;
            cylinder.TextureCoords = true;
            cylinder.NormalGeneration = Normals.Smooth;
            cylinder.NormalOrientation = Orientation.Outside;
            gl.Translate(-2500.0f, GoalHeight - 4000f, -m_sceneDistance);
            gl.Rotate(-90f, 1.0f, 0.0f, 0.0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            // desna vodoravna
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhitePlastic]);
            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            cylinder.CreateInContext(gl);
            cylinder.Height = 6500;
            cylinder.TopRadius = 80f;
            cylinder.BaseRadius = 80f;
            cylinder.TextureCoords = true;
            cylinder.NormalGeneration = Normals.Smooth;
            cylinder.NormalOrientation = Orientation.Outside;
            gl.Translate(2500.0f, GoalHeight - 4000f, -m_sceneDistance);
            gl.Rotate(-90f, 1.0f, 0.0f, 0.0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawBall(OpenGL gl) {
            gl.PushMatrix();
            gl.Translate(BallX, BallY, BallZ);
            gl.Scale(ballSize*900, ballSize*900, ballSize*900);
            if (isBallBouncing)
                RotateBall(gl);
            m_scene.Draw();
            gl.PopMatrix();
        }

        private void DrawText(OpenGL gl) {

            if (m_width <= 400 && m_height <= 300)
            {
                gl.Viewport(m_width * 2 / 3, m_height  / 2, m_width * 1 / 3, m_height  / 2);
                gl.PushMatrix();

                gl.DrawText3D(font, 10f, 1f, 0f, "");
                gl.DrawText(0, 400, 1.0f, 1.1f, 0.0f, font, 10, "Predmet: Racunarska grafika");
                gl.DrawText(0, 400, 1.0f, 1.1f, 0.0f, font, 10, "_______________________");
                gl.DrawText(0, 300, 1.0f, 1.1f, 0.0f, font, 10, "Sk.god: 2021/22");
                gl.DrawText(0, 300, 1.0f, 1.1f, 0.0f, font, 10, "_____________");
                gl.DrawText(0, 200, 1.0f, 1.1f, 0.0f, font, 10, "Ime: Petra");
                gl.DrawText(0, 200, 1.0f, 1.1f, 0.0f, font, 10, "_________");
                gl.DrawText(0, 100, 1.0f, 1.1f, 0.0f, font, 10, "Prezime: Jovic");
                gl.DrawText(0, 100, 1.0f, 1.1f, 0.0f, font, 10, "___________");
                gl.DrawText(0, 0, 1.0f, 1.1f, 0.0f, font, 10, "Sifra zad: 7.1");
                gl.DrawText(0, 0, 1.0f, 1.1f, 0.0f, font, 10, "___________");

                gl.PopMatrix();
            }
            else if (m_width <= 1000 && m_height <= 700) {
                gl.Viewport(m_width * 4 / 5, m_height * 4 / 5, m_width * 1 / 5, m_height * 1 / 5);
                gl.PushMatrix();

                gl.DrawText3D(font, 10f, 1f, 0f, "");
                gl.DrawText(0, 400, 1.0f, 1.1f, 0.0f, font, 10, "Predmet: Racunarska grafika");
                gl.DrawText(0, 400, 1.0f, 1.1f, 0.0f, font, 10, "_______________________");
                gl.DrawText(0, 300, 1.0f, 1.1f, 0.0f, font, 10, "Sk.god: 2021/22");
                gl.DrawText(0, 300, 1.0f, 1.1f, 0.0f, font, 10, "_____________");
                gl.DrawText(0, 200, 1.0f, 1.1f, 0.0f, font, 10, "Ime: Petra");
                gl.DrawText(0, 200, 1.0f, 1.1f, 0.0f, font, 10, "_________");
                gl.DrawText(0, 100, 1.0f, 1.1f, 0.0f, font, 10, "Prezime: Jovic");
                gl.DrawText(0, 100, 1.0f, 1.1f, 0.0f, font, 10, "___________");
                gl.DrawText(0, 0, 1.0f, 1.1f, 0.0f, font, 10, "Sifra zad: 7.1");
                gl.DrawText(0, 0, 1.0f, 1.1f, 0.0f, font, 10, "___________");

                gl.PopMatrix();
            }
            else
            {
                gl.Viewport(m_width * 7 / 8, m_height * 7 / 8, m_width * 1 / 8, m_height * 1 / 8);

                gl.PushMatrix();

                gl.DrawText3D(font, 10f, 1f, 0f, "");
                gl.DrawText(100, 600, 1.0f, 1.1f, 0.0f, font, 10, "Predmet: Racunarska grafika");
                gl.DrawText(100, 600, 1.0f, 1.1f, 0.0f, font, 10, "_______________________");
                gl.DrawText(100, 450, 1.0f, 1.1f, 0.0f, font, 10, "Sk.god: 2021/22");
                gl.DrawText(100, 450, 1.0f, 1.1f, 0.0f, font, 10, "_____________");
                gl.DrawText(100, 300, 1.0f, 1.1f, 0.0f, font, 10, "Ime: Petra");
                gl.DrawText(100, 300, 1.0f, 1.1f, 0.0f, font, 10, "_________");
                gl.DrawText(100, 150, 1.0f, 1.1f, 0.0f, font, 10, "Prezime: Jovic");
                gl.DrawText(100, 150, 1.0f, 1.1f, 0.0f, font, 10, "___________");
                gl.DrawText(100, 0, 1.0f, 1.1f, 0.0f, font, 10, "Sifra zad: 7.1");
                gl.DrawText(100, 0, 1.0f, 1.1f, 0.0f, font, 10, "___________");

                gl.PopMatrix();
            }
                

          


            gl.Viewport(0, 0, m_width, m_height);

        }

        public float[] FindFaceNormal(float x1, float y1, float z1, float x2, float y2,
           float z2, float x3, float y3, float z3)
        {
            float[] normal = new float[3];
            normal[0] = (y1 - y2) * (z2 - z3) - (y2 - y3) * (z1 - z2);
            normal[1] = (x2 - x3) * (z1 - z2) - (x1 - x2) * (z2 - z3);
            normal[2] = (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
            float len = (float)(Math.Sqrt((normal[0] * normal[0]) + (normal[1] * normal[1]) + (normal[2] * normal[2])));

            if (len == 0.0f)
            {
                len = 1.0f;
            }

            normal[0] /= len;
            normal[1] /= len;
            normal[2] /= len;

            return normal;
        }


        private void RotateBall(OpenGL gl)
        {
            if(SpeedOfRotation > 30)
                SpeedOfRotation = 0.0f;
            else
                SpeedOfRotation += MainWindow.speedOfRotation;
           
            gl.Rotate(SpeedOfRotation, 1, 0, 0);
        }

        /// <summary>
        /// Definiše offset kocki
        /// </summary>
        private void MoveBall(object sender, EventArgs e)
        {
            if (BallUp)
                BallY += 50f;
            else
                BallY -= 50f;
        }

        /// <summary>
        /// Obrće smer pomeranja lopte
        /// </summary>
        private void ChangeDirection(object sender, EventArgs e)
        {
            if (!BallUp)
            {
                BallY = -2500f;
            }
            BallUp = !BallUp;
        }

        private void RotateBall(object sender, EventArgs e)
        {
            if (!BallUp)
            {
                BallY = -2500f;
            }
            BallUp = !BallUp;
        }

        private void StartAnimation(object sender, EventArgs e)
        {

            if (BallX < 2500.0f)
                BallX += 50;
            if (BallY < GoalHeight)
                BallY += 150;
            if (BallZ > -m_sceneDistance)
                BallZ += -200;
            if (BallX >= 2500.0f && BallY >= GoalHeight && BallZ >= -m_sceneDistance) {
                if (BallX == 4000.0f)
                {
                    BallX = 0.0f;
                    BallY = -2500f;
                    BallZ = -2000f;
                }
                else {
                    BallX += 50;
                    BallY -= 250;
                    BallZ += 600;
                }
            }
        }

        public void KickBall()
        {
            Timer1.Stop();
            Timer2.Stop();
            IsBallBouncing = false;
            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(2);
            timer3.Tick += new EventHandler(StartAnimation);
            timer3.Start();
        }

        public void StartPosition() {
            timer3.Stop();
            BallX = 0.0f;
            BallY = -2500f;
            BallZ = -2000f;
            BallUp = true;
            IsBallBouncing = true;
            timer1.Start();
            timer2.Start();
        }

        private void InitializeTexture(OpenGL gl)
        {
            m_textures = new uint[2];
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly,
                                                      PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
