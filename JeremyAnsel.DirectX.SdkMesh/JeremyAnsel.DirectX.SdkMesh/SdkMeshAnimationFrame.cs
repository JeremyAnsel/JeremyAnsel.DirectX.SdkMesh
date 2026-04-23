namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshAnimationFrame
    {
        internal SdkMeshAnimationFrame(SdkMeshRawAnimFrameData rawFrame)
        {
            this.FrameName = rawFrame.FrameName;

            foreach (SdkMeshRawAnimData rawData in rawFrame.AnimationKeys)
            {
                var key = new SdkMeshAnimationKey(rawData);
                this.AnimationKeys.Add(key);
            }
        }

        public string? FrameName { get; private set; }

        public IList<SdkMeshAnimationKey> AnimationKeys { get; } = new List<SdkMeshAnimationKey>();
    }
}
