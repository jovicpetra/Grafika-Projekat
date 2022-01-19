using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;
        public static double goalHeight = 6500.0;
        public ObservableCollection<double> GoalHeight
        {
            get;
            set;
        }

        public static double ballSize = 0.7;
        public ObservableCollection<double> BallSize
        {
            get;
            set;
        }

        public static double speedOfRotation = 10.0;
        public ObservableCollection<double> SpeedOfRotation
        {
            get;
            set;
        }


        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            GoalHeight = new ObservableCollection<double>();
            GoalHeight.Add(4000.0);
            GoalHeight.Add(4500.0);
            GoalHeight.Add(5000.0);
            GoalHeight.Add(5500.0);
            GoalHeight.Add(6000.0);
            GoalHeight.Add(6500.0);

            BallSize = new ObservableCollection<double>();
            BallSize.Add(0.5);
            BallSize.Add(0.6);
            BallSize.Add(0.7);
            BallSize.Add(0.8);
            BallSize.Add(0.9);
            BallSize.Add(1.0);

            SpeedOfRotation = new ObservableCollection<double>();
            SpeedOfRotation.Add(2.0);
            SpeedOfRotation.Add(3.0);
            SpeedOfRotation.Add(5.0);
            SpeedOfRotation.Add(10.0);
            SpeedOfRotation.Add(20.0);
            SpeedOfRotation.Add(30.0);

            InitializeComponent();
            DataContext = this;

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\obj"), "rugby_ball.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    {
                        if (m_world.IsBallBouncing)
                            this.Close();
                        break;
                    }
                case Key.E:
                    {
                        if (m_world.IsBallBouncing && m_world.RotationX > -10)
                            m_world.RotationX -= 5.0f;
                        break;
                    }
                case Key.D:
                    {
                        if (m_world.IsBallBouncing && m_world.RotationX < 20)
                            m_world.RotationX += 5.0f;
                        break;
                    }
                case Key.S:
                    {
                        if (m_world.IsBallBouncing)
                            m_world.RotationY -= 5.0f;
                        break;
                    }
                case Key.F:
                    {
                        if (m_world.IsBallBouncing)
                            m_world.RotationY += 5.0f;
                        break;
                    }
                case Key.Add: {
                        if (m_world.IsBallBouncing)
                            m_world.SceneDistance -= 700.0f;
                        break;
                    } 
                case Key.Subtract:
                    {
                        if (m_world.IsBallBouncing)
                            m_world.SceneDistance += 700.0f;
                        break;
                    }
                case Key.V: {
                        if (m_world.IsBallBouncing)
                            m_world.KickBall();
                        break;
                }
            }
        }

        private void goalHeightCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (goalHeightCombo.SelectedIndex.Equals(-1))
                m_world.GoalHeight = 6500.0;
            else if (goalHeightCombo.SelectedItem.Equals(4000.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(4000.0);
                m_world.GoalHeight = GoalHeight[idx];
            }
            else if (goalHeightCombo.SelectedItem.Equals(4500.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(4500.0);
                m_world.GoalHeight = GoalHeight[idx];
            }
            else if (goalHeightCombo.SelectedItem.Equals(5000.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(5000.0);
                m_world.GoalHeight = GoalHeight[idx];
            }
            else if (goalHeightCombo.SelectedItem.Equals(5500.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(5500.0);
                m_world.GoalHeight = GoalHeight[idx];
            }
            else if (goalHeightCombo.SelectedItem.Equals(6000.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(6000.0);
                m_world.GoalHeight = GoalHeight[idx];
            }
            else if (goalHeightCombo.SelectedItem.Equals(6500.0) && m_world.IsBallBouncing)
            {
                int idx = GoalHeight.IndexOf(6500.0);
                m_world.GoalHeight = GoalHeight[idx];
            }

        }

        private void ballSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ballSizeCombo.SelectedIndex.Equals(-1) && m_world.IsBallBouncing)
                m_world.BallSize = 0.7;
            else if (ballSizeCombo.SelectedItem.Equals(0.5) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(0.5);
                m_world.BallSize = BallSize[idx];
            }
            else if (ballSizeCombo.SelectedItem.Equals(0.6) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(0.6);
                m_world.BallSize = BallSize[idx];
            }
            else if (ballSizeCombo.SelectedItem.Equals(0.7) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(0.7);
                m_world.BallSize = BallSize[idx];
            }
            else if (ballSizeCombo.SelectedItem.Equals(0.8) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(0.8);
                m_world.BallSize = BallSize[idx];
            }
            else if (ballSizeCombo.SelectedItem.Equals(0.9) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(0.9);
                m_world.BallSize = BallSize[idx];
            }
            else if (ballSizeCombo.SelectedItem.Equals(1.0) && m_world.IsBallBouncing)
            {
                int idx = BallSize.IndexOf(1.0);
                m_world.BallSize = BallSize[idx];
            }

        }

        private void speedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (speedCombo.SelectedIndex.Equals(-1) && m_world.IsBallBouncing)
                m_world.SpeedOfRotation = 10.0;
            else if (speedCombo.SelectedItem.Equals(2.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(2.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }
            else if (speedCombo.SelectedItem.Equals(3.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(3.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }
            else if (speedCombo.SelectedItem.Equals(5.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(5.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }
            else if (speedCombo.SelectedItem.Equals(10.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(10.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }
            else if (speedCombo.SelectedItem.Equals(20.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(20.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }
            else if (speedCombo.SelectedItem.Equals(30.0) && m_world.IsBallBouncing)
            {
                int idx = SpeedOfRotation.IndexOf(30.0);
                speedOfRotation = SpeedOfRotation[idx];
                m_world.SpeedOfRotation = speedOfRotation;
            }

        }

        private void kickButton_Click(object sender, RoutedEventArgs e)
        {
            m_world.KickBall();
        }

        private void startPositionButton_Click(object sender, RoutedEventArgs e)
        {
            m_world.StartPosition();
        }
    }
}
