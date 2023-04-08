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
                    position = newPosition;
            }
            else
            {
                var newPosition = (position - target) * (1 - offset);
                if (newPosition.Length + 5.0f <= far)
                    position = newPosition;
            }
        }

        public void Update(ref Point start, ref Point end)
        {
            if (enableRotate)
                UpdateAngles(ref start, ref end);
            CreateViewMatrix();
            projection = Matrix4.CreatePerspectiveFieldOfView(fov, canvas.AspectRatio, near, far);
        }

        private void UpdateAngle(ref float angle)
        {
            if (angle > 2 * Math.PI)
                angle = angle - 2 * (float)Math.PI;
            else if (angle < 0)
                angle = 2 * (float)Math.PI + angle;
        }

        private void UpdateAngles(ref Point start, ref Point end)
        {
            float yDelta = -(end.Y - start.Y) * 2;
            float xDelta = (end.X - start.X) * 2;
            yDelta = yDelta / canvas.Height * 2 * (float)Math.PI;
            xDelta = xDelta / canvas.Width * 2 * (float)Math.PI;
            yaw += xDelta;
            pitch += yDelta;
            UpdateAngle(ref pitch);
            UpdateAngle(ref yaw);
        }

        private void CreateViewMatrix()
        {
            var yDelta = (float)Math.PI - pitch;
            var xDelta = (float)Math.PI / 2 + yaw;
            var newPitch = new Quaternion(yDelta, 0.0f, 0.0f);
            var newYaw = new Quaternion(0.0f, xDelta, 0.0f);
            var newView = Matrix3.CreateFromQuaternion(newYaw * newPitch);
            newView.Transpose();
            view = new Matrix4(newView);
            view[3, 0] = -(position + target).X;
            view[3, 1] = -(position + target).Y;
            view[3, 2] = -(position + target).Z;
        }

        private void UpdateMatrices()
        {
            view = Matrix4.LookAt(position, target, Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView(fov, canvas.AspectRatio, near, far);
        }
    }
}
