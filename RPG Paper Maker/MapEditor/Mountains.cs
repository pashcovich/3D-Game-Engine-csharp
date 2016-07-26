﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Paper_Maker
{
    [Serializable]
    public class Mountains
    {
        public Dictionary<int, MountainsGroup> Groups = new Dictionary<int, MountainsGroup>();

        #region CLASS MoutainsGroup

        [Serializable]
        public class MountainsGroup
        {
            public Dictionary<int[], Mountain> Tiles = new Dictionary<int[], Mountain>(new IntArrayComparer());

            [NonSerialized()]
            VertexBuffer VB;
            [NonSerialized()]
            VertexPositionTexture[] VerticesArray;
            [NonSerialized()]
            IndexBuffer IB;
            [NonSerialized()]
            int[] IndexesArray;


            // -------------------------------------------------------------------
            // CreateCopy
            // -------------------------------------------------------------------

            public MountainsGroup CreateCopy()
            {
                MountainsGroup newMountains = new MountainsGroup();
                foreach (KeyValuePair<int[], Mountain> entry in Tiles)
                {
                    newMountains.Tiles[entry.Key] = Tiles[entry.Key].CreateCopy();
                }

                return newMountains;
            }

            // -------------------------------------------------------------------
            // GenMountains
            // -------------------------------------------------------------------

            public void GenMountains(GraphicsDevice device, int id)
            {
                DisposeBuffers(device);
                CreatePortion(device, id);
            }

            // -------------------------------------------------------------------
            // CreatePortion
            // -------------------------------------------------------------------

            public void CreatePortion(GraphicsDevice device, int id)
            {
                List<VertexPositionTexture> verticesList = new List<VertexPositionTexture>();
                List<int> indexesList = new List<int>();
                int[] indexes = new int[]
                {
                    0, 1, 2, 0, 2, 3,
                    4, 5, 6, 4, 6, 7,
                    8, 9, 10, 8, 10, 11,
                    12, 13, 14, 12, 14, 15,
                };
                int offset = 0;

                foreach (KeyValuePair<int[], Mountain> entry in Tiles)
                {
                    foreach (VertexPositionTexture vertex in CreateTex(MapEditor.TexReliefs[id], entry.Key, entry.Value))
                    {
                        verticesList.Add(vertex);
                    }
                    for (int n = 0; n < 24; n++)
                    {
                        indexesList.Add(indexes[n] + offset);
                    }
                    offset += 16;
                }

                VerticesArray = verticesList.ToArray();
                IndexesArray = indexesList.ToArray();
                IB = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, IndexesArray.Length, BufferUsage.None);
                IB.SetData(IndexesArray);
                VB = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, VerticesArray.Length, BufferUsage.None);
                VB.SetData(VerticesArray);
            }

            // -------------------------------------------------------------------
            // CreateTex
            // -------------------------------------------------------------------

            protected VertexPositionTexture[] CreateTex(Texture2D texture, int[] coords, Mountain mountain)
            {
                int x = coords[0], y = coords[1] * WANOK.SQUARE_SIZE + coords[2], z = coords[3];

                // Texture coords
                float left = 0;
                float top = 0;
                float bot = ((float)WANOK.SQUARE_SIZE) / texture.Height;
                float right = ((float)WANOK.SQUARE_SIZE) / texture.Width;

                // Adjust in order to limit risk of textures flood
                float width = left + right;
                float height = top + bot;
                left += width / WANOK.COEF_BORDER_TEX;
                right -= width / WANOK.COEF_BORDER_TEX;
                top += height / WANOK.COEF_BORDER_TEX;
                bot -= height / WANOK.COEF_BORDER_TEX;

                // Vertex Position and Texture
                return new VertexPositionTexture[]
                {
                    new VertexPositionTexture(new Vector3(x, y + WANOK.SQUARE_SIZE, z), new Vector2(left, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y + WANOK.SQUARE_SIZE, z), new Vector2(right, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y, z), new Vector2(right, bot)),
                    new VertexPositionTexture(new Vector3(x, y, z), new Vector2(left, bot)),
                    new VertexPositionTexture(new Vector3(x, y + WANOK.SQUARE_SIZE, z + 1), new Vector2(left, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y + WANOK.SQUARE_SIZE, z + 1), new Vector2(right, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y, z + 1), new Vector2(right, bot)),
                    new VertexPositionTexture(new Vector3(x, y, z + 1), new Vector2(left, bot)),
                    new VertexPositionTexture(new Vector3(x, y + WANOK.SQUARE_SIZE, z), new Vector2(left, top)),
                    new VertexPositionTexture(new Vector3(x, y + WANOK.SQUARE_SIZE, z + 1), new Vector2(right, top)),
                    new VertexPositionTexture(new Vector3(x, y, z + 1), new Vector2(right, bot)),
                    new VertexPositionTexture(new Vector3(x, y, z), new Vector2(left, bot)),
                    new VertexPositionTexture(new Vector3(x + 1, y + WANOK.SQUARE_SIZE, z), new Vector2(left, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y + WANOK.SQUARE_SIZE, z + 1), new Vector2(right, top)),
                    new VertexPositionTexture(new Vector3(x + 1, y, z + 1), new Vector2(right, bot)),
                    new VertexPositionTexture(new Vector3(x + 1, y, z), new Vector2(left, bot))
                };
            }

            // -------------------------------------------------------------------
            // Draw
            // -------------------------------------------------------------------

            public void Draw(GraphicsDevice device, AlphaTestEffect effect, int id)
            {
                if (VB != null)
                {
                    if (!MapEditor.TexAutotiles.ContainsKey(id)) effect.Texture = MapEditor.TexNone;
                    else effect.Texture = MapEditor.TexReliefs[id];

                    device.SetVertexBuffer(VB);
                    device.Indices = IB;
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VerticesArray, 0, VerticesArray.Length, IndexesArray, 0, VerticesArray.Length / 2);
                    }
                }
            }

            // -------------------------------------------------------------------
            // DisposeBuffers
            // -------------------------------------------------------------------

            public void DisposeBuffers(GraphicsDevice device, bool nullable = true)
            {
                if (VB != null)
                {
                    device.SetVertexBuffer(null);
                    device.Indices = null;
                    VB.Dispose();
                    IB.Dispose();
                    if (nullable)
                    {
                        VB = null;
                        IB = null;
                    }
                }
            }
        }

        #endregion

        #region Mountains

        // -------------------------------------------------------------------
        // CreateCopy
        // -------------------------------------------------------------------

        public Mountains CreateCopy()
        {
            Mountains newMountains = new Mountains();
            foreach (int id in Groups.Keys)
            {
                newMountains.Groups[id] = Groups[id].CreateCopy();
            }

            return newMountains;
        }

        // -------------------------------------------------------------------
        // IsEmpty
        // -------------------------------------------------------------------

        public bool IsEmpty()
        {
            return Groups.Count == 0;
        }

        // -------------------------------------------------------------------
        // ContainsInGroup
        // -------------------------------------------------------------------

        public bool ContainsInGroup(int[] coords)
        {
            foreach (int id in Groups.Keys)
            {
                if (Groups[id].Tiles.ContainsKey(coords)) return true;
            }

            return false;
        }

        // -------------------------------------------------------------------
        // Add
        // -------------------------------------------------------------------

        public void Add(int[] coords, int id, bool update = true)
        {
            if (!ContainsInGroup(coords))
            {
                if (!Groups.ContainsKey(id)) Groups[id] = new MountainsGroup();
                Groups[id].Tiles[coords] = new Mountain();
            }
        }

        // -------------------------------------------------------------------
        // GenMountains
        // -------------------------------------------------------------------

        public void GenMountains(GraphicsDevice device)
        {
            foreach (int id in Groups.Keys)
            {
                Groups[id].GenMountains(device, id);
            }
        }

        // -------------------------------------------------------------------
        // Draw
        // -------------------------------------------------------------------

        public void Draw(GraphicsDevice device, AlphaTestEffect effect)
        {
            foreach (int id in Groups.Keys)
            {
                Groups[id].Draw(device, effect, id);
            }
        }

        // -------------------------------------------------------------------
        // DisposeBuffers
        // -------------------------------------------------------------------

        public void DisposeBuffers(GraphicsDevice device, bool nullable = true)
        {
            foreach (int id in Groups.Keys)
            {
                Groups[id].DisposeBuffers(device, nullable);
            }
        }

        #endregion
    }
}