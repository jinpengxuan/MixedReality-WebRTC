// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.MixedReality.WebRTC.Interop
{
    internal class RemoteVideoTrackInterop
    {
        internal sealed class RemoteVideoTrackHandle : ObjectHandle
        {
            public RemoteVideoTrackHandle(IntPtr value) : base(value) { }
        }

        #region Native functions

        [DllImport(Utils.dllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi,
            EntryPoint = "mrsRemoteVideoTrackRegisterI420AFrameCallback")]
        public static extern void RemoteVideoTrack_RegisterI420AFrameCallback(RemoteVideoTrackHandle trackHandle,
            LocalVideoTrackInterop.I420AVideoFrameUnmanagedCallback callback, IntPtr userData);

        [DllImport(Utils.dllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi,
            EntryPoint = "mrsRemoteVideoTrackRegisterArgb32FrameCallback")]
        public static extern void RemoteVideoTrack_RegisterArgb32FrameCallback(RemoteVideoTrackHandle trackHandle,
            LocalVideoTrackInterop.Argb32VideoFrameUnmanagedCallback callback, IntPtr userData);

        [DllImport(Utils.dllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi,
            EntryPoint = "mrsRemoteVideoTrackIsEnabled")]
        public static extern int RemoteVideoTrack_IsEnabled(RemoteVideoTrackHandle trackHandle);

        #endregion


        #region Marshaling data structures

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CreateConfig
        {
            public string TrackName;
        }

        #endregion


        #region Native callbacks

        public class InteropCallbackArgs
        {
            public RemoteVideoTrack Track;
            public LocalVideoTrackInterop.I420AVideoFrameUnmanagedCallback I420AFrameCallback;
            public LocalVideoTrackInterop.Argb32VideoFrameUnmanagedCallback Argb32FrameCallback;
        }

        [MonoPInvokeCallback(typeof(LocalVideoTrackInterop.I420AVideoFrameUnmanagedCallback))]
        public static void I420AFrameCallback(IntPtr userData, in I420AVideoFrame frame)
        {
            var track = Utils.ToWrapper<RemoteVideoTrack>(userData);
            track.OnI420AFrameReady(frame);
        }

        [MonoPInvokeCallback(typeof(LocalVideoTrackInterop.Argb32VideoFrameUnmanagedCallback))]
        public static void Argb32FrameCallback(IntPtr userData, in Argb32VideoFrame frame)
        {
            var track = Utils.ToWrapper<RemoteVideoTrack>(userData);
            track.OnArgb32FrameReady(frame);
        }

        #endregion


        #region Utilities

        public static RemoteVideoTrack CreateWrapper(PeerConnection parent, in PeerConnectionInterop.RemoteVideoTrackAddedInfo config)
        {
            // Create a new wrapper
            var wrapper = new RemoteVideoTrack(new RemoteVideoTrackHandle(config.trackHandle), parent, config.trackName);

            // Assign a reference to it inside the UserData of the native object so it can be retrieved whenever needed
            IntPtr wrapperRef = Utils.MakeWrapperRef(wrapper);
            ObjectInterop.Object_SetUserData(wrapper._nativeHandle, wrapperRef);

            return wrapper;
        }

        #endregion
    }
}
