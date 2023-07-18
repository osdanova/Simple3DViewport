using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Simple3DViewport.Objects;
using Simple3DViewport.Utils;

namespace Simple3DViewport.Controls
{
    public partial class Simple3DViewport_Control : UserControl
    {
        public List<SimpleModel> VPModels { get; set; }
        public PerspectiveCamera VPCamera { get; set; }
        public Light VPLight { get; set; }
        public SimpleModel VPOriginVisual { get; set; }

        // CAMERA HELPERS
        public Point L_previousPosition = new Point();
        public Point L_currentPosition = new Point();
        public Point R_previousPosition = new Point();
        public Point R_currentPosition = new Point();
        public Point3D AnchorPoint { get; set; }
        public Point3D AnchorPointTemp { get; set; }
        public Vector3D AnchorPointHorVec { get; set; }
        public Vector3D AnchorPointVerVec { get; set; }
        public bool AnchorPointLocked { get; set; }

        // CONSTRUCTOR
        public Simple3DViewport_Control()
        {
            InitializeComponent();

            // MODELS
            VPModels = new List<SimpleModel>();

            // CAMERA
            restartCamera();

            // LIGHTNING
            VPLight = Simple3DUtils.getDefaultLightning();

            // ORIGIN
            VPOriginVisual = Simple3DUtils.getOriginSimpleModel();
            viewportFrame.Children.Add(VPOriginVisual.getVisual());

            // RENDER
            render();
        }
        public Simple3DViewport_Control(List<SimpleModel> vpModels, PerspectiveCamera? vpCamera = null)
        {
            InitializeComponent();

            // MODELS
            VPModels = vpModels;
            if(VPModels == null) VPModels= new List<SimpleModel>();

            // CAMERA
            if(vpCamera == null) restartCamera();

            // LIGHTNING
            VPLight = Simple3DUtils.getDefaultLightning();

            // ORIGIN
            VPOriginVisual = Simple3DUtils.getOriginSimpleModel();
            viewportFrame.Children.Add(VPOriginVisual.getVisual());

            // RENDER
            render();
        }

        // ACTIONS
        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scale = 0.3;
            Point3D position = VPCamera.Position;
            Vector3D lookVector = Simple3DUtils.getVectorToTarget(VPCamera.Position, AnchorPoint);
            double length = lookVector.Length;
            lookVector.Normalize();

            if (e.Delta > 0)
                lookVector *= length * scale;

            else if (e.Delta < 0)
                lookVector *= length * (-scale);

            VPCamera.Position = new Point3D(position.X + lookVector.X, position.Y + lookVector.Y, position.Z + lookVector.Z);
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                R_currentPosition = e.GetPosition(viewportFrame);

                if (R_previousPosition != R_currentPosition)
                    moveCamera(R_previousPosition, R_currentPosition);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                L_currentPosition = e.GetPosition(viewportFrame);

                if (L_previousPosition != L_currentPosition)
                    rotateCamera((L_currentPosition.X - L_previousPosition.X), (L_currentPosition.Y - L_previousPosition.Y), 0);
            }

