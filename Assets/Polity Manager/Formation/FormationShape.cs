using System.Collections.Generic;
using UnityEngine;

namespace Polity
{
    public static class FormationShape
    {
        /// <summary>
        /// Returns a list of local offsets for a square grid formation.
        /// The grid is centered on the origin so the leader sits at (0,0,0).
        /// </summary>
        /// <param name="columns">Number of columns.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="spacing">World-unit spacing between slots.</param>
        public static List<Vector3> CreateSquareFormation(int columns, int rows, float spacing = 1.5f)
        {
            var offsets = new List<Vector3>(columns * rows);

            // Centre the grid so the leader is in the middle
            float xOrigin = -(columns - 1) * spacing * 0.5f;
            float zOrigin = -(rows - 1) * spacing * 0.5f;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    offsets.Add(new Vector3(
                        xOrigin + col * spacing,
                        0f,
                        zOrigin + row * spacing
                    ));
                }
            }

            return offsets;
        }

        // Easy to extend — add CreateCircleFormation, CreateWedgeFormation, etc. here
    }
}