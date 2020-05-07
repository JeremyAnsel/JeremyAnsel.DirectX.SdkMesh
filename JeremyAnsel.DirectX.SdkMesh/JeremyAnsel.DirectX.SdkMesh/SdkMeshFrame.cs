using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshFrame
    {
        internal SdkMeshFrame(SdkMeshRawFrame rawFrame)
        {
            this.Name = rawFrame.Name;
            this.MeshIndex = rawFrame.MeshIndex;
            this.ParentFrameIndex = rawFrame.ParentFrameIndex;
            this.ChildFrameIndex = rawFrame.ChildFrameIndex;
            this.SiblingFrameIndex = rawFrame.SiblingFrameIndex;
            this.Matrix = rawFrame.Matrix;
            this.AnimationFrameIndex = rawFrame.AnimationDataIndex;

            this.BindPoseFrameMatrix = XMMatrix.Identity;
            this.TransformedFrameMatrix = XMMatrix.Identity;
            this.WorldPoseFrameMatrix = XMMatrix.Identity;
        }

        public string Name { get; private set; }

        public int MeshIndex { get; private set; }

        public int ParentFrameIndex { get; private set; }

        public int ChildFrameIndex { get; private set; }

        public int SiblingFrameIndex { get; private set; }

        public XMFloat4X4 Matrix { get; set; }

        public int AnimationFrameIndex { get; private set; }

        public XMFloat4X4 BindPoseFrameMatrix { get; set; }

        public XMFloat4X4 TransformedFrameMatrix { get; set; }

        public XMFloat4X4 WorldPoseFrameMatrix { get; set; }

        internal void UpdateAnimationFrameIndex(int index)
        {
            this.AnimationFrameIndex = index;
        }

        internal void TransformBindPoseFrame(SdkMeshFile file, XMMatrix parentWorld)
        {
            // Transform ourselves
            XMMatrix mLocalWorld = this.Matrix * parentWorld;
            this.BindPoseFrameMatrix = mLocalWorld;

            // Transform our siblings
            if (this.SiblingFrameIndex != -1)
            {
                file.Frames[this.SiblingFrameIndex].TransformBindPoseFrame(file, parentWorld);
            }

            // Transform our children
            if (this.ChildFrameIndex != -1)
            {
                file.Frames[this.ChildFrameIndex].TransformBindPoseFrame(file, mLocalWorld);
            }
        }

        internal void TransformFrame(SdkMeshFile file, XMMatrix parentWorld, double time)
        {
            // Get the tick data
            XMMatrix mLocalTransform;

            int tick = file.GetAnimationKeyFromTime(time);

            if (this.AnimationFrameIndex == -1)
            {
                mLocalTransform = this.Matrix;
            }
            else
            {
                SdkMeshAnimationFrame animationFrame = file.AnimationFrames[this.AnimationFrameIndex];
                SdkMeshAnimationKey animationKey = animationFrame.AnimationKeys[tick];

                // turn it into a matrix (Ignore scaling for now)
                XMFloat3 parentPos = animationKey.Translation;
                XMMatrix mTranslate = XMMatrix.Translation(parentPos.X, parentPos.Y, parentPos.Z);

                XMVector quat = animationKey.Orientation;

                if (XMVector4.Equal(quat, XMVector.Zero))
                {
                    quat = XMQuaternion.Identity;
                }

                quat = XMQuaternion.Normalize(quat);
                XMMatrix mQuat = XMMatrix.RotationQuaternion(quat);
                mLocalTransform = mQuat * mTranslate;
            }

            // Transform ourselves
            XMMatrix mLocalWorld = mLocalTransform * parentWorld;
            this.TransformedFrameMatrix = mLocalWorld;
            this.WorldPoseFrameMatrix = mLocalWorld;

            // Transform our siblings
            if (this.SiblingFrameIndex != -1)
            {
                file.Frames[this.SiblingFrameIndex].TransformFrame(file, parentWorld, time);
            }

            // Transform our children
            if (this.ChildFrameIndex != -1)
            {
                file.Frames[this.ChildFrameIndex].TransformFrame(file, mLocalWorld, time);
            }
        }

        internal void TransformFrameAbsolute(SdkMeshFile file, double time)
        {
            int tick = file.GetAnimationKeyFromTime(time);

            if (this.AnimationFrameIndex != -1)
            {
                SdkMeshAnimationFrame animationFrame = file.AnimationFrames[this.AnimationFrameIndex];
                SdkMeshAnimationKey animationKey = animationFrame.AnimationKeys[tick];
                SdkMeshAnimationKey animationKeyOrig = animationFrame.AnimationKeys[0];

                XMMatrix mTrans1 = XMMatrix.Translation(-animationKeyOrig.Translation.X, -animationKeyOrig.Translation.Y, -animationKeyOrig.Translation.Z);
                XMMatrix mTrans2 = XMMatrix.Translation(animationKey.Translation.X, animationKey.Translation.Y, animationKey.Translation.Z);

                XMVector quat1 = animationKeyOrig.Orientation;
                quat1 = XMQuaternion.Inverse(quat1);
                XMMatrix mRot1 = XMMatrix.RotationQuaternion(quat1);
                XMMatrix mInvTo = mTrans1 * mRot1;

                XMVector quat2 = animationKey.Orientation;
                XMMatrix mRot2 = XMMatrix.RotationQuaternion(quat2);
                XMMatrix mFrom = mRot2 * mTrans2;

                XMMatrix mOutput = mInvTo * mFrom;
                this.TransformedFrameMatrix = mOutput;
            }
        }
    }
}
