using System;
using System.Drawing;
using OpenTK;

namespace DemoCube
{
    internal class Camera
    {
        GLControl canvas;
        Matrix4 view;
        Matrix4 projection;
        Vector3 position;
        Vector3 target;
        float yaw;
        float pitch;
        float radius;
        float near;
        float far;
        float fov;
        bool enableRotate;

        public ref Matrix4 View { get { return ref view; } }
        public ref Matrix4 Projection { get { return ref projection; } }
        public ref bool EnableRotate { get { return ref enableRotate; } }
        public Camera(GLControl canvas, Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
            var vector = position - target;
            radius = vector.Length;
            vector.Normalize();
            pitch = (float)Math.Acos(Vector3.Dot(vector, Vector3.UnitZ));
            yaw = (float)Math.Acos(Vector3.Dot(vector, Vector3.UnitX));
            near = 0.1f;
            far = 64.0f;
            fov = (float)Math.PI / 3;
            enableRotate = false;
            this.canvas = canvas;
        }

        public void UpdatePosition(int delta)
        {
            var offset = (float)delta * 0.001f;
            if (offset > 0)
            {
                var newPosition = Vector3.Lerp(position, target, Math.Abs(offset));
                if (Vector3.Dot(position, newPosition) > 0)
                {
                    position = newPosition;
                    radius = position.Length;
                }
            }
            else
            {
                var newPosition = (position - target) * (1 - offset);
                if (newPosition.Length + 5.0f <= far)
                {
                    position = newPosition;
                    radius = position.Length;
                }
            }
        }

        public void Update(ref Point start, ref Point end)
        {
            if(enableRotate)
            {
                float yDelta = -(end.Y - start.Y) * 2;
                float xDelta = (end.X - start.X) * 2;
                yDelta = yDelta / canvas.Height * (float)Math.PI;
                xDelta = xDelta / canvas.Width * 2 * (float)Math.PI;
                if (Math.Abs(pitch + yDelta) > Math.PI / 2)
                    pitch -= yDelta;
                else
                    pitch += yDelta;
                if (yaw + xDelta > 2 * Math.PI)
                    yaw = xDelta;
                else if (yaw + xDelta < 0)
                    yaw = 2 * (float)Math.PI + xDelta;
                else
                    yaw += xDelta;
                Vector3 dir = Vector3.Zero;
                dir.X = (float)(Math.Cos(yaw) * Math.Cos(pitch));
                dir.Y = (float)(Math.Sin(pitch));
                dir.Z = (float)(Math.Cos(pitch) * -Math.Sin(yaw));
                dir.Normalize();
                position = -dir * radius;
            }
            UpdateMatrices();
        }

        private void UpdateMatrices()
        {
            view = Matrix4.LookAt(position, target, Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView(fov, canvas.AspectRatio, near, far);
        }
    }
}
