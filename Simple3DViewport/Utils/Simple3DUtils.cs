using Simple3DViewport.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Simple3DViewport.Utils
{
    public class Simple3DUtils
    {
        /*
         * DEFAULT STRUCTURES
         */
        public static PerspectiveCamera getDefaultCamera(int distance = 500)
        {
            PerspectiveCamera myPCamera = new PerspectiveCamera();
            myPCamera.Position = new Point3D(0, 0, distance);
            myPCamera.LookDirection = new Vector3D(0, 0, -1);
            myPCamera.FieldOfView = 60;

            return myPCamera;
        }
        public static Light getDefaultLightning()
        {
            return new AmbientLight(Brushes.White.Color);
        }
        public static DiffuseMaterial getDefaultMaterial()
        {
            LinearGradientBrush myHorizontalGradient = new LinearGradientBrush();
            myHorizontalGradient.StartPoint = new Point(0, 0.5);
            myHorizontalGradient.EndPoint = new Point(1, 0.5);
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));

            return new DiffuseMaterial(myHorizontalGradient);
        }
        public static SimpleModel getOriginSimpleModel(double? size = 0.1)
        {
            SimpleModel model = new SimpleModel();
            model.ModelId = "ORIGIN";

            model.Meshes.Add(new SimpleMesh(GeometryShapes.getCube(size.Value, new Vector3D(), Color.FromRgb(200, 200, 200))));
            model.Meshes.Add(new SimpleMesh(GeometryShapes.getVector(Matrix4x4.CreateFromAxisAngle(new Vector3(1, 0, 0), (float)(Math.PI / 2)), size.Value * 5, size.Value / 2, Color.FromRgb(255, 0, 0))));
            model.Meshes.Add(new SimpleMesh(GeometryShapes.getVector(Matrix4x4.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)(Math.PI / 2)), size.Value * 5, size.Value / 2, Color.FromRgb(0, 0, 255))));
            model.Meshes.Add(new SimpleMesh(GeometryShapes.getVector(Matrix4x4.CreateFromAxisAngle(new Vector3(0, 0, 1), - (float)(Math.PI / 2)), size.Value * 5, size.Value / 2, Color.FromRgb(0, 255, 0))));

            return model;
        }

        /*
         * CAMERA
         */
        public static PerspectiveCamera getCameraByBoundingBox(Rect3D boundingBox)
        {
            double maxSize = boundingBox.SizeX > boundingBox.SizeY ? boundingBox.SizeX : boundingBox.SizeY;
            PerspectiveCamera myPCamera = new PerspectiveCamera();
            myPCamera.Position = new Point3D(0, 0, maxSize * 1.2);
            myPCamera.LookDirection = new Vector3D(0, 0, -1);
            myPCamera.FieldOfView = 60;

            return myPCamera;
        }
        public static Vector3D getVectorToTarget(Point3D position, Point3D targetPosition = new Point3D())
        {
            Vector3D vector = new Vector3D(position.X, position.Y, position.Z);
            Vector3D targetVector = new Vector3D(targetPosition.X, targetPosition.Y, targetPosition.Z);
            return getVectorToTarget(vector, targetVector);
        }
        public static Vector3D getVectorToTarget(Vector3D position, Vector3D targetPosition = new Vector3D())
        {
            return -(position - targetPosition);
        }

        // Returns the horizontal perpendicular vector of length 1
        public static Vector3D getHorizontalPerpendicularVector(Vector3D vector)
        {
            // Counterclockwise
            Vector3D perpendicularVector = new Vector3D(vector.Z, 0, -vector.X);
            if (perpendicularVector.X != 0 || perpendicularVector.Y != 0 || perpendicularVector.Z != 0)
                perpendicularVector.Normalize();

            return perpendicularVector;
        }
        // Returns the horizontal perpendicular vector of length 1
        public static Vector3D getVerticalPerpendicularVector(Vector3D cameraVector, Vector3D horizontalVector)
        {
            // Upwards
            Vector3D perpendicularVector = Vector3D.CrossProduct(cameraVector, horizontalVector);
            if (perpendicularVector.X != 0 || perpendicularVector.Y != 0 || perpendicularVector.Z != 0)
                perpendicularVector.Normalize();

            return perpendicularVector;
        }

        /*
         * 3D MODELS
         */
        public static void addTri(Int32Collection myTriangleIndicesCollection, int i1, int i2, int i3)
        {
            myTriangleIndicesCollection.Add(i1);
            myTriangleIndicesCollection.Add(i2);
            myTriangleIndicesCollection.Add(i3);
        }

        public static void setModelTransform(GeometryModel3D model, Matrix4x4 transformationMatrix)
        {
            // Convert the Matrix4x4 to Matrix3D
            Matrix3D matrix3D = new Matrix3D(
                transformationMatrix.M11, transformationMatrix.M12, transformationMatrix.M13, transformationMatrix.M14,
                transformationMatrix.M21, transformationMatrix.M22, transformationMatrix.M23, transformationMatrix.M24,
                transformationMatrix.M31, transformationMatrix.M32, transformationMatrix.M33, transformationMatrix.M34,
                transformationMatrix.M41, transformationMatrix.M42, transformationMatrix.M43, transformationMatrix.M44
            );

            model.Transform = new MatrixTransform3D(matrix3D);
        }

        public static void applyTransform(GeometryModel3D model, Matrix4x4 transformationMatrix)
        {
            // Convert the Matrix4x4 to Matrix3D
            Matrix3D matrix3D = new Matrix3D(
                transformationMatrix.M11, transformationMatrix.M12, transformationMatrix.M13, transformationMatrix.M14,
                transformationMatrix.M21, transformationMatrix.M22, transformationMatrix.M23, transformationMatrix.M24,
                transformationMatrix.M31, transformationMatrix.M32, transformationMatrix.M33, transformationMatrix.M34,
                transformationMatrix.M41, transformationMatrix.M42, transformationMatrix.M43, transformationMatrix.M44
            );

            // Get the existing transform of the model
            Transform3D existingTransform = model.Transform;

            // Convert the existing transform to Matrix3D
            Matrix3D existingMatrix = existingTransform.Value;

            // Multiply the existing matrix with the new matrix
            Matrix3D combinedMatrix = Matrix3D.Multiply(existingMatrix, matrix3D);

            // Create a new transform with the combined matrix
            MatrixTransform3D newTransform = new MatrixTransform3D(combinedMatrix);

            // Apply the new transform to the model
            model.Transform = newTransform;
        }

        public static Rect3D getBoundingBox(List<GeometryModel3D> vpMeshes)
        {
            Rect3D boundingBox = new Rect3D();
            float minX = 0;
            float maxX = 0;
            float minY = 0;
            float maxY = 0;
            float minZ = 0;
            float maxZ = 0;
            foreach (GeometryModel3D mesh in vpMeshes)
            {
                float localMinX = (float)(mesh.Geometry.Bounds.Location.X - mesh.Geometry.Bounds.SizeX);
                float localMaxX = (float)(mesh.Geometry.Bounds.Location.X + mesh.Geometry.Bounds.SizeX);
                float localMinY = (float)(mesh.Geometry.Bounds.Location.Y - mesh.Geometry.Bounds.SizeY);
                float localMaxY = (float)(mesh.Geometry.Bounds.Location.Y + mesh.Geometry.Bounds.SizeY);
                float localMinZ = (float)(mesh.Geometry.Bounds.Location.Z - mesh.Geometry.Bounds.SizeZ);
                float localMaxZ = (float)(mesh.Geometry.Bounds.Location.Z + mesh.Geometry.Bounds.SizeZ);

                if (localMinX < minX)
                    minX = localMinX;
                if (localMaxX > maxX)
                    maxX = localMaxX;
                if (localMinY < minY)
                    minY = localMinY;
                if (localMaxY > maxY)
                    maxY = localMaxY;
                if (localMinZ < minZ)
                    minZ = localMinZ;
                if (localMaxZ > maxZ)
                    maxZ = localMaxZ;
            }

            boundingBox.SizeX = Math.Abs(maxX - minX);
            boundingBox.SizeY = Math.Abs(maxY - minY);
            boundingBox.SizeZ = Math.Abs(maxZ - minZ);

            double X = minX + (boundingBox.SizeX / 2);
            double Y = minY + (boundingBox.SizeY / 2);
            double Z = minZ + (boundingBox.SizeZ / 2);

            boundingBox.Location = new Point3D(X, Y, Z);

            return boundingBox;
        }
    }
}
