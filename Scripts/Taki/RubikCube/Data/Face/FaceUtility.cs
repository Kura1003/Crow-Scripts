using System.Collections.Generic;
using Taki.Utility;

namespace Taki.RubiksCube.Data
{
    internal static class FaceUtility
    {
        private static readonly Face[] _allFaces =
        {
            Face.Front,
            Face.Back,
            Face.Left,
            Face.Right,
            Face.Top,
            Face.Bottom
        };

        internal static Face[] GetAllFaces() => (Face[])_allFaces.Clone();

        internal static int GetFaceCount() => _allFaces.Length;

        internal static Face GetRandomFace()
        {
            var random = SeedGenerator.GetRandom();
            var randomIndex = random.Next(0, _allFaces.Length);
            return _allFaces[randomIndex];
        }

        internal static List<Face> GetFaces(Face faces)
        {
            var list = new List<Face>();
            foreach (Face face in _allFaces)
            {
                if (faces.HasFlag(face))
                {
                    list.Add(face);
                }
            }
            return list;
        }
    }
}