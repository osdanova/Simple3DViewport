using Simple3DViewport.Controls;
using Simple3DViewport.Objects;
using Simple3DViewport.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Simple3DViewport
{
    public partial class MainWindow : Window
    {
        private Simple3DViewport_Control myVP { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            SimpleMesh meshCube = new SimpleMesh(GeometryShapes.getCube(2, new Vector3D(0,5,0), Color.FromRgb(255, 0, 0)));
            meshCube.Labels.Add("CUBE");
            SimpleMesh meshSphere = new SimpleMesh(GeometryShapes.getSphere(2, 10, new Vector3D(5, 0, 0), Color.FromRgb(0, 255, 0)));
            meshSphere.Labels.Add("SPHERE");
            SimpleMesh meshCapsule = new SimpleMesh(GeometryShapes.getCapsule(2, 3, 10, new Vector3D(-5, 0, 0), Color.FromRgb(0, 0, 255)));
            meshCapsule.Labels.Add("CAPSULE");
            /*SimpleMesh meshSphere2 = new SimpleMesh(GeometryShapes.getSphere(2, 10, new Vector3D(0, -5, 0), Color.FromRgb(255, 255, 0)));
            meshSphere2.Labels.Add("SPHERE");*/
            SimpleMesh meshEllipsoid = new SimpleMesh(GeometryShapes.getEllipsoid(2, 3, 10, new Vector3D(0, 0, -5), Color.FromRgb(255, 255, 0)));
            meshEllipsoid.Labels.Add("ELLIPSOID");
            SimpleMesh meshCylinder = new SimpleMesh(GeometryShapes.getCylinder(2, 3, 10, new Vector3D(0, -5, 0), Color.FromRgb(255, 0, 255)));
            meshEllipsoid.Labels.Add("CYLINDER");
            SimpleMesh meshCone = new SimpleMesh(GeometryShapes.getCone(2, 3, 10, new Vector3D(0, 0, 5), Color.FromRgb(0, 255, 255)));
            meshEllipsoid.Labels.Add("CONE");

            SimpleModel model = new SimpleModel(new List<SimpleMesh> { meshCube, meshSphere, meshCapsule, meshEllipsoid, meshCylinder, meshCone });

            myVP = new Simple3DViewport_Control(new List<SimpleModel> { model });

            viewportFrame.Content = myVP;
        }

        private void menu_addItem(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();

            // Shape
            int shape = rnd.Next(1, 6);

            // Position
            float posX = (float)rnd.NextDouble() * 20;
            if (rnd.Next(0, 2) == 1 ) posX *= -1;
            float posY = (float)rnd.NextDouble() * 20;
            if (rnd.Next(0, 2) == 1) posY *= -1;
            float posZ = (float)rnd.NextDouble() * 20;
            if (rnd.Next(0, 2) == 1) posZ *= -1;
            float size = 1 + (float)rnd.NextDouble();

            // Rotation
            float rotX = (float)(rnd.NextDouble() * Math.PI);
            float rotY = (float)(rnd.NextDouble() * Math.PI);
            float rotZ = (float)(rnd.NextDouble() * Math.PI);
            Matrix4x4 mrotX = Matrix4x4.CreateFromAxisAngle(new Vector3(1, 0, 0), rotX);
            Matrix4x4 mrotY = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), rotX);
            Matrix4x4 mrotZ = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 0, 1), rotX);

            // Color
            byte colorA = (byte) rnd.Next(1, 256);
            byte colorR = (byte) rnd.Next(0, 256);
            byte colorG = (byte) rnd.Next(0, 256);
            byte colorB = (byte) rnd.Next(0, 256);

            SimpleMesh mesh = new SimpleMesh();
            if (shape == 1)
            {
                mesh = new SimpleMesh(GeometryShapes.getCube(size, new Vector3D(posX, posY, posZ), Color.FromArgb(colorA, colorR, colorG, colorB)));
            }
            else if (shape == 2)
            {
                mesh = new SimpleMesh(GeometryShapes.getSphere(size, 10, new Vector3D(posX, posY, posZ), Color.FromArgb(colorA, colorR, colorG, colorB)));
            }
            else if (shape == 3)
            {
                mesh = new SimpleMesh(GeometryShapes.getCapsule(size, 3, 10, new Vector3D(posX, posY, posZ), Color.FromArgb(colorA, colorR, colorG, colorB)));
            }
            else if (shape == 4)
            {
                mesh = new SimpleMesh(GeometryShapes.getCylinder(size, 3, 10, new Vector3D(posX, posY, posZ), Color.FromArgb(colorA, colorR, colorG, colorB)));
            }
            else if (shape == 5)
            {
                mesh = new SimpleMesh(GeometryShapes.getCone(size, 3, 10, new Vector3D(posX, posY, posZ), Color.FromArgb(colorA, colorR, colorG, colorB)));
            }
            mesh.Labels.Add("RANDOM");

            Simple3DUtils.applyTransform(mesh.Geometry, mrotX);
            Simple3DUtils.applyTransform(mesh.Geometry, mrotY);
            Simple3DUtils.applyTransform(mesh.Geometry, mrotZ);

            SimpleModel model = new SimpleModel(new List<SimpleMesh> { mesh });

            myVP.VPModels.Add(model);
            myVP.render();
        }

        private void menu_removeItems(object sender, RoutedEventArgs e)
        {
            myVP.removeItemByLabel("RANDOM");
            myVP.render();
        }

        private void menu_changeBackgroundColor(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            byte colorR = (byte)rnd.Next(0, 100);
            byte colorG = (byte)rnd.Next(0, 100);
            byte colorB = (byte)rnd.Next(0, 100);

            myVP.setBackgroundColor(Color.FromRgb(colorR, colorG, colorB));
        }

        private void menu_changeOpacity(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            myVP.setOpacityByLabel(rnd.NextDouble(), "RANDOM");
            myVP.render();
        }
    }
}
