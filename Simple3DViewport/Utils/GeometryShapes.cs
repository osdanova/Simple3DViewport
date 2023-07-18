using System;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Simple3DViewport.Utils
{
    public class GeometryShapes
    {
        /*
         * CUBOID
         */
        public static GeometryModel3D getCuboid(double width, double height, double depth, Vector3D position, Color? color = null)
        {
            MeshGeometry3D cuboidGeometry = new MeshGeometry3D();

            // Create a collection of vertex positions for the MeshGeometry3D.
            cuboidGeometry.Positions.Add(new Point3D(position.X - width, position.Y - height, position.Z - depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X + width, position.Y - height, position.Z - depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X - width, position.Y + height, position.Z - depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X + width, position.Y + height, position.Z - depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X - width, position.Y - height, position.Z + depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X + width, position.Y - height, position.Z + depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X - width, position.Y + height, position.Z + depth));
            cuboidGeometry.Positions.Add(new Point3D(position.X + width, position.Y + height, position.Z + depth));

            // Create a collection of triangle indices for the MeshGeometry3D.
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 3, 1); // Back
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 1, 0); // Back
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 7, 1, 3); // Left
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 7, 5, 1); // Left
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 6, 5, 7); // Front
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 6, 4, 5); // Front
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 4, 6); // Left
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 0, 4); // Left
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 7, 3); // Up
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 2, 6, 7); // Up
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 0, 1, 5); // Down
            Simple3DUtils.addTri(cuboidGeometry.TriangleIndices, 0, 5, 4); // Down

            GeometryModel3D cuboidModel = new GeometryModel3D();
            cuboidModel.Geometry = cuboidGeometry;

            if (color != null) cuboidModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return cuboidModel;
        }

        public static GeometryModel3D getCube(double size, Vector3D position, Color? color)
        {
            return getCuboid(size, size, size, position, color);
        }

        /*
         * SPHERE
         */
        public static GeometryModel3D getSphere(double radius, int subdivisions, Vector3D position, Color? color = null)
        {
            return getEllipsoid(radius, radius, subdivisions, position, color);
        }
        public static GeometryModel3D getEllipsoid(double radius, double height, int subdivisions, Vector3D position, Color? color = null)
        {
            // Create the sphere geometry
            MeshGeometry3D sphereGeometry = new MeshGeometry3D();

            // Generate the vertices for the sphere
            for (int lat = 0; lat <= subdivisions; lat++)
            {
                double theta = lat * Math.PI / subdivisions;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= subdivisions; lon++)
                {
                    double phi = lon * 2 * Math.PI / subdivisions;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = cosPhi * sinTheta;
                    double y = cosTheta;
                    double z = sinPhi * sinTheta;

                    sphereGeometry.Positions.Add(new Point3D(position.X + (radius * x), position.Y + (height * y), position.Z + (radius * z)));
                    //sphereGeometry.TextureCoordinates.Add(new Point(lon / (double)subdivisions, lat / (double)subdivisions));
                }
            }

            // Generate the triangles for the sphere
            for (int lat = 0; lat < subdivisions; lat++)
            {
                for (int lon = 0; lon < subdivisions; lon++)
                {
                    int current = lat * (subdivisions + 1) + lon;
                    int next = current + subdivisions + 1;

                    Simple3DUtils.addTri(sphereGeometry.TriangleIndices, current, current + 1, next + 1);
                    Simple3DUtils.addTri(sphereGeometry.TriangleIndices, current, next + 1, next);
                }
            }

            // Create the model and apply a material
            GeometryModel3D sphereModel = new GeometryModel3D();
            sphereModel.Geometry = sphereGeometry;
            if (color != null) sphereModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return sphereModel;
        }

        /*
         * SEMISPHERE
         */
        public static GeometryModel3D getSemisphere(double radius, int subdivisions, Vector3D position, Boolean? upwards = true, Color? color = null)
        {
            // Create the semisphere geometry
            MeshGeometry3D semisphereGeometry = new MeshGeometry3D();

            // Generate the vertices for the semisphere
            for (int lat = 0; lat <= subdivisions / 2; lat++)
            {
                double theta = lat * Math.PI / subdivisions;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= subdivisions; lon++)
                {
                    double phi = lon * 2 * Math.PI / subdivisions;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = position.X + radius * sinTheta * cosPhi;
                    double y = position.Y;
                    y += (upwards.Value) ? radius * cosTheta : - radius * cosTheta;
                    double z = position.Z + radius * sinTheta * sinPhi;

                    semisphereGeometry.Positions.Add(new Point3D(x, y, z));
                }
            }

            // Generate the triangles for the semisphere
            for (int lat = 0; lat < subdivisions / 2; lat++)
            {
                for (int lon = 0; lon < subdivisions; lon++)
                {
                    int current = lat * (subdivisions + 1) + lon;
                    int next = current + subdivisions + 1;

                    if(upwards.Value)
                    {
                        Simple3DUtils.addTri(semisphereGeometry.TriangleIndices, current, current + 1, next + 1);
                        Simple3DUtils.addTri(semisphereGeometry.TriangleIndices, current, next + 1, next);
                    }
                    else
                    {
                        Simple3DUtils.addTri(semisphereGeometry.TriangleIndices, current, next + 1, current + 1);
                        Simple3DUtils.addTri(semisphereGeometry.TriangleIndices, current, next, next + 1);
                    }
                }
            }

            // Create the model and apply a material
            GeometryModel3D semisphereModel = new GeometryModel3D();
            semisphereModel.Geometry = semisphereGeometry;

            if (color != null) semisphereModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return semisphereModel;
        }

        /*
         * CYLINDER
         */
        public static GeometryModel3D getCylinder(double radius, double height, int subdivisions, Vector3D position, Color? color = null)
        {
            // Create the cylinder geometry
            MeshGeometry3D cylinderGeometry = new MeshGeometry3D();

            // Generate the vertices for the cylinder
            for (int i = 0; i <= subdivisions; i++)
            {
                double phi = i * 2 * Math.PI / subdivisions;
                double x = radius * Math.Cos(phi);
                double z = radius * Math.Sin(phi);

                cylinderGeometry.Positions.Add(new Point3D(position.X + x, position.Y - height / 2, position.Z + z));
                cylinderGeometry.Positions.Add(new Point3D(position.X + x, position.Y + height / 2, position.Z + z));
            }

            // Top and bottom
            cylinderGeometry.Positions.Add(new Point3D(position.X, position.Y - height / 2, position.Z));
            cylinderGeometry.Positions.Add(new Point3D(position.X, position.Y + height / 2, position.Z));

            // Generate the triangles for the sides of the cylinder
            for (int i = 0; i < subdivisions; i++)
            {
                int currentBottom = 2 * i;
                int currentTop = currentBottom + 1;
                int nextBottom = (currentBottom + 2) % (2 * subdivisions);
                int nextTop = (currentTop + 2) % (2 * subdivisions);

                Simple3DUtils.addTri(cylinderGeometry.TriangleIndices, currentBottom, currentTop, nextTop);
                Simple3DUtils.addTri(cylinderGeometry.TriangleIndices, currentBottom, nextTop, nextBottom);

                // Top and bottom triangles
                Simple3DUtils.addTri(cylinderGeometry.TriangleIndices, cylinderGeometry.Positions.Count - 2, currentBottom, nextBottom);
                Simple3DUtils.addTri(cylinderGeometry.TriangleIndices, cylinderGeometry.Positions.Count - 1, nextTop, currentTop);
            }


            // Create the model and apply a material
            GeometryModel3D cylinderModel = new GeometryModel3D();
            cylinderModel.Geometry = cylinderGeometry;

            if (color != null) cylinderModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return cylinderModel;
        }

        /*
         * CYLINDER
         */
        public static GeometryModel3D getCone(double radius, double height, int subdivisions, Vector3D position, Color? color = null)
        {
            MeshGeometry3D coneGeometry = new MeshGeometry3D();

            for (int i = 0; i <= subdivisions; i++)
            {
                double phi = i * 2 * Math.PI / subdivisions;
                double x = radius * Math.Cos(phi);
                double z = radius * Math.Sin(phi);

                coneGeometry.Positions.Add(new Point3D(position.X + x, position.Y - height / 2, position.Z + z));
            }

            coneGeometry.Positions.Add(new Point3D(position.X, position.Y - height / 2, position.Z));
            coneGeometry.Positions.Add(new Point3D(position.X, position.Y + height / 2, position.Z));

            for (int i = 0; i < subdivisions; i++)
            {
                Simple3DUtils.addTri(coneGeometry.TriangleIndices, i, i + 1, coneGeometry.Positions.Count - 2);
                Simple3DUtils.addTri(coneGeometry.TriangleIndices, i, coneGeometry.Positions.Count - 1, i + 1);
            }

            GeometryModel3D coneModel = new GeometryModel3D();
            coneModel.Geometry = coneGeometry;

            if (color != null) coneModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return coneModel;
        }

        /*
         * CAPSULE
         * Note: If the subdivisions are less than 6 it'll start looking off because the hemispheres' reach is too low
         */
        public static GeometryModel3D getCapsule(double radius, double height, int subdivisions, Vector3D position, Color? color = null)
        {
            // Create the capsule geometry
            MeshGeometry3D capsuleGeometry = new MeshGeometry3D();

            // Cylinder part
            if(height > 0)
            {
                for (int i = 0; i <= subdivisions; i++)
                {
                    double phi = i * 2 * Math.PI / subdivisions;
                    double x = radius * Math.Cos(phi);
                    double z = radius * Math.Sin(phi);

                    capsuleGeometry.Positions.Add(new Point3D(position.X + x, position.Y - height / 2, position.Z + z));
                    capsuleGeometry.Positions.Add(new Point3D(position.X + x, position.Y + height / 2, position.Z + z));
                }
                for (int i = 0; i < subdivisions; i++)
                {
                    int currentBottom = 2 * i;
                    int currentTop = currentBottom + 1;
                    int nextBottom = (currentBottom + 2) % (2 * subdivisions);
                    int nextTop = (currentTop + 2) % (2 * subdivisions);

                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, currentBottom, currentTop, nextTop);
                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, currentBottom, nextTop, nextBottom);
                }
            }

            // Top hemisphere
            int topHemisphereIndexBase = capsuleGeometry.Positions.Count;

            for (int lat = 0; lat <= subdivisions / 2; lat++)
            {
                double theta = lat * Math.PI / subdivisions;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= subdivisions; lon++)
                {
                    double phi = lon * 2 * Math.PI / subdivisions;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = position.X + radius * sinTheta * cosPhi;
                    double y = position.Y + height/2 + radius * cosTheta;
                    double z = position.Z + radius * sinTheta * sinPhi;

                    capsuleGeometry.Positions.Add(new Point3D(x, y, z));
                }
            }

            // Generate the triangles for the semisphere
            for (int lat = 0; lat < subdivisions / 2; lat++)
            {
                for (int lon = 0; lon < subdivisions; lon++)
                {
                    int current = topHemisphereIndexBase + lat * (subdivisions + 1) + lon;
                    int next = current + subdivisions + 1;

                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, current, current + 1, next + 1);
                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, current, next + 1, next);
                }
            }

            // Bottom hemisphere
            int bottomHemisphereIndexBase = capsuleGeometry.Positions.Count;

            for (int lat = 0; lat <= subdivisions / 2; lat++)
            {
                double theta = lat * Math.PI / subdivisions;
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (int lon = 0; lon <= subdivisions; lon++)
                {
                    double phi = lon * 2 * Math.PI / subdivisions;
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = position.X + radius * sinTheta * cosPhi;
                    double y = position.Y - height/2 - radius * cosTheta;
                    double z = position.Z + radius * sinTheta * sinPhi;

                    capsuleGeometry.Positions.Add(new Point3D(x, y, z));
                }
            }

            for (int lat = 0; lat < subdivisions / 2; lat++)
            {
                for (int lon = 0; lon < subdivisions; lon++)
                {
                    int current = bottomHemisphereIndexBase + lat * (subdivisions + 1) + lon;
                    int next = current + subdivisions + 1;

                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, current, next + 1, current + 1);
                    Simple3DUtils.addTri(capsuleGeometry.TriangleIndices, current, next, next + 1);
                }
            }

            // Create the model and apply a material
            GeometryModel3D capsuleModel = new GeometryModel3D();
            capsuleModel.Geometry = capsuleGeometry;

            if (color != null) capsuleModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return capsuleModel;
        }

        /*
         * SQUARE
         */
        public static GeometryModel3D getSquare(int size, Vector3D position, Color? color = null)
        {
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            // Create a collection of vertex positions for the MeshGeometry3D.
            myMeshGeometry3D.Positions.Add(new Point3D(-size, -size, size));
            myMeshGeometry3D.Positions.Add(new Point3D(size, -size, size));
            myMeshGeometry3D.Positions.Add(new Point3D(size, size, size));
            myMeshGeometry3D.Positions.Add(new Point3D(size, size, size));
            myMeshGeometry3D.Positions.Add(new Point3D(-size, size, size));
            myMeshGeometry3D.Positions.Add(new Point3D(-size, -size, size));

            // Create a collection of texture coordinates for the MeshGeometry3D.
            myMeshGeometry3D.TextureCoordinates.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates.Add(new Point(1, 1));
            myMeshGeometry3D.TextureCoordinates.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates.Add(new Point(0, 0));
            myMeshGeometry3D.TextureCoordinates.Add(new Point(1, 1));

            // Create a collection of triangle indices for the MeshGeometry3D.
            Simple3DUtils.addTri(myMeshGeometry3D.TriangleIndices, 0, 1, 2);
            Simple3DUtils.addTri(myMeshGeometry3D.TriangleIndices, 3, 4, 5);

            GeometryModel3D myGeometryModel = new GeometryModel3D();
            myGeometryModel.Geometry = myMeshGeometry3D;
            if (color != null) myGeometryModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));

            return myGeometryModel;
        }

        /*
         * VECTOR
         */
        public static GeometryModel3D getVector(Matrix4x4 translationMatrix, double length, double width, Color? color = null)
        {
            MeshGeometry3D VectorGeometry = new MeshGeometry3D();

            // Create a collection of vertex positions for the MeshGeometry3D.
            VectorGeometry.Positions.Add(new Point3D(0 - width / 2, 0, 0));
            VectorGeometry.Positions.Add(new Point3D(0 + width / 2, 0, 0));
            VectorGeometry.Positions.Add(new Point3D(0 - width / 2, length, 0));
            VectorGeometry.Positions.Add(new Point3D(0 + width / 2, length, 0));

            VectorGeometry.Positions.Add(new Point3D(0, 0, 0 - width / 2));
            VectorGeometry.Positions.Add(new Point3D(0, 0, 0 + width / 2));
            VectorGeometry.Positions.Add(new Point3D(0, length, 0 - width / 2));
            VectorGeometry.Positions.Add(new Point3D(0, length, 0 + width / 2));

            // Create a collection of triangle indices for the MeshGeometry3D.
            Simple3DUtils.addTri(VectorGeometry.TriangleIndices, 0, 1, 2);
            Simple3DUtils.addTri(VectorGeometry.TriangleIndices, 2, 1, 3);
            Simple3DUtils.addTri(VectorGeometry.TriangleIndices, 4, 5, 6);
            Simple3DUtils.addTri(VectorGeometry.TriangleIndices, 6, 5, 7);

            GeometryModel3D vectorModel = new GeometryModel3D();
            vectorModel.Geometry = VectorGeometry;

            if (color != null)
            {
                vectorModel.Material = new DiffuseMaterial(new SolidColorBrush(color.Value));
                vectorModel.BackMaterial = new DiffuseMaterial(new SolidColorBrush(color.Value));
            }

            Simple3DUtils.applyTransform(vectorModel, translationMatrix);

            return vectorModel;
        }
    }
}