            R_previousPosition = e.GetPosition(viewportFrame);
            L_previousPosition = e.GetPosition(viewportFrame);
        }

        private void Viewport_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            AnchorPointLocked = true;
            AnchorPointTemp = new Point3D(AnchorPoint.X, AnchorPoint.Y, AnchorPoint.Z);

            Point3D position = VPCamera.Position;
            AnchorPointHorVec = Simple3DUtils.getHorizontalPerpendicularVector(new Vector3D(position.X - AnchorPoint.X, position.Y - AnchorPoint.Y, position.Z - AnchorPoint.Z));
            AnchorPointVerVec = Simple3DUtils.getVerticalPerpendicularVector(new Vector3D(position.X - AnchorPoint.X, position.Y - AnchorPoint.Y, position.Z - AnchorPoint.Z), AnchorPointHorVec);
        }
        private void Viewport_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            AnchorPointLocked = false;
            AnchorPoint = new Point3D(AnchorPointTemp.X, AnchorPointTemp.Y, AnchorPointTemp.Z);
        }

        // FUNCTIONS
        public void restartCamera()
        {
            if (VPModels.Count > 0)
            {
                List<GeometryModel3D> geometries = new List<GeometryModel3D>();
                foreach(SimpleModel model in VPModels)
                {
                    foreach(SimpleMesh mesh in model.Meshes)
                    {
                        geometries.Add(mesh.Geometry);
                    }
                }
                Rect3D boundingBox = Simple3DUtils.getBoundingBox(geometries);
                VPCamera = Simple3DUtils.getCameraByBoundingBox(boundingBox);
            }
            else
            {
                VPCamera = Simple3DUtils.getDefaultCamera(10);
            }

            viewportFrame.Camera = VPCamera;
            AnchorPoint = new Point3D();
            AnchorPointTemp = new Point3D();
            AnchorPointLocked = false;
        }
        public void render()
        {
            viewportFrame.Children.Clear();

            ModelVisual3D lightning = new ModelVisual3D();
            lightning.Content = VPLight;
            viewportFrame.Children.Add(lightning);

            viewportFrame.Children.Add(VPOriginVisual.getVisual());
            foreach (SimpleModel model in VPModels)
            {
                viewportFrame.Children.Add(model.getVisual());
            }
        }

        public void moveCamera(Point pre, Point cur)
        {
            Point3D position = VPCamera.Position;
            Vector3D positionVector = new Vector3D(VPCamera.Position.X, VPCamera.Position.Y, VPCamera.Position.Z);
            double speed = positionVector.Length / 600;

            Vector3D moveHorVec = AnchorPointHorVec * (cur.X - pre.X) * speed;
            Vector3D moveVerVec = AnchorPointVerVec * (cur.Y - pre.Y) * speed;


            VPCamera.Position = new Point3D(position.X - moveHorVec.X, position.Y + moveVerVec.Y, position.Z - moveHorVec.Z);
            AnchorPointTemp = new Point3D(AnchorPointTemp.X - moveHorVec.X, AnchorPointTemp.Y + moveVerVec.Y, AnchorPointTemp.Z - moveHorVec.Z);
        }
        public void rotateCamera(Point pre, Point cur)
        {
            Point3D position = VPCamera.Position;
            double speed = 1;

            VPCamera.Position = new Point3D(position.X - (cur.X - pre.X) * speed, position.Y + (cur.Y - pre.Y) * speed, position.Z);
        }

        private void rotateCamera(double rX, double rY, double rZ)
        {
            Vector3D vector = new Vector3D(VPCamera.Position.X - AnchorPoint.X, VPCamera.Position.Y - AnchorPoint.Y, VPCamera.Position.Z - AnchorPoint.Z);

            double length = vector.Length;
            double theta = Math.Acos(vector.Y / length); // Vertical angle
            double phi = Math.Atan2(-vector.Z, vector.X); // Horizontal angle

            theta -= rY * 0.01;
            phi -= rX * 0.01;
            length *= 1.0 - 0.1 * rZ;

            theta = Math.Clamp(theta, 0.0001, Math.PI - 0.0001);

            vector.X = length * Math.Sin(theta) * Math.Cos(phi);
            vector.Z = -length * Math.Sin(theta) * Math.Sin(phi);
            vector.Y = length * Math.Cos(theta);

            VPCamera.Position = new Point3D(AnchorPoint.X + vector.X, AnchorPoint.Y + vector.Y, AnchorPoint.Z + vector.Z);
            VPCamera.LookDirection = Simple3DUtils.getVectorToTarget(VPCamera.Position, AnchorPoint);
        }

        public void setVisibilityById(bool visibility, string itemId)
        {
            foreach(SimpleModel model in VPModels)
            {
                model.setVisibilityById(visibility, itemId);
            }
            render();
        }
        public void setVisibilityByLabel(bool visibility, string label)
        {
            foreach (SimpleModel model in VPModels)
            {
                model.setVisibilityByLabel(visibility, label);
            }
            render();
        }

        public void removeItemById(string itemId)
        {
            for (int i = VPModels.Count - 1; i >= 0; i--)
            {
                SimpleModel model = VPModels[i];
                if (model.ModelId == itemId)
                {
                    VPModels.Remove(model);
                }
                else
                {
                    model.removeMeshById(itemId);
                }
            }
            render();
        }
        public void removeItemByLabel(string label)
        {
            for (int i = VPModels.Count - 1; i >= 0; i--)
            {
                SimpleModel model = VPModels[i];
                if (model.Labels.Contains(label))
                {
                    VPModels.Remove(model);
                }
                else
                {
                    model.removeMeshByLabel(label);
                }
            }
            render();
        }

        // Only applies to diffuse materials
        public void setOpacityById(double opacity, string itemId)
        {
            foreach (SimpleModel model in VPModels)
            {
                model.setOpacityById(opacity, itemId);
            }
            render();
        }
        public void setOpacityByLabel(double opacity, string label)
        {
            foreach (SimpleModel model in VPModels)
            {
                model.setOpacityByLabel(opacity, label);
            }
            render();
        }

        public void setBackgroundColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            viewportBackground.Background = brush;
        }
    }
}
