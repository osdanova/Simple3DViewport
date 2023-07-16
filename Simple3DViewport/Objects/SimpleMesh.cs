using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Simple3DViewport.Objects
{
    public class SimpleMesh
    {
        public string MeshId { get; set; }
        public List<string> Labels { get; set; }
        public GeometryModel3D Geometry { get; set; }
        public bool IsVisible { get; set; }

        public SimpleMesh()
        {
            Labels = new List<string>();
            IsVisible = true;
        }
        public SimpleMesh(GeometryModel3D geometry, string? meshId = null, List<string>? labels = null)
        {
            Geometry = geometry;
            MeshId = meshId;
            Labels = labels;
            if(Labels == null) Labels = new List<string>();
            IsVisible= true;
        }
    }
}
