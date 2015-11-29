﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using VoxelEngine.Camera;
using VoxelEngine.GameData;

namespace VoxelEngine
{
    public class Engine : GameWindow
    {
        public static Engine Instance;
        public GameCameraController Camera;
        public Map Map;
        private Matrix4 _matrixProjection;
        private int _timer, _counter;
        public static Vector2 ScreenSize;
        public static Vector2 ScreenPos;

        [STAThread]
        public static void Main()
        {
            using (var game = new Engine())
            {
                Instance = game;
                // Run the game at 60 updates per second
                game.Run(60);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnResize(e);
            OnMove(e);
            CursorVisible = false;
            // Load stuff
            Camera = new GameCameraController();
            Map = new Map(8);

            //Settings
            VSync = VSyncMode.On;
            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            _matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _matrixProjection);
            ScreenSize = new Vector2(Width, Height);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            ScreenPos = new Vector2(X, Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _counter++;
            _timer += (int)(1000*e.Time);
            if (_timer >= 1000)
            {
                Console.WriteLine(_counter);
                _timer = 0;
                _counter = 0;
            }
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Camera.OnRenderFrame(e);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); //PolygonMode important, MaterialFace.Front only renders front side?
            Map.OnRenderFrame(e);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape])
            {
                Exit();
            }

            Camera.OnUpdateFrame(e);
            
        }
    }
}