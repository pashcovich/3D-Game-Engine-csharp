﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

// -------------------------------------------------------------------
// STATIC Class for global variables
// -------------------------------------------------------------------

namespace RPG_Paper_Maker
{
    static class WANOK
    {
        public static Vector3[] VERTICESFLOOR = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 1.0f)
        };
        public static Vector3[] VERTICESSPRITE = new Vector3[]
        {
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f)
        };
        public static int BASIC_SQUARE_SIZE = 32;
        public static int SQUARE_SIZE = 16;
        public static float RELATION_SIZE { get { return (float)(BASIC_SQUARE_SIZE) / SQUARE_SIZE; } }
        public static int PORTION_SIZE = 16;
        public static int PORTION_RADIUS = 4;
        public static string ProjectName = null;
        public static EngineSettings Settings = null;
        public static DemoSteps DemoStep = DemoSteps.None;
        public static Form CurrentDemoDialog = null;
        public static KeyboardManager KeyboardManager = new KeyboardManager();
        public static MouseManager TilesetMouseManager = new MouseManager();
        public static MouseManager MapMouseManager = new MouseManager();

        // PATHS
        public static string ABSOLUTEENGINEPATH;
        public static string PATHSETTINGS = "Config/EngineSettings.JSON";
        public static string CurrentDir = ".";
        public static string ExcecutablePath { get { return Application.ExecutablePath; } }
        public static string SystemPath { get { return Path.Combine(CurrentDir, "Content", "Datas", "System.rpmd"); } }
        public static string MapsDirectoryPath { get { return Path.Combine(CurrentDir, "Content", "Datas", "Maps"); } }



        // -------------------------------------------------------------------
        // CopyAll
        // -------------------------------------------------------------------

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into its new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        // -------------------------------------------------------------------
        // SaveDatas
        // -------------------------------------------------------------------

        public static void SaveDatas(object obj, string path)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(json);
                sw.Close();
                fs.Close();
            } catch(Exception e)
            {
                PathErrorMessage(e);
            }
        }

        // -------------------------------------------------------------------
        // SaveBinaryDatas
        // -------------------------------------------------------------------

        public static void SaveBinaryDatas(object obj, string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
                fs.Close();
            }
            catch (Exception e)
            {
                PathErrorMessage(e);
            }
        }

        // -------------------------------------------------------------------
        // LoadDatas
        // -------------------------------------------------------------------

        public static T LoadDatas<T>(string path)
        {
            T obj;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string json = sr.ReadToEnd();
                obj = JsonConvert.DeserializeObject<T>(json);
                sr.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                obj = default(T);
                PathErrorMessage(e);
            }

            return obj;
        }

        // -------------------------------------------------------------------
        // LoadBinaryDatas
        // -------------------------------------------------------------------

        public static T LoadBinaryDatas<T>(string path)
        {
            T obj;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                obj = (T)formatter.Deserialize(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                obj = default(T);
                PathErrorMessage(e);
            }

            return obj;
        }

        // -------------------------------------------------------------------
        // GetImageData
        // -------------------------------------------------------------------

        public static Color[] GetImageData(Color[] colorData, int width, Rectangle rectangle)
        {
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
                for (int y = 0; y < rectangle.Height; y++)
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
            return color;
        }

        // -------------------------------------------------------------------
        // GetSubImage
        // -------------------------------------------------------------------

        public static Texture2D GetSubImage(GraphicsDevice GraphicsDevice, Texture2D image, Rectangle rectangle)
        {
            Color[] imageData = new Color[image.Width * image.Height];
            image.GetData<Color>(imageData);
            Color[] imagePiece = WANOK.GetImageData(imageData, image.Width, rectangle);
            Texture2D subtexture = new Texture2D(GraphicsDevice, rectangle.Width, rectangle.Height);
            subtexture.SetData<Color>(imagePiece);

            return subtexture;
        }

        // -------------------------------------------------------------------
        // SaveTree
        // -------------------------------------------------------------------

        public static void SaveTree(TreeView tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, tree.Nodes.Cast<TreeNode>().ToList());
            }
        }

        // -------------------------------------------------------------------
        // LoadTree
        // -------------------------------------------------------------------

        public static void LoadTree(TreeView tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(file);
                TreeNode[] nodeList = (obj as IEnumerable<TreeNode>).ToArray();
                SetIconsTreeNodes(nodeList);
                tree.Nodes.AddRange(nodeList);
            }
        }

        // -------------------------------------------------------------------
        // SetIconsTreeNodes
        // -------------------------------------------------------------------

        public static void SetIconsTreeNodes(IEnumerable<TreeNode> treeNodeCollection)
        {
            foreach (TreeNode node in treeNodeCollection)
            {
                if (((TreeTag)node.Tag).IsMap)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }

                TreeNode[] nodes = new TreeNode[node.Nodes.Count];
                node.Nodes.CopyTo(nodes, 0);
                SetIconsTreeNodes(nodes);
            }
        }

        // -------------------------------------------------------------------
        // PathErrorMessage
        // -------------------------------------------------------------------

        public static void PathErrorMessage(Exception e)
        {
            MessageBox.Show("You get a path error. You can send a report to Wanok.rpm@gmail.com.\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // -------------------------------------------------------------------
        // Print
        // -------------------------------------------------------------------

        public static void Print(string m)
        {
            MessageBox.Show(m, "Print", MessageBoxButtons.OK);
        }

        // -------------------------------------------------------------------
        // CalculateRay : thanks to http://rbwhitaker.wikidot.com/picking
        // -------------------------------------------------------------------

        public static Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(
                    new Vector3(mouseLocation.X,mouseLocation.Y, 0.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(
                    new Vector3(mouseLocation.X,mouseLocation.Y, 1.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public static Vector3 GetPointOnRay(Ray ray, Camera camera, float distance)
        {
            return new Vector3((ray.Direction.X * distance) + camera.Position.X, (ray.Direction.Y * distance) + camera.Position.Y, (ray.Direction.Z * distance) + camera.Position.Z);
        }

        public static Vector3 GetCorrectPointOnRay(Ray ray, Camera camera, float distance, int height)
        {
            Vector3 point = GetPointOnRay(ray, camera, distance);
            Vector3 correctedPoint = new Vector3((int)(point.X / SQUARE_SIZE), (int)(point.Y / SQUARE_SIZE), (int)(point.Z / SQUARE_SIZE));
            if (point.X < 0) correctedPoint.X -= 1;
            correctedPoint.Y = height;
            if (point.Z < 0) correctedPoint.Z -= 1;

            return correctedPoint;
        }

        public static float? IntersectDistance(BoundingSphere sphere, Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(sphere);
        }

        public static bool IntersectsModel(Vector2 mouseLocation, Model model, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            for (int index = 0; index < model.Meshes.Count; index++)
            {
                BoundingSphere sphere = model.Meshes[index].BoundingSphere;
                sphere = sphere.Transform(world);
                float? distance = IntersectDistance(sphere, mouseLocation, view, projection, viewport);

                if (distance != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IntersectsPlane(Vector2 mouseLocation, int height, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            Plane plane = new Plane(new Vector3(0, 0, 0), new Vector3(64, 0, 0), new Vector3(0, 0, 64));
            Ray mouseRay = CalculateRay(mouseLocation, view, projection, viewport);
            return mouseRay.Intersects(plane) != null;
        }

        // -------------------------------------------------------------------
        // GetPixelHeight
        // -------------------------------------------------------------------

        public static int GetPixelHeight(int[] height)
        {
            return (height[0] * SQUARE_SIZE) + height[1];
        }
    }
}
