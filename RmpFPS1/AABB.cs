using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace RmpFPS1
{
    public class AABB
    {
        public AABB()
        {
        }

        public AABB(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;
            UpdateFromMinMax();
        }
       
        public void Corners(){
            FTL = new Vector3(Min.X, Max.Y, Max.Z);
            FTR = new Vector3(Max.X, Max.Y, Max.Z);
            FBR = new Vector3(Max.X, Min.Y, Max.Z);
            FBL = new Vector3(Min.X, Min.Y, Max.Z);

            BTL = new Vector3(Min.X, Max.Y, Min.Z);
            BTR = new Vector3(Max.X, Max.Y, Min.Z);
            BBR = new Vector3(Max.X, Min.Y, Min.Z);
            BBL = new Vector3(Min.X, Min.Y, Min.Z);
        }
        public Vector3[] getCorners()
        { 
            Vector3[] corners = {FTL, FTR, FBR, FBL, BTL, BTR, BBR, BBL};
            return corners;
        }
        
        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(_min, _max);
        }
        public bool Instersects(AABB a)
        {
            if (Max.X < a.Min.X || Min.X > a.Max.X) return false;
            if (Max.Y < a.Min.Y || Min.Y > a.Max.Y) return false;
            if (Max.Z < a.Min.Z || Min.Z > a.Max.Z) return false;
            return true;
        }
        public bool Contains(Vector3 a)
        {
            if (Max.X < a.X || Min.X > a.X) return false;
            if (Max.Y < a.Y || Min.Y > a.Y) return false;
            if (Max.Z < a.Z || Min.Z > a.Z) return false;
            return true;
        }
        public bool Contains(AABB a)
        {
            if (Min.X >= a.Min.X && Max.X <= a.Max.X &&
               Min.Y >= a.Min.Y && Max.Y <= a.Max.Y &&
               Min.Z >= a.Min.Z && Max.Z <= a.Max.Z)
                return true;
            return false;
        }
        
        
        public Vector3 minimumTranslation(AABB a){
            Vector3 mtd = new Vector3();

            float left = (a.Min.X - Max.X);
            float right = (a.Max.X - Min.X);
            float bottom = (a.Min.Y - Max.Y);
            float top = (a.Max.Y - Min.Y);
            float far = (a.Min.Z - Max.Z);
            float near = (a.Max.Z - Min.Z);

            if(Math.Abs(left) < right)
                mtd.X = left;
            else
                mtd.X = right;
            if(Math.Abs(bottom) < top)
                mtd.Y = bottom;
            else 
                mtd.Y = top;
            if(Math.Abs(far) < near)
                mtd.Z = far;
            else
                mtd.Z = near;

            if(Math.Abs(mtd.X) <= Math.Abs(mtd.Y) && Math.Abs(mtd.X) <= Math.Abs(mtd.Z)){
                mtd.Y = 0;
                mtd.Z = 0;
            }
            else if(Math.Abs(mtd.Y) <= Math.Abs(mtd.X) && Math.Abs(mtd.Y) <= Math.Abs(mtd.Z)){
                mtd.X = 0;
                mtd.Z = 0;
            }
            else if(Math.Abs(mtd.Z) <= Math.Abs(mtd.X) && Math.Abs(mtd.Z) <= Math.Abs(mtd.Y)){
                mtd.X = 0;
                mtd.Y = 0;
            }
            if (mtd != Vector3.Zero)
            {
                _collisionNormal = mtd;
                _collisionNormal.Normalize();
            }
            return mtd;
        }
        public Vector3 CollisionNormal
        {
            get { return _collisionNormal; }
            set { _collisionNormal = value; }
        }
        public Vector3 ChangeVelocity()
        {
            Vector3 velocity = Vector3.Zero;
            if (_collisionNormal.X == 0) velocity.X = 1;
            else velocity.X = 0;
            if (_collisionNormal.Y == 0) velocity.Y = 1;
            else velocity.Y = 0;
            if (_collisionNormal.Z == 0) velocity.Z = 1;
            else velocity.Z = 0;
            //Console.Out.WriteLine("norm" + _collisionNormal);
            return velocity;
        }
        public static Vector3[] Normals = new Vector3[6]{
            new Vector3(-1, 0, 0), //left -x
            new Vector3(1, 0, 0), //right x
            new Vector3(0, -1, 0), //bottom -y
            new Vector3(0, 1, 0), //top y
            new Vector3(0, 0, -1), //far -z
            new Vector3(0, 0, 1), //near z
        };
        public bool HoversOver(AABB a)
        {
            if (Min.Y > a.Max.Y) return true;
            if (Max.X < a.Min.X || Min.X > a.Max.X) return false;
            if (Max.Z < a.Min.Z || Min.Z > a.Max.Z) return false;
            return false;
        }
        public Vector3 FTL, FTR, FBR, FBL, BTL, BTR, BBR, BBL;
        //======================================================================
        public void UpdateFromMinMax()
        {
            Corners();
            _hw = Math.Abs((Max.X - Min.X) / 2);
            _hh = Math.Abs((Max.Y - Min.Y) / 2);
            _hd = Math.Abs((Max.Z - Min.Z) / 2);
            _extents = new Vector3(_hw, _hh, _hd);
            _center = Max - _extents;
        }
        public void UpdateFromCenter()
        {
            _min = _center - _extents;
            _max = _center + _extents;
            Corners();
        }
        public Vector3 Min
        {
            get { return _min; }
            set { _min = value; UpdateFromMinMax();  }
        }

        public Vector3 Max
        {
            get { return _max; }
            set { _max = value; UpdateFromMinMax(); }
        }
        public Matrix MatrixMin
        {
            get { return Matrix.CreateTranslation(_min); }
            set { _min = value.Translation; UpdateFromMinMax(); }
        }
        public Matrix MatrixMax
        {
            get { return Matrix.CreateTranslation(_max); }
            set { _max = value.Translation; UpdateFromMinMax(); }
        }
        public Vector3 Center
        {
            get { return _center; }
            set { _center = value; UpdateFromCenter(); }
        }
        public Vector3 Extents
        {
            get { return _extents; }
            set { _extents = value; }
        }
        protected float _hw;
        protected float _hh;
        protected float _hd;
        protected Vector3 _center;
        protected Vector3 _extents;
        protected Vector3 _min;
        protected Vector3 _max;
        protected Vector3 _collisionNormal;

        internal bool Intersects(AABB a)
        {
            if (Max.X < a.Min.X || Min.X > a.Max.X) return false;
            if (Max.Y < a.Min.Y || Min.Y > a.Max.Y) return false;
            if (Max.Z < a.Min.Z || Min.Z > a.Max.Z) return false;
            return true;
        }
    }
}

