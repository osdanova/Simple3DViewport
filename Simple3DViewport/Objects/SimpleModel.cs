using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Simple3DViewport.Objects
{
    public class SimpleModel
    {
        public string ModelId { get; set; }
        public List<string> Labels { get; set; }
        public List<SimpleMesh> Meshes { get; set; }
        public bool IsVisible { get; set; }

        public SimpleModel()
        {
            Labels = new List<string>();
            Meshes = new List<SimpleMesh>();
            IsVisible = true;
        }
        public SimpleModel(List<SimpleMesh> meshes, string? modelId = null, List<string>? labels = null)
        {
            Meshes = meshes;
            if (Meshes == null) Meshes = new List<SimpleMesh>();
            ModelId = modelId;
            Labels = labels;
            if (Labels == null) Labels = new List<string>();
            IsVisible = true;
        }

        public ModelVisual3D getVisual()
        {
            ModelVisual3D visual = new ModelVisual3D();
            Model3DGroup modelGroup = new Model3DGroup();

            foreach(SimpleMesh mesh in Meshes)
            {
                if (mesh.IsVisible)
                {
                    modelGroup.Children.Add(mesh.Geometry);
                }
            }

            visual.Content = modelGroup;

            return visual;
        }

        public void setVisibilityById(bool visibility, string itemId)
        {
            if(itemId == ModelId)
            {
                IsVisible = visibility;
            }

            foreach (SimpleMesh mesh in Meshes)
            {
                if (itemId == mesh.MeshId)
                {
                    mesh.IsVisible = visibility;
                }
            }
        }
        public void setVisibilityByLabel(bool visibility, string label)
        {
            if (Labels.Contains(label))
            {
                IsVisible = visibility;
            }

            foreach (SimpleMesh mesh in Meshes)
            {
                if (mesh.Labels.Contains(label))
                {
                    mesh.IsVisible = visibility;
                }
            }
        }

        public void removeMeshById(string meshId)
        {
            for(int i = Meshes.Count - 1; i >= 0; i--)
            {
                SimpleMesh mesh = Meshes[i];
                if (mesh.MeshId == meshId)
                {
                    Meshes.Remove(mesh);
                }
            }
        }
        public void removeMeshByLabel(string label)
        {
            for (int i = Meshes.Count - 1; i >= 0; i--)
            {
                SimpleMesh mesh = Meshes[i];
                if (mesh.Labels.Contains(label))
                {
                    Meshes.Remove(mesh);
                }
            }
        }
    }
}
